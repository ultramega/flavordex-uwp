using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Base for classes to store objects in memory.
    /// </summary>
    /// <typeparam name="T">The type of objects to store.</typeparam>
    public abstract class ObjectCache<T>
    {
        /// <summary>
        /// Container for cache items.
        /// </summary>
        private struct CacheItem
        {
            /// <summary>
            /// The key to the item.
            /// </summary>
            public object Key { get; }

            /// <summary>
            /// The size of the item.
            /// </summary>
            public ulong Size { get; }

            /// <summary>
            /// The item being stored.
            /// </summary>
            public T Item { get; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="key">The key to the item.</param>
            /// <param name="size">The size of the item.</param>
            /// <param name="item">The item to store.</param>
            public CacheItem(object key, ulong size, T item)
            {
                Key = key;
                Size = size;
                Item = item;
            }
        }

        /// <summary>
        /// The total size available in this cache.
        /// </summary>
        private ulong _capacity;

        /// <summary>
        /// Gets or sets the total size available in this cache.
        /// </summary>
        public ulong Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Capacity", value, "Capacity must be greater than 0");
                }
                _capacity = value;
                TrimToSize(value);
            }
        }

        /// <summary>
        /// Gets or sets the current total size occupied by items in this cache.
        /// </summary>
        public ulong CurrentSize { get; private set; }

        /// <summary>
        /// The items stored in this cache.
        /// </summary>
        private LinkedList<CacheItem> _items = new LinkedList<CacheItem>();

        /// <summary>
        /// Map of keys to items stored in this cache.
        /// </summary>
        private Dictionary<object, CacheItem> _keyMap = new Dictionary<object, CacheItem>();

        /// <summary>
        /// Constructor. Sets the capacity to 25% of the system memory limit up to 512MB.
        /// </summary>
        public ObjectCache()
        {
            Capacity = Math.Min(512 * 1024 * 1024, MemoryManager.AppMemoryUsageLimit / 4);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capacity">The total size available in this cache.</param>
        public ObjectCache(ulong capacity)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// Gets the size of a single item.
        /// </summary>
        /// <param name="item">The item to measure.</param>
        /// <returns>The size of the item.</returns>
        protected abstract ulong GetSizeOf(T item);

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>Whether the item was stored.</returns>
        public bool Store(object key, T item)
        {
            if (KeyExists(key))
            {
                Remove(key);
            }

            var size = GetSizeOf(item);
            if (CurrentSize + size > Capacity)
            {
                if (size > Capacity)
                {
                    return false;
                }
                TrimToSize(Capacity - size);
            }

            var cacheItem = new CacheItem(key, size, item);
            _items.AddFirst(cacheItem);
            _keyMap.Add(key, cacheItem);
            CurrentSize += size;

            return true;
        }

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">The key to the item.</param>
        /// <returns>The item.</returns>
        public T Get(object key)
        {
            if (!KeyExists(key))
            {
                return default(T);
            }

            var item = _keyMap[key];
            MoveToFront(item);
            return item.Item;
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key to the item.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool Remove(object key)
        {
            if (!KeyExists(key))
            {
                return false;
            }

            var item = _keyMap[key];
            _keyMap.Remove(key);
            lock (_items)
            {
                _items.Remove(item);
            }
            CurrentSize -= item.Size;

            return true;
        }

        /// <summary>
        /// Removes all items from the cache.
        /// </summary>
        public void Empty()
        {
            lock (_items)
            {
                _items.Clear();
            }
            _keyMap.Clear();
            CurrentSize = 0;
        }

        /// <summary>
        /// Checks whether a key exists in the cache.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether an item corresponding to the key is stored in the cache.</returns>
        public bool KeyExists(object key)
        {
            return _keyMap.ContainsKey(key);
        }

        /// <summary>
        /// Removes the least recently accessed items until the CurrentSize is at or below the
        /// specified size.
        /// </summary>
        /// <param name="size">The size to which to reduce the cache.</param>
        private void TrimToSize(ulong size)
        {
            lock (_items)
            {
                while (CurrentSize > size)
                {
                    var item = _items.Last;
                    if (item != null)
                    {
                        _items.RemoveLast();
                        _keyMap.Remove(item.Value.Key);
                        CurrentSize -= item.Value.Size;
                    }
                }
            }
        }

        /// <summary>
        /// Moves an item to the front of the queue.
        /// </summary>
        /// <param name="item">The item to move.</param>
        private async void MoveToFront(CacheItem item)
        {
            await Task.Run(() =>
            {
                lock (_items)
                {
                    if (_items.Remove(item))
                    {
                        _items.AddFirst(item);
                    }
                }
            });
        }
    }
}

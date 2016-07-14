using System;
using System.Collections.Generic;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// Cache to store instances of Models for distinct database records.
    /// </summary>
    /// <typeparam name="T">The Type of Model to store.</typeparam>
    internal class ModelCache<T> where T : Model, new()
    {
        /// <summary>
        /// The data stored in the cache.
        /// </summary>
        private Dictionary<long, WeakReference<T>> _cache =
            new Dictionary<long, WeakReference<T>>();

        /// <summary>
        /// Gets an item from the cache, creating it if needed.
        /// </summary>
        /// <param name="data">The data for the Model.</param>
        /// <returns>The instance from the cache.</returns>
        public T Get(ContentValues data)
        {
            T model;
            var id = data.GetLong(BaseColumns._ID);
            if (id > 0)
            {
                if (!_cache.ContainsKey(id) || !_cache[id].TryGetTarget(out model))
                {
                    model = new T();
                    _cache[id] = new WeakReference<T>(model);
                }
            }
            else
            {
                model = new T();
            }
            model.SetData(data);
            return model;
        }

        /// <summary>
        /// Gets an item from the cache based on the primary ID.
        /// </summary>
        /// <param name="id">The primary ID of the Model.</param>
        /// <returns>The Model, or null if it does not exist.</returns>
        public T Get(long id)
        {
            T model = null;
            if (_cache.ContainsKey(id))
            {
                _cache[id].TryGetTarget(out model);
            }
            return model;
        }

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="model">The item to add.</param>
        public void Put(T model)
        {
            _cache[model.ID] = new WeakReference<T>(model);
        }

        /// <summary>
        /// Notifies a Model that its data has changed if it exists in the cache.
        /// </summary>
        /// <param name="id">The primary ID of the Model.</param>
        public void Changed(long id)
        {
            var model = Get(id);
            if (model != null)
            {
                model.Changed();
            }
        }
    }
}

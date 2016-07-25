/*
  The MIT License (MIT)
  Copyright © 2016 Steve Guidetti

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the “Software”), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
*/
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

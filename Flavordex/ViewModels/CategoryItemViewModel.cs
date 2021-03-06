﻿/*
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
using Flavordex.Models;
using Flavordex.Utilities;
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Category in a list.
    /// </summary>
    public class CategoryItemViewModel : ModelViewModel<Category>
    {
        /// <summary>
        /// The format string for the entry count string.
        /// </summary>
        private static readonly string _countFormat =
            ResourceLoader.GetForCurrentView().GetString("EntryCount");

        /// <summary>
        /// Gets the name of the Category.
        /// </summary>
        public string Name
        {
            get
            {
                return Presets.GetRealCategoryName(Model.Name);
            }
        }

        /// <summary>
        /// Gets a formatted string representing the number of journal entries in the Category.
        /// </summary>
        public string EntryCount
        {
            get
            {
                var entries = Plurals.GetWord("Entries", Model.EntryCount);
                return string.Format(_countFormat, Model.EntryCount, entries);
            }
        }

        /// <summary>
        /// Gets whether this is a preset Category.
        /// </summary>
        public bool IsPreset
        {
            get
            {
                return Model.IsPreset;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category">The Category to represent.</param>
        public CategoryItemViewModel(Category category) : base(category) { }
    }
}

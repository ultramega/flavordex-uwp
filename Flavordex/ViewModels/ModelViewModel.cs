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
using Flavordex.Utilities.Databases;
using System;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// The base class for all ViewModels with a backing Model.
    /// </summary>
    /// <typeparam name="T">The Type of Model to back this ViewModel implementation.</typeparam>
    public abstract class ModelViewModel<T> : ViewModel where T : Model
    {
        /// <summary>
        /// Gets the Model backing this ViewModel.
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The Model to back this ViewModel.</param>
        public ModelViewModel(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Model = model;
            Model.RecordChanged += OnRecordChanged;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ModelViewModel()
        {
            Model.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Raises the PropertyChanged for all properties when the Model changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRecordChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(null);
        }
    }
}

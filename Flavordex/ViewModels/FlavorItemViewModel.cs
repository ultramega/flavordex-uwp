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
using Flavordex.Models;
using Flavordex.UI;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Flavor in a RadarControl.
    /// </summary>
    public class FlavorItemViewModel : ModelViewModel<Flavor>, RadarItem
    {
        /// <summary>
        /// Gets or sets the name of the Flavor.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the Flavor.
        /// </summary>
        public long Value
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Whether the Flavor has been deleted.
        /// </summary>
        private bool _isDeleted;

        /// <summary>
        /// Gets or sets whether the Flavor has been deleted.
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="flavor">The Flavor to represent.</param>
        public FlavorItemViewModel(Flavor flavor) : base(flavor) { }
    }
}

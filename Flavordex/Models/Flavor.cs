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
using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a flavor.
    /// </summary>
    public class Flavor : Model
    {
        /// <summary>
        /// The primary ID of the category this flavor is associated with.
        /// </summary>
        public long CategoryID
        {
            get { return _data.GetLong(Tables.Flavors.CAT); }
            set { _data.SetLong(Tables.Flavors.CAT, value); }
        }

        /// <summary>
        /// The name of the flavor.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Flavors.NAME); }
            set { _data.SetString(Tables.Flavors.NAME, value); }
        }

        /// <summary>
        /// The sorting position in the list of flavors.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.Flavors.POS); }
            set { _data.SetLong(Tables.Flavors.POS, value); }
        }
    }
}

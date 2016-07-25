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
using System.IO;

namespace Flavordex.Utilities.CSV
{
    /// <summary>
    /// Simple reader for CSV files.
    /// </summary>
    public class CsvReader : IDisposable
    {
        /// <summary>
        /// Whether this disposable object has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The TextReader representing the CSV file.
        /// </summary>
        private TextReader _reader;

        /// <summary>
        /// The field values of the current row.
        /// </summary>
        private string[] _currentRow;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The TextReader representing the CSV file.</param>
        public CsvReader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
        }

        /// <summary>
        /// Reads the next row.
        /// </summary>
        /// <returns>Whether a row was successfully read.</returns>
        public bool Read()
        {
            var fields = new List<string>();

            var field = "";
            var useQuotes = false;
            var inValue = false;
            char character;
            while (true)
            {
                character = (char)_reader.Read();
                if (character == '\uffff')
                {
                    break;
                }

                if (!inValue)
                {
                    if (character == '\r' || character == '\n')
                    {
                        while (_reader.Peek() == '\r' || _reader.Peek() == '\n')
                        {
                            _reader.Read();
                        }
                        break;
                    }

                    inValue = true;

                    if (character == '"')
                    {
                        useQuotes = true;
                        continue;
                    }
                    else
                    {
                        useQuotes = false;
                    }
                }

                if (character == '"' && _reader.Peek() == '"')
                {
                    field += (char)_reader.Read();
                    continue;
                }

                if ((!useQuotes && (character == ',' || character == '\r' || character == '\n'))
                    || (useQuotes && character == '"'))
                {
                    inValue = false;
                    fields.Add(field);
                    field = "";
                    if (useQuotes && _reader.Peek() == ',')
                    {
                        _reader.Read();
                    }
                    continue;
                }

                field += character;
            }

            _currentRow = fields.ToArray();
            return _currentRow.Length > 0;
        }

        /// <summary>
        /// Gets the current row as a record object.
        /// </summary>
        /// <returns>The data from the row.</returns>
        public string[] GetRecord()
        {
            return _currentRow;
        }

        /// <summary>
        /// Disposes this disposable object.
        /// </summary>
        /// <param name="disposing">Whether to clean up managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _reader.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes this disposable object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}

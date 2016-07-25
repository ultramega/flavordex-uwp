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
    /// Simple writer for CSV files.
    /// </summary>
    public class CsvWriter : IDisposable
    {
        /// <summary>
        /// Whether this disposable object has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The TextWriter representing the CSV file.
        /// </summary>
        private TextWriter _writer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">The TextWriter representing the CSV file.</param>
        public CsvWriter(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            _writer = writer;
        }

        /// <summary>
        /// Writes a row to the CSV file.
        /// </summary>
        /// <param name="record">The record to write.</param>
        public void WriteRecord(string[] record)
        {
            var fields = new List<string>();
            foreach (var field in record)
            {
                fields.Add(PrepareValue(field));
            }

            _writer.WriteLine(string.Join(",", fields));
        }

        /// <summary>
        /// Quotes and escapes a value to be placed in a field.
        /// </summary>
        /// <param name="value">The value to quote and escape.</param>
        /// <returns>The value as a quoted and escaped string.</returns>
        private static string PrepareValue(object value)
        {
            return '"' + value.ToString().Replace("\"", "\"\"") + '"';
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
                    _writer.Dispose();
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

﻿using System;
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

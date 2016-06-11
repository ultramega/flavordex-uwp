using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        /// The column headers.
        /// </summary>
        private string[] _headers;

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
            ReadHeaders();
        }

        /// <summary>
        /// Reads the header row.
        /// </summary>
        private void ReadHeaders()
        {
            Read();
            _headers = _currentRow;
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

                if ((!useQuotes && (character == ',' || character == '\r' || character == '\n')) || (useQuotes && character == '"'))
                {
                    inValue = false;
                    fields.Add(field);
                    field = "";
                    if (useQuotes)
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
        /// <typeparam name="T">The Type of object to hold the data.</typeparam>
        /// <returns>An instance of the given Type populated with data from the row.</returns>
        public T GetRecord<T>()
        {
            T record = Activator.CreateInstance<T>();

            for (var i = 0; i < _headers.Length; i++)
            {
                SetField(record, _headers[i], _currentRow[i]);
            }

            return record;
        }

        /// <summary>
        /// Sets the value of a property of the given object identified by name.
        /// </summary>
        /// <param name="record">The object with the property to set.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the value to.</param>
        private static void SetField(object record, string name, string value)
        {
            var property = record.GetType().GetTypeInfo().GetDeclaredProperty(name);
            if (property != null)
            {
                object parsed = null;
                switch (property.PropertyType.Name)
                {
                    case "Boolean":
                        bool parsedBoolean;
                        bool.TryParse(value, out parsedBoolean);
                        parsed = parsedBoolean;
                        break;
                    case "Byte":
                        byte parsedByte;
                        byte.TryParse(value, out parsedByte);
                        parsed = parsedByte;
                        break;
                    case "SByte":
                        sbyte parsedsSByte;
                        sbyte.TryParse(value, out parsedsSByte);
                        parsed = parsedsSByte;
                        break;
                    case "Char":
                        char parsedChar;
                        char.TryParse(value, out parsedChar);
                        parsed = parsedChar;
                        break;
                    case "Decimal":
                        decimal parsedDecimal;
                        decimal.TryParse(value, out parsedDecimal);
                        parsed = parsedDecimal;
                        break;
                    case "Double":
                        double parsedDouble;
                        double.TryParse(value, out parsedDouble);
                        parsed = parsedDouble;
                        break;
                    case "Single":
                        float parsedSingle;
                        float.TryParse(value, out parsedSingle);
                        parsed = parsedSingle;
                        break;
                    case "Int32":
                        int parsedInt32;
                        int.TryParse(value, out parsedInt32);
                        parsed = parsedInt32;
                        break;
                    case "UInt32":
                        uint parsedUInt32;
                        uint.TryParse(value, out parsedUInt32);
                        parsed = parsedUInt32;
                        break;
                    case "Int64":
                        long parsedInt64;
                        long.TryParse(value, out parsedInt64);
                        parsed = parsedInt64;
                        break;
                    case "UInt64":
                        ulong parsedUInt64;
                        ulong.TryParse(value, out parsedUInt64);
                        parsed = parsedUInt64;
                        break;
                    case "Int16":
                        short parsedInt16;
                        short.TryParse(value, out parsedInt16);
                        parsed = parsedInt16;
                        break;
                    case "UInt16":
                        ushort parsedUInt16;
                        ushort.TryParse(value, out parsedUInt16);
                        parsed = parsedUInt16;
                        break;
                    case "String":
                    case "Object":
                        parsed = value;
                        break;
                }

                property.SetValue(record, parsed);
            }
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

using System.Data;
using System.Globalization;
using System.Resources;

namespace Tellurian.Utilities.Data;

/// <summary>
/// Provides extension methods for the IDataRecord interface to simplify and standardize the retrieval and conversion of
/// column values from data records.
/// </summary>
/// <remarks>These extension methods offer convenient ways to access and convert column values by name or index,
/// handle default values, and manage common data type conversions, also for the types Microsoft Access gives.
/// A default value is used when the column is missing or its value is null; without one, a missing column or a null value
/// throws <see cref="InvalidOperationException"/>, as does a value that cannot be converted to the requested type.</remarks>
public static class IDataRecordExtensions
{
    extension(IDataRecord record)
    {
        /// <summary>Gets the string in the column, an empty string for an empty value, or the default for a null value.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value; without one, a missing column throws and
        /// a null value gives an empty string.</param>
        /// <exception cref="InvalidOperationException">The column is missing without a default, or its value is not a string.</exception>
        public string GetString(int columnIndex, string? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw MissingValueException(record, columnIndex);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? string.Empty;
            try
            {
                return record.GetString(columnIndex) ?? string.Empty;
            }
            catch (InvalidCastException exception)
            {
                throw new InvalidOperationException($"Column {record.GetName(columnIndex)} has unsupported value type {record.GetFieldType(columnIndex).Name}.", exception);
            }
        }

        /// <summary>Gets the string in the named column, an empty string for an empty value, or the default for a null value.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value; without one, a missing column throws and
        /// a null value gives an empty string.</param>
        /// <exception cref="InvalidOperationException">The column is missing without a default, or its value is not a string.</exception>
        public string GetString(string columnName, string? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetString(columnIndex, defaultValue);
        }

        /// <summary>
        /// Gets the text of the resource whose key is in the named column, in the current culture; the key itself when the
        /// resource is missing, and <paramref name="defaultValue"/> when the column is empty or missing.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="resourceManager">The resources to look the key up in.</param>
        /// <param name="defaultValue">The value for an empty or missing column.</param>
        public string GetStringResource(string columnName, ResourceManager resourceManager, string defaultValue = "")
        {
            ArgumentNullException.ThrowIfNull(resourceManager);
            var resourceKey = record.GetString(columnName, defaultValue);
            if (resourceKey.HasValue)
            {
                var resourceValue = resourceManager.GetString(resourceKey, CultureInfo.CurrentCulture);
                return resourceValue.HasValue ? resourceValue : resourceKey;
            }
            return defaultValue;
        }

        /// <summary>Gets the value as a byte, at most <paramref name="maxValue"/>; zero for a missing column or a null value.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="maxValue">The greatest value returned; a greater value is capped.</param>
        /// <exception cref="InvalidOperationException">The value is not a byte, an int or a double.</exception>
        public byte GetByte(int columnIndex, byte maxValue = 255)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return 0;
            var value = record.GetValue(columnIndex);
            return value switch
            {
                byte a => a < maxValue ? a : maxValue,
                int c => (byte)(c < maxValue ? c : maxValue),
                double b => (byte)(b < maxValue ? b : maxValue),
                _ => throw UnsupportedTypeException(value, record.GetName(columnIndex)),
            };
        }

        /// <summary>Gets the value in the named column as a byte, at most <paramref name="maxValue"/>; zero for a null value.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="maxValue">The greatest value returned; a greater value is capped.</param>
        /// <exception cref="InvalidOperationException">The column is missing, or its value is not a byte, an int or a double.</exception>
        public byte GetByte(string columnName, byte maxValue = 255)
        {
            var columnIndex = record.GetColumIndex(columnName);
            return record.GetByte(columnIndex, maxValue);
        }

        /// <summary>Gets the value as an int, converting a short.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value is not an int or a short.</exception>
        public int GetInt(int columnIndex, int? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw MissingValueException(record, columnIndex);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            var value = record.GetValue(columnIndex);
            return value switch
            {
                int b => b,
                short a => a,
                _ => throw UnsupportedTypeException(value, record.GetName(columnIndex)),
            };
        }

        /// <summary>Gets the value in the named column as an int, converting a short.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value is not an int or a short.</exception>
        public int GetInt(string columnName, int? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, !defaultValue.HasValue);
            return record.GetInt(columnIndex, defaultValue);
        }

        /// <summary>Gets the value as an int, converting a short, or <paramref name="defaultValue"/> for a missing column or a null value.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The value is not an int or a short.</exception>
        public int? GetIntOrNull(int columnIndex, int? defaultValue = null)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return defaultValue;
            var value = record.GetValue(columnIndex);
            return value switch
            {
                int b => b,
                short a => a,
                _ => throw UnsupportedTypeException(value, record.GetName(columnIndex)),
            };
        }

        /// <summary>Gets the value in the named column as an int, converting a short, or <paramref name="defaultValue"/> for a null value.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value; without one, a missing column throws.</param>
        /// <exception cref="InvalidOperationException">The column is missing without a default, or its value is not an int or a short.</exception>
        public int? GetIntOrNull(string columnName, int? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetIntOrNull(columnIndex, defaultValue);
        }

        /// <summary>Gets the value as a double, converting a float.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value is not a double or a float.</exception>
        public double GetDouble(int columnIndex, double? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw MissingValueException(record, columnIndex);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            var value = record.GetValue(columnIndex);
            return value switch
            {
                double b => b,
                float a => a,
                _ => throw UnsupportedTypeException(value, record.GetName(columnIndex)),
            };
        }

        /// <summary>Gets the value in the named column as a double, converting a float.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value is not a double or a float.</exception>
        public double GetDouble(string columnName, double? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetDouble(columnIndex, defaultValue);
        }

        /// <summary>Gets the date of a date and time value.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default.</exception>
        public DateOnly GetDate(int columnIndex, DateOnly? defaultValue = null)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            return record.GetDateTime(columnIndex).DateOnly;
        }

        /// <summary>Gets the date of a date and time value in the named column.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default.</exception>
        public DateOnly GetDate(string columnName, DateOnly? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetDate(columnIndex, defaultValue);
        }

        /// <summary>Gets the time of day of a date and time value.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default.</exception>
        public TimeOnly GetTime(int columnIndex, TimeOnly? defaultValue = null)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            return record.GetDateTime(columnIndex).TimeOnly;
        }

        /// <summary>Gets the time of day of a date and time value in the named column.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default.</exception>
        public TimeOnly GetTime(string columnName, TimeOnly? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetTime(columnIndex, defaultValue);
        }

        /// <summary>Gets the time of day of a date and time value; midnight for a null value.</summary>
        /// <param name="columnIndex">The zero-based column index.</param>
        /// <exception cref="InvalidOperationException">The column is missing, or its value is not a date and time.</exception>
        public TimeOnly GetTimeAsTimespan(int columnIndex)
        {
            if (columnIndex < 0) throw MissingValueException(record, columnIndex);
            if (record.IsDBNull(columnIndex)) return TimeOnly.MinValue;
            var value = record.GetValue(columnIndex);
            return value is DateTime d ? d.TimeOnly : throw UnsupportedTypeException(value, record.GetName(columnIndex));
        }

        /// <summary>Gets the time of day of a date and time value in the named column; midnight for a null value.</summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="InvalidOperationException">The column is missing, or its value is not a date and time.</exception>
        public TimeOnly GetTimeAsTimespan(string columnName)
        {
            var columnIndex = record.GetColumIndex(columnName);
            return record.GetTimeAsTimespan(columnIndex);
        }

        /// <summary>Gets a date and time value as its day of the year plus the time of day as a fraction of a day (see <c>ToDouble</c>).</summary>
        /// <param name="columnIndex">The zero-based column index.</param>
        /// <param name="defaultValue">The value for a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing, or null without a default.</exception>
        public double GetTimeAsDouble(int columnIndex, double? defaultValue = null)
        {
            if (columnIndex < 0) throw MissingValueException(record, columnIndex);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            return record.GetDateTime(columnIndex).ToDouble;
        }

        /// <summary>Gets a date and time value in the named column as its day of the year plus the time of day as a fraction of a day.</summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null.</exception>
        public double GetTimeAsDouble(string columnName) => record.GetTimeAsDouble(columnName, null);

        /// <summary>Gets a date and time value in the named column as its day of the year plus the time of day as a fraction of a day.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column or a null value.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default.</exception>
        public double GetTimeAsDouble(string columnName, double? defaultValue)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return columnIndex < 0 && defaultValue.HasValue ? defaultValue.Value : record.GetTimeAsDouble(columnIndex, defaultValue);
        }

        /// <summary>Gets the value as a bool; a number is true when it is not zero.</summary>
        /// <param name="columnIndex">The zero-based column index; a negative index means the column is missing.</param>
        /// <param name="defaultValue">The value for a missing column, a null value or a value of another type.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value has another type.</exception>
        public bool GetBool(int columnIndex, bool? defaultValue = null)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return defaultValue ?? throw MissingValueException(record, columnIndex);
            var value = record.GetValue(columnIndex);
            return value switch
            {
                bool a => a,
                short b => b != 0,
                int c => c != 0,
                double d => d != 0,
                _ => defaultValue ?? throw UnsupportedTypeException(value, record.GetName(columnIndex)),
            };
        }

        /// <summary>Gets the value in the named column as a bool; a number is true when it is not zero.</summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultValue">The value for a missing column, a null value or a value of another type.</param>
        /// <exception cref="InvalidOperationException">The column is missing or null without a default, or its value has another type.</exception>
        public bool GetBool(string columnName, bool? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetBool(columnIndex, defaultValue);
        }

        /// <summary>Determines whether the value in the named column is null.</summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="InvalidOperationException">The column is missing.</exception>
        public bool IsDBNull(string columnName) => record.IsDBNull(record.GetColumIndex(columnName));

        /// <summary>Gets the ordinal of each named column.</summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns>The ordinal of each column, by name.</returns>
        /// <exception cref="InvalidOperationException">A column is missing.</exception>
        public IReadOnlyDictionary<string, int> GetColumnIndexes(string[] columnNames)
        {
            ArgumentNullException.ThrowIfNull(columnNames);
            var result = new Dictionary<string, int>(columnNames.Length);
            foreach (var columnName in columnNames)
            {
                result[columnName] = record.GetColumIndex(columnName);
            }
            return result;
        }

        // IDataRecord.GetOrdinal throws IndexOutOfRangeException for an unknown name; DataTableReader throws ArgumentException.
        private int GetColumIndex(string columnName, bool throwOnNotFound = true)
        {
            try
            {
                return record.GetOrdinal(columnName);
            }
            catch (Exception exception) when (exception is IndexOutOfRangeException or ArgumentException)
            {
                if (throwOnNotFound) throw new InvalidOperationException($"Column {columnName} was not found in data record.", exception);
                return -1;
            }
        }
    }

    private static InvalidOperationException MissingValueException(IDataRecord record, int columnIndex) =>
        columnIndex < 0
            ? new InvalidOperationException("The column is missing and has no default value.")
            : new InvalidOperationException($"Column {record.GetName(columnIndex)} is null and has no default value.");

    private static InvalidOperationException UnsupportedTypeException(object value, string column) =>
        new($"Column {column} has unsupported value type {value.GetType().Name}.");
}

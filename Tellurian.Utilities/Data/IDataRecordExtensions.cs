using System.Data;
using System.Globalization;
using System.Resources;

namespace Tellurian.Utilities.Data;

/// <summary>
/// Provides extension methods for the IDataRecord interface to simplify and standardize the retrieval and conversion of
/// column values from data records.
/// </summary>
/// <remarks>These extension methods offer convenient ways to access and convert column values by name or index,
/// handle default values, and manage common data type conversions. They are designed to reduce boilerplate code and
/// improve the safety and clarity of data access operations when working with IDataRecord implementations, such as
/// those returned by data readers. Methods in this class may throw exceptions if columns are missing or if values
/// cannot be converted to the requested type. Callers should ensure that column names and types are valid for the
/// underlying data source.</remarks>
public static class IDataRecordExtensions
{
    extension(IDataRecord record)
    {
        public string GetString(int columnIndex, string? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, "NoColumn");

            if (record.IsDBNull(columnIndex)) return string.Empty;
            try
            {
                var s = record.GetString(columnIndex);
                if (string.IsNullOrEmpty(s)) return string.Empty;
                return s;
            }
            catch (Exception)
            {
                throw TypeErrorException(defaultValue, record.GetName(columnIndex));
            }
        }

        public string GetString(string columnName, string? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetString(columnIndex, defaultValue);
        }

        public string GetStringResource(string columnName, ResourceManager resourceManager, string defaultValue = "")
        {
            var resourceKey = record.GetString(columnName, defaultValue);
            if (resourceKey.HasValue)
            {
                var resourceValue = resourceManager.GetString(resourceKey, CultureInfo.CurrentCulture);
                if (resourceValue.HasValue) return resourceValue;
                return resourceKey;
            }
            return defaultValue;
        }

        public byte GetByte(int columnIndex, byte maxValue = 255)
        {
            if (columnIndex < 0 || record.IsDBNull(columnIndex)) return 0;
            var value = record.GetValue(columnIndex);
            if (value is byte a) return a < maxValue ? a : maxValue;
            if (value is int c) return (byte)(c < maxValue ? c : maxValue);
            if (value is double b) return (byte)(b < maxValue ? b : maxValue);
            throw TypeErrorException(value, record.GetName(columnIndex));
        }

        public byte GetByte(string columnName, byte maxValue = 255)
        {
            var columnIndex = record.GetColumIndex(columnName);
            return record.GetByte(columnIndex, maxValue);
        }

        public int GetInt(int columnIndex, int? defaultValue = null)
        {
            var columnName = record.GetName(columnIndex);
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, columnName);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(defaultValue, columnName);
            var value = record.GetValue(columnIndex);
            if (value is int b) return b;
            if (value is short a) return a;
            throw TypeErrorException(value, columnName);
        }

        public int GetInt(string columnName, int? defaultValue = null)
        {
            var i = record.GetColumIndex(columnName, !defaultValue.HasValue);
            return record.GetInt(i, defaultValue);
        }

        public int? GetIntOrNull(int columnIndex, int? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue;
            if (record.IsDBNull(columnIndex)) return defaultValue;
            var value = record.GetValue(columnIndex);
            if (value is int b) return b;
            if (value is short a) return a;
            throw TypeErrorException(value, record.GetName(columnIndex));
        }

        public int? GetIntOrNull(string columnName, int? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetIntOrNull(columnIndex, defaultValue);
        }

        public double GetDouble(int columnIndex, double? defaultValue = null)
        {
            var columnName = record.GetName(columnIndex);
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, columnName);
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(defaultValue, columnName);
            var value = record.GetValue(columnIndex);
            if (value is double b) return b;
            if (value is float a) return a;
            throw TypeErrorException(value, columnName);
        }

        public double GetDouble(string columnName, double? defaultValue = null)
        {
            var colimnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetDouble(colimnIndex, defaultValue);
        }

        public DateOnly GetDate(int columnIndex, DateOnly? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, record.GetName(columnIndex));
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(defaultValue, record.GetName(columnIndex));
            return record.GetDateTime(columnIndex).DateOnly;
        }

        public DateOnly GetDate(string columnName, DateOnly? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName);
            return record.GetDate(columnIndex, defaultValue);
        }

        public TimeOnly GetTime(int columnIndex, TimeOnly? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, record.GetName(columnIndex));
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(null, record.GetName(columnIndex));
            return record.GetDateTime(columnIndex).TimeOnly;
        }

        public TimeOnly GetTime(string columnName, TimeOnly? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetTime(columnIndex, defaultValue);
        }

        public TimeOnly GetTimeAsTimespan(int columnIndex)
        {
            if (columnIndex < 0) throw TypeErrorException(null, record.GetName(columnIndex));
            if (record.IsDBNull(columnIndex)) return TimeOnly.MinValue;
            var value = record.GetValue(columnIndex);
            if (value is DateTime d) return d.TimeOnly;
            throw TypeErrorException(value, record.GetName(columnIndex));
        }

        public TimeOnly GetTimeAsTimespan(string columnName)
        {
            var columnIndex = record.GetColumIndex(columnName);
            return record.GetTimeAsTimespan(columnIndex);
        }

        public double GetTimeAsDouble(int columnIndex, double? defaultValue = null)
        {
            if (columnIndex < 0) throw TypeErrorException(null, record.GetName(columnIndex));
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(null, record.GetName(columnIndex));
            var t = record.GetDateTime(columnIndex);
            return t.ToDouble;
        }

        public double GetTimeAsDouble(string columnName)
        {
            var columnIndex = record.GetOrdinal(columnName);
            return record.GetTimeAsDouble(columnIndex);
        }

        public bool GetBool(int columnIndex, bool? defaultValue = null)
        {
            if (columnIndex < 0) return defaultValue ?? throw TypeErrorException(defaultValue, record.GetName(columnIndex));
            if (record.IsDBNull(columnIndex)) return defaultValue ?? throw TypeErrorException(null, record.GetName(columnIndex));
            var value = record.GetValue(columnIndex);
            if (value is bool a) return a;
            else if (value is short b) return b != 0;
            else if (value is int c) return c != 0;
            else if (value is double d) return d != 0;
            else if (defaultValue.HasValue) return defaultValue.Value;
            throw TypeErrorException(value, record.GetName(columnIndex));
        }

        public bool GetBool(string columnName, bool? defaultValue = null)
        {
            var columnIndex = record.GetColumIndex(columnName, defaultValue is null);
            return record.GetBool(columnIndex, defaultValue);
        }

        public bool IsDBNull(string columnName)
        {
            var i = record.GetOrdinal(columnName);
            return record.IsDBNull(i);
        }

        private int GetColumIndex(string columnName, bool throwOnNotFound = true)
        {
            int i = -1;
            try { i = record.GetOrdinal(columnName); }
            catch (IndexOutOfRangeException)
            {
                if (throwOnNotFound) throw new InvalidOperationException($"Column {columnName} was not found in data record.");
            }
            return i;
        }

        public Dictionary<string, int> GetColumnIndexes(string[] columnNames)
        {
            var result = new Dictionary<string, int>(columnNames.Length);
            foreach (var columnName in columnNames)
            {
                var i = record.GetColumIndex(columnName, true);
                result[columnName] = i;
            }
            return result;
        }
    }


    private static InvalidOperationException TypeErrorException(object? value, string column)
    {
        if (value is null) return new InvalidOperationException($"Column {column} is null and have no default value.");
        var type = value.GetType();
        return new InvalidOperationException($"Column {column} has unsupported value type {type.Name}.");
    }
}
/// <summary>
/// Provides a mapping between column names and their ordinal positions within a data record.
/// </summary>
/// <remarks>Use this class to efficiently retrieve the ordinal position of specified columns by name when working
/// with data records, such as those returned from a data reader. This can help avoid repeated lookups and improve code
/// clarity when accessing column values by index.</remarks>
/// <param name="record">The data record containing the columns to map. Must not be null.</param>
/// <param name="columns">An array of column names to include in the mapping. Each name must correspond to a column in the data record.</param>
/// <exception cref="ArgumentOutOfRangeException">Throw if any of the <paramref name="columns"/> is not found in the <paramref name="record"/></exception>
public class Columns(IDataRecord record, params string[] columns)
{
    private readonly Dictionary<string, int> _columns = record.GetColumnIndexes(columns);
    public int Ordinal(string columnName) { return _columns[columnName]; }
}

using System.Data;

namespace Tellurian.Utilities.Data;

/// <summary>
/// Provides a mapping between column names and their ordinal positions within a data record.
/// </summary>
/// <remarks>Use this class to efficiently retrieve the ordinal position of specified columns by name when working
/// with data records, such as those returned from a data reader. This can help avoid repeated lookups and improve code
/// clarity when accessing column values by index.</remarks>
/// <param name="record">The data record containing the columns to map. Must not be null.</param>
/// <param name="columns">An array of column names to include in the mapping. Each name must correspond to a column in the data record.</param>
/// <exception cref="InvalidOperationException">Thrown if any of the <paramref name="columns"/> is not found in the <paramref name="record"/>.</exception>
public sealed class Columns(IDataRecord record, params string[] columns)
{
    private readonly IReadOnlyDictionary<string, int> _columns = record.GetColumnIndexes(columns);

    /// <summary>Gets the ordinal of a column given when this mapping was created.</summary>
    /// <param name="columnName">The column name.</param>
    /// <exception cref="KeyNotFoundException">The column was not given when this mapping was created.</exception>
    public int Ordinal(string columnName) => _columns[columnName];
}

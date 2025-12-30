using System.Data;

namespace Tellurian.Utilities.Data;

public static class DataSetExtensions
{
    extension(DataRow row)
    {
        /// <summary>
        /// Returns an array of string representations of all fields in the current row.
        /// </summary>
        /// <returns>An array of strings containing the values of each field in the row. If a field is null, its value in the
        /// array will be an empty string. If the row contains no fields, returns an empty array.</returns>
        public string[] GetRowFields()
        {
            var items = row.ItemArray;
            if (items is null) return [];
            return items.Select(i => i is null ? string.Empty : i.ToString()).ToArray()!;
        }

        /// <summary>
        /// Determines whether all fields in the current row are empty.
        /// </summary>
        /// <returns>true if every field in the row is empty; otherwise, false.</returns>
        public bool IsBlankRow() =>
            row.GetRowFields().AreAllEmpty;

        /// <summary>
        /// Retrieves the background color value expected to be stored in a specific column when reading data.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column whose background color is to be retrieved. If <paramref
        /// name="columnIndex"/> is <see langword="null"/>, or if the column does not exist, the method returns <see
        /// langword="null"/>.</param>
        /// <returns>A string containing the background color value for the specified column, or <see langword="null"/> if the
        /// column index is <see langword="null"/> or the column does not exist.</returns>
        public string? BackgroundColor(int? columnIndex) =>
            columnIndex.HasValue && row.Table.Columns.Contains($"Column{columnIndex}")
                ? row[columnIndex.Value] as string
                : null;
    }
}

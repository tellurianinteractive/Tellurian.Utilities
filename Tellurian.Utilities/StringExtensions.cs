using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tellurian.Utilities;

public static partial class StringExtensions
{
    [GeneratedRegex("<.*?>")]
    private static partial Regex Html();

    extension([NotNullWhen(true)] string? value)
    {
        /// <summary>
        /// Gets a value indicating whether the current instance contains a non-empty value.
        /// </summary>
        public bool HasValue => !value.IsEmpty;

        /// <summary>
        /// Returns the current value if it is not empty; otherwise, returns the specified alternative value.
        /// </summary>
        /// <param name="orElseValue">The value to return if the current value is empty. Can be null or any string.</param>
        /// <returns>The current value if it is not empty; otherwise, the value of <paramref name="orElseValue"/>.</returns>
        public string OrElse(string orElseValue) => value.IsEmpty ? orElseValue : value;

        /// <summary>
        /// Gets the value of the current string with all HTML tags and non-breaking space entities removed.
        /// </summary>
        public string WithHtmlRemoved =>
            value is null ? string.Empty :
            Html().Replace(value, string.Empty).Replace("&nbsp", " ").Replace(';', ' ');

        /// <summary>
        /// Determines whether the current value matches any of the specified strings, using a case-insensitive
        /// comparison.
        /// </summary>
        /// <param name="values">An array of strings to compare with the current value. Can be empty.</param>
        /// <returns>true if the current value equals any of the specified strings, ignoring case; otherwise, false.</returns>
        public bool AnyOf(params string[] values) =>
            values?.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Determines whether any of the specified comma-separated values are present in the current collection.
        /// </summary>
        /// <param name="commaSeparatedValues">A string containing one or more values separated by commas. Entries are trimmed of whitespace and empty
        /// entries are ignored. Can be null.</param>
        /// <returns>true if at least one of the specified values exists in the collection; otherwise, false.</returns>
        public bool AnyOf(string? commaSeparatedValues) =>
            commaSeparatedValues is not null &&
            value.AnyOf(commaSeparatedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        /// <summary>
        /// Determines whether any of the specified strings occur within the current value, using a case-insensitive
        /// comparison.
        /// </summary>
        /// <remarks>The comparison is performed using ordinal, case-insensitive matching. If the current
        /// value is not set, the method returns false.</remarks>
        /// <param name="values">An array of strings to search for within the current value. Cannot be null or empty.</param>
        /// <returns>true if at least one of the specified strings is found within the current value; otherwise, false.</returns>
        public bool AnyPartOf(params string[] values) =>
            values?.Length > 0 && value.HasValue && values.Any(v => value.Contains(v, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Determines whether any of the specified comma-separated values are present in the current value.
        /// </summary>
        /// <remarks>Empty or whitespace-only entries in the input are ignored. Comparison behavior
        /// depends on the implementation of the underlying value's AnyPartOf method.</remarks>
        /// <param name="values">A comma-separated list of values to check for presence. Entries are trimmed of whitespace. Can be null or
        /// empty.</param>
        /// <returns>true if at least one of the specified values is present in the current value; otherwise, false.</returns>
        public bool AnyPartOf(string? values) =>
           value.AnyPartOf(values?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []);

        /// <summary>
        /// Splits the current string value into an array of items using the specified separator character.
        /// </summary>
        /// <remarks>Empty entries and leading or trailing white-space in each item are removed from the
        /// result.</remarks>
        /// <param name="separator">The character to use as the delimiter for splitting the string. The default value is ';'.</param>
        /// <returns>An array of strings containing the separated items. Returns an empty array if the value is null, empty, or
        /// consists only of white-space characters.</returns>
        public string[] ToItems(char separator = ';') =>
            value.IsEmpty ? [] :
            value.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    }

    extension([NotNullWhen(false)] string? value)
    {
        public bool IsEmpty => string.IsNullOrWhiteSpace(value);
    }
}

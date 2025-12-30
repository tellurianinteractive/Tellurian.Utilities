using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        /// Determines whether the current value matches the specified string, using a case-insensitive comparison.
        /// </summary>
        /// <param name="expectedValue">The string to compare with the current value. If <paramref name="expectedValue"/> is <see langword="null"/>,
        /// the method returns <see langword="false"/>.</param>
        /// <returns>true if the current value equals <paramref name="expectedValue"/> when compared using ordinal,
        /// case-insensitive rules; otherwise, false.</returns>
        public bool Is(string? expectedValue) =>
                       value is not null && expectedValue is not null &&
                       value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the current string represents a valid number in either integer or
        /// floating-point format.
        /// </summary>
        /// <remarks>The property returns <see langword="true"/> if the string can be parsed as a 64-bit
        /// signed integer or a floating-point number using the invariant culture. Otherwise, it returns <see
        /// langword="false"/>.</remarks>
        public bool IsNumber =>
            long.TryParse(value, out var _) ||
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var _);

        /// <summary>
        /// Gets a value indicating whether the current value is either empty or represents a number.
        /// </summary>
        public bool IsNumberOrEmpty => value.IsEmpty || value.IsNumber;

        /// <summary>
        /// Gets a value indicating whether the current value is either empty or consists only of zero ('0') characters.
        /// </summary>
        public bool IsZeroesOrEmpty =>
                value.IsEmpty || value.All(c => c == '0');

        /// <summary>
        /// Returns the current value if it is not empty; otherwise, returns the specified alternative value.
        /// </summary>
        /// <param name="orElseValue">The value to return if the current value is empty. Can be null or any string.</param>
        /// <returns>The current value if it is not empty; otherwise, the value of <paramref name="orElseValue"/>.</returns>
        public string OrElse(string orElseValue) => value.IsEmpty ? orElseValue : value;

        /// <summary>
        /// Determines whether all characters in the current value are contained in the specified array of characters.
        /// </summary>
        /// <param name="chars">An array of characters to test for inclusion. Each character in the current value is checked against this
        /// array.</param>
        /// <returns>true if every character in the current value exists in the chars array; otherwise, false. Returns false if
        /// the current value is null.</returns>
        public bool IsAllOf(char[] chars) => value?.All(c => chars.Contains(c)) == true;

        /// <summary>
        /// Gets the value of the current string with all HTML tags and non-breaking space entities removed.
        /// </summary>
        public string WithHtmlRemoved =>
            value is null ? string.Empty :
            Html().Replace(value, string.Empty).Replace("&nbsp;", " ").Replace("&nbsp", " ");

        /// <summary>
        /// Determines whether the current value matches any of the specified strings, using a case-insensitive
        /// comparison.
        /// </summary>
        /// <param name="values">An array of strings to compare with the current value. Can be empty.</param>
        /// <returns>true if the current value equals any of the specified strings, ignoring case; otherwise, false.</returns>
        public bool IsAnyOf(params string[] values) =>
            values?.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Determines whether any of the specified comma-separated values are present in the current collection.
        /// </summary>
        /// <param name="commaSeparatedValues">A string containing one or more values separated by commas. Entries are trimmed of whitespace and empty
        /// entries are ignored. Can be null.</param>
        /// <returns>true if at least one of the specified values exists in the collection; otherwise, false.</returns>
        public bool IsAnyOf(string? commaSeparatedValues) =>
            commaSeparatedValues is not null &&
            value.IsAnyOf(commaSeparatedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        /// <summary>
        /// Determines whether any of the specified strings occur within the current value, using a case-insensitive
        /// comparison.
        /// </summary>
        /// <remarks>The comparison is performed using ordinal, case-insensitive matching. If the current
        /// value is not set, the method returns false.</remarks>
        /// <param name="values">An array of strings to search for within the current value. Cannot be null or empty.</param>
        /// <returns>true if at least one of the specified strings is found within the current value; otherwise, false.</returns>
        public bool IsAnyPartOf(params string[] values) =>
            values?.Length > 0 && value.HasValue && values.Any(v => value.Contains(v, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Determines whether any of the specified comma-separated values are present in the current value.
        /// </summary>
        /// <remarks>Empty or whitespace-only entries in the input are ignored. Comparison behavior
        /// depends on the implementation of the underlying value's AnyPartOf method.</remarks>
        /// <param name="values">A comma-separated list of values to check for presence. Entries are trimmed of whitespace. Can be null or
        /// empty.</param>
        /// <returns>true if at least one of the specified values is present in the current value; otherwise, false.</returns>
        public bool IsAnyPartOf(string? values) =>
           value.IsAnyPartOf(values?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []);

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

        /// <summary>
        /// Returns the current value if it is set; otherwise, returns the specified default value.
        /// </summary>
        /// <param name="defaultValue">The value to return if the current value is not set. Can be null.</param>
        /// <returns>The current value if it is set; otherwise, the specified default value.</returns>
        public string OrDefault(string defaultValue) =>
            value.HasValue ? value : defaultValue;

        /// <summary>
        /// Gets the value if it is present; otherwise, throws a NullReferenceException.
        /// </summary>
        public string OrException =>
            value.HasValue ? value : throw new NullReferenceException(nameof(value));

        /// <summary>
        /// Gets the value of the string if present; otherwise, returns an empty string.
        /// </summary>
        /// <remarks>Use this property to safely retrieve a string value without needing to check for null
        /// or missing values. This is useful when a non-null string is required for further processing or
        /// display.</remarks>
        public string OrEmpty => value.HasValue ? value : string.Empty;

        /// <summary>
        /// Returns a substring from the start of the value up to, but not including, the first occurrence of any
        /// specified stop characters. Returns an empty string if no stop character is found or if the value is null or
        /// empty.
        /// </summary>
        /// <param name="stopAt">An array of characters at which to stop extracting the substring. If empty, the method returns an empty
        /// string.</param>
        /// <returns>A substring from the start of the value up to the first occurrence of any character in <paramref
        /// name="stopAt"/>. Returns the entire value if no stop character is found. Returns an empty string if the
        /// value is null or empty, or if <paramref name="stopAt"/> is empty.</returns>
        public string UntilOrEmpty(char[] stopAt)
        {
            if (stopAt.Length > 0 && value.HasValue)
            {
                int endIndex = value.IndexOfAny(stopAt);
                if (endIndex > 0)
                {
                    return value[..endIndex];
                }
                else if (endIndex == -1)
                {
                    return value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines whether the specified string is equal to the current value, using a case-insensitive ordinal
        /// comparison.
        /// </summary>
        /// <param name="text">The string to compare with the current value. Can be null.</param>
        /// <returns>true if the specified string and the current value are equal, ignoring case; otherwise, false.
        /// Returns true if both values are null.</returns>
        public bool EqualsCaseInsensitive(string? text) =>
            (value is null && text is null) ||
            (value is not null && text is not null && value.Equals(text, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Returns the value as a single-element string array, or an empty array if the value is null.
        /// </summary>
        /// <returns>A string array containing the value if it is not null; otherwise, an empty array.</returns>
        public string[] AsArray() =>
            value is null ? [] : [value];

        /// <summary>
        /// Returns the first item from a comma-separated list, or a specified default value if the list is empty.
        /// </summary>
        /// <param name="defaultValue">The value to return if the list is empty or the first item is not found. The default is an empty string.</param>
        /// <returns>The first item in the comma-separated list, or the specified default value if the list is empty or the first
        /// item is not found.</returns>
        public string FirstItem(string defaultValue = "") =>
            value.IsEmpty ? defaultValue : value.Split(',')[0] ?? defaultValue;

        /// <summary>
        /// Gets the integer value represented by the current string, or zero if the string is not a valid integer.
        /// </summary>
        public int ToIntOrZero =>
            int.TryParse(value, out var number) ? number : 0;

        /// <summary>
        /// Gets the value of the current string as a double-precision floating-point number, or zero if the conversion
        /// fails.
        /// </summary>
        /// <remarks>The conversion uses the invariant culture and accepts standard floating-point formats.
        /// If the string cannot be parsed as a valid double, the property returns 0.0.</remarks>
        public double ToDoubleOrZero =>
                double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ? number : 0.0;

    }

    extension([NotNullWhen(false)] string? value)
    {
        /// <summary>
        /// Gets a value indicating whether the current value is null, empty, or consists only of white-space
        /// characters.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(value);
    }

    extension(IEnumerable<string> values)
    {
        /// <summary>
        /// Gets a value indicating whether all items in the collection are empty.
        /// </summary>
        public bool AreAllEmpty =>
            values.All(i => i.IsEmpty);
    }

    extension(string? filePath)
    {
        public bool HasFileExtension(params string[] extensions) =>
            filePath is not null &&
            extensions.Any(e => Path.GetExtension(filePath).Equals(e, StringComparison.OrdinalIgnoreCase));
    }
}


using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Tellurian.Utilities.Linq;

namespace Tellurian.Utilities;

public static partial class StringExtensions
{
    [GeneratedRegex("<.*?>")]
    private static partial Regex Html();

    private static readonly char[] QuotationMarks = [
        '"', '\'',              // Straight quotes (ASCII)
        '\u201C', '\u201D',     // " " Curly double quotes (English, Swedish, Finnish)
        '\u2018', '\u2019',     // ' ' Curly single quotes (English, Swedish, Finnish)
        '\u201E', '\u201A',     // „ ‚ Low-9 quotes (German, Polish, Czech, Icelandic)
        '\u00AB', '\u00BB',     // « » Guillemets (French, Italian, Spanish, Portuguese, Norwegian)
        '\u2039', '\u203A'      // ‹ › Single guillemets (Swiss French, Danish, Swedish, Finnish)
    ];

    extension([NotNullWhen(true)] string? value)
    {
        /// <summary>
        /// Gets a value indicating whether the current instance contains a non-empty value.
        /// </summary>
        public bool HasValue => !value.IsEmpty;

    }

    extension([NotNullWhen(false)] string? value)
    {
        /// <summary>
        /// Gets a value indicating whether the current value is null, empty, or consists only of white-space
        /// characters.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Gets a value indicating whether the current value is either empty or consists only of zero ('0') characters.
        /// </summary>
        public bool IsZeroesOrEmpty =>
                value.IsEmpty || value.All(c => c == '0');
    }

    extension([NotNullWhen(true)] string? value)
    {
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
        /// Determines whether all characters in the current value are contained in the specified array of characters.
        /// </summary>
        /// <param name="chars">An array of characters to test for inclusion. Each character in the current value is checked against this
        /// array.</param>
        /// <returns>true if every character in the current value exists in the chars array; otherwise, false. Returns false if
        /// the current value is null.</returns>
        public bool IsAllOf(char[] chars) => value?.All(c => chars.Contains(c)) == true;

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
        /// Determines whether the current value matches the specified string, using a case-insensitive comparison.
        /// </summary>
        /// <param name="expectedValue">The string to compare with the current value. If <paramref name="expectedValue"/> is <see langword="null"/>,
        /// the method returns <see langword="false"/>.</param>
        /// <returns>true if the current value equals <paramref name="expectedValue"/> when compared using ordinal,
        /// case-insensitive rules; otherwise, false.</returns>
        public bool IsExpected(string? expectedValue) =>
                       value is not null && expectedValue is not null &&
                       value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
    }

    extension(string? value)
    {

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
        /// Returns the current value if it is not empty; otherwise, returns the specified alternative value.
        /// </summary>
        /// <param name="orElseValue">The value to return if the current value is empty. Can be null or any string.</param>
        /// <returns>The current value if it is not empty; otherwise, the value of <paramref name="orElseValue"/>.</returns>
        public string OrElse(string orElseValue) =>
            value.IsEmpty ? orElseValue : value;

        /// <summary>
        /// Gets the value of the string if present; otherwise, returns an empty string.
        /// </summary>
        /// <remarks>Use this property to safely retrieve a string value without needing to check for null
        /// or missing values. This is useful when a non-null string is required for further processing or
        /// display.</remarks>
        public string OrEmpty => value.HasValue ? value : string.Empty;

        /// <summary>
        /// Gets the value if it is present; otherwise, throws a NullReferenceException.
        /// </summary>
        public string OrException() =>
            value.HasValue ? value : throw new NullReferenceException(nameof(value));

        /// <summary>
        /// Returns the value if it is present; otherwise, throws a NullReferenceException with a specified message.
        /// </summary>
        /// <param name="parameterName">The name of the parameter associated with the value being checked. Used in the exception message if the
        /// value is not present.</param>
        /// <param name="exceptionMessage">The custom message to include in the exception if the value is not present. Can be null or empty.</param>
        /// <returns>The value if it is present.</returns>
        /// <exception cref="NullReferenceException">Thrown if the value is not present. The exception message includes the specified parameter name and message.</exception>
        public string OrException(string parameterName, string? exceptionMessage) =>
            value.HasValue ? value :
            throw new NullReferenceException($"{parameterName} {exceptionMessage}");

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
        /// Gets the value of the current string with all HTML tags and non-breaking space entities removed.
        /// </summary>
        public string WithHtmlRemoved =>
            value is null ? string.Empty :
            Html().Replace(value, string.Empty).Replace("&nbsp;", " ").Replace("&nbsp", " ");

        /// <summary>
        /// Gets the value of the current string with all quotation marks removed.
        /// </summary>
        /// <remarks>
        /// Removes quotation marks used in European languages including straight quotes,
        /// curly quotes, low-9 quotes, and guillemets.
        /// </remarks>
        public string WithQuotationMarksRemoved =>
            value is null ? string.Empty :
            new string([.. value.Where(c => !QuotationMarks.Contains(c))]);
    }

    extension(string? number)
    {
        /// <summary>
        /// Gets a value indicating whether the current string represents a valid number in either integer or
        /// floating-point format.
        /// </summary>
        /// <remarks>The property returns <see langword="true"/> if the string can be parsed as a 64-bit
        /// signed integer or a floating-point number using the invariant culture. Otherwise, it returns <see
        /// langword="false"/>.</remarks>
        public bool IsNumber =>
            long.TryParse(number, out var _) ||
            double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var _);

        /// <summary>
        /// Gets a value indicating whether the current value is either empty or represents a number.
        /// </summary>
        public bool IsNumberOrEmpty => number.IsEmpty || number.IsNumber;

        /// <summary>
        /// Gets the value of the current string as a double-precision floating-point number, or zero if the conversion
        /// fails.
        /// </summary>
        /// <remarks>The conversion uses the invariant culture and accepts standard floating-point formats.
        /// If the string cannot be parsed as a valid double, the property returns 0.0.</remarks>
        public double ToDoubleOrZero =>
                double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : 0.0;

        /// <summary>
        /// Gets the integer value represented by the current string, or zero if the string is not a valid integer.
        /// </summary>
        public int ToIntOrZero =>
            int.TryParse(number, out var value) ? value : 0;
    }

    extension(IEnumerable<string> values)
    {
        /// <summary>
        /// Gets a value indicating whether all items in the collection are empty.
        /// </summary>
        public bool AreAllEmpty =>
            values.All(i => i.IsEmpty);

        /// <summary>
        /// Returns the zero-based index of the first element in the collection that contains the specified substring.
        /// </summary>
        /// <param name="searchValue">The substring to locate within the elements of the collection. Can be null or empty.</param>
        /// <returns>The zero-based index of the first element that contains the specified substring; otherwise, -1 if no such
        /// element is found or the collection is empty.</returns>
        public int IndexOfContains(string? searchValue)
        {
            if (searchValue.IsEmpty || !values.Any()) return -1;
            var actualValues = values.Where(v => v.Contains(searchValue));
            if (actualValues.IsEmpty) return -1;
            return values.IndexOfContains(actualValues.First());
        }
    }

    extension(string? filePath)
    {
        public bool HasFileExtension(params string[] extensions) =>
            filePath is not null &&
            extensions.Any(e => Path.GetExtension(filePath).Equals(e, StringComparison.OrdinalIgnoreCase));
    }

    #region These methods need to be classical extensions because a bug in C#14 extension using params keyword casuing the compiler incorrectly emits CS8620 warnings

    /// <summary>
    /// Determines whether the current value matches any of the specified strings, using a case-insensitive
    /// comparison.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="values">An array of strings to compare with the current value. Can be empty.</param>
    /// <returns>true if the current value equals any of the specified strings, ignoring case; otherwise, false.</returns>
    public static bool IsAnyOf(this string? value, params string[] values) =>
        values.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Determines whether any of the specified comma-separated values match the current value.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="commaSeparatedValues">A string containing one or more values separated by commas. Entries are trimmed of whitespace and empty
    /// entries are ignored. Can be null.</param>
    /// <returns>true if at least one of the specified values matches the current value; otherwise, false.</returns>
    public static bool IsAnyOf(this string? value, string? commaSeparatedValues) =>
        commaSeparatedValues is not null &&
        value.IsAnyOf(commaSeparatedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    #endregion
}


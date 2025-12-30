# Tellurian.Utilities
A .NET library with utility functions that tends
to be repeatedly implemented in projects.
Now, they are added in this utility NuGet package
to be included in most projects. 

Most utilities are implemented as extension properties 
and methods and provides short cuts for various 
repeted functionality.

## String Extensions
Namespace `Tellurian.Utilities`

Extensions on `string?`:
- `IsEmpty` - Returns true if string is null or whitespace.
- `HasValue` - Returns true if string contains a non-empty value.
- `Is(string?)` - Case-insensitive equality comparison (returns false if either is null).
- `IsNumber` - Returns true if string represents a valid integer or floating-point number.
- `IsNumberOrEmpty` - Returns true if string is empty or represents a number.
- `IsZeroesOrEmpty` - Returns true if string is empty or consists only of '0' characters.
- `OrElse(string)` - Returns the value or an alternative if empty.
- `OrDefault(string)` - Returns the value or a specified default if empty.
- `OrException` - Gets the value or throws NullReferenceException.
- `OrEmpty` - Gets the value or returns empty string if null.
- `WithHtmlRemoved` - Returns string with all HTML tags and `&nbsp;` removed.
- `IsAllOf(char[])` - Returns true if all characters in the string are contained in the specified array.
- `IsAnyOf(params string[])` - Case-insensitive check if value matches any of the provided strings.
- `IsAnyOf(string?)` - Same as above but accepts comma-separated values.
- `IsAnyPartOf(params string[])` - Case-insensitive check if any of the provided strings are contained in the value.
- `IsAnyPartOf(string?)` - Same as above but accepts comma-separated values.
- `ToItems(char separator = ';')` - Splits string into trimmed, non-empty items.
- `UntilOrEmpty(char[])` - Returns substring up to first occurrence of any stop character.
- `EqualsCaseInsensitive(string?)` - Case-insensitive equality comparison.
- `AsArray()` - Returns the value as a single-element array, or empty array if null.
- `FirstItem(string defaultValue = "")` - Returns first item from a comma-separated list.
- `ToIntOrZero` - Parses string to int, returns 0 if parsing fails.
- `ToDoubleOrZero` - Parses string to double (invariant culture), returns 0.0 if parsing fails.

Extensions on `IEnumerable<string>`:
- `AreAllEmpty` - Returns true if all strings in the collection are empty.

Extensions on file path strings:
- `HasFileExtension(params string[])` - Returns true if file path has one of the specified extensions (case-insensitive).

## Number Extensions
Namespace `Tellurian.Utilities`

Extensions on `int`:
- `IsAlsoOddOrEven(int)` - Returns true if both numbers have the same parity (both odd or both even).

## Bool Extensions
Namespace `Tellurian.Utilities`

Extensions on `bool`:
- `IfTrueThrows(string, string?)` - Throws ArgumentOutOfRangeException if value is true.

## Object Extensions
Namespace `Tellurian.Utilities`

Extensions on `object?`:
- `HasValue` - Returns true if object is not null.
- `HasValueExcept(object)` - Returns true if object is not null and not equal to the specified value.

Extensions on `T?` where T is a class:
- `ValueOrException(string, string?)` - Returns value or throws ArgumentNullException.
- `IfNotEqualsThrow(T, string, string?)` - Throws ArgumentOutOfRangeException if value is null or not equal.
- `AsArray()` - Returns the value as a single-element array, or empty array if null.
- `AsEnumerable()` - Returns the value as a single-element IEnumerable, or empty collection if null.

## LINQ Extensions
Namespace `Tellurian.Utilities.Linq`

Extensions on `IEnumerable<T>`:
- `IndexOf(Func<T, bool>)` - Returns zero-based index of first element matching the predicate, or -1 if not found.
- `TryGetFirstValue(Func<T,bool>, out T?)` - Try-get pattern for finding first matching item.

## Date and Time Extensions
Namespace `Tellurian.Utilities`

Extensions on `DateTime`:
- `DateOnly` - Gets the date component as `DateOnly`.
- `TimeOnly` - Gets the time component as `TimeOnly`.
- `ToDouble` - Gets the date/time as a fractional day of year.

Extensions on `DateTimeOffset`:
- `DateOnly` - Gets the date component as `DateOnly`.
- `TimeOnly` - Gets the time component as `TimeOnly`.

Extensions on `TimeSpan`:
- `TimeOnly` - Gets the time as `TimeOnly`.
- `ToDouble` - Gets the time as fractional days.
- `Max(TimeSpan)` - Returns the greater of the current and specified TimeSpan.
- `Min(TimeSpan)` - Returns the smaller of the current and specified TimeSpan.

Extensions on `DateOnly`:
- `WeekdayName` - Gets the name of the day of the week.

Extensions on `TimeOnly`:
- `HHMM` - Gets the time in 24-hour format (HH:mm).

Extensions on `double`:
- `TimeOnly` - Converts minutes to `TimeOnly`.

## Data Extensions
Namespace `Tellurian.Utilities.Data`

Provides extension methods on `IDataRecord` to get values of different types.
These methods have evolved based on experiences
of retrieving data from **Microsoft Access**.
However, the methods can be used for any data
source that supports `IDataRecord`.

Extensions on `IDataRecord`:
- `GetString(columnIndex/columnName, defaultValue?)` - Gets string value.
- `GetStringResource(columnName, ResourceManager, defaultValue)` - Gets localized string from resource.
- `GetByte(columnIndex/columnName, maxValue)` - Gets byte value with optional max clamping.
- `GetInt(columnIndex/columnName, defaultValue?)` - Gets integer value.
- `GetIntOrNull(columnIndex/columnName, defaultValue?)` - Gets nullable integer value.
- `GetDouble(columnIndex/columnName, defaultValue?)` - Gets double value.
- `GetDate(columnIndex/columnName, defaultValue?)` - Gets `DateOnly` value.
- `GetTime(columnIndex/columnName, defaultValue?)` - Gets `TimeOnly` value.
- `GetTimeAsTimespan(columnIndex/columnName)` - Gets time from DateTime column as `TimeOnly`.
- `GetTimeAsDouble(columnIndex/columnName, defaultValue?)` - Gets time as fractional days.
- `GetBool(columnIndex/columnName, defaultValue?)` - Gets boolean value (handles various numeric types).
- `IsDBNull(columnName)` - Checks if column value is DBNull.
- `GetColumnIndexes(string[])` - Gets dictionary mapping column names to ordinals.

Also includes a `Columns` helper class for efficient column ordinal lookups.

Extensions on `DataRow`:
- `GetRowFields()` - Returns all field values in the row as a string array.
- `IsBlankRow()` - Returns true if all fields in the row are empty.
- `BackgroundColor(int?)` - Gets background color value from a column by index.

## Markdown Extensions
Namespace `Tellurian.Utilities.Web`

Methods for converting markdown to HTML, using
a default `MarkdownPipeline`. This part is based
on the popular **Markdig** library.

Extensions on `string?`:
- `HtmlFromMarkdown()` - Converts markdown content to HTML.

## Color Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `string?` for handling web colours (color names are case-insensitive):
- `IsHexColor` - Returns true if string is a valid hex color code (e.g., `#FF00AA`).
- `TextColor` - Returns contrasting text color (`#000000` or `#FFFFFF`) for readability.
- `IsWhite` - Returns true if color is white (hex or name) or not set.
- `IsNotWhiteColor` - Returns true if color is not white.
- `HexColor` - Converts color name to hex code (case-insensitive). Supports 70+ named colors.

## Markup String Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `string?` for generating HTML markup:
- `Span(string cssClass = "")` - Wraps content in a `<span>` element with optional CSS class.
- `Div(string cssClass = "")` - Wraps content in a `<div>` element with optional CSS class.

## Http Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `HttpStatusCode`:
- `IsSuccess` - Returns true if status code is in the 2xx range.



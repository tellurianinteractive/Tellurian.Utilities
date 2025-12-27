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
- `OrElse(string)` - Returns the value or an alternative if empty.
- `WithHtmlRemoved` - Returns string with all HTML tags and `&nbsp;` removed.
- `AnyOf(params string[])` - Case-insensitive check if value matches any of the provided strings.
- `AnyOf(string?)` - Same as above but accepts comma-separated values.
- `AnyPartOf(params string[])` - Case-insensitive check if any of the provided strings are contained in the value.
- `AnyPartOf(string?)` - Same as above but accepts comma-separated values.
- `ToItems(char separator = ';')` - Splits string into trimmed, non-empty items.

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

## Markdown Extensions
Namespace `Tellurian.Utilities.Web`

Methods for converting markdown to HTML, using
a default `MarkdownPipeline`. This part is based
on the popular **Markdig** library.

Extensions on `string?`:
- `HtmlFromMarkdown()` - Converts markdown content to HTML.

## Color Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `string?` for handling web colours:
- `IsHexColor` - Returns true if string is a valid hex color code (e.g., `#FF00AA`).
- `TextColor` - Returns contrasting text color (`#000000` or `#FFFFFF`) for readability.
- `IsWhiteColor` - Returns true if color is white or not set.
- `ToHexColor` - Converts color name to hex code.

## Http Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `HttpStatusCode`:
- `IsSuccess` - Returns true if status code is in the 2xx range.



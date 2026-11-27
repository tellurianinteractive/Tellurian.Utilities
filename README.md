# Tellurian.Utilities
A .NET library with utility functions that tends
to be repeatedly implemented in projects.
Now, they are added in this utility NuGet package
to be included in most projects. 

Most utilities are implemented as extension properties 
and methods and provides short cuts for various 
repeted functionality.

> **EF Core:** don't use these extensions inside database queries.
> Extension *properties* can't appear in an expression tree at all (compile error CS9296).
> Extension *methods* compile, but EF Core can't translate them and throws
> "could not be translated" in `Where`, `OrderBy`, `Any` and the like; they only work in the final `Select`.
> Inside queries, use code EF Core translates, such as `string.IsNullOrWhiteSpace`, `ToLower() ==`, `Contains` and `??`.

> **Requirements:** .NET 11 from version 2.4.0; version 2.3.0 is the last for .NET 10.

## Breaking changes in 2.0
- `AsEnumerable()` on any class is renamed `ToSingleItemEnumerable()`. It won over LINQ's `AsEnumerable()`
  for every list, collection and query, so `list.AsEnumerable()` became a sequence holding the list.
- `IsAnyOf(string?)` and `IsAnyPartOf(string?)` with comma-separated values are renamed
  `IsAnyOfCommaSeparated` and `IsAnyPartOfCommaSeparated`: a single string argument silently picked them,
  so `"a".IsAnyOf("a,b")` split the value.
- `FirstItem` trims the item and skips empty items, and `HexColor` returns capitals and accepts `#RGB` and
  hex without `#`.

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
- `OrException` - Gets the value or throws NullReferenceException.
- `OrExceptionWithMessage(string, string?)` - Gets the value or throws NullReferenceException with parameter name and message.
- `OrEmpty` - Gets the value or returns empty string if null.
- `OrNullIfEmpty` - Gets the value trimmed, or null if empty or white-space (for optional database columns).
- `WithHtmlRemoved` - Returns string with all HTML tags and `&nbsp;` removed.
- `WithQuotationMarksRemoved` - Returns string without quotation marks, also apostrophes.
- `WithoutQuotationMarks(bool keepApostrophes = true)` - Returns string without quotation marks, keeping apostrophes by default ("O'Brien").
- `IsAllOf(char[])` - Returns true if all characters in the string are contained in the specified array.
- `IsAnyOf(params string[])` - Case-insensitive check if value matches any of the provided strings.
- `IsAnyOfCommaSeparated(string?)` - Same as above but accepts comma-separated values.
- `IsAnyPartOf(params string[])` - Case-insensitive check if any of the provided strings are contained in the value.
- `IsAnyPartOfCommaSeparated(string?)` - Same as above but accepts comma-separated values.
- `HasValueExcept(params string[])` - Returns true if the value is not empty and none of the given strings (case-insensitive).
- `ToItems(char separator = ';')` - Splits string into trimmed, non-empty items.
- `UntilOrEmpty(char[])` - Returns substring up to first occurrence of any stop character.
- `EqualsCaseInsensitive(string?)` - Case-insensitive equality comparison.
- `AsArray()` - Returns the value as a single-element array, or empty array if null.
- `FirstItem(string defaultValue = "", char separator = ',')` - Returns the first item, trimmed, of a list separated by commas or the given separator.
- `ToIntOrZero` - Parses string to int, returns 0 if parsing fails.
- `ToDoubleOrZero` - Parses string to double (invariant culture), returns 0.0 if parsing fails.
- `ToGuidOrNull` and `ToGuidOrNew()` - Parses string to `Guid`; null, or a new `Guid`, if parsing fails.
- `Truncated(int maxLength, bool ellipsis = false)` - At most `maxLength` characters; with `ellipsis`, cut at the last space and ended with "...".
- `WordWrapped(int width, string lineBreak = "\n")` - Lines of at most `width` characters, broken at spaces, joined by `lineBreak` (such as `"<br/>"`).

Extensions on `IEnumerable<string>`:
- `AreAllEmpty` - Returns true if all strings in the collection are empty.

Extensions on file path strings:
- `HasFileExtension(params string[])` - Returns true if file path has one of the specified extensions (case-insensitive).

## Number Extensions
Namespace `Tellurian.Utilities`

Extensions on `int`:
- `IsAlsoOddOrEven(int)` - Returns true if both numbers have the same parity (both odd or both even).

Extensions on any nullable number (`short?`, `int?`, `double?`, `decimal?`...):
- `IsNullOrZero()` - Returns true if null or zero.
- `IsNullOrZeroOrNegative()` - Returns true if null, zero or negative.
- `OrNullIfZero()` - Returns null for zero, as when zero in an optional field means "not given".
- `OrNullIfZeroOrNegative()` - Returns null for zero or a negative number, as for a year.

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
- `ToSingleItemEnumerable()` - Returns the value as a single-element IEnumerable, or empty collection if null.

## Result
Namespace `Tellurian.Utilities`

`Result<T>` is a C# 15 union type (requires .NET 11) of the outcomes of an operation. It is a class, so it always holds
one of the cases: there is no empty `default` result, and nullable reference types keep a null result away.
- `SuccessWithData<T>(T Data)` and `SuccessNoData` - Successes.
- `FailureWithException(Exception Exception)`, `FailureWithStatusCode(int StatusCode)` and `TimedOut` - Failures.

Each case converts implicitly to `Result<T>`, and a `switch` over the five cases is exhaustive:
```csharp
Result<Module> result = new SuccessWithData<Module>(module);
var text = result switch
{
    SuccessWithData<Module> success => success.Data.Name,
    SuccessNoData => "Saved",
    FailureWithException failure => failure.Exception.Message,
    FailureWithStatusCode failure => $"Status {failure.StatusCode}",
    TimedOut => "Timed out",
};
```
Members of `Result<T>`:
- `IsSuccess` and `IsFailure` - Timed out counts as a failure.
- `TryGetData(out T data)` and `DataOrDefault` - The data of a `SuccessWithData<T>`.
- `GetMessage()` - An extension method giving a short text for every case: "Succeeded.", the exception message,
  "Failed with status code 404." or "Timed out."
- `Equals`, `==` and `!=` - Results are equal when they hold equal cases.

Extensions on `Exception`:
- `AsResult<T>()` - `TimedOut` for a `TimeoutException` and for the `TaskCanceledException` that `HttpClient` throws on its
  timeout, otherwise `FailureWithException`.

## LINQ Extensions
Namespace `Tellurian.Utilities.Linq`

Extensions on `IEnumerable<T>`:
- `IndexOf(Func<T, bool>)` - Returns zero-based index of first element matching the predicate, or -1 if not found.
- `TryGetFirstValue(Func<T,bool>, out T?)` - Try-get pattern for finding first matching item.

## Pagination Extensions
Namespace `Tellurian.Utilities.Printing`

Extensions on `IEnumerable<T>` for splitting a sequence into pages for printing:
- `TotalPages(int itemsPerPage)` - Returns the total number of pages needed to display all items, or 0 if the sequence is null or empty.
- `Page(int itemPerPage, int pageNumber)` - Returns the items on the specified one-based page, or an empty sequence if the page number is out of range.
- `ItemsPerPage(int itemsPerPage)` - Splits the sequence into pages, each containing at most the specified number of items.
- `Pages(Func<T,double> height, int maxHeight, Func<T,T,bool>? pagebreak, double initialHeight = 0.0)` - Splits the sequence into pages based on accumulated item height, with an optional rule to force page breaks.

## Date and Time Extensions
Namespace `Tellurian.Utilities`

Extensions on `DateTime`:
- `DateOnly` - Gets the date component as `DateOnly`.
- `TimeOnly` - Gets the time component as `TimeOnly`.
- `ToDouble` - Gets the date/time as a fractional day of year.

Extensions on `DateTimeOffset` and `DateTimeOffset?`:
- `DateOnly` - Gets the date component as `DateOnly` (not nullable).
- `TimeOnly` - Gets the time component as `TimeOnly` (not nullable).
- `ToDateText()` and `ToDateAndTimeText()` - The date ("d"), or date and time ("g"), in the current culture; empty for no value.

Extensions on periods, `(DateTime? from, DateTime? to)`, `(int? from, int? to)` and `(short? from, short? to)`:
- `ToPeriodText()` - "from - to"; an open end is left out ("1950 - ", "- 1965"), none gives an empty string. Dates take a format, `"d"` by default.

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
- `IsDark` - returns true if the color gives good contrast as background to white text.
- `IsHexColor` - Returns true if string is a valid hex color code (e.g., `#FF00AA`).
- `IsWhite` - Returns true if color is white (hex or name) or not set.
- `IsNotWhiteColor` - Returns true if color is not white.
- `HexColor` - Converts color name to hex code (case-insensitive). Supports 70+ named colors.
- `TextColor` - Returns contrasting text color (`#000000` or `#FFFFFF`) for readability.
- `ContrastingTextColor(int darkBelowBrightness)` - As `TextColor`, with another brightness threshold than 170.
- `NormalizedHexColor` - Gets the color as `#RRGGBB` in capitals (from `#RRGGBB`, `#RGB`, hex without `#` or a color name), or null if no color.
- `IsKnownColor` - Returns true if the value is a hex color or a known color name.
- `TryGetHexColor(out string? hexColor)` - Gets the hex code, telling an unknown value apart from white (`HexColor` gives white for both).

Extensions on `IEnumerable<string>` of color names:
- `ToNamedColors()` - Returns `NamedColor(Name, HexColor)` records, with `TextColor`, in the given order, for example to build a color drop-down or swatches; an unknown name throws `ArgumentException`.

## Text Extensions
Namespace `Tellurian.Utilities.Text`

Corrections of typed or pasted input, for applying before saving, so users only need to be told about what can't be corrected:
- `Corrected(TextCase, TextCleanup = OrdinaryText)` - One declarative correction per field: the cleanup first, then the casing.
  `IsCorrected(...)` tells whether the correction would change nothing.
  ```csharp
  cargo.Name = cargo.Name.Corrected(TextCase.Sentence);
  cargo.Note = cargo.Note.Corrected(TextCase.AsTyped, TextCleanup.OrdinaryText | TextCleanup.NullIfEmpty);
  station.Signature = station.Signature.Corrected(TextCase.UpperCase, TextCleanup.OrdinaryText | TextCleanup.NoSpaces);
  ```
  - `TextCase` (one per field): `AsTyped`; `Capitalized` ("CST" → "Cst"); `Sentence` (as `Capitalized`, but abbreviations
    are kept: "STYCKEGODS" → "Styckegods", "Gas (LPG)", "LPG"); `InitialCapital` (only the first letter, the rest as typed:
    "Stockholm Södra"); `UpperCase`; `LowerCase`; and the name styles `Place`, `GivenName`, `FamilyName` and `Organisation`,
    made for names in 17 European countries: connecting words ("Frankfurt am Main"), name particles ("von Sydow"), elisions
    ("Cortina d'Ampezzo"), Dutch "IJ", "McDonald", legal forms ("GmbH", "Sp. z o.o.").
  - `TextCleanup` (flags that combine): `Trimmed`, `OrdinaryText` (see `AsOrdinaryText`, includes `Trimmed`), `NoSpaces`,
    `NoHyphens`, `NullIfEmpty`.
- `TextCasings` - The `TextCase` per kind of text, for declaring the rules of a language or country in one place:
  `new TextCasings { ["StationSignature"] = TextCase.UpperCase }.Of("StationSignature")`; an undeclared kind is `AsTyped`
  (or the given default).
- `AsCapitalizedName(NameStyle)` and `IsCapitalizedName(NameStyle)` - Obsolete: use `Corrected` (`FirstLetter` is
  `InitialCapital`, `AllWords` is `Place`).
- `AsOrdinaryText(bool multiline = false)` - Typographic quotes and dashes, other spaces, separate accent marks,
  zero-width and control characters made plain; trimmed.
- `AsEmailAddresses()` - Email addresses separated by semicolons, trimmed, in lower case, without duplicates.
- `IsEmailAddress` and `IsEmailAddresses` - One email address (the HTML standard's rule), or addresses separated by semicolons
  as `AsEmailAddresses` gives them; check `text.AsEmailAddresses().IsEmailAddresses` to accept what the correction understands.
- `WithInitialCapital` and `WithInitialLowerCase` - For display: the first letter a capital, or lower case, the rest as typed.
- `AsMacAddress()` and `IsMacAddress` - A MAC address in any common notation as `AA:BB:CC:DD:EE:FF`.
- `AsNumberRanges(int minimum, int maximum)`, `TryParseNumberRanges(...)` and `ToCollapsedRanges()` - Numbers and ranges
  such as `3,10-15`, sorted, without duplicates, consecutive numbers as ranges.

## Markup String Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `string?` for generating HTML markup:
- `Span(string cssClass = "")` - Wraps content in a `<span>` element with optional CSS class.
- `Div(string cssClass = "")` - Wraps content in a `<div>` element with optional CSS class.

## Http Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on `HttpStatusCode`:
- `IsSuccess` - Returns true if status code is in the 2xx range.

Extensions on `HttpResponseMessage` and `Task<HttpResponseMessage>`, giving a [`Result<T>`](#result):
- `ToResultAsync<T>(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)` - For a 2xx status
  `SuccessWithData<T>` with the JSON body (`JsonSerializerOptions.Web` by default), or `SuccessNoData` when the body is
  empty or JSON `null`; otherwise `FailureWithStatusCode`. A body that isn't valid JSON gives `FailureWithException`.
  On a `Task<HttpResponseMessage>` it also awaits the request and disposes the response: a failed request gives
  `FailureWithException`, and the `HttpClient` timeout `TimedOut`. A cancellation through the token is thrown, not returned.
```csharp
var result = await client.GetAsync($"modules/{id}", cancellationToken).ToResultAsync<Module>(cancellationToken: cancellationToken);
```



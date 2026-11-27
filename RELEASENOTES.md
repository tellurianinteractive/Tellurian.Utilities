# Release Notes
> Release notes are only published in English.

The release notes are summaries of important changes and fixes in each release. Earlier releases are listed as tags in the
repository.

## Release 3.0.0
Released 2026-11-27
Breaking changes:
- `ToDouble` on `DateTime` and `TimeSpan` gives the time of day as a fraction of a day: 12:00 is 0.5, not 0.2. `TimeSpan.ToDouble`
  is `TotalDays`. `GetTimeAsDouble` on data records follows.
- `OrException()` on strings throws `InvalidOperationException` instead of `NullReferenceException`.
- `GetColumnIndexes` returns `IReadOnlyDictionary<string, int>`; `Columns` is sealed.
- `Pages()` takes `maxHeight` as a `double`.
- `GetString` on data records gives the default value, when one is given, for a null value.

Fixes:
- `Span()` and `Div()` wrap any non-empty text (they gave an empty string for all text but colour names), close the `div`
  element, and HTML-encode the text and the CSS class.
- `IndexOfContains` no longer overflows the stack when it finds a match.
- `HHMM` is 24-hour `HH:mm` in every culture.
- `TryGetFirstValue` tells a match from no match also for value types.
- `Pages()` no longer adds an empty page when the first item is taller than a page.
- Data record getters use the default value for a missing column, also by index, and for `GetDate`, `GetByte` and
  `GetTimeAsDouble` by name; a missing column without default throws `InvalidOperationException` with the column name.
- Pagination with zero or negative items per page throws `ArgumentOutOfRangeException`.
- The generic extension members of `LinqExtensions` and `ObjectExtensions` are classic extension methods, so they load under
  Blazor WebAssembly.

New:
- `ToResultAsync<T>(JsonTypeInfo<T>)` on `HttpResponseMessage` and `Task<HttpResponseMessage>`, with source-generated JSON
  metadata, safe under trimming and Native AOT. The overloads with `JsonSerializerOptions` are marked as needing reflection.
- `TextCleanup.PlainCharacters`: the character cleanup of `OrdinaryText` without the name for trimming.
- The package is marked AOT-compatible and documents every public member.

## Release 2.4.0
Released 2026-11-27
- Requires .NET 11.
- `Result<T>`: a C# 15 union of `SuccessWithData<T>`, `SuccessNoData`, `FailureWithException`, `FailureWithStatusCode` and
  `TimedOut`, with `IsSuccess`, `IsFailure`, `TryGetData`, `DataOrDefault` and `GetMessage()`; `AsResult<T>()` on exceptions.
- `ToResultAsync<T>()` on `HttpResponseMessage` and `Task<HttpResponseMessage>`: the JSON body, the status code, a failed request
  or the `HttpClient` timeout as a `Result<T>`.
- Markdig 1.6.0.

## Release 2.3.0
Released 2026-11-21
- `IsEmailAddress` (one address, the HTML standard's rule) and `IsEmailAddresses` (addresses separated by semicolons, as
  `AsEmailAddresses` gives them).
- Strings: `Truncated(maxLength, ellipsis)`, `WordWrapped(width, lineBreak)`, `ToGuidOrNull` and `ToGuidOrNew()`.
- Text for display: `WithInitialCapital` and `WithInitialLowerCase`.
- Dates: `ToDateText()` and `ToDateAndTimeText()` on `DateTimeOffset` and `DateTimeOffset?`, `ToPeriodText()` on periods of dates,
  ints and shorts.

## Release 2.2.0
Released 2026-11-20
- `Corrected(TextCase, TextCleanup = OrdinaryText)`: one declarative correction per field, the cleanup first, then the casing;
  `IsCorrected` tells whether it would change nothing.
- `TextCase` (one per field): `AsTyped`, `Capitalized`, `Sentence`, `InitialCapital`, `UpperCase`, `LowerCase`, `Place`,
  `GivenName`, `FamilyName` and `Organisation`.
- `TextCleanup` (flags that combine): `Trimmed`, `OrdinaryText`, `NoSpaces`, `NoHyphens`, `NullIfEmpty`.
- `TextCasings`: the `TextCase` per kind of text, for declaring the rules of a language or country in one place.
- The first letter is capitalised also after a leading quotation mark or digit.
- Obsolete: `AsCapitalizedName` and `IsCapitalizedName` with `NameStyle`; use `Corrected`.

## Release 2.1.0
Released 2026-11-19
- `NamedColor(Name, HexColor)`, with `TextColor`, and `ToNamedColors()` on color names, for example to build a color drop-down or
  swatches; an unknown name throws `ArgumentException`.

## Release 2.0.0
Released 2026-11-19
Breaking changes:
- `AsEnumerable()` on any class is renamed `ToSingleItemEnumerable()`.
- `IsAnyOf(string?)` and `IsAnyPartOf(string?)` with comma-separated values are renamed `IsAnyOfCommaSeparated` and
  `IsAnyPartOfCommaSeparated`.
- `FirstItem` trims the item and skips empty items; `HexColor` returns capitals and accepts `#RGB` and hex without `#`.

New:
- Nullable numbers: `IsNullOrZero`, `IsNullOrZeroOrNegative`, `OrNullIfZero`, `OrNullIfZeroOrNegative`.
- Strings: `OrNullIfEmpty`, `FirstItem` with a separator, `HasValueExcept(params string[])`, `WithoutQuotationMarks`.
- Colors: `NormalizedHexColor`, `IsKnownColor`, `TryGetHexColor`, `ContrastingTextColor(threshold)`.
- `Tellurian.Utilities.Text`: name capitalization for 17 European countries, `AsOrdinaryText`, `AsEmailAddresses`, `AsMacAddress`
  and `IsMacAddress`, number ranges.

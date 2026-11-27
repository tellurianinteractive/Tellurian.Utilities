# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tellurian.Utilities is a .NET utility library providing extension methods for common operations. It is published as a NuGet package.

## Build Commands

```bash
# Build all projects
dotnet build

# Run all tests
dotnet test

# Run tests in specific project
dotnet test --project Tellurian.Utilities.Tests

# Run a specific test class
dotnet test --project Tellurian.Utilities.Tests --filter "FullyQualifiedName~DateAndTimeExtensionsTests"

# Build in Release mode (generates NuGet package)
dotnet build -c Release

# Pack NuGet package
dotnet pack
```

## Architecture

**Target Framework:** .NET 11 (C# 15 unions) (configured in Directory.Build.props)

**Project Structure:**
- `Tellurian.Utilities/` - Main library
- `Tellurian.Utilities.Tests/` - MSTest-based unit tests

**Namespaces:**
- `Tellurian.Utilities` - String, number, object, bool and date/time extensions
- `Tellurian.Utilities.Data` - IDataRecord extensions for database access
- `Tellurian.Utilities.Linq` - LINQ extensions
- `Tellurian.Utilities.Printing` - Pagination extensions
- `Tellurian.Utilities.Text` - Corrections of typed input: declarative corrections (`Corrected` with `TextCase` and `TextCleanup`, `TextCasings`), ordinary text, email and MAC addresses, number ranges
- `Tellurian.Utilities.Web` - Markdown, color, markup and HTTP extensions

Keep `README.md` in step with every public member: it is the package's documentation on nuget.org. Every public type and
member has an XML doc comment (`GenerateDocumentationFile`; warnings are errors). User-visible changes get a bullet in
`RELEASENOTES.md`.

Extensions must not be needed inside EF Core queries: extension properties can't appear in expression trees (CS9296), and
extension methods aren't translated. Avoid names that compete with LINQ or the BCL (`AsEnumerable()` on any class won over
LINQ's until 2.0; the `IDataRecord` getters `GetString(string)`, `GetDouble(string)`, `GetByte(string)` and `IsDBNull(string)`
lose to the BCL's `DbDataReader` extensions in `System.Data`). With C# 14 extension blocks, use classic static methods for
`params` (CS8620) and for all generic members (TypeLoadException under Blazor WebAssembly); this is the one exception to the
"extension blocks only" rule. The package is `IsAotCompatible`: reflection-based code needs a source-generated alternative.

## C# Language Features

This codebase uses the latest released C# (`LangVersion` `latest`, C# 15 with .NET 11), including **C# 14 extension members**
and the C# 15 `union` (`Result<T>`). Extensions are declared using the `extension(Type target)` block syntax instead of
traditional `this` parameter syntax, except for the cases above:

```csharp
extension(string? value)
{
    public bool HasValue => !value.IsEmpty;
    public string OrElse(string orElseValue) => value.IsEmpty ? orElseValue : value;
}
```

## Dependencies

- **Markdig** - Used for Markdown to HTML conversion in `MarkdownExtensions.cs`
- **MSTest.Sdk 4.5** - Test framework, on Microsoft Testing Platform

## Testing Conventions

Use `Assert.Throws<T>()` for exception testing (MSTest.Sdk 4 API), never asserts in a `catch` block. Assembly attributes are
in `_AssemblySettings.cs`. `TestContext` is taken through the test class's primary constructor.

```csharp
[TestMethod]
public void Method_Condition_ThrowsException() =>
    Assert.Throws<ArgumentException>(() => myObject.Method(invalidValue));
```

## Key Patterns

- Prefer collection expressions over `.ToArray()`: `[.. enumerable]` instead of `enumerable.ToArray()`
- Most utilities are extension properties/methods on common types (string, DateTime, IDataRecord, etc.)
- Generated regex patterns are used via `[GeneratedRegex]` attribute
- Nullable annotations are enabled (`<Nullable>enable</Nullable>`)
- IDataRecord extensions handle type coercion for Microsoft Access compatibility

## CI/CD

**Build Pipeline** (`.github/workflows/build.yml`):
- Triggers on push/PR to master
- Builds and runs tests

**Publish Pipeline** (`.github/workflows/publish.yml`):
- Triggers automatically when `Directory.Build.props` changes on master
- Can also be triggered manually via workflow dispatch
- Reads version from `Directory.Build.props`, and does nothing when the tag `v<Version>` exists (already released)
- Builds, tests, packs, and publishes to NuGet.org
- Automatically creates git tag and GitHub Release

**Publishing uses nuget.org Trusted Publishing** (OIDC, `NuGet/login`); no API key is stored.
- nuget.org policy: owner `tellurianinteractive`, repository `Tellurian.Utilities`, workflow file `publish.yml`.
- The nuget.org profile name `tellurian` is set in `publish.yml` (`NuGet/login`, `user`).

**To release:**
1. Update `<Version>` in `Directory.Build.props`, and give the release its section in `RELEASENOTES.md` and `PackageReleaseNotes`
2. Commit and push to master
3. Release runs automatically

## Local NuGet Development

This repo produces NuGet packages consumed by other Tellurian projects (such as the Module Registry app). During active development, iterate through the local feed `C:\NuGets` instead of publishing every experimental version to nuget.org.

- Per iteration: bump `<Version>` in `Directory.Build.props` (patch), `dotnet build -c Release --no-incremental` (it also packs; a plain `dotnet pack` once packed a stale Release build); `Directory.Build.targets` copies the package to `C:\NuGets`. Do NOT edit the release notes per iteration, and do NOT push a changed `Directory.Build.props` to master: the release workflow publishes on every such push.
- At publication: consolidate iterations into one `PackageReleaseNotes` entry, decide patch/minor/major, merge to master.
- CI runners don't have `C:\NuGets`: a consumer pinned to a local-only version cannot ship (NU1101). Intended safety net.

Full workflow: user-level `local-nuget-development` skill.

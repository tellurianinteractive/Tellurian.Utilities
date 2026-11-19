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

**Target Framework:** .NET 10 (configured in Directory.Build.props)

**Project Structure:**
- `Tellurian.Utilities/` - Main library
- `Tellurian.Utilities.Tests/` - MSTest-based unit tests

**Namespaces:**
- `Tellurian.Utilities` - String and date/time extensions
- `Tellurian.Utilities.Data` - IDataRecord extensions for database access
- `Tellurian.Utilities.Web` - Markdown, color, and HTTP extensions

## C# Language Features

This codebase uses **C# 14 extension members** (preview feature). Extensions are declared using the new `extension(Type target)` block syntax instead of traditional `this` parameter syntax:

```csharp
extension(string? value)
{
    public bool HasValue => !value.IsEmpty;
    public string OrElse(string orElseValue) => value.IsEmpty ? orElseValue : value;
}
```

## Dependencies

- **Markdig** - Used for Markdown to HTML conversion in `MarkdownExtensions.cs`
- **MSTest.Sdk 4.0** - Test framework

## Testing Conventions

Use `Assert.Throws<T>()` for exception testing (MSTest.Sdk 4.0 API):

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

**Release Pipeline** (`.github/workflows/release.yml`):
- Triggers automatically when `Directory.Build.props` changes on master
- Can also be triggered manually via workflow dispatch
- Reads version from `Directory.Build.props`
- Builds, tests, packs, and publishes to NuGet.org
- Automatically creates git tag and GitHub Release

**Publishing uses nuget.org Trusted Publishing** (OIDC, `NuGet/login`); no API key is stored.
- nuget.org policy: owner `tellurianinteractive`, repository `Tellurian.Utilities`, workflow file `release.yml`.
- The nuget.org profile name `tellurian` is set in `release.yml` (`NuGet/login`, `user`).

**To release:**
1. Update `<Version>` in `Directory.Build.props`
2. Commit and push to master
3. Release runs automatically

## Local NuGet Development

This repo produces NuGet packages consumed by other Tellurian projects (such as the Module Registry app). During active development, iterate through the local feed `C:\NuGets` instead of publishing every experimental version to nuget.org.

- Per iteration: bump `<Version>` in `Directory.Build.props` (patch), `dotnet pack -c Release`; `Directory.Build.targets` copies the package to `C:\NuGets`. Do NOT edit the release notes per iteration, and do NOT push a changed `Directory.Build.props` to master: the release workflow publishes on every such push.
- At publication: consolidate iterations into one `PackageReleaseNotes` entry, decide patch/minor/major, merge to master.
- CI runners don't have `C:\NuGets`: a consumer pinned to a local-only version cannot ship (NU1101). Intended safety net.

Full workflow: user-level `local-nuget-development` skill.

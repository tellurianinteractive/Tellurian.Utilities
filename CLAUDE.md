# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tellurian.Utilities is a .NET utility library providing extension methods for common operations. It is published as a NuGet package.

## Build Commands

```bash
# Build all projects
dotnet build

# Run tests
dotnet test

# Run a specific test class
dotnet test --filter "FullyQualifiedName~DateAndTimeExtensionsTests"

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

## Key Patterns

- Most utilities are extension properties/methods on common types (string, DateTime, IDataRecord, etc.)
- Generated regex patterns are used via `[GeneratedRegex]` attribute
- Nullable annotations are enabled (`<Nullable>enable</Nullable>`)
- IDataRecord extensions handle type coercion for Microsoft Access compatibility

## CI/CD

**Build Pipeline** (`.github/workflows/build.yml`):
- Triggers on push/PR to master
- Builds and runs tests

**Release Pipeline** (`.github/workflows/release.yml`):
- Triggered manually via workflow dispatch
- Reads version from `Directory.Build.props`
- Builds, tests, packs, and publishes to NuGet.org
- Automatically creates git tag and GitHub Release

**Required Secret:** `NUGET_API_KEY` - NuGet.org API key for publishing

**To release:**
1. Update `<Version>` in `Directory.Build.props`
2. Commit and push to master
3. Go to Actions → Release → Run workflow

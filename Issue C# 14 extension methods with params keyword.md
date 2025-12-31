# CS8620 warning with params arrays in C# 14 extension members

## Summary

When using C# 14 extension members with `params` array parameters, the compiler incorrectly emits CS8620 warnings ("Argument of type 'string' cannot be used for parameter 'values' of type 'string[]' due to differences in the nullability of reference types") even when passing non-nullable string literals to a `params string[]` parameter.

## Environment

- .NET 10 Preview
- C# 14 with extension members enabled
- Nullable reference types enabled

## Minimal Reproduction

```csharp
#nullable enable

public static partial class StringExtensions
{
    extension(string? value)
    {
        public bool IsAnyOf(params string[] values) =>
            values.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
    }
}

// Usage - triggers CS8620 on each string argument:
var result = "test".IsAnyOf("foo", "test", "bar");
//                          ^^^^^ CS8620
//                                ^^^^^^ CS8620
//                                        ^^^^^ CS8620
```

**Warning message:**
```
CS8620: Argument of type 'string' cannot be used for parameter 'values' of type 'string[]'
in 'bool extension(string?).IsAnyOf(params string[] values)' due to differences in the
nullability of reference types.
```

## Approaches Attempted (all failed)

### 1. Change parameter to `params string?[]`

```csharp
extension(string? value)
{
    public bool IsAnyOf(params string?[] values) =>
        values.Length > 0 && values.Any(v => v is not null && v.Equals(value, StringComparison.OrdinalIgnoreCase));
}
```

**Result:** Same CS8620 warning, just with `string?[]` in the message.

### 2. Use `params ReadOnlySpan<string?>`

```csharp
extension(string? value)
{
    public bool IsAnyOf(params ReadOnlySpan<string?> values)
    {
        foreach (var v in values)
            if (v is not null && v.Equals(value, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
```

**Result:** Same CS8620 warning with `ReadOnlySpan<string?>`.

### 3. Use `params ReadOnlySpan<string>` (non-nullable)

```csharp
extension(string? value)
{
    public bool IsAnyOf(params ReadOnlySpan<string> values)
    {
        foreach (var v in values)
            if (v.Equals(value, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
```

**Result:** Same CS8620 warning with `ReadOnlySpan<string>`.

### 4. Separate extension block on non-nullable `string`

```csharp
extension(string value)  // non-nullable
{
    public bool IsAnyOf(params string[] values) =>
        values.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
}
```

**Result:** Same CS8620 warning persists.

## Workaround

Using the traditional extension method syntax instead of C# 14 extension members eliminates the warning:

```csharp
public static bool IsAnyOf(this string? value, params string[] values) =>
    values.Length > 0 && values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
```

**Result:** No warnings, all tests pass.

## Expected Behavior

The C# 14 extension member syntax should behave identically to the traditional extension method syntax with respect to nullability analysis. Passing non-nullable `string` arguments to a `params string[]` parameter should not produce CS8620 warnings.

## Actual Behavior

The compiler emits CS8620 for each argument passed to the `params` array, regardless of:
- Whether the array element type is nullable (`string?[]`) or non-nullable (`string[]`)
- Whether `ReadOnlySpan<T>` is used instead of an array
- Whether the extension is defined on `string?` or `string`

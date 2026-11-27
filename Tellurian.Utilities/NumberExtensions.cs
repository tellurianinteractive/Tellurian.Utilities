using System.Numerics;

namespace Tellurian.Utilities;

/// <summary>
/// Extensions for numbers and nullable numbers.
/// </summary>
public static class NumberExtensions
{
    extension(int number)
    {
        /// <summary>
        /// Determines whether the specified number has the same parity (odd or even) as the current number.
        /// </summary>
        /// <param name="otherNumber">The number to compare with the current number to check for matching parity.</param>
        /// <returns>true if both numbers are odd or both are even; otherwise, false.</returns>
        public bool IsAlsoOddOrEven(int otherNumber) => (number - otherNumber) % 2 == 0;
    }

    // Classic generic extension methods: a C# 14 generic extension block threw TypeLoadException under the Blazor WebAssembly
    // Mono interpreter (see PaginationExtensions).

    /// <summary>Determines whether the nullable number is null or zero, as when zero in an optional field means "not given".</summary>
    public static bool IsNullOrZero<T>(this T? value) where T : struct, INumber<T> =>
        value is null || T.IsZero(value.Value);

    /// <summary>Determines whether the nullable number is null, zero or negative.</summary>
    public static bool IsNullOrZeroOrNegative<T>(this T? value) where T : struct, INumber<T> =>
        value is null || T.IsZero(value.Value) || T.IsNegative(value.Value);

    /// <summary>Returns null for zero, otherwise the value: zero in an optional field means "not given".</summary>
    public static T? OrNullIfZero<T>(this T? value) where T : struct, INumber<T> =>
        value.IsNullOrZero() ? null : value;

    /// <summary>Returns null for zero or a negative number, otherwise the value, as for a year or a quantity that must be positive.</summary>
    public static T? OrNullIfZeroOrNegative<T>(this T? value) where T : struct, INumber<T> =>
        value.IsNullOrZeroOrNegative() ? null : value;
}

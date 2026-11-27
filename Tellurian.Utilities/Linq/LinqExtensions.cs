using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Tellurian.Utilities.Linq;

/// <summary>
/// Extensions for sequences.
/// </summary>
/// <remarks>
/// The generic members are classic <c>this</c>-parameter extension methods rather than a C# 14 generic
/// <c>extension&lt;T&gt;</c> block: the Blazor WebAssembly (Mono) runtime fails to load the type synthesised for a generic
/// extension block and throws <see cref="TypeLoadException"/>. The properties are in a non-generic block, which loads correctly.
/// </remarks>
public static class LinqExtensions
{
    extension([NotNullWhen(false)] IEnumerable? values)
    {
        /// <summary>
        /// Determines whether the sequence is null or contains no elements.
        /// </summary>
        /// <returns>true if the sequence is null or empty; otherwise, false.</returns>
        public bool IsEmpty => values is null || !HasAnyItem(values);

        /// <summary>
        /// Determines whether the sequence is null.
        /// </summary>
        public bool IsNull => values is null;
    }

    /// <summary>
    /// Returns the zero-based index of the first element that matches the specified predicate.
    /// </summary>
    /// <param name="values">The sequence to search.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The zero-based index of the first element that matches the predicate; otherwise, -1 if no such element is
    /// found.</returns>
    public static int IndexOf<T>(this IEnumerable<T> values, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(predicate);
        var index = 0;
        foreach (var item in values)
        {
            if (predicate(item)) return index;
            index++;
        }
        return -1;
    }

    /// <summary>
    /// Attempts to find the first element that matches the specified predicate.
    /// </summary>
    /// <param name="values">The sequence to search.</param>
    /// <param name="predicate">A function that defines the conditions of the element to search for.</param>
    /// <param name="result">When this method returns, contains the first element that matches the predicate, if found; otherwise, the
    /// default value for the type.</param>
    /// <returns>true if an element matching the predicate is found, also for a value type whose match equals its default;
    /// otherwise, false.</returns>
    public static bool TryGetFirstValue<T>(this IEnumerable<T> values, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(predicate);
        foreach (var item in values)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }
        result = default;
        return false;
    }

    private static bool HasAnyItem(IEnumerable values)
    {
        if (values is ICollection collection) return collection.Count > 0;
        var enumerator = values.GetEnumerator();
        try
        {
            return enumerator.MoveNext();
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using Tellurian.Utilities;
using Tellurian.Utilities.Linq;

namespace Tellurian.Utilities.Linq;

public static class LinqExtensions
{
    extension<T>(IEnumerable<T> values)
    {
        /// <summary>
        /// Returns the zero-based index of the first element that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition. Cannot be null.</param>
        /// <returns>The zero-based index of the first element that matches the predicate; otherwise, -1 if no such element is
        /// found.</returns>
        public int IndexOf(Func<T, bool> predicate) =>
            !values.Any() ? -1 :
            values
                .Select((item, index) => new { item, index })
                .FirstOrDefault(x => predicate(x.item))?.index ?? -1;

        /// <summary>
        /// Attempts to find the first element that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">A function that defines the conditions of the element to search for. Cannot be null.</param>
        /// <param name="result">When this method returns, contains the first element that matches the predicate, if found; otherwise, the
        /// default value for the type.</param>
        /// <returns>true if an element matching the predicate is found; otherwise, false.</returns>
        public bool TryGetFirstValue(Func<T, bool> predicate, [NotNullWhen(true)] out T? result)
        {
            result = values.FirstOrDefault(item => predicate(item));
            return result is not null;
        }
    }

    extension<T>([NotNullWhen(false)] IEnumerable<T>? values)
    {
        /// <summary>
        /// Determines whether the collection contains no elements.
        /// </summary>
        /// <returns>true if the collection is empty; otherwise, false.</returns>
        public bool IsEmpty => values.IsNull || !values.Any();

        public bool IsNull => values is null;

    }
}


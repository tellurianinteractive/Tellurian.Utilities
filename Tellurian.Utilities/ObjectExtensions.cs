using System.Diagnostics.CodeAnalysis;

namespace Tellurian.Utilities;

public static class ObjectExtensions
{
    extension([NotNullWhen(true)] object? value)
    {
        /// <summary>
        /// Gets a value indicating whether the current instance has a valid value assigned.
        /// </summary>
        public bool HasValue => value is not null;

        /// <summary>
        /// Determines whether the current value is set and is not equal to the specified exception value.
        /// </summary>
        /// <param name="except">The value to compare against the current value. If the current value equals this parameter, the method
        /// returns false.</param>
        /// <returns>true if the current value is not null and does not equal the specified exception value; otherwise, false.</returns>
        public bool HasValueExcept(object except) => value is not null && !value.Equals(except);
    }

    extension<T>([AllowNull] T? value) where T : class
    {
        /// <summary>
        /// Returns the value if it is not null; otherwise, throws an exception indicating a required parameter was
        /// null.
        /// </summary>
        /// <param name="parameterName">The name of the parameter associated with the value. Used in the exception if the value is null.</param>
        /// <param name="nullMessage">An optional custom message for the exception if the value is null. If null, a default message is used.</param>
        /// <returns>The value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the value is null. The exception references the specified parameter name and message.</exception>
        public T ValueOrException(string parameterName, string? nullMessage = null) =>
            value ?? throw new ArgumentNullException(parameterName, nullMessage);

        /// <summary>
        /// Throws an exception if the current value does not equal the specified value.
        /// </summary>
        /// <param name="other">The value to compare with the current value for equality.</param>
        /// <param name="parameterName">The name of the parameter that caused the exception if the values are not equal.</param>
        /// <param name="notEqualsMessage">The error message to include in the exception if the values are not equal. This parameter is optional.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the current value is null or does not equal <paramref name="other"/>.</exception>
        public void IfNotEqualsThrow(T other, string parameterName, string? notEqualsMessage = null)
        {
            if (value is null || !value.Equals(other)) throw new ArgumentOutOfRangeException(parameterName, notEqualsMessage);
        }

        /// <summary>
        /// Returns the value as a single-element array, or an empty array if the value is null.
        /// </summary>
        /// <returns>An array containing the value if it is not null; otherwise, an empty array.</returns>
        public T[] AsArray() => value is null ? [] : [value];

        /// <summary>
        /// Returns an enumerable collection containing the current value, or an empty collection if the value is null.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the value if it is not null; otherwise, an empty collection.</returns>
        public IEnumerable<T> AsEnumerable() => value is null ? [] : [value];


    }
}

using System.Diagnostics.CodeAnalysis;

namespace Tellurian.Utilities;

public static class ObjectExtensions
{
    extension([NotNullWhen(true)] object? value)
    {
        public bool HasValue => value is not null;
        public bool HasValueExcept(object except) => value is not null && !value.Equals(except);
    }

    extension<T>([AllowNull] T? value) where T : class
    {
        public T ValueOrException(string parameterName, string? nullMessage = null) =>
            value ?? throw new ArgumentNullException(parameterName, nullMessage);

        public void IfNotEqualsThrow(T other, string parameterName, string? notEqualsMessage = null)
        {
            if (value is null || !value.Equals(other)) throw new ArgumentOutOfRangeException(parameterName, notEqualsMessage);
        }
    }
}

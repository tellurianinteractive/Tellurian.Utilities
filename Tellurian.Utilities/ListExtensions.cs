using System.Diagnostics.CodeAnalysis;

namespace Tellurian.Utilities;

public static class ListExtensions
{
    extension<T>(List<T> list)
    {
        public bool TryGetFirstValue(Func<T, bool> predicate, [NotNullWhen(true)] out T? result)
        {
            result = list.FirstOrDefault(item => predicate(item));
            return result is not null;
        }
    }
}


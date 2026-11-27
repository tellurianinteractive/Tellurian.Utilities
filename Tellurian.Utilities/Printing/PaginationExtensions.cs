namespace Tellurian.Utilities.Printing;

/// <summary>
/// Provides extension methods for splitting a sequence of items into pages for printing.
/// </summary>
/// <remarks>
/// These are written as classic <c>this</c>-parameter generic extension methods rather than a
/// C# 14 generic <c>extension&lt;T&gt;</c> block: the Blazor WebAssembly (Mono) runtime
/// fails to resolve the synthesised type for a generic extension block, throwing
/// <see cref="TypeLoadException"/> when a consuming component is rendered. Non-generic extension
/// blocks load correctly, but generic ones do not, so this type must use the classic form.
/// </remarks>
public static class PaginationExtensions
{
    /// <summary>
    /// Calculates the total number of pages required to display all items, given a fixed number of items per page.
    /// </summary>
    /// <param name="items">The sequence to paginate.</param>
    /// <param name="itemsPerPage">The maximum number of items that fit on a single page; must be positive.</param>
    /// <returns>The total number of pages, or 0 if the sequence is null or empty.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="itemsPerPage"/> is zero or negative.</exception>
    public static int TotalPages<T>(this IEnumerable<T>? items, int itemsPerPage)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemsPerPage);
        if (items is null) return 0;
        var count = items.Count();
        return (count + itemsPerPage - 1) / itemsPerPage;
    }

    /// <summary>
    /// Returns the items belonging to the specified page.
    /// </summary>
    /// <param name="items">The sequence to paginate.</param>
    /// <param name="itemPerPage">The maximum number of items that fit on a single page; must be positive.</param>
    /// <param name="pageNumber">The one-based page number to return.</param>
    /// <returns>The items on the requested page, or an empty sequence if <paramref name="pageNumber"/> is out of range.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="itemPerPage"/> is zero or negative.</exception>
    public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int itemPerPage, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemPerPage);
        return pageNumber < 1 ? [] : items.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage);
    }

    /// <summary>
    /// Splits the sequence into pages, each containing at most the specified number of items.
    /// The sequence is enumerated once.
    /// </summary>
    /// <param name="items">The sequence to paginate.</param>
    /// <param name="itemsPerPage">The maximum number of items that fit on a single page; must be positive.</param>
    /// <returns>A sequence of pages, where each page is a sequence of items.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="itemsPerPage"/> is zero or negative.</exception>
    public static IEnumerable<IEnumerable<T>> ItemsPerPage<T>(this IEnumerable<T> items, int itemsPerPage)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemsPerPage);
        return items.Chunk(itemsPerPage);
    }

    /// <summary>
    /// Splits the sequence into pages based on the accumulated height of the items rather than a fixed item count.
    /// Items are added to the current page until adding the next item would exceed <paramref name="maxHeight"/>,
    /// or until an optional page break rule forces a new page. An item taller than a page gets a page of its own.
    /// </summary>
    /// <param name="items">The sequence to paginate.</param>
    /// <param name="height">A function that returns the height of an individual item.</param>
    /// <param name="maxHeight">The maximum total height allowed on a single page.</param>
    /// <param name="pagebreak">An optional function that, given the previous and current item, returns true to force a page break before the current item; or null to apply no such rule.</param>
    /// <param name="initialHeight">The height already consumed at the top of each page, for example by a header. Defaults to 0.</param>
    /// <returns>A sequence of pages, where each page is a non-empty sequence of items whose combined height fits within
    /// <paramref name="maxHeight"/>, unless a single item is taller.</returns>
    public static IEnumerable<IEnumerable<T>> Pages<T>(this IEnumerable<T> items, Func<T, double> height, double maxHeight, Func<T, T, bool>? pagebreak, double initialHeight = 0.0)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(height);
        List<List<T>> pages = [];
        List<T> page = [];
        var currentHeight = initialHeight;
        foreach (var item in items)
        {
            var itemHeight = height(item);
            var isPageBreak = page.Count > 0 &&
                (currentHeight + itemHeight > maxHeight || (pagebreak is not null && pagebreak(page[^1], item)));
            if (isPageBreak)
            {
                pages.Add(page);
                page = [];
                currentHeight = initialHeight;
            }
            currentHeight += itemHeight;
            page.Add(item);
        }
        if (page.Count > 0) pages.Add(page);
        return pages;
    }
}

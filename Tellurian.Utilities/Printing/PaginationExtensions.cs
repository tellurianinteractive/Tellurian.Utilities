namespace Tellurian.Utilities.Printing;

/// <summary>
/// Provides extension methods for splitting a sequence of items into pages for printing.
/// </summary>
public static class PaginationExtensions
{
    extension<T>(IEnumerable<T> items)
    {
        /// <summary>
        /// Calculates the total number of pages required to display all items, given a fixed number of items per page.
        /// </summary>
        /// <param name="itemsPerPage">The maximum number of items that fit on a single page.</param>
        /// <returns>The total number of pages, or 0 if the sequence is null or empty.</returns>
        public int TotalPages(int itemsPerPage)
        {
            if (items is null) return 0;
            var count = items.Count();
            if (count == 0) return 0;
            return (count + itemsPerPage - 1) / itemsPerPage;
        }

        /// <summary>
        /// Returns the items belonging to the specified page.
        /// </summary>
        /// <param name="itemPerPage">The maximum number of items that fit on a single page.</param>
        /// <param name="pageNumber">The one-based page number to return.</param>
        /// <returns>The items on the requested page, or an empty sequence if <paramref name="pageNumber"/> is out of range.</returns>
        public IEnumerable<T> Page(int itemPerPage, int pageNumber) =>
            pageNumber < 1 || pageNumber > items.TotalPages(itemPerPage) ? [] :
            items.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage);

        /// <summary>
        /// Splits the sequence into pages, each containing at most the specified number of items.
        /// </summary>
        /// <param name="itemsPerPage">The maximum number of items that fit on a single page.</param>
        /// <returns>A sequence of pages, where each page is a sequence of items.</returns>
        public IEnumerable<IEnumerable<T>> ItemsPerPage(int itemsPerPage)
        {
            var totalPages = items.TotalPages(itemsPerPage);
            return Enumerable.Range(1, totalPages).Select(page => items.Page(itemsPerPage, page));
        }

        /// <summary>
        /// Splits the sequence into pages based on the accumulated height of the items rather than a fixed item count.
        /// Items are added to the current page until adding the next item would exceed <paramref name="maxHeight"/>,
        /// or until an optional page break rule forces a new page.
        /// </summary>
        /// <param name="height">A function that returns the height of an individual item.</param>
        /// <param name="maxHeight">The maximum total height allowed on a single page.</param>
        /// <param name="pagebreak">An optional function that, given the previous and current item, returns true to force a page break before the current item; or null to apply no such rule.</param>
        /// <param name="initialHeight">The height already consumed at the top of each page, for example by a header. Defaults to 0.</param>
        /// <returns>A sequence of pages, where each page is a sequence of items whose combined height fits within <paramref name="maxHeight"/>.</returns>
        public IEnumerable<IEnumerable<T>> Pages(Func<T, double> height, int maxHeight, Func<T, T, bool>? pagebreak, double initialHeight = 0.0)
        {
            List<List<T>> pages = [];
            List<T> page = [];
            var currentHeight = initialHeight;
            foreach (var item in items)
            {
                var itemHeight = height(item);
                var nextHeight = currentHeight + itemHeight;
                if (nextHeight > maxHeight || (pagebreak is not null && page.Count != 0 && pagebreak(page.Last(), item)))
                {
                    pages.Add(page);
                    page = [];
                    currentHeight = initialHeight + itemHeight;
                }
                else
                {
                    currentHeight += itemHeight;
                }
                page.Add(item);
            }
            if (page.Count > 0) { pages.Add(page); }
            return pages;
        }
    }
}

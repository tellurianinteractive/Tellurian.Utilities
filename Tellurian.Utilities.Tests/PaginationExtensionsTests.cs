using Tellurian.Utilities.Printing;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class PaginationExtensionsTests
{
    [TestMethod]
    public void Pages_FirstItemTallerThanPage_GetsAPageOfItsOwnWithoutAnEmptyPage()
    {
        var pages = new[] { 5.0, 1.0 }.Pages(h => h, 3, null).ToList();

        Assert.HasCount(2, pages);
        Assert.AreSequenceEqual(new[] { 5.0 }, pages[0].ToArray());
        Assert.AreSequenceEqual(new[] { 1.0 }, pages[1].ToArray());
    }

    [TestMethod]
    public void Pages_ItemsFillPagesUpToMaxHeight()
    {
        var pages = new[] { 1.0, 1.0, 1.0, 1.0, 1.0 }.Pages(h => h, 2, null).ToList();

        Assert.HasCount(3, pages);
        Assert.IsTrue(pages.All(page => page.Any()));
    }

    [TestMethod]
    public void Pages_InitialHeightCountsOnEveryPage()
    {
        var pages = new[] { 1.0, 1.0, 1.0 }.Pages(h => h, 3, null, initialHeight: 1).ToList();

        Assert.HasCount(2, pages);
        Assert.HasCount(2, pages[0]);
    }

    [TestMethod]
    public void Pages_PageBreakRuleStartsANewPage()
    {
        var pages = new[] { 1, 1, 2, 2 }.Pages(_ => 1, 10, (previous, current) => previous != current).ToList();

        Assert.HasCount(2, pages);
        Assert.AreSequenceEqual(new[] { 2, 2 }, pages[1].ToArray());
    }

    [TestMethod]
    public void ItemsPerPage_SplitsInPagesOfAtMostTheCount()
    {
        var pages = Enumerable.Range(1, 5).ItemsPerPage(2).ToList();

        Assert.HasCount(3, pages);
        Assert.AreSequenceEqual(new[] { 5 }, pages[2].ToArray());
    }

    [TestMethod]
    public void Page_OutOfRange_IsEmpty()
    {
        Assert.IsEmpty(Enumerable.Range(1, 5).Page(2, 0));
        Assert.IsEmpty(Enumerable.Range(1, 5).Page(2, 4));
        Assert.AreSequenceEqual(new[] { 3, 4 }, Enumerable.Range(1, 5).Page(2, 2).ToArray());
    }

    [TestMethod]
    public void TotalPages_CountsAPartialPage()
    {
        Assert.AreEqual(3, Enumerable.Range(1, 5).TotalPages(2));
        Assert.AreEqual(0, Enumerable.Empty<int>().TotalPages(2));
        Assert.AreEqual(0, ((IEnumerable<int>?)null).TotalPages(2));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ItemsPerPageZeroOrNegative_Throws(int itemsPerPage)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(1, 5).TotalPages(itemsPerPage));
        Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(1, 5).ItemsPerPage(itemsPerPage));
        Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(1, 5).Page(itemsPerPage, 1));
    }
}

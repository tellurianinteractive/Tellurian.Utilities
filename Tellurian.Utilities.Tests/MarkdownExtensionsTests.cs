using Tellurian.Utilities.Web;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class MarkdownExtensionsTests
{
    [TestMethod]
    public void HtmlFromMarkdown_Heading_ReturnsHeadingElement() =>
        Assert.AreEqual("<h2 id=\"release-1.0.0\">Release 1.0.0</h2>", "## Release 1.0.0".HtmlFromMarkdown().Trim());

    [TestMethod]
    public void HtmlFromMarkdown_BulletsAndBold_ReturnsListWithStrong()
    {
        var html = "- First **bold** item\n- Second item".HtmlFromMarkdown();

        Assert.Contains("<ul>", html);
        Assert.Contains("<li>First <strong>bold</strong> item</li>", html);
        Assert.Contains("<li>Second item</li>", html);
    }

    [TestMethod]
    public void HtmlFromMarkdown_Table_UsesAdvancedExtensions() =>
        Assert.Contains("<table>", "| A | B |\n|---|---|\n| 1 | 2 |".HtmlFromMarkdown());

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void HtmlFromMarkdown_Empty_ReturnsEmptyString(string? markdown) =>
        Assert.AreEqual(string.Empty, markdown.HtmlFromMarkdown());

    [TestMethod]
    public void DefaultPipeline_IsAvailable() =>
        Assert.IsNotNull(MarkdownExtensions.DefaultPipeline);
}
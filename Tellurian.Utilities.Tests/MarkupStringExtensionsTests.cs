using Tellurian.Utilities.Web;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class MarkupStringExtensionsTests
{
    [TestMethod]
    public void Span_Text_IsWrapped() =>
        Assert.AreEqual("""<span class="note">Hello</span>""", "Hello".Span("note"));

    [TestMethod]
    public void Div_Text_IsWrappedAndClosed() =>
        Assert.AreEqual("""<div class="note">Hello</div>""", "Hello".Div("note"));

    [TestMethod]
    public void Span_WithoutClass_HasNoClassAttribute() =>
        Assert.AreEqual("<span>Hello</span>", "Hello".Span());

    [TestMethod]
    public void Span_ColorName_IsWrappedLikeAnyText() =>
        Assert.AreEqual("<span>white</span>", "white".Span());

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void SpanAndDiv_EmptyText_AreEmpty(string? text)
    {
        Assert.AreEqual(string.Empty, text.Span("note"));
        Assert.AreEqual(string.Empty, text.Div("note"));
    }

    [TestMethod]
    public void Div_TextAndClass_AreHtmlEncoded() =>
        Assert.AreEqual("""<div class="a&quot;b">&lt;script&gt;x&lt;/script&gt;</div>""", "<script>x</script>".Div("a\"b"));
}

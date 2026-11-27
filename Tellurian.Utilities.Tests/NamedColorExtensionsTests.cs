using Tellurian.Utilities.Web;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class NamedColorExtensionsTests
{
    [TestMethod]
    public void ToNamedColors_ReturnsNamesAndStandardHexCodesInOrder()
    {
        var colors = new[] { "Yellow", "Orange", "Pink", "darkblue" }.ToNamedColors();

        Assert.AreSequenceEqual(new[] { "Yellow", "Orange", "Pink", "darkblue" }, colors.Select(c => c.Name).ToArray());
        Assert.AreSequenceEqual(new[] { "#FFFF00", "#FFA500", "#FFC0CB", "#00008B" }, colors.Select(c => c.HexColor).ToArray());
    }

    [TestMethod]
    public void ToNamedColors_HasTheTextColor()
    {
        var colors = new[] { "Black", "Orange" }.ToNamedColors();

        Assert.AreEqual("#FFFFFF", colors[0].TextColor);
        Assert.AreEqual("#000000", colors[1].TextColor, "Standard orange (#FFA500) is light enough for black text.");
    }

    [TestMethod]
    public void ToNamedColors_UnknownName_Throws() =>
        Assert.Throws<ArgumentException>(() => new[] { "Red", "Reddish" }.ToNamedColors());

    [TestMethod]
    public void ToNamedColors_Empty_ReturnsEmpty() =>
        Assert.IsEmpty(Array.Empty<string>().ToNamedColors());
}
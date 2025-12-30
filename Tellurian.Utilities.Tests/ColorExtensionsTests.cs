using Tellurian.Utilities.Web;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ColorExtensionsTests
{
    #region IsHexColor

    [TestMethod]
    public void IsHexColor_ValidHex_ReturnsTrue() =>
        Assert.IsTrue("#FF00AA".IsHexColor);

    [TestMethod]
    public void IsHexColor_ValidHexLowercase_ReturnsTrue() =>
        Assert.IsTrue("#ff00aa".IsHexColor);

    [TestMethod]
    public void IsHexColor_MixedCase_ReturnsTrue() =>
        Assert.IsTrue("#Ff00Aa".IsHexColor);

    [TestMethod]
    public void IsHexColor_WithoutHash_ReturnsFalse() =>
        Assert.IsFalse("FF00AA".IsHexColor);

    [TestMethod]
    public void IsHexColor_TooShort_ReturnsFalse() =>
        Assert.IsFalse("#FFF".IsHexColor);

    [TestMethod]
    public void IsHexColor_TooLong_ReturnsFalse() =>
        Assert.IsFalse("#FF00AABB".IsHexColor);

    [TestMethod]
    public void IsHexColor_InvalidChars_ReturnsFalse() =>
        Assert.IsFalse("#GGGGGG".IsHexColor);

    [TestMethod]
    public void IsHexColor_ColorName_ReturnsFalse() =>
        Assert.IsFalse("Red".IsHexColor);

    [TestMethod]
    public void IsHexColor_Null_ReturnsFalse()
    {
        string? color = null;
        Assert.IsFalse(color.IsHexColor);
    }

    [TestMethod]
    public void IsHexColor_Empty_ReturnsFalse() =>
        Assert.IsFalse("".IsHexColor);

    #endregion

    #region HexColor

    [TestMethod]
    public void HexColor_AlreadyHex_ReturnsSame() =>
        Assert.AreEqual("#FF0000", "#FF0000".HexColor);

    [TestMethod]
    public void HexColor_KnownColorName_ReturnsHex() =>
        Assert.AreEqual("#FF0000", "Red".HexColor);

    [TestMethod]
    public void HexColor_Black_ReturnsCorrectHex() =>
        Assert.AreEqual("#000000", "Black".HexColor);

    [TestMethod]
    public void HexColor_White_ReturnsCorrectHex() =>
        Assert.AreEqual("#FFFFFF", "White".HexColor);

    [TestMethod]
    public void HexColor_UnknownColor_ReturnsWhite() =>
        Assert.AreEqual("#FFFFFF", "UnknownColor".HexColor);

    [TestMethod]
    [Description("Color names are now case-insensitive")]
    public void HexColor_LowercaseColorName_ReturnsCorrectHex() =>
        Assert.AreEqual("#000000", "black".HexColor);

    [TestMethod]
    [Description("Color names are now case-insensitive")]
    public void HexColor_LowercaseRed_ReturnsCorrectHex() =>
        Assert.AreEqual("#FF0000", "red".HexColor);

    [TestMethod]
    public void HexColor_MixedCaseColorName_ReturnsCorrectHex() =>
        Assert.AreEqual("#0000FF", "bLuE".HexColor);

    [TestMethod]
    public void HexColor_NullValue_ReturnsWhite()
    {
        string? color = null;
        Assert.AreEqual("#FFFFFF", color.HexColor);
    }

    [TestMethod]
    public void HexColor_EmptyValue_ReturnsWhite() =>
        Assert.AreEqual("#FFFFFF", "".HexColor);

    [TestMethod]
    public void HexColor_GrayBritishSpelling_ReturnsCorrectHex() =>
        Assert.AreEqual("#808080", "Grey".HexColor);

    [TestMethod]
    public void HexColor_GrayAmericanSpelling_ReturnsCorrectHex() =>
        Assert.AreEqual("#808080", "Gray".HexColor);

    #endregion

    #region IsWhite

    [TestMethod]
    public void IsWhite_HexWhite_ReturnsTrue() =>
        Assert.IsTrue("#FFFFFF".IsWhite);

    [TestMethod]
    public void IsWhite_HexWhiteLowercase_ReturnsTrue() =>
        Assert.IsTrue("#ffffff".IsWhite);

    [TestMethod]
    public void IsWhite_EmptyString_ReturnsTrue() =>
        Assert.IsTrue("".IsWhite);

    [TestMethod]
    public void IsWhite_Null_ReturnsTrue()
    {
        string? color = null;
        Assert.IsTrue(color.IsWhite);
    }

    [TestMethod]
    [Description("'White' color name is now recognized as white (case-insensitive)")]
    public void IsWhite_WhiteColorName_ReturnsTrue() =>
        Assert.IsTrue("White".IsWhite);

    [TestMethod]
    public void IsWhite_LowercaseWhiteColorName_ReturnsTrue() =>
        Assert.IsTrue("white".IsWhite);

    [TestMethod]
    public void IsWhite_MixedCaseWhiteColorName_ReturnsTrue() =>
        Assert.IsTrue("WHITE".IsWhite);

    [TestMethod]
    public void IsWhite_BlackHex_ReturnsFalse() =>
        Assert.IsFalse("#000000".IsWhite);

    [TestMethod]
    public void IsWhite_RedHex_ReturnsFalse() =>
        Assert.IsFalse("#FF0000".IsWhite);

    #endregion

    #region TextColor

    [TestMethod]
    public void TextColor_WhiteBackground_ReturnsBlackText() =>
        Assert.AreEqual("#000000", "#FFFFFF".TextColor);

    [TestMethod]
    public void TextColor_BlackBackground_ReturnsWhiteText() =>
        Assert.AreEqual("#FFFFFF", "#000000".TextColor);

    [TestMethod]
    public void TextColor_DarkBlue_ReturnsWhiteText() =>
        Assert.AreEqual("#FFFFFF", "#000080".TextColor);

    [TestMethod]
    public void TextColor_LightYellow_ReturnsBlackText() =>
        Assert.AreEqual("#000000", "#FFFF00".TextColor);

    [TestMethod]
    public void TextColor_MidGray_ReturnsBlackText() =>
        Assert.AreEqual("#000000", "#808080".TextColor);

    [TestMethod]
    public void TextColor_ColorName_WorksViaHexConversion() =>
        Assert.AreEqual("#FFFFFF", "Black".TextColor);

    [TestMethod]
    public void TextColor_UnknownColorName_DefaultsToWhiteBackground() =>
        Assert.AreEqual("#000000", "UnknownColor".TextColor);

    [TestMethod]
    public void TextColor_NullValue_DefaultsToWhiteBackground()
    {
        string? color = null;
        Assert.AreEqual("#000000", color.TextColor);
    }

    [TestMethod]
    public void TextColor_EmptyValue_DefaultsToWhiteBackground() =>
        Assert.AreEqual("#000000", "".TextColor);

    [TestMethod]
    [Description("Lowercase color names now work correctly")]
    public void TextColor_LowercaseBlack_ReturnsWhiteText() =>
        Assert.AreEqual("#FFFFFF", "black".TextColor);

    #endregion
}

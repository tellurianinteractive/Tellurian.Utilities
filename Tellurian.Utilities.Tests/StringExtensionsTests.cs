namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class StringExtensionsTests
{
    #region IsEmpty / HasValue

    [TestMethod]
    public void IsEmpty_Null_ReturnsTrue() =>
        Assert.IsTrue(((string?)null).IsEmpty);

    [TestMethod]
    public void IsEmpty_EmptyString_ReturnsTrue() =>
        Assert.IsTrue("".IsEmpty);

    [TestMethod]
    public void IsEmpty_WhitespaceOnly_ReturnsTrue() =>
        Assert.IsTrue("   ".IsEmpty);

    [TestMethod]
    public void HasValue_ValidString_ReturnsTrue() =>
        Assert.IsTrue("test".HasValue);

    [TestMethod]
    public void HasValue_WhitespaceOnly_ReturnsFalse() =>
        Assert.IsFalse("   ".HasValue);

    #endregion

    #region OrElse / OrDefault

    [TestMethod]
    public void OrElse_NullValue_ReturnsAlternative()
    {
        string? value = null;
        Assert.AreEqual("default", value.OrElse("default"));
    }

    [TestMethod]
    public void OrElse_HasValue_ReturnsOriginal() =>
        Assert.AreEqual("original", "original".OrElse("default"));

    [TestMethod]
    public void OrElse_WhitespaceValue_ReturnsAlternative() =>
        Assert.AreEqual("default", "   ".OrElse("default"));

    #endregion

    #region OrException

    [TestMethod]
    public void OrException_HasValue_ReturnsValue() =>
        Assert.AreEqual("test", "test".OrException());

    [TestMethod]
    public void OrException_NullValue_ThrowsNullReferenceException()
    {
        string? value = null;
        Assert.Throws<NullReferenceException>(() => value.OrException());
    }

    [TestMethod]
    public void OrException_WhitespaceOnly_ThrowsNullReferenceException() =>
        Assert.Throws<NullReferenceException>(() => "   ".OrException());

    #endregion

    #region UntilOrEmpty

    [TestMethod]
    public void UntilOrEmpty_StopCharFound_ReturnsSubstring() =>
        Assert.AreEqual("hello", "hello@world.com".UntilOrEmpty(['@']));

    [TestMethod]
    public void UntilOrEmpty_MultipleStopChars_StopsAtFirst() =>
        Assert.AreEqual("hello", "hello.world@test".UntilOrEmpty(['@', '.']));

    [TestMethod]
    public void UntilOrEmpty_NoStopCharFound_ReturnsEntireValue() =>
        Assert.AreEqual("hello", "hello".UntilOrEmpty(['@']));

    [TestMethod]
    public void UntilOrEmpty_StopCharAtPositionZero_ReturnsEmpty() =>
        Assert.AreEqual(string.Empty, "@hello".UntilOrEmpty(['@']));

    [TestMethod]
    public void UntilOrEmpty_EmptyStopAtArray_ReturnsEmpty() =>
        Assert.AreEqual(string.Empty, "hello".UntilOrEmpty([]));

    [TestMethod]
    public void UntilOrEmpty_NullValue_ReturnsEmpty()
    {
        string? value = null;
        Assert.AreEqual(string.Empty, value.UntilOrEmpty(['@']));
    }

    [TestMethod]
    public void UntilOrEmpty_EmptyValue_ReturnsEmpty() =>
        Assert.AreEqual(string.Empty, "".UntilOrEmpty(['@']));

    #endregion

    #region EqualsCaseInsensitive

    [TestMethod]
    public void EqualsCaseInsensitive_SameCase_ReturnsTrue() =>
        Assert.IsTrue("Hello".EqualsCaseInsensitive("Hello"));

    [TestMethod]
    public void EqualsCaseInsensitive_DifferentCase_ReturnsTrue() =>
        Assert.IsTrue("HELLO".EqualsCaseInsensitive("hello"));

    [TestMethod]
    public void EqualsCaseInsensitive_DifferentStrings_ReturnsFalse() =>
        Assert.IsFalse("Hello".EqualsCaseInsensitive("World"));

    [TestMethod]
    [Description("Both null values are now considered equal")]
    public void EqualsCaseInsensitive_BothNull_ReturnsTrue()
    {
        string? value = null;
        Assert.IsTrue(value.EqualsCaseInsensitive(null));
    }

    [TestMethod]
    public void EqualsCaseInsensitive_ValueNull_ReturnsFalse()
    {
        string? value = null;
        Assert.IsFalse(value.EqualsCaseInsensitive("test"));
    }

    [TestMethod]
    public void EqualsCaseInsensitive_OtherNull_ReturnsFalse() =>
        Assert.IsFalse("test".EqualsCaseInsensitive(null));

    [TestMethod]
    public void EqualsCaseInsensitive_BothEmpty_ReturnsTrue() =>
        Assert.IsTrue("".EqualsCaseInsensitive(""));

    #endregion

    #region IsAnyOf

    [TestMethod]
    public void IsAnyOf_MatchFound_ReturnsTrue() =>
        Assert.IsTrue("test".IsAnyOf("foo", "test", "bar"));

    [TestMethod]
    public void IsAnyOf_CaseInsensitiveMatch_ReturnsTrue() =>
        Assert.IsTrue("TEST".IsAnyOf("test"));

    [TestMethod]
    public void IsAnyOf_NoMatch_ReturnsFalse() =>
        Assert.IsFalse("test".IsAnyOf("foo", "bar"));

    [TestMethod]
    public void IsAnyOf_EmptyArray_ReturnsFalse() =>
        Assert.IsFalse("test".IsAnyOf());

    [TestMethod]
    public void IsAnyOf_CommaSeparated_MatchFound_ReturnsTrue() =>
        Assert.IsTrue("test".IsAnyOfCommaSeparated("foo, test, bar"));

    [TestMethod]
    public void IsAnyOf_CommaSeparated_NullInput_ReturnsFalse() =>
        Assert.IsFalse("test".IsAnyOfCommaSeparated(null));

    #endregion

    #region IsAnyPartOf

    [TestMethod]
    public void IsAnyPartOf_ContainsMatch_ReturnsTrue() =>
        Assert.IsTrue("hello world".IsAnyPartOf("wor"));

    [TestMethod]
    public void IsAnyPartOf_CaseInsensitive_ReturnsTrue() =>
        Assert.IsTrue("Hello World".IsAnyPartOf("WORLD"));

    [TestMethod]
    public void IsAnyPartOf_NoMatch_ReturnsFalse() =>
        Assert.IsFalse("hello".IsAnyPartOf("xyz"));

    [TestMethod]
    public void IsAnyPartOf_NullValue_ReturnsFalse()
    {
        string? value = null;
        Assert.IsFalse(value.IsAnyPartOf("test"));
    }


    #endregion

    #region ToItems

    private static readonly string[] expected_abc = ["a", "b", "c"];
    private static readonly string[] expected_ab = ["a", "b"];

    [TestMethod]
    public void ToItems_DefaultSeparator_SplitsCorrectly()
    {
        var result = "a;b;c".ToItems();
        CollectionAssert.AreEqual(expected_abc, result);
    }

    [TestMethod]
    public void ToItems_CustomSeparator_SplitsCorrectly()
    {
        var result = "a,b,c".ToItems(',');
        CollectionAssert.AreEqual(expected_abc, result);
    }

    [TestMethod]
    public void ToItems_TrimsWhitespace()
    {
        var result = " a ; b ; c ".ToItems();
        CollectionAssert.AreEqual(expected_abc, result);
    }

    [TestMethod]
    public void ToItems_RemovesEmptyEntries()
    {
        var result = "a;;b".ToItems();
        CollectionAssert.AreEqual(expected_ab, result);
    }

    [TestMethod]
    public void ToItems_NullValue_ReturnsEmptyArray()
    {
        string? value = null;
        var result = value.ToItems();
        Assert.AreEqual(0, result.Length);
    }

    #endregion

    #region WithHtmlRemoved

    [TestMethod]
    public void WithHtmlRemoved_RemovesTags() =>
        Assert.AreEqual("Hello World", "<p>Hello</p> <b>World</b>".WithHtmlRemoved);

    [TestMethod]
    public void WithHtmlRemoved_RemovesNbsp() =>
        Assert.AreEqual("Hello World", "Hello&nbsp;World".WithHtmlRemoved);

    [TestMethod]
    public void WithHtmlRemoved_RemovesNbspWithoutSemicolon() =>
        Assert.AreEqual("Hello World", "Hello&nbspWorld".WithHtmlRemoved);

    [TestMethod]
    public void WithHtmlRemoved_NullValue_ReturnsEmpty()
    {
        string? value = null;
        Assert.AreEqual(string.Empty, value.WithHtmlRemoved);
    }

    #endregion

    #region WithQuotationMarksRemoved

    [TestMethod]
    public void WithQuotationMarksRemoved_StraightDoubleQuotes_Removed() =>
        Assert.AreEqual("Hello World", "\"Hello World\"".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_StraightSingleQuotes_Removed() =>
        Assert.AreEqual("Hello World", "'Hello World'".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_CurlyDoubleQuotes_Removed() =>
        Assert.AreEqual("Hello World", "\u201CHello World\u201D".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_CurlySingleQuotes_Removed() =>
        Assert.AreEqual("Hello World", "\u2018Hello World\u2019".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_GermanQuotes_Removed() =>
        Assert.AreEqual("Hallo Welt", "\u201EHallo Welt\u201C".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_FrenchGuillemets_Removed() =>
        Assert.AreEqual("Bonjour le monde", "\u00ABBonjour le monde\u00BB".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_SingleGuillemets_Removed() =>
        Assert.AreEqual("Hej verden", "\u2039Hej verden\u203A".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_MixedQuotes_AllRemoved() =>
        Assert.AreEqual("She said Hello to him", "She said \u00ABHello\u00BB to 'him'".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_NoQuotes_ReturnsOriginal() =>
        Assert.AreEqual("Hello World", "Hello World".WithQuotationMarksRemoved);

    [TestMethod]
    public void WithQuotationMarksRemoved_NullValue_ReturnsEmpty()
    {
        string? value = null;
        Assert.AreEqual(string.Empty, value.WithQuotationMarksRemoved);
    }

    [TestMethod]
    public void WithQuotationMarksRemoved_EmptyString_ReturnsEmpty() =>
        Assert.AreEqual(string.Empty, "".WithQuotationMarksRemoved);

    #endregion

    #region Version 2.0

    [TestMethod]
    public void IsAnyOf_SingleValueWithComma_IsOneValue() =>
        Assert.IsFalse("a".IsAnyOf("a,b"), "A single string is one value; IsAnyOfCommaSeparated splits.");

    [TestMethod]
    public void IsAnyPartOfCommaSeparated_SplitsOnCommas() =>
        Assert.IsTrue("hello world".IsAnyPartOfCommaSeparated("xyz, wor"));

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", null)]
    [DataRow("   ", null)]
    [DataRow(" text ", "text")]
    public void OrNullIfEmpty(string? value, string? expected) =>
        Assert.AreEqual(expected, value.OrNullIfEmpty);

    [TestMethod]
    [DataRow("a@b.se;c@d.se", ';', "a@b.se")]
    [DataRow(" a@b.se ; c@d.se", ';', "a@b.se")]
    [DataRow("sv,en", ',', "sv")]
    [DataRow("", ';', "default")]
    public void FirstItem_WithSeparator(string value, char separator, string expected) =>
        Assert.AreEqual(expected, value.FirstItem("default", separator));

    [TestMethod]
    public void FirstItem_CommaByDefault() =>
        Assert.AreEqual("sv", "sv,en".FirstItem());

    [TestMethod]
    [DataRow("Piece", false)]
    [DataRow("piece", false)]
    [DataRow("Wagon", true)]
    [DataRow("", false)]
    [DataRow(null, false)]
    public void HasValueExcept_Strings_CaseInsensitive(string? value, bool expected) =>
        Assert.AreEqual(expected, value.HasValueExcept("Piece", "NotApplicable"));

    [TestMethod]
    [DataRow("O'Brien", "O'Brien")]
    [DataRow("Cortina d\u2019Ampezzo", "Cortina d\u2019Ampezzo")]
    [DataRow("'s-Hertogenbosch", "'s-Hertogenbosch")]
    [DataRow("Karl \"Kalle\" Svensson", "Karl Kalle Svensson")]
    [DataRow("\u201EZug\u201C \u00ABtrain\u00BB", "Zug train")]
    public void WithoutQuotationMarks_KeepsApostrophes(string value, string expected) =>
        Assert.AreEqual(expected, value.WithoutQuotationMarks());

    [TestMethod]
    public void WithoutQuotationMarks_ApostrophesToo() =>
        Assert.AreEqual("OBrien", "O'Brien".WithoutQuotationMarks(keepApostrophes: false));

    #endregion

    #region Truncated

    [TestMethod]
    [DataRow(null, 5, false, "")]
    [DataRow("", 5, false, "")]
    [DataRow("Kort", 5, false, "Kort")]
    [DataRow("Exakt", 5, false, "Exakt")]
    [DataRow("Längre text", 5, false, "Längr")]
    [DataRow("Längre text här", 13, true, "Längre text...")]
    [DataRow("Ordutanmellanslag", 5, true, "Ordut...")]
    [DataRow("Kort", 10, true, "Kort")]
    [DataRow("Text", 0, false, "")]
    public void Truncated(string? text, int maxLength, bool ellipsis, string expected) =>
        Assert.AreEqual(expected, text.Truncated(maxLength, ellipsis));

    #endregion

    #region WordWrapped

    [TestMethod]
    [DataRow(null, 10, "")]
    [DataRow("Kort text", 10, "Kort text")]
    [DataRow("Ett två tre fyra", 7, "Ett två|tre|fyra")]
    [DataRow("Ett två tre fyra", 8, "Ett två|tre fyra")]
    [DataRow("Långtordutanmellanslag", 10, "Långtordut|anmellansl|ag")]
    [DataRow("Rad ett som är lång\nKort", 10, "Rad ett|som är|lång|Kort")]
    public void WordWrapped(string? text, int width, string expected) =>
        Assert.AreEqual(expected, text.WordWrapped(width, "|"));

    [TestMethod]
    public void WordWrappedBreaksLinesWithNewLineByDefault() =>
        Assert.AreEqual("Ett\ntvå", "Ett två".WordWrapped(4));

    #endregion

    #region Guid

    [TestMethod]
    [DataRow("3F2504E0-4F89-11D3-9A0C-0305E82C3301", true)]
    [DataRow(" 3f2504e0-4f89-11d3-9a0c-0305e82c3301 ", true)]
    [DataRow("{3F2504E0-4F89-11D3-9A0C-0305E82C3301}", true)]
    [DataRow("inte en guid", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    public void ToGuidOrNull(string? text, bool expected) =>
        Assert.AreEqual(expected, text.ToGuidOrNull.HasValue);

    [TestMethod]
    public void ToGuidOrNewKeepsAGuid() =>
        Assert.AreEqual(Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3301"), "3F2504E0-4F89-11D3-9A0C-0305E82C3301".ToGuidOrNew());

    [TestMethod]
    public void ToGuidOrNewGivesANewGuid() =>
        Assert.AreNotEqual(Guid.Empty, ((string?)null).ToGuidOrNew());

    #endregion
}
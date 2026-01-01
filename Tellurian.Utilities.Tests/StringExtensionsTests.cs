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
        Assert.IsTrue("test".IsAnyOf("foo, test, bar"));

    [TestMethod]
    public void IsAnyOf_CommaSeparated_NullInput_ReturnsFalse() =>
        Assert.IsFalse("test".IsAnyOf((string?)null));

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
}

using Tellurian.Utilities;

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
    public void OrDefault_NullValue_ReturnsDefault()
    {
        string? value = null;
        Assert.AreEqual("default", value.OrDefault("default"));
    }

    [TestMethod]
    public void OrDefault_HasValue_ReturnsOriginal() =>
        Assert.AreEqual("original", "original".OrDefault("default"));

    [TestMethod]
    public void OrElse_WhitespaceValue_ReturnsAlternative() =>
        Assert.AreEqual("default", "   ".OrElse("default"));

    #endregion

    #region OrException

    [TestMethod]
    public void OrException_HasValue_ReturnsValue() =>
        Assert.AreEqual("test", "test".OrException);

    [TestMethod]
    public void OrException_NullValue_ThrowsNullReferenceException()
    {
        string? value = null;
        try
        {
            _ = value.OrException;
            Assert.Fail("Expected NullReferenceException");
        }
        catch (NullReferenceException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void OrException_WhitespaceOnly_ThrowsNullReferenceException()
    {
        try
        {
            _ = "   ".OrException;
            Assert.Fail("Expected NullReferenceException");
        }
        catch (NullReferenceException)
        {
            // Expected
        }
    }

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

    #region AnyOf

    [TestMethod]
    public void AnyOf_MatchFound_ReturnsTrue() =>
        Assert.IsTrue("test".AnyOf("foo", "test", "bar"));

    [TestMethod]
    public void AnyOf_CaseInsensitiveMatch_ReturnsTrue() =>
        Assert.IsTrue("TEST".AnyOf("test"));

    [TestMethod]
    public void AnyOf_NoMatch_ReturnsFalse() =>
        Assert.IsFalse("test".AnyOf("foo", "bar"));

    [TestMethod]
    public void AnyOf_EmptyArray_ReturnsFalse() =>
        Assert.IsFalse("test".AnyOf());

    [TestMethod]
    public void AnyOf_CommaSeparated_MatchFound_ReturnsTrue() =>
        Assert.IsTrue("test".AnyOf("foo, test, bar"));

    [TestMethod]
    public void AnyOf_CommaSeparated_NullInput_ReturnsFalse() =>
        Assert.IsFalse("test".AnyOf((string?)null));

    #endregion

    #region AnyPartOf

    [TestMethod]
    public void AnyPartOf_ContainsMatch_ReturnsTrue() =>
        Assert.IsTrue("hello world".AnyPartOf("wor"));

    [TestMethod]
    public void AnyPartOf_CaseInsensitive_ReturnsTrue() =>
        Assert.IsTrue("Hello World".AnyPartOf("WORLD"));

    [TestMethod]
    public void AnyPartOf_NoMatch_ReturnsFalse() =>
        Assert.IsFalse("hello".AnyPartOf("xyz"));

    [TestMethod]
    public void AnyPartOf_NullValue_ReturnsFalse()
    {
        string? value = null;
        Assert.IsFalse(value.AnyPartOf("test"));
    }

    #endregion

    #region ToItems

    [TestMethod]
    public void ToItems_DefaultSeparator_SplitsCorrectly()
    {
        var result = "a;b;c".ToItems();
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result);
    }

    [TestMethod]
    public void ToItems_CustomSeparator_SplitsCorrectly()
    {
        var result = "a,b,c".ToItems(',');
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result);
    }

    [TestMethod]
    public void ToItems_TrimsWhitespace()
    {
        var result = " a ; b ; c ".ToItems();
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result);
    }

    [TestMethod]
    public void ToItems_RemovesEmptyEntries()
    {
        var result = "a;;b".ToItems();
        CollectionAssert.AreEqual(new[] { "a", "b" }, result);
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
}

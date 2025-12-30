using Tellurian.Utilities;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ObjectExtensionsTests
{
    #region HasValue

    [TestMethod]
    public void HasValue_NonNullObject_ReturnsTrue()
    {
        object obj = new();
        Assert.IsTrue(obj.HasValue);
    }

    [TestMethod]
    public void HasValue_NullObject_ReturnsFalse()
    {
        object? obj = null;
        Assert.IsFalse(obj.HasValue);
    }

    [TestMethod]
    public void HasValue_String_ReturnsTrue()
    {
        object obj = "test";
        Assert.IsTrue(obj.HasValue);
    }

    [TestMethod]
    public void HasValue_BoxedValueType_ReturnsTrue()
    {
        object obj = 42;
        Assert.IsTrue(obj.HasValue);
    }

    #endregion

    #region HasValueExcept

    [TestMethod]
    public void HasValueExcept_NonNullDifferentValue_ReturnsTrue()
    {
        object obj = "test";
        Assert.IsTrue(obj.HasValueExcept("other"));
    }

    [TestMethod]
    public void HasValueExcept_NonNullSameValue_ReturnsFalse()
    {
        object obj = "test";
        Assert.IsFalse(obj.HasValueExcept("test"));
    }

    [TestMethod]
    public void HasValueExcept_NullValue_ReturnsFalse()
    {
        object? obj = null;
        Assert.IsFalse(obj.HasValueExcept("test"));
    }

    [TestMethod]
    public void HasValueExcept_NullExcept_ReturnsTrue()
    {
        object obj = "test";
        Assert.IsTrue(obj.HasValueExcept(null!));
    }

    [TestMethod]
    public void HasValueExcept_BothNull_ReturnsFalse()
    {
        object? obj = null;
        Assert.IsFalse(obj.HasValueExcept(null!));
    }

    [TestMethod]
    public void HasValueExcept_IntegerComparison_WorksCorrectly()
    {
        object obj = 42;
        Assert.IsFalse(obj.HasValueExcept(42));
        Assert.IsTrue(obj.HasValueExcept(43));
    }

    #endregion

    #region ValueOrException

    [TestMethod]
    public void ValueOrException_NonNullValue_ReturnsValue()
    {
        string? value = "test";
        Assert.AreEqual("test", value.ValueOrException("paramName"));
    }

    [TestMethod]
    public void ValueOrException_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        try
        {
            _ = value.ValueOrException("paramName");
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void ValueOrException_NullValue_ExceptionContainsParameterName()
    {
        string? value = null;
        try
        {
            _ = value.ValueOrException("myParameter", "Value cannot be null");
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual("myParameter", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Value cannot be null"));
        }
    }

    [TestMethod]
    public void ValueOrException_WithCustomObject_ReturnsObject()
    {
        var obj = new TestClass { Value = 42 };
        TestClass? nullable = obj;
        Assert.AreSame(obj, nullable.ValueOrException("param"));
    }

    #endregion

    #region IfNotEqualsThrow

    [TestMethod]
    public void IfNotEqualsThrow_EqualValues_DoesNotThrow()
    {
        string value = "test";
        // Should not throw
        value.IfNotEqualsThrow("test", "paramName", "Values must be equal");
    }

    [TestMethod]
    public void IfNotEqualsThrow_DifferentValues_Throws()
    {
        string value = "test";
        try
        {
            value.IfNotEqualsThrow("other", "paramName", "Values must be equal");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void IfNotEqualsThrow_NullValue_Throws()
    {
        string? value = null;
        try
        {
            value.IfNotEqualsThrow("test", "paramName", "Values must be equal");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void IfNotEqualsThrow_ExceptionContainsCorrectParamName()
    {
        try
        {
            string value = "test";
            value.IfNotEqualsThrow("other", "myParameter", "Values must be equal");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("myParameter", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Values must be equal"));
        }
    }

    [TestMethod]
    public void IfNotEqualsThrow_WithoutMessage_StillThrows()
    {
        string value = "test";
        try
        {
            value.IfNotEqualsThrow("other", "paramName");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("paramName", ex.ParamName);
        }
    }

    #endregion

    private class TestClass
    {
        public int Value { get; set; }
    }
}

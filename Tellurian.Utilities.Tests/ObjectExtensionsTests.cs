using Tellurian.Utilities;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ObjectExtensionsTests
{
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

    [TestMethod]
    public void ValueOrException_NonNullValue_ReturnsValue()
    {
        var value = "test";
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
        var ex = Assert.Throws<ArgumentNullException>(() => value.ValueOrException("myParameter", "Value cannot be null"));
        Assert.AreEqual("myParameter", ex.ParamName);
        Assert.Contains("Value cannot be null", ex.Message);
    }

    [TestMethod]
    public void ValueOrException_WithCustomObject_ReturnsObject()
    {
        var obj = new TestClass { Value = 42 };
        var nullable = obj;
        Assert.AreSame(obj, nullable.ValueOrException("param"));
    }

    [TestMethod]
    public void IfNotEqualsThrow_EqualValues_DoesNotThrow()
    {
        var value = "test";
        // Should not throw
        value.IfNotEqualsThrow("test", "paramName", "Values must be equal");
    }

    [TestMethod]
    public void IfNotEqualsThrow_DifferentValues_Throws()
    {
        var value = "test";
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
        var value = "test";
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.IfNotEqualsThrow("other", "myParameter", "Values must be equal"));
        Assert.AreEqual("myParameter", ex.ParamName);
        Assert.Contains("Values must be equal", ex.Message);
    }

    [TestMethod]
    public void IfNotEqualsThrow_WithoutMessage_StillThrows()
    {
        var value = "test";
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.IfNotEqualsThrow("other", "paramName"));
        Assert.AreEqual("paramName", ex.ParamName);
    }

    private sealed class TestClass
    {
        public int Value { get; set; }
    }

    [TestMethod]
    public void ToSingleItemEnumerable_Value_ReturnsOneItem() =>
        Assert.HasCount(1, "text".ToSingleItemEnumerable());

    [TestMethod]
    public void ToSingleItemEnumerable_Null_ReturnsEmpty() =>
        Assert.IsEmpty(((string?)null).ToSingleItemEnumerable());

    [TestMethod]
    public void LinqAsEnumerable_OnAList_IsTheListItself()
    {
        List<int> numbers = [1, 2, 3];
        var asEnumerable = numbers.AsEnumerable();
        Assert.HasCount(3, asEnumerable, "Before 2.0, the package's AsEnumerable() won and wrapped the list as one item.");
    }
}
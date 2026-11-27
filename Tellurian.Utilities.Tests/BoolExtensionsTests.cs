using Tellurian.Utilities;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class BoolExtensionsTests
{
    [TestMethod]
    public void IfTrueThrows_False_DoesNotThrow()
    {
        // Should not throw
        false.IfTrueThrows("paramName");
    }

    [TestMethod]
    public void IfTrueThrows_True_ThrowsArgumentOutOfRangeException()
    {
        try
        {
            true.IfTrueThrows("paramName");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void IfTrueThrows_True_ExceptionContainsParameterName()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => true.IfTrueThrows("myParameter", "Value is invalid"));
        Assert.AreEqual("myParameter", ex.ParamName);
        Assert.Contains("Value is invalid", ex.Message);
    }

    [TestMethod]
    public void IfTrueThrows_True_WithNullMessage_ThrowsWithParameterNameOnly()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => true.IfTrueThrows("myParameter", null));
        Assert.AreEqual("myParameter", ex.ParamName);
    }
}
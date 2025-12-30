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
        try
        {
            true.IfTrueThrows("myParameter", "Value is invalid");
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("myParameter", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Value is invalid"));
        }
    }

    [TestMethod]
    public void IfTrueThrows_True_WithNullMessage_ThrowsWithParameterNameOnly()
    {
        try
        {
            true.IfTrueThrows("myParameter", null);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("myParameter", ex.ParamName);
        }
    }
}

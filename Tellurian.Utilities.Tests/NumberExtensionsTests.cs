namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class NumberExtensionsTests
{
    [TestMethod]
    public void IsNullOrZero()
    {
        Assert.IsTrue(((int?)null).IsNullOrZero());
        Assert.IsTrue(((short?)0).IsNullOrZero());
        Assert.IsTrue(((double?)0.0).IsNullOrZero());
        Assert.IsFalse(((decimal?)-1m).IsNullOrZero());
        Assert.IsFalse(((int?)5).IsNullOrZero());
    }

    [TestMethod]
    public void IsNullOrZeroOrNegative()
    {
        Assert.IsTrue(((int?)-1).IsNullOrZeroOrNegative());
        Assert.IsTrue(((short?)0).IsNullOrZeroOrNegative());
        Assert.IsFalse(((double?)0.5).IsNullOrZeroOrNegative());
    }

    [TestMethod]
    public void OrNullIfZero()
    {
        Assert.IsNull(((short?)0).OrNullIfZero());
        Assert.AreEqual((short?)5, ((short?)5).OrNullIfZero());
        Assert.AreEqual(-2.5m, ((decimal?)-2.5m).OrNullIfZero());
        Assert.IsNull(((int?)null).OrNullIfZero());
    }

    [TestMethod]
    public void OrNullIfZeroOrNegative()
    {
        Assert.IsNull(((short?)-1).OrNullIfZeroOrNegative());
        Assert.IsNull(((int?)0).OrNullIfZeroOrNegative());
        Assert.AreEqual((short?)1950, ((short?)1950).OrNullIfZeroOrNegative());
    }

    [TestMethod]
    public void IsAlsoOddOrEven()
    {
        Assert.IsTrue(3.IsAlsoOddOrEven(5));
        Assert.IsFalse(3.IsAlsoOddOrEven(4));
    }
}
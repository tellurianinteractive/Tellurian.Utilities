using Tellurian.Utilities;
using Tellurian.Utilities.Linq;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ListExtensionsTests
{
    [TestMethod]
    public void TryGetFirstValue_MatchFound_ReturnsTrueAndValue()
    {
        var list = new List<string> { "apple", "banana", "cherry" };

        var result = list.TryGetFirstValue(s => s.StartsWith("ban", StringComparison.Ordinal), out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("banana", value);
    }

    [TestMethod]
    public void TryGetFirstValue_NoMatch_ReturnsFalseAndNull()
    {
        var list = new List<string> { "apple", "banana", "cherry" };

        var result = list.TryGetFirstValue(s => s.StartsWith("xyz", StringComparison.Ordinal), out var value);

        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TryGetFirstValue_EmptyList_ReturnsFalse()
    {
        var list = new List<string>();

        var result = list.TryGetFirstValue(s => true, out var value);

        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void TryGetFirstValue_MultipleMatches_ReturnsFirst()
    {
        var list = new List<string> { "apple", "apricot", "avocado" };

        var result = list.TryGetFirstValue(s => s is ['a', ..], out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("apple", value);
    }

    [TestMethod]
    public void TryGetFirstValue_WithComplexObject_WorksCorrectly()
    {
        var list = new List<Person>
        {
            new() { Name = "Alice", Age = 30 },
            new() { Name = "Bob", Age = 25 },
            new() { Name = "Charlie", Age = 35 }
        };

        var result = list.TryGetFirstValue(p => p.Age > 30, out var value);

        Assert.IsTrue(result);
        Assert.IsNotNull(value);
        Assert.AreEqual("Charlie", value.Name);
    }

    [TestMethod]
    public void TryGetFirstValue_ValueTypeNoMatch_ReturnsFalse()
    {
        int[] numbers = [1, 2];

        var result = numbers.TryGetFirstValue(i => i > 5, out var value);

        Assert.IsFalse(result);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void TryGetFirstValue_ValueTypeMatchEqualToDefault_ReturnsTrue()
    {
        int[] numbers = [3, 0, 2];

        var result = numbers.TryGetFirstValue(i => i == 0, out var value);

        Assert.IsTrue(result);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void TryGetFirstValue_AllNullExceptOne_FindsNonNull()
    {
        var list = new List<string?> { null, null, "found", null };

        var result = list.TryGetFirstValue(s => s != null, out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("found", value);
    }

    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    [TestMethod]
    public void IndexOf_Match_ReturnsIndex() =>
        Assert.AreEqual(2, new[] { 5, 6, 7 }.IndexOf(i => i == 7));

    [TestMethod]
    public void IndexOf_NoMatch_ReturnsMinusOne() =>
        Assert.AreEqual(-1, new[] { 5, 6, 7 }.IndexOf(i => i == 8));

    [TestMethod]
    public void IsEmpty_NullEmptyAndNonEmptySequences()
    {
        List<int>? none = null;
        Assert.IsTrue(none.IsEmpty);
        Assert.IsTrue(none.IsNull);
        Assert.IsTrue(new List<int>().IsEmpty);
        Assert.IsTrue(Enumerable.Empty<string>().IsEmpty);
        Assert.IsFalse(Enumerable.Range(1, 3).Where(i => i > 2).IsEmpty);
        Assert.IsFalse(new[] { "a" }.IsEmpty);
    }

    [TestMethod]
    public void IsEmpty_OnString_IsTheStringRule() =>
        Assert.IsTrue("   ".IsEmpty);
}

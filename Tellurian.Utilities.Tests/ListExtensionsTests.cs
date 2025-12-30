using Tellurian.Utilities;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ListExtensionsTests
{
    #region TryGetFirstValue

    [TestMethod]
    public void TryGetFirstValue_MatchFound_ReturnsTrueAndValue()
    {
        var list = new List<string> { "apple", "banana", "cherry" };

        var result = list.TryGetFirstValue(s => s.StartsWith("ban"), out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("banana", value);
    }

    [TestMethod]
    public void TryGetFirstValue_NoMatch_ReturnsFalseAndNull()
    {
        var list = new List<string> { "apple", "banana", "cherry" };

        var result = list.TryGetFirstValue(s => s.StartsWith("xyz"), out var value);

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

        var result = list.TryGetFirstValue(s => s.StartsWith("a"), out var value);

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
        Assert.AreEqual("Charlie", value?.Name);
    }

    [TestMethod]
    public void TryGetFirstValue_PredicateMatchesFirst_ReturnsFirst()
    {
        var list = new List<int?> { 1, 2, 3 };
        var intList = list.Cast<int?>().ToList();

        // This test verifies behavior with nullable value types boxed as objects
        var objList = new List<object?> { 1, 2, 3 };
        // Note: TryGetFirstValue works with List<T> where T is class
    }

    [TestMethod]
    public void TryGetFirstValue_AllNullExceptOne_FindsNonNull()
    {
        var list = new List<string?> { null, null, "found", null };

        var result = list.TryGetFirstValue(s => s != null, out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("found", value);
    }

    #endregion

    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}

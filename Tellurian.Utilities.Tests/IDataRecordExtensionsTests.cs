using System.Data;
using Tellurian.Utilities.Data;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class IDataRecordExtensionsTests
{
    private static DataTableReader Reader(object?[] values)
    {
        using var table = new DataTable();
        table.Columns.Add("Text", typeof(string));
        table.Columns.Add("Number", typeof(int));
        table.Columns.Add("Short", typeof(short));
        table.Columns.Add("Real", typeof(double));
        table.Columns.Add("When", typeof(DateTime));
        table.Columns.Add("Flag", typeof(bool));
        table.Rows.Add([.. values.Select(v => v ?? DBNull.Value)]);
        var reader = table.CreateDataReader();
        reader.Read();
        return reader;
    }

    // The tests read through IDataRecord: on a DbDataReader variable, the BCL's own extensions in System.Data
    // (GetString(string), GetDouble(string), ...) are chosen before these.
    private static DataTableReader WithValues() =>
        Reader(["Station", 42, (short)7, 1.5, new DateTime(2026, 1, 1, 12, 0, 0), true]);

    private static DataTableReader WithNulls() =>
        Reader([null, null, null, null, null, null]);

    [TestMethod]
    public void Getters_ReadValues()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        Assert.AreEqual("Station", record.GetString("Text"));
        Assert.AreEqual(42, record.GetInt("Number"));
        Assert.AreEqual(7, record.GetInt("Short"));
        Assert.AreEqual(1.5, record.GetDouble("Real"));
        Assert.AreEqual(new DateOnly(2026, 1, 1), record.GetDate("When"));
        Assert.AreEqual(new TimeOnly(12, 0), record.GetTime("When"));
        Assert.AreEqual(1.5, record.GetTimeAsDouble("When"), 1e-12);
        Assert.IsTrue(record.GetBool("Flag"));
    }

    [TestMethod]
    public void MissingColumn_WithDefault_GivesTheDefault()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        Assert.AreEqual("none", record.GetString("Missing", "none"));
        Assert.AreEqual(5, record.GetInt("Missing", 5));
        Assert.AreEqual(5, record.GetIntOrNull("Missing", 5));
        Assert.AreEqual(2.5, record.GetDouble("Missing", 2.5));
        Assert.AreEqual(new DateOnly(2020, 2, 2), record.GetDate("Missing", new DateOnly(2020, 2, 2)));
        Assert.AreEqual(new TimeOnly(8, 0), record.GetTime("Missing", new TimeOnly(8, 0)));
        Assert.AreEqual(3.0, record.GetTimeAsDouble("Missing", 3.0));
        Assert.IsTrue(record.GetBool("Missing", true));
    }

    [TestMethod]
    public void MissingColumnByIndex_WithDefault_GivesTheDefault()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        Assert.AreEqual(5, record.GetInt(-1, 5));
        Assert.AreEqual(2.5, record.GetDouble(-1, 2.5));
        Assert.AreEqual(new DateOnly(2020, 2, 2), record.GetDate(-1, new DateOnly(2020, 2, 2)));
        Assert.IsFalse(record.GetBool(-1, false));
    }

    [TestMethod]
    public void MissingColumn_WithoutDefault_ThrowsInvalidOperationException()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        Assert.Throws<InvalidOperationException>(() => record.GetString("Missing"));
        Assert.Throws<InvalidOperationException>(() => record.GetInt("Missing"));
        Assert.Throws<InvalidOperationException>(() => record.GetDate("Missing"));
        Assert.Throws<InvalidOperationException>(() => record.GetTimeAsDouble("Missing"));
        Assert.Throws<InvalidOperationException>(() => record.GetInt(-1));
        Assert.Throws<InvalidOperationException>(() => record.GetDouble(-1, null));
    }

    [TestMethod]
    public void NullValue_WithDefault_GivesTheDefault()
    {
        using var reader = WithNulls();
        IDataRecord record = reader;
        Assert.AreEqual("none", record.GetString("Text", "none"));
        Assert.AreEqual(5, record.GetInt("Number", 5));
        Assert.IsNull(record.GetIntOrNull("Number"));
        Assert.AreEqual(new TimeOnly(8, 0), record.GetTime("When", new TimeOnly(8, 0)));
    }

    [TestMethod]
    public void NullValue_WithoutDefault()
    {
        using var reader = WithNulls();
        IDataRecord record = reader;
        Assert.AreEqual(string.Empty, record.GetString("Text"));
        Assert.AreEqual(TimeOnly.MinValue, record.GetTimeAsTimespan("When"));
        var exception = Assert.Throws<InvalidOperationException>(() => record.GetInt("Number"));
        Assert.Contains("Number", exception.Message);
    }

    [TestMethod]
    public void UnsupportedType_ThrowsWithTheCause()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        var exception = Assert.Throws<InvalidOperationException>(() => record.GetString("Number"));
        Assert.IsInstanceOfType<InvalidCastException>(exception.InnerException);
        Assert.Throws<InvalidOperationException>(() => record.GetInt("Text"));
    }

    [TestMethod]
    public void GetColumnIndexes_AndColumns_GiveOrdinals()
    {
        using var reader = WithValues();
        IDataRecord record = reader;
        var indexes = record.GetColumnIndexes(["Real", "Text"]);
        Assert.AreEqual(3, indexes["Real"]);
        Assert.AreEqual(0, indexes["Text"]);
        Assert.AreEqual(3, new Columns(record, "Real").Ordinal("Real"));
        Assert.Throws<InvalidOperationException>(() => new Columns(record, "Missing"));
    }
}

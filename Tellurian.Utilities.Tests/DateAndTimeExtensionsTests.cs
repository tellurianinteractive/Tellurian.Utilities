using Tellurian.Utilities;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class DateAndTimeExtensionsTests
{
    [TestMethod]
    public void DateTime_ToDouble_ReturnsCorrectFractionalDayOfYear()
    {
        var dateTime = new DateTime(2024, 1, 1, 12, 30, 0); // Jan 1, 12:30
        var result = dateTime.ToDouble;

        // DayOfYear = 1, Hour/60 = 12/60 = 0.2, Minute/3600 = 30/3600 ≈ 0.0083
        Assert.AreEqual(1 + 12.0 / 60 + 30.0 / 3600, result);
    }

    [TestMethod]
    public void DateTime_ToDouble_AtMidnight_ReturnsDayOfYear()
    {
        var dateTime = new DateTime(2024, 3, 1, 0, 0, 0); // March 1, midnight
        var result = dateTime.ToDouble;

        Assert.AreEqual(61, result); // Day 61 in leap year (31 Jan + 29 Feb + 1)
    }

    [TestMethod]
    public void TimeSpan_ToDouble_ReturnsCorrectFractionalDays()
    {
        var timeSpan = new TimeSpan(1, 12, 30, 0); // 1 day, 12 hours, 30 minutes
        var result = timeSpan.ToDouble;

        // TotalDays = 1.52083..., Hours/60 = 12/60 = 0.2, Minutes/3600 = 30/3600 ≈ 0.0083
        Assert.AreEqual(timeSpan.TotalDays + 12.0 / 60 + 30.0 / 3600, result);
    }

    [TestMethod]
    public void TimeSpan_ToDouble_ZeroTimeSpan_ReturnsZero()
    {
        var timeSpan = TimeSpan.Zero;
        var result = timeSpan.ToDouble;

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Double_TimeOnly_ConvertsMinutesToTimeOnly()
    {
        double minutes = 90.0; // 1 hour 30 minutes
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(1, 30), result);
    }

    [TestMethod]
    public void Double_TimeOnly_ZeroMinutes_ReturnsMidnight()
    {
        double minutes = 0.0;
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(0, 0), result);
    }

    [TestMethod]
    public void Double_TimeOnly_FullDay_ReturnsCorrectTime()
    {
        double minutes = 1439.0; // 23 hours 59 minutes
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(23, 59), result);
    }
}

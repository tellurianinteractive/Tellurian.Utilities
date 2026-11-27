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

        Assert.AreEqual(1 + (12.5 / 24), result, 1e-12);
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

        Assert.AreEqual(1 + (12.5 / 24), result, 1e-12);
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
        var minutes = 90.0; // 1 hour 30 minutes
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(1, 30), result);
    }

    [TestMethod]
    public void Double_TimeOnly_ZeroMinutes_ReturnsMidnight()
    {
        var minutes = 0.0;
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(0, 0), result);
    }

    [TestMethod]
    public void Double_TimeOnly_FullDay_ReturnsCorrectTime()
    {
        var minutes = 1439.0; // 23 hours 59 minutes
        var result = minutes.TimeOnly;

        Assert.AreEqual(new TimeOnly(23, 59), result);
    }

    [TestMethod]
    public void ToDateTextAndToDateAndTimeText()
    {
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("sv-SE");
        DateTimeOffset? value = new DateTimeOffset(2026, 11, 21, 14, 5, 0, TimeSpan.Zero);
        Assert.AreEqual("2026-11-21", value.ToDateText());
        Assert.AreEqual("2026-11-21 14:05", value.ToDateAndTimeText());
        Assert.AreEqual(string.Empty, ((DateTimeOffset?)null).ToDateText());
        Assert.AreEqual(string.Empty, ((DateTimeOffset?)null).ToDateAndTimeText());
        Assert.AreEqual("2026-11-21", value.Value.ToDateText(), "Not nullable.");
        Assert.AreEqual("2026-11-21 14:05", value.Value.ToDateAndTimeText(), "Not nullable.");
    }

    [TestMethod]
    [DataRow(1950, 1965, "1950 - 1965")]
    [DataRow(1950, null, "1950 - ")]
    [DataRow(null, 1965, "- 1965")]
    [DataRow(null, null, "")]
    public void ToPeriodTextOfYears(int? from, int? to, string expected)
    {
        Assert.AreEqual(expected, (from, to).ToPeriodText());
        Assert.AreEqual(expected, ((short?)from, (short?)to).ToPeriodText());
    }

    [TestMethod]
    public void ToPeriodTextOfDates()
    {
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("sv-SE");
        DateTime? from = new DateTime(2026, 5, 1), to = new DateTime(2026, 5, 3);
        Assert.AreEqual("2026-05-01 - 2026-05-03", (from, to).ToPeriodText());
        Assert.AreEqual("2026 - ", (from, (DateTime?)null).ToPeriodText("yyyy"));
        Assert.AreEqual(string.Empty, ((DateTime?)null, (DateTime?)null).ToPeriodText());
    }

    [TestMethod]
    [DataRow("en-US")]
    [DataRow("sv-SE")]
    public void HHMM_IsTwentyFourHourInEveryCulture(string culture)
    {
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
        Assert.AreEqual("14:30", new TimeOnly(14, 30).HHMM);
        Assert.AreEqual("07:05", new TimeOnly(7, 5).HHMM);
    }

    [TestMethod]
    public void DateTime_ToDouble_AtNoon_IsHalfADay() =>
        Assert.AreEqual(1.5, new DateTime(2026, 1, 1, 12, 0, 0).ToDouble, 1e-12);

    [TestMethod]
    public void TimeSpan_ToDouble_TwelveHours_IsHalfADay() =>
        Assert.AreEqual(0.5, TimeSpan.FromHours(12).ToDouble, 1e-12);
}

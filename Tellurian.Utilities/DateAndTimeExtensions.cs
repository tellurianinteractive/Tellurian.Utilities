using System.Globalization;

namespace Tellurian.Utilities;

/// <summary>
/// Extensions for dates, times and periods.
/// </summary>
public static class DateAndTimeExtensions
{
    private const double MinutesPerDay = 1440.0;
    private const double HoursPerDay = 24.0;

    extension(DateTime dateTime)
    {
        /// <summary>
        /// Gets the date component of the current value, excluding the time portion.
        /// </summary>
        public DateOnly DateOnly => DateOnly.FromDateTime(dateTime);
        /// <summary>
        /// Gets the time component of the current date and time as a TimeOnly value.
        /// </summary>
        public TimeOnly TimeOnly => TimeOnly.FromDateTime(dateTime);
        /// <summary>
        /// Gets the value of the current date and time as a fractional day of the year.
        /// </summary>
        /// <remarks>The returned value is the day of the year plus the time of day as a fraction of a day, to the minute:
        /// 1 January 12:00 gives 1.5. This can be used for calculations that require a continuous representation of
        /// the date and time within the year.</remarks>
        public double ToDouble => dateTime.DayOfYear + (dateTime.Hour / HoursPerDay) + (dateTime.Minute / MinutesPerDay);
    }

    extension(DateTimeOffset dateTimeOffset)
    {
        /// <summary>
        /// Gets the date component of the value converted to the local time zone of the machine running the code
        /// (<see cref="DateTimeOffset.LocalDateTime"/>), not the date at the value's own offset.
        /// </summary>
        public DateOnly DateOnly => DateOnly.FromDateTime(dateTimeOffset.LocalDateTime);
        /// <summary>
        /// Gets the time of day of the value converted to the local time zone of the machine running the code
        /// (<see cref="DateTimeOffset.LocalDateTime"/>), not the time at the value's own offset.
        /// </summary>
        public TimeOnly TimeOnly => TimeOnly.FromDateTime(dateTimeOffset.LocalDateTime);

        /// <summary>The date in the current culture's short date format ("d").</summary>
        public string ToDateText() => dateTimeOffset.ToString("d", CultureInfo.CurrentCulture);

        /// <summary>The date and time in the current culture's short format ("g").</summary>
        public string ToDateAndTimeText() => dateTimeOffset.ToString("g", CultureInfo.CurrentCulture);
    }

    extension(DateTimeOffset? dateTimeOffset)
    {
        /// <summary>The date in the current culture's short date format ("d"), or an empty string for no value.</summary>
        public string ToDateText() => dateTimeOffset.HasValue ? dateTimeOffset.Value.ToDateText() : string.Empty;

        /// <summary>The date and time in the current culture's short format ("g"), or an empty string for no value.</summary>
        public string ToDateAndTimeText() => dateTimeOffset.HasValue ? dateTimeOffset.Value.ToDateAndTimeText() : string.Empty;
    }

    extension((DateTime? from, DateTime? to) period)
    {
        /// <summary>
        /// The period as "from - to" in <paramref name="format"/> and the current culture; an open end is left out ("from - ",
        /// "- to"), and no dates give an empty string.
        /// </summary>
        public string ToPeriodText(string format = "d") =>
            PeriodText(period.from?.ToString(format, CultureInfo.CurrentCulture), period.to?.ToString(format, CultureInfo.CurrentCulture));
    }

    extension((int? from, int? to) period)
    {
        /// <summary>A period of numbers, such as years, as "1950 - 1965"; an open end is left out, and none gives an empty string.</summary>
        public string ToPeriodText() => PeriodText(period.from?.ToString(CultureInfo.InvariantCulture), period.to?.ToString(CultureInfo.InvariantCulture));
    }

    extension((short? from, short? to) period)
    {
        /// <summary>A period of numbers, such as years, as "1950 - 1965"; an open end is left out, and none gives an empty string.</summary>
        public string ToPeriodText() => PeriodText(period.from?.ToString(CultureInfo.InvariantCulture), period.to?.ToString(CultureInfo.InvariantCulture));
    }

    private static string PeriodText(string? from, string? to) =>
        from is null && to is null ? string.Empty :
        from is null ? $"- {to}" :
        to is null ? $"{from} - " :
        $"{from} - {to}";

    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Gets the time of day represented by this instance as a TimeOnly value.
        /// </summary>
        public TimeOnly TimeOnly => TimeOnly.FromTimeSpan(timeSpan);
        /// <summary>
        /// Gets the value of the represented time interval as a fractional number of days: 12 hours give 0.5.
        /// </summary>
        public double ToDouble => timeSpan.TotalDays;

        /// <summary>
        /// Returns the greater of the current TimeSpan value and the specified TimeSpan value.
        /// </summary>
        /// <param name="other">The TimeSpan value to compare with the current value.</param>
        /// <returns>A TimeSpan that is the greater of the current value and the specified value.</returns>
        public TimeSpan Max(TimeSpan other) => timeSpan > other ? timeSpan : other;

        /// <summary>
        /// Returns the smaller of the current TimeSpan value and the specified TimeSpan.
        /// </summary>
        /// <param name="other">The TimeSpan value to compare with the current instance.</param>
        /// <returns>A TimeSpan that is the lesser of the current value and <paramref name="other"/>.</returns>
        public TimeSpan Min(TimeSpan other) => timeSpan < other ? timeSpan : other;

    }

    extension(DateOnly date)
    {
        /// <summary>
        /// Gets the English name of the day of the week, as <see cref="DayOfWeek"/> names it ("Monday"); it is not
        /// translated to the current culture.
        /// </summary>
        public string WeekdayName => date.DayOfWeek.ToString();
    }

    extension(TimeOnly time)
    {
        /// <summary>
        /// Gets the time represented by this instance in 24-hour format (HH:mm) in every culture.
        /// </summary>
        public string HHMM => time.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    extension(double minutes)
    {
        /// <summary>
        /// Gets the time of day represented by the current instance as a <see cref="TimeOnly"/> value.
        /// </summary>
        public TimeOnly TimeOnly => new((int)(minutes / 60), (int)(minutes % 60));
    }

}

using System.Globalization;

namespace Tellurian.Utilities;

public static class DateAndTimeExtensions
{
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
        /// <remarks>The returned value represents the day of the year, including fractional components
        /// based on the hour and minute. This can be used for calculations that require a continuous representation of
        /// the date and time within the year.</remarks>
        public double ToDouble => dateTime.DayOfYear + dateTime.Hour / 60.0 + dateTime.Minute / 3600.0;
    }

    extension(DateTimeOffset dateTimeOffset)
    {
        /// <summary>
        /// Gets the date component of the current value, excluding the time of day.
        /// </summary>
        public DateOnly DateOnly => DateOnly.FromDateTime(dateTimeOffset.LocalDateTime);
        /// <summary>
        /// Gets the time of day component from the underlying date and time value.
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
        /// Gets the value of the represented time interval as a fractional number of days.
        /// </summary>
        public double ToDouble => timeSpan.TotalDays + timeSpan.Hours / 60.0 + timeSpan.Minutes / 3600.0;

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
        /// Gets the name of the day of the week represented by the current date.
        /// </summary>
        public string WeekdayName => date.DayOfWeek.ToString();
    }

    extension(TimeOnly time)
    {
        /// <summary>
        /// Gets the time represented by this instance in 24-hour format (HH:mm).
        /// </summary>
        public string HHMM => time.ToShortTimeString();
    }

    extension(double minutes)
    {
        /// <summary>
        /// Gets the time of day represented by the current instance as a <see cref="TimeOnly"/> value.
        /// </summary>
        public TimeOnly TimeOnly => new((int)(minutes / 60), (int)(minutes % 60));
    }

}

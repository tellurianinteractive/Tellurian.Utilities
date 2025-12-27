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
    }

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

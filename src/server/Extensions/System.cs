
using System;
using System.Globalization;
using Toucan.Contract;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static DateTime? ToSourceUtc(this string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            DateTime? dateTime = null;

            if (DateTime.TryParse(value, culture, DateTimeStyles.AssumeLocal, out DateTime date))
            {
                TimeSpan? offset = null;

                if (date.TimeOfDay == TimeSpan.Zero && HasExplicitOffset(value))
                    offset = DateTimeOffset.Parse(value, culture).Offset;

                if (sourceTimeZone.Id == TimeZoneInfo.Local.Id)
                    date = TimeZoneInfo.ConvertTime(date, sourceTimeZone);

                dateTime = date.ToSourceUtc(sourceTimeZone, offset);
            }

            return dateTime;
        }

        public static DateTime? ToSourceUtc(this DateTime date, TimeZoneInfo sourceTimeZone, TimeSpan? offset = null)
        {
            DateTime? dateTime = null;

            if (date.TimeOfDay == TimeSpan.Zero)
            {
                dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            }
            else if (sourceTimeZone.Id == TimeZoneInfo.Local.Id)
            {
                dateTime = date.ToUniversalTime();
            }
            else
            {
                DateTime sourceDateTime = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, sourceTimeZone);

                if (offset.HasValue)
                    sourceDateTime = sourceDateTime.Add(offset.Value);

                dateTime = TimeZoneInfo.ConvertTimeToUtc(sourceDateTime, sourceTimeZone);
            }

            return dateTime;
        }

        private static bool HasExplicitOffset(string value)
        {
            return value.Contains("+") || value.Contains("-") || value.Trim().ToLower().EndsWith("gmt");
        }
    }
}
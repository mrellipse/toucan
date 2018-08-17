
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Toucan.Contract;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        private delegate DateTime? ConversionStrategy(string value, CultureInfo culture, TimeZoneInfo sourceTimeZone);
        private static ConversionStrategy[] ConversionStrategies = new ConversionStrategy[] { StrategyISO, StrategyUTC, StrategyGMT, StrategyUserCulture };

        public static DateTime? FromSourceUtc(this string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            DateTime? result = null;

            foreach (var strategy in ConversionStrategies)
            {
                result = strategy(value, culture, sourceTimeZone);

                if (result.HasValue)
                    break;
            }

            return result;
        }

        public static DateTime? ToSourceUtc(this DateTime date, TimeZoneInfo sourceTimeZone)
        {
            DateTime? dateTime = null;

            DateTime sourceDateTime = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, sourceTimeZone);

            dateTime = TimeZoneInfo.ConvertTimeToUtc(sourceDateTime, sourceTimeZone);

            return dateTime;
        }

        private static bool ApplySpecifiedOffset(string value)
        {
            return !value.Contains("+0000") && !value.Contains("Greenwich", StringComparison.CurrentCultureIgnoreCase);
        }

        private static TimeSpan GetSpecifiedOffset(string value)
        {
            var match = Regex.Match(value, @"(?:GMT)([\d\+|-]{5})").ToString().Replace("GMT", "");

            bool positive = match.StartsWith("+");
            int hours = Int32.Parse(match.Substring(1, 2));
            int minutes = Int32.Parse(match.Substring(3, 2));

            return positive ? new TimeSpan(hours, minutes, 0) : new TimeSpan(hours, minutes, 0).Negate();
        }

        private static DateTime? StrategyISO(string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            if (value.EndsWith("Z") && value.Contains("T"))  //  Javascript = new Date().toISOString()
                return DateTime.Parse(value, culture, DateTimeStyles.AdjustToUniversal);

            return null;
        }

        private static DateTime? StrategyGMT(string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            if (Regex.IsMatch(value, @"(GMT\+[\d]+\s\([\w\s]+\))+"))  // Javascript = new Date().toString()
            {
                var values = value.Split(" ").ToArray();

                if (ApplySpecifiedOffset(value))
                {
                    var date = DateTime.Parse($"{values[0]} {values[1]} {values[2]} {values[3]} {values[4]} GMT", culture, DateTimeStyles.AdjustToUniversal);

                    TimeSpan adjustment = GetSpecifiedOffset(value);

                    if (adjustment >= TimeSpan.Zero)
                        adjustment = adjustment.Negate();

                    return date.Add(adjustment);
                }
                else
                {
                    var date = DateTime.Parse($"{values[0]} {values[1]} {values[2]} {values[3]} {values[4]} GMT", culture, DateTimeStyles.AssumeLocal);
                    date = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.Local);
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
                }
            }

            return null;
        }

        private static DateTime? StrategyUserCulture(string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            DateTimeStyles styles = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;
            DateTime date;

            if (DateTime.TryParse(value, culture.DateTimeFormat, styles, out date))
                return new Nullable<DateTime>(date);

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, styles, out date))
                return new Nullable<DateTime>(date);

            string[] values = value.Split(" ");

            if (values.Length == 3 && !value.Contains(",") && value.EndsWith("M"))
            {
                if (DateTime.TryParse($"{values[0]}, {values[1]} {values[2]}", culture.DateTimeFormat, styles, out date))
                    return new Nullable<DateTime>(date);
            }

            return null;
        }

        private static DateTime? StrategyUTC(string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            if (value.EndsWith("GMT"))  //  Javascript = new Date().toUTCString()
            {
                var values = value.Split(" ").ToArray();
                return DateTime.Parse($"{values[0]} {values[1]} {values[2]} {values[3]} {values[4]} GMT", culture, DateTimeStyles.AdjustToUniversal);
            }

            return null;
        }
    }
}
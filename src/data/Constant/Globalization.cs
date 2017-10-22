using System;
using System.Globalization;
using System.Threading;

namespace Toucan.Data
{
    public static class Globalization
    {
        public static string DefaultTimeZoneId
        {
            get
            {
                return TimeZoneInfo.Local.Id;
            }
        }
    }
}
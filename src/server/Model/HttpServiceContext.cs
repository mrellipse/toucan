
using System;
using System.Globalization;
using System.Linq;
using Toucan.Contract;

namespace Toucan.Server.Model
{
    public class HttpServiceContext : IDomainContext
    {
        internal HttpServiceContext(IUser user, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            this.User = user;
            this.Culture = culture;
            this.SourceTimeZone = sourceTimeZone;
        }

        public CultureInfo Culture { get; set; }
        public TimeZoneInfo SourceTimeZone { get; set; }
        public IUser User { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class UserProvider
    {
        public string ProviderId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExternalId { get; set; }

        public virtual Provider Provider { get; set; }
        public virtual User User { get; set; }
    }
}

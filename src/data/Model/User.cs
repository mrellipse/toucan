using System;
using System.Collections.Generic;
using Toucan.Contract;

namespace Toucan.Data.Model
{
    public partial class User : IUser
    {
        public User()
        {
            Providers = new HashSet<UserProvider>();
            Roles = new HashSet<UserRole>();
            Verifications = new HashSet<Verification>();
        }

        public long UserId { get; set; }

        public string CultureName { get; set; }
        public string DisplayName { get; set; }
        public string Email
        {
            get
            {
                return this.Username;
            }
        }
        public bool Enabled { get; set; }
        public bool Verified { get; set; }
        public string TimeZoneId { get; set; }
        public string Username { get; set; }

        public long? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public long? LastUpdatedBy { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual User LastUpdatedByUser { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<UserProvider> Providers { get; set; }
        public virtual ICollection<Verification> Verifications { get; set; }
    }
}

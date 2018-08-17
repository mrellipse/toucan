using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class SecurityClaim : IAuditable
    {
        public SecurityClaim()
        {
            this.Roles = new HashSet<RoleSecurityClaim>();
        }

        public string SecurityClaimId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Origin { get; set; }
        public string ValidationPattern { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public User CreatedByUser { get; set; }
        public User LastUpdatedByUser { get; set; }
        public virtual ICollection<RoleSecurityClaim> Roles { get; set; }
    }
}
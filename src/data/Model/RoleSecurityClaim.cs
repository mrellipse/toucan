using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class RoleSecurityClaim
    {
        public RoleSecurityClaim()
        {

        }

        public string RoleId { get; set; }
        public virtual Role Role { get; set; }
        public string SecurityClaimId { get; set; }
        public virtual SecurityClaim SecurityClaim { get; set; }
        public string Value { get; set; }
    }
}
using System;

namespace Toucan.Service.Model
{
    public class RoleSecurityClaim : ICloneable
    {
        public string RoleId { get; set; }
        public string SecurityClaimId { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            var roleSecurityClaim = (RoleSecurityClaim)this.MemberwiseClone();
            roleSecurityClaim.RoleId = null;
            return roleSecurityClaim;
        }
    }
}
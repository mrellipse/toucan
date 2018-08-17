using System;
using System.Collections.Generic;
using System.Linq;

namespace Toucan.Service.Model
{
    public class Role : ICloneable
    {
        public Role()
        {
            this.SecurityClaims = new RoleSecurityClaim[] { };
        }

        public string RoleId { get; set; }
        public string ParentRoleId { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public RoleSecurityClaim[] SecurityClaims { get; set; }
        public int AssignedUserCount { get; set; }
        public User CreatedByUser { get; set; }
        public User LastUpdatedByUser { get; set; }
        public Role Parent { get; set; }
        public bool IsSystemRole
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ParentRoleId);
            }
        }

        public object Clone()
        {
            Role clone = null;

            if (this.IsSystemRole)
            {
                Role parent = BaseClone();
                var claims = this.SecurityClaims.Select(o => (RoleSecurityClaim)o.Clone());

                clone = new Role()
                {
                    Enabled = this.Enabled,
                    ParentRoleId = this.RoleId,
                    Parent = parent,
                    SecurityClaims = claims.ToArray()
                };
            }
            else
            {
                var role = BaseClone();

                role.CreatedByUser = null;
                role.LastUpdatedBy = null;
                role.LastUpdatedByUser = null;

                clone = role;
            }

            return clone;
        }

        private Role BaseClone()
        {
            var role = (Role)this.MemberwiseClone();

            if (role.SecurityClaims.Any())
            {
                var claims = role.SecurityClaims.Select(o => (RoleSecurityClaim)o.Clone());
                role.SecurityClaims = claims.ToArray();
            }

            return role;
        }
    }
}
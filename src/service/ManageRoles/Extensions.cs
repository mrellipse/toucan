using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Toucan.Data;
using Toucan.Data.Model;
using model = Toucan.Data.Model;

namespace Toucan.Service.Model
{
    public static partial class Extensions
    {
        public static Model.SecurityClaim MapToVm(this model.SecurityClaim value)
        {
            if (value == null)
                return null;

            return new Model.SecurityClaim()
            {
                SecurityClaimId = value.SecurityClaimId,
                Description = value.Description,
                Enabled = value.Enabled,
                Origin = value.Origin,
                ValidationPattern = value.ValidationPattern
            };
        }

        public static Model.RoleSecurityClaim MapToVm(this model.RoleSecurityClaim value)
        {
            if (value == null)
                return null;

            return new Model.RoleSecurityClaim()
            {
                RoleId = value.RoleId,
                SecurityClaimId = value.SecurityClaimId,
                Value = value.Value
            };
        }

        public static Model.Role MapToVm(this model.Role value)
        {
            if (value == null)
                return null;

            return new Model.Role()
            {
                AssignedUserCount = value.Users.Count(),
                SecurityClaims = value.SecurityClaims.Select(o => o.MapToVm()).ToArray(),
                CreatedOn = value.CreatedOn,
                CreatedByUser = new Model.User()
                {
                    UserId = value.CreatedByUser?.UserId ?? 0,
                    DisplayName = value.CreatedByUser?.DisplayName
                },
                Enabled = value.Enabled,
                RoleId = value.RoleId,
                LastUpdatedByUser = new Model.User()
                {
                    UserId = value.LastUpdatedByUser?.UserId ?? 0,
                    DisplayName = value.LastUpdatedByUser?.DisplayName
                },
                LastUpdatedOn = value.LastUpdatedOn,
                Name = value.Name,
                Parent = value.Parent?.MapToVm(),
                ParentRoleId = value.ParentRoleId
            };
        }

        public static void UpdateRoleClaims(this model.Role dbRole, Model.Role role)
        {
            var securityClaimIds = role.SecurityClaims.Select(o => o.SecurityClaimId).ToArray();

            var remove = (from c in dbRole.SecurityClaims
                          where !securityClaimIds.Contains(c.SecurityClaimId)
                          select c).ToList();

            remove.ForEach(o => dbRole.SecurityClaims.Remove(o));

            var add = (from c in role.SecurityClaims
                       where !dbRole.SecurityClaims.Any(o => o.SecurityClaimId == c.SecurityClaimId)
                       select c).ToList();

            add.ForEach(o =>
            {
                dbRole.SecurityClaims.Add(new model.RoleSecurityClaim()
                {
                    SecurityClaimId = o.SecurityClaimId,
                    Value = o.Value
                });
            });

            var update = role.SecurityClaims.Except(add).ToList();

            update.ForEach(claim =>
            {
                var match = dbRole.SecurityClaims.Single(o => o.SecurityClaimId == claim.SecurityClaimId);
                match.Value = claim.Value;
            });

        }
    }
}
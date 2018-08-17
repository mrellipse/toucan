using System;
using System.Linq;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public static partial class Extensions
    {
        public static void UpdateUserRoles(this DbContextBase db, User user, string[] roleIds)
        {
            var remove = user.Roles.Where(o => !roleIds.Contains(o.RoleId)).ToArray();
            db.UserRole.RemoveRange(remove);

            for (int i = 0; i < remove.Length; i++)
            {
                user.Roles.Remove(remove[i]);
            }

            var add = (from r in db.Role.Where(o => roleIds.Contains(o.RoleId))
                       where !user.Roles.Any(o => o.RoleId == o.RoleId)
                       select r);

            Func<Role, UserRole> map = (o) => new UserRole()
            {
                Role = o,
                User = user
            };

            db.UserRole.AddRange(add.Select(map));
        }
    }
}
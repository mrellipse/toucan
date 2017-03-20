using System.Linq;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public static partial class Extensions
    {
        public static void UpdateUserRoles(this DbContextBase db, User user, string[] roleIds)
        {
            var removeRoles = user.Roles.Where(o => !roleIds.Contains(o.RoleId)).ToList();

            removeRoles.ForEach(o => db.Remove(o));

            var addRoles = roleIds.Where(o => !user.Roles.Any(r => r.RoleId == o)).ToList();

            addRoles.ForEach(o =>
                db.UserRole.Add(new UserRole()
                {
                    RoleId = o,
                    UserId = user.UserId
                })
            );
        }
    }
}
using System;
using System.Linq;
using Toucan.Data.Model;
using Toucan.Contract;

namespace Toucan.Data
{
    public static partial class Extensions
    {
        private const string AdminEmail = "webmaster@toucan.org";

        public static void EnsureSeedData(this ToucanContext db, ICryptoService crypto)
        {
            EnsureLocalProvider(db);
            User admin = EnsureAdmin(db, crypto);
            EnsureRoles(db);
        }

        private static Provider EnsureLocalProvider(ToucanContext db)
        {
            Provider provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Local);

            if (!db.Provider.Any())
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Local,
                    Name = "Site",
                    Description = "Authenticate with a username/password provider by this site",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            return provider;
        }

        private static void EnsureRoles(ToucanContext db)
        {
            User adminUser = db.User.SingleOrDefault(o => o.Username == AdminEmail);
            Role userRole = db.Role.FirstOrDefault(o => o.RoleId == RoleTypes.User);

            if (userRole == null)
            {
                userRole = new Role()
                {
                    CreatedBy = adminUser.UserId,
                    Enabled = true,
                    Name = "User",
                    RoleId = RoleTypes.User
                };
                db.Role.Add(userRole);
                db.SaveChanges();
            }

        }

        private static User EnsureAdmin(ToucanContext db, ICryptoService crypto)
        {
            

            User adminUser = db.User.SingleOrDefault(o => o.Username == AdminEmail);

            if (adminUser == null)
            {
                adminUser = new User()
                {
                    Username = AdminEmail,
                    Enabled = true,
                    DisplayName = "Webmaster",
                    Verified = true
                };

                db.User.Add(adminUser);
                db.SaveChanges();
            }

            Role adminRole = db.Role.FirstOrDefault(o => o.RoleId == RoleTypes.Admin);

            if (adminRole == null)
            {
                adminRole = new Role()
                {
                    CreatedByUser = adminUser,
                    Enabled = true,
                    Name = "Administrator",
                    RoleId = RoleTypes.Admin
                };
                db.Role.Add(adminRole);
                db.SaveChanges();
            }

            if (!db.UserRole.Any())
            {
                var userRole = new UserRole()
                {
                    Role = adminRole,
                    User = adminUser
                };

                string salt = crypto.CreateSalt();
                string hash = crypto.CreateKey(salt, "password");

                var userProvider = new UserProviderLocal
                {
                    CreatedOn = DateTime.Now,
                    ProviderId = ProviderTypes.Local,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    User = adminUser,
                };

                db.UserRole.Add(userRole);
                db.UserProvider.Add(userProvider);

                db.SaveChanges();
            }

            return adminUser;
        }
    }
}
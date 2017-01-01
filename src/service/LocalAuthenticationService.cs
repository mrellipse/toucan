using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public class LocalAuthenticationService : ILocalAuthenticationService
    {
        private ToucanContext db;
        private ICryptoService crypto;

        public LocalAuthenticationService(ToucanContext db, ICryptoService crypto)
        {
            this.db = db;
            this.crypto = crypto;
        }

        public Task<ClaimsIdentity> ResolveUser(string email, string password)
        {
            UserProviderLocal login = (from p in this.db.LocalProvider.Include(o => o.User)
                                       where p.User.Email == email
                                       select p).FirstOrDefault();

            if (login != null)
            {
                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, password))
                    return Task.FromResult(MapUserToClaimsIdentity(login.User));
            }

            return Task.FromResult<ClaimsIdentity>(null);
        }

        public Task<ClaimsIdentity> SignupUser(ISignupOptions options)
        {
            UserProviderLocal login = (from p in this.db.LocalProvider.Include(o => o.User)
                                       where p.User.Email == options.Email
                                       select p).FirstOrDefault();

            if (login != null)
            {
                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, options.Password))
                    return Task.FromResult(MapUserToClaimsIdentity(login.User));
            }

            User user = new User()
            {
                CreatedOn = DateTime.Now,
                Email = options.Email,
                Enabled = true,
                Name = options.Name,
                Verified = false
            };
            
            db.User.Add(user);

            string salt = crypto.CreateSalt();

            db.LocalProvider.Add(new UserProviderLocal()
            {
                CreatedOn = DateTime.Now,
                PasswordSalt = salt,
                PasswordHash = crypto.CreateKey(salt, options.Password),
                User = user,
                Provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Local)
            });

            foreach (string roleId in options.Roles)
            {
                db.UserRole.Add(new UserRole()
                {
                    User = user,
                    RoleId = roleId
                });
            }

            return Task.FromResult<ClaimsIdentity>(null);

        }
        private static ClaimsIdentity MapUserToClaimsIdentity(User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));

            var roles = (from r in user.Roles
                         select new Claim(ClaimTypes.Role, r.RoleId));

            claims.AddRange(roles);

            return new ClaimsIdentity(
                new System.Security.Principal.GenericIdentity(user.Email, "Token"),
                claims.ToArray()
                );
        }
    }
}

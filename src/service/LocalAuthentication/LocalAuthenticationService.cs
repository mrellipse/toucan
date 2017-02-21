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

        public Task<ClaimsIdentity> ResolveUser(string username, string password)
        {
            UserProviderLocal login = (from p in this.db.LocalProvider.Include(o => o.User).Include(o => o.User.Roles)
                                       where p.User.Username == username
                                       select p).FirstOrDefault();

            if (login != null)
            {
                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, password))
                    return Task.FromResult(login.User.ToClaimsIdentity());
            }

            return Task.FromResult<ClaimsIdentity>(null);
        }

        public async Task<ClaimsIdentity> SignupUser(ILocalSignupOptions options)
        {
            UserProviderLocal login = await (from p in this.db.LocalProvider.Include(o => o.User)
                                             where p.User.Username == options.Username
                                             select p).FirstOrDefaultAsync();

            if (login != null)
                throw new ServiceException($"A user account for {options.Username} already exists");

            User user = new User()
            {
                CreatedOn = DateTime.Now,
                Username = options.Username,
                Enabled = true,
                DisplayName = options.DisplayName,
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

            Role role = db.Role.FirstOrDefault(o => o.RoleId == RoleTypes.User);

            user.Roles.Add(new UserRole()
            {
                User = user,
                Role = role
            });

            db.SaveChanges();

            return user.ToClaimsIdentity();
        }
        public async Task<bool> ValidateUser(string username)
        {
            UserProviderLocal login = await (from p in this.db.LocalProvider.Include(o => o.User)
                                             where p.User.Username == username
                                             select p).FirstOrDefaultAsync();

            return login == null;
        }

    }
}

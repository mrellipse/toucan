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
        private DbContextBase db;
        private ICryptoService crypto;

        public LocalAuthenticationService(DbContextBase db, ICryptoService crypto)
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

        public async Task<IUser> ResolveUser(string username)
        {
            return await this.db.User.SingleOrDefaultAsync(o => o.Username == username);
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

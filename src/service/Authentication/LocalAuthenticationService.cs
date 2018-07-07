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
        private ICryptoService crypto;
        private DbContextBase db;
        private readonly IDeviceProfiler deviceProfiler;

        public LocalAuthenticationService(DbContextBase db, ICryptoService crypto, IDeviceProfiler deviceProfiler)
        {
            this.crypto = crypto;
            this.db = db;
            this.deviceProfiler = deviceProfiler;
        }

        public Task<ClaimsIdentity> ResolveUser(string username, string password)
        {
            UserProviderLocal login = (from p in this.db.LocalProvider.Include(o => o.User)
                    .Include(o => o.User.Roles)
                    .Include(o => o.User.Verifications)
                where p.User.Username == username
                select p).FirstOrDefault();

            if (login != null)
            {
                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, password))
                {
                    string fingerprint = this.deviceProfiler.DeriveFingerprint(login.User);
                    ClaimsIdentity identity = login.User.ToClaimsIdentity(fingerprint);

                    return Task.FromResult(identity);
                }
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

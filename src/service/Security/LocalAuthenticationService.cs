using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public class LocalAuthenticationService : ILocalAuthenticationService
    {
        private readonly Config config;
        private ICryptoService crypto;
        private DbContextBase db;
        private readonly IDeviceProfiler deviceProfiler;

        public LocalAuthenticationService(DbContextBase db, IOptions<Config> config, ICryptoService crypto, IDeviceProfiler deviceProfiler)
        {
            this.config = config.Value;
            this.crypto = crypto;
            this.db = db;
            this.deviceProfiler = deviceProfiler;
        }

        public async Task<ClaimsIdentity> ResolveUser(string username, string password)
        {
            UserProviderLocal login = await this.db.LocalProvider.Where(o => o.User.Username == username).FirstOrDefaultAsync();

            if (login != null)
            {
                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, password))
                {
                    string fingerprint = this.deviceProfiler.DeriveFingerprint(login.User);
                    ClaimsIdentity identity = login.User.ToClaimsIdentity(this.config.ClaimsNamespace, fingerprint);

                    return identity;
                }
            }

            return null;
        }

        public async Task<IUser> ResolveUser(string username)
        {
            return await this.db.User.SingleOrDefaultAsync(o => o.Username == username);
        }

        public async Task<bool> ValidateUser(string username)
        {
            UserProviderLocal login = await this.db.LocalProvider.Where(o => o.User.Username == username).FirstOrDefaultAsync();

            return login == null;
        }
    }
}

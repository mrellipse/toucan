using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public class SignupService : ISignupService
    {
        private readonly ICryptoService crypto;
        private readonly DbContextBase db;
        private readonly Config config;
        private readonly IDeviceProfiler deviceProfiler;
        private readonly IServiceProvider serviceProvider;

        public SignupService(DbContextBase db, IOptions<Config> config, ICryptoService crypto, IDeviceProfiler deviceProfiler, System.IServiceProvider serviceProvider)
        {
            this.config = config.Value;
            this.crypto = crypto;
            this.db = db;
            this.deviceProfiler = deviceProfiler;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ClaimsIdentity> SignupUser(ISignupServiceOptions options)
        {
            UserProviderLocal login = await this.db.LocalProvider.Where(o => o.User.Username == options.Username).FirstOrDefaultAsync();

            if (login != null)
                throw new ServiceException($"A user account for {options.Username} already exists");

            User user = new User()
            {
                CultureName = options.CultureName,
                Enabled = true,
                Username = options.Username,
                DisplayName = options.DisplayName,
                TimeZoneId = options.TimeZoneId
            };

            db.User.Add(user);

            string salt = crypto.CreateSalt();

            db.LocalProvider.Add(new UserProviderLocal()
            {
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

            var fingerprint = this.deviceProfiler.DeriveFingerprint(user);

            return user.ToClaimsIdentity(this.config.ClaimsNamespace, fingerprint);
        }

        public async Task<ClaimsIdentity> RedeemVerificationCode(IUser user, string code)
        {
            var provider = (from p in this.serviceProvider.GetServices<IVerificationProvider>()
                            where p.CanHandle(user, code)
                            select p).FirstOrDefault();

            if (provider != null)
            {
                if (await provider.RedeemCode(user, code))
                {
                    var dbUser = await this.db.User.Where(o => o.UserId == user.UserId).FirstOrDefaultAsync();

                    var fingerprint = this.deviceProfiler.DeriveFingerprint(dbUser);

                    return dbUser.ToClaimsIdentity(this.config.ClaimsNamespace, fingerprint);
                }
            }

            return null;
        }

        public async Task<string> SendVerificationCode(IUser user, string providerKey)
        {
            var q = this.serviceProvider.GetServices<IVerificationProvider>();
            var provider = q.FirstOrDefault(o => o.Key == providerKey);

            return await provider.IssueCode(user);
        }
    }
}
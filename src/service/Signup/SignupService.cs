using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public class SignupService : ISignupService
    {
        private readonly ICryptoService crypto;
        private readonly DbContextBase db;
        private readonly IDeviceProfiler deviceProfiler;
        private readonly IServiceProvider serviceProvider;

        public SignupService(DbContextBase db, ICryptoService crypto, IDeviceProfiler deviceProfiler, System.IServiceProvider serviceProvider)
        {
            this.crypto = crypto;
            this.db = db;
            this.deviceProfiler = deviceProfiler;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ClaimsIdentity> SignupUser(ISignupServiceOptions options)
        {
            UserProviderLocal login = await (from p in this.db.LocalProvider.Include(o => o.User)
                                             where p.User.Username == options.Username
                                             select p).FirstOrDefaultAsync();

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

            return user.ToClaimsIdentity(fingerprint);
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
                    var dbUser = await (from u in this.db.User.Include(o => o.Roles)
                        .Include(o => o.Verifications)
                            where u.UserId == user.UserId
                            select u).FirstOrDefaultAsync();

                    var fingerprint = this.deviceProfiler.DeriveFingerprint(dbUser);

                    return dbUser.ToClaimsIdentity(fingerprint);
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
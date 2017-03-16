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
    public class SignupService : ISignupService
    {
        private readonly DbContextBase db;
        private readonly ICryptoService crypto;
        private readonly IVerificationProvider verificationProvider;

        public SignupService(DbContextBase db, ICryptoService crypto, IVerificationProvider verificationProvider)
        {
            this.db = db;
            this.crypto = crypto;
            this.verificationProvider = verificationProvider;
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

        public async Task<ClaimsIdentity> RedeemCode(string code, IUser user)
        {
            Verification verification = await this.GetPendingVerificationForUser(user);

            if (verification != null && code == verification.Code)
            {
                verification.RedeemedAt = DateTime.Now;
                verification.User.Verified = true;

                await this.db.SaveChangesAsync();
                return verification.User.ToClaimsIdentity();
            }

            return null;
        }

        public async Task<string> IssueCode(IVerificationProvider provider, IUser user)
        {
            Verification verification = await this.GetPendingVerificationForUser(user);

            if (verification == null)
            {
                verification = new Verification()
                {
                    Code = Guid.NewGuid().ToString(),
                    UserId = user.UserId
                };

                this.db.Verification.Add(verification);
                this.db.SaveChanges();
            }

            string code = verification.Code;

            provider.Send(user, code);

            return code;
        }

        private async Task<Verification> GetPendingVerificationForUser(IUser user)
        {
            return await (from v in this.db.Verification.Include(o => o.User).Include(o => o.User.Roles)
                          where v.UserId == user.UserId && v.RedeemedAt == null && v.IssuedAt >= DateTime.Now.AddMinutes(-30)
                          select v).FirstOrDefaultAsync();
        }
    }
}

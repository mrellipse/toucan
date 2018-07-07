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
    public abstract class VerificationProviderBase : IVerificationProvider
    {
        protected readonly DbContextBase db;
        public abstract string Key { get; }

        public VerificationProviderBase(DbContextBase db)
        {
            this.db = db;
        }

        public bool CanHandle(IUser user, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException($"{nameof(code)}");

            Verification verification = this.GetPendingVerificationForUser(user);

            return verification != null && code == verification.Code;
        }

        public async Task<string> IssueCode(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)}");

            Verification verification = this.GetPendingVerificationForUser(user);

            if (verification == null)
            {
                string fingerprint = this.DeriveFingerprint(user);

                if (string.IsNullOrWhiteSpace(fingerprint))
                    throw new ApplicationException($"Null or empty verification fingerprint");

                string code = this.CreateVerificationCode();

                if (string.IsNullOrWhiteSpace(code))
                    throw new ApplicationException($"Null or empty verification code");

                verification = new Verification()
                {
                    Code = code,
                    Fingerprint = fingerprint,
                    ProviderKey = this.Key,
                    UserId = user.UserId
                };

                this.db.Verification.Add(verification);
                this.db.SaveChanges();
            }

            await this.Send(user, verification.Code);

            return verification.Code;
        }

        public async Task<bool> RedeemCode(IUser user, string code)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)}");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException($"{nameof(code)}");

            Verification verification = this.GetPendingVerificationForUser(user);

            if (verification != null && code == verification.Code)
            {
                verification.RedeemedAt = DateTime.UtcNow;

                await this.db.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public abstract Task Send(IUser user, string code);
        protected virtual string CreateVerificationCode() => Guid.NewGuid().ToString();
        protected abstract string DeriveFingerprint(IUser user);
        private Verification GetPendingVerificationForUser(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)}");

            DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
            string fingerprint = this.DeriveFingerprint(user);

            if (string.IsNullOrWhiteSpace(fingerprint))
                throw new ApplicationException($"Null or empty verification fingerprint");

            var q = from v in this.db.Verification.Include(o => o.User).Include(o => o.User.Roles)
                    where v.UserId == user.UserId && v.RedeemedAt == null && v.IssuedAt >= cutoff
                        && v.Fingerprint == fingerprint
                    select v;

            return q.FirstOrDefault();
        }
    }
}
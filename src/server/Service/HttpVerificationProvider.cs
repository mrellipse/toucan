using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service;
using Wangkanai.Detection;

namespace Toucan.Server
{
    public sealed class HttpVerificationProvider : VerificationProviderBase
    {
        public static string ProviderKey = $"{nameof(HttpVerificationProvider)}";
        private IDeviceProfiler deviceProfiler;

        public HttpVerificationProvider(DbContextBase db, IDeviceProfiler deviceProfiler) : base(db)
        {
            this.deviceProfiler = deviceProfiler;
        }

        public override string Key => ProviderKey;

        public override Task Send(IUser user, string code)
        {
            return Task.CompletedTask;
        }

        protected override string DeriveFingerprint(IUser user)
        {
            return this.deviceProfiler.DeriveFingerprint(user);
        }
    }
}

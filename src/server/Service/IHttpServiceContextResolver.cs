using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Toucan.Contract;

namespace Toucan.Server
{
    public interface IHttpServiceContextResolver : IDomainContextResolver
    {
        IUser Resolve(HttpContext context, bool cache = true);
    }
}
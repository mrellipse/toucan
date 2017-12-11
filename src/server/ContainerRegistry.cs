using System;
using StructureMap;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Toucan.Data;
using Toucan.Contract;
using Microsoft.AspNetCore.Http;
using Toucan.Service;
using Microsoft.Extensions.Localization;
using Toucan.Service.Localization;

namespace Toucan.Server
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            var targets = new Filters.ApiExceptionFilterTargets()
            {
                { typeof(Toucan.Service.ServiceException), PayloadMessageType.Failure}
            };

            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
            //For<DbContextBase>().Use<NpgSqlContext>();
            //For<DbContextBase>().Use<MsSqlContext>();

            For<HttpServiceContextFactory>();
            For<IHttpContextAccessor>().Use<HttpContextAccessor>().Transient();
            For<IHttpServiceContextResolver>().Use<HttpServiceContextResolver>();
            For<IDomainContextResolver>().Use<HttpServiceContextResolver>();
            For<ILocalizationResolver>().Add<LocalizationResolver>().Singleton();
            For<ILocalizationService>().Add<LocalizationService>();

            For<Filters.ApiResultFilter>();
            For<Filters.ApiExceptionFilterTargets>().Use(targets);
            For<Filters.ApiExceptionFilter>();
            For<Filters.IdentityMappingFilter>();

            For<CultureService>();

        }
    }
}
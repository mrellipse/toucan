using System;
using StructureMap;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Toucan.Server
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            var targets = new Dictionary<Type, PayloadMessageType>()
            {
                { typeof(Toucan.Service.ServiceException), PayloadMessageType.Failure}
            };

            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
            For<Filters.ApiResultFilter>();
            For<Filters.ApiExceptionFilter>().Use(() => new Filters.ApiExceptionFilter(targets));
            For<Filters.IdentityMappingFilter>();
        }
    }
}
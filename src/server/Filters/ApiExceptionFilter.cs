
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Toucan.Server.Model;

namespace Toucan.Server.Filters
{
    public class ApiExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> logger;
        private readonly ApiExceptionFilterTargets targets;

        public ApiExceptionFilter(ApiExceptionFilterTargets targets, ILogger<ApiExceptionFilter> logger)
        {
            this.logger = logger;
            this.targets = targets;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();

            PayloadMessageType messageType = PayloadMessageType.Error;

            if (this.targets.Keys.Contains(exceptionType))
            {
                messageType = this.targets[exceptionType];

                this.logger.LogWarning(context.Exception, $"Targetted exception of type {context.Exception.GetType().FullName} was converted to api payload with status {messageType}");
            }
            else
            {
                this.logger.LogWarning(context.Exception, $"Untargetted exception of type {context.Exception.GetType().FullName} was converted to api payload with status {messageType}");
            }

            var payload = new Model.Payload<object>()
            {
                Data = context.Exception.StackTrace,
                Message = new PayloadMessage()
                {
                    MessageType = messageType,
                    Text = context.Exception.Message
                }
            };

            context.Exception = null;
            context.Result = new JsonResult(payload);

            return Task.FromResult<bool>(true);
        }
    }
}
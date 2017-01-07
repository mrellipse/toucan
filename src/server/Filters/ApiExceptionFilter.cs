
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Toucan.Server.Model;

namespace Toucan.Server.Filters
{
    public class ApiExceptionFilter : IAsyncExceptionFilter
    {
        private readonly Dictionary<Type, PayloadMessageType> targets;

        public ApiExceptionFilter(Dictionary<Type, PayloadMessageType> targets)
        {
            this.targets = targets;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();

            if (this.targets.Keys.Contains(exceptionType))
            {
                PayloadMessageType messageType = this.targets[exceptionType];
                
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
            }

            return Task.FromResult<bool>(true);
        }
    }
}
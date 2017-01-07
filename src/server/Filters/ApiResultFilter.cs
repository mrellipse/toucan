
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Toucan.Server.Model;

namespace Toucan.Server.Filters
{
    public class ApiResultFilter : IAsyncResultFilter
    {

        public ApiResultFilter(ILoggerFactory logger)
        {
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            ObjectResult obj = context.Result as ObjectResult;

            if (obj != null)
            {
                var payload = new Model.Payload<object>()
                {
                    Data = obj.Value,
                    Message = new PayloadMessage()
                };

                obj.Value = payload;
            }

            await next.Invoke();
        }
    }
}
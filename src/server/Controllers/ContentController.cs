using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toucan.Server.Model;

namespace Toucan.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    public class ContentController : Controller
    {
        public ContentController()
        {

        }

        [Authorize()]
        [HttpGet()]
        [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
        public async Task<object> RikerIpsum()
        {
            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1 * 1000);

                var payload = new Model.Payload<object>()
                {
                    Data = "That might've been one of the shortest assignments in the history of Starfleet. Shields up! Rrrrred alert! I think you've let your personal feelings cloud your judgement.",
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Info,
                        Text = "secureContent"
                    },
                };

                return payload;
            });
        }
    }
}

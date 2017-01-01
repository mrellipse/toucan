using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toucan.Server.Model;

namespace Toucan.Server.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {

        }

        protected JsonResult Payload<T>(string messageTypeId = PayloadMessageType.Success, string message = "")
        {
            var payload = new Model.Payload<T>()
            {
                Message = new PayloadMessage()
                {
                    Message = message,
                    MessageTypeId = messageTypeId
                }
            };

            this.Response.ContentType = "application/json";

            return Json(payload);
        }

        protected JsonResult Payload<T>(T data, string messageTypeId = PayloadMessageType.Success, string message = "")
        {
            Payload<T> payload = this.CreatePayload<T>(data, messageTypeId, message);

            this.Response.ContentType = "application/json";

            return Json(payload);
        }
        private Payload<T> CreatePayload<T>(T data, string messageTypeId = PayloadMessageType.Success, string message = "")
        {
            var payload = new Model.Payload<T>()
            {
                Data = data,
                Message = new PayloadMessage()
                {
                    Message = message,
                    MessageTypeId = messageTypeId
                }
            };

            return payload;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toucan.Server.Model;

namespace Toucan.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public ActionResult Index()
        {
            string virtualPath = "index.html";
            string contentType = "text/html";

            return File(virtualPath, contentType);
        }

        public ActionResult Admin()
        {
            string virtualPath = "admin.html";
            string contentType = "text/html";

            return File(virtualPath, contentType);
        }
    }
}

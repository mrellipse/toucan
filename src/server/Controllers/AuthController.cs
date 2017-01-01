using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toucan.Contract;
using Toucan.Service;

namespace Toucan.Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly ILocalAuthenticationService authService;
        private readonly ITokenProviderService<Token> tokenService;

        public AuthController(ILocalAuthenticationService authService, ITokenProviderService<Token> tokenService)
        {
            this.authService = authService;
            this.tokenService = tokenService;
        }

        [HttpPost("{path}")]
        public async Task<JsonResult> Token(string username, string password)
        {
            var identity = await this.authService.ResolveUser(username, password);

            if (identity == null)
                return Payload(PayloadMessageType.Failure, "Invalid username or password");

            var token = await this.tokenService.IssueToken(identity, identity.Name);

            return Payload(token);
        }

        // [HttpGet]
        // public IEnumerable<string> Get()
        // {
        //     return new string[] { "value1", "value2" };
        // }

        // [HttpGet("{id}")]
        // [Authorize]
        // public string Get(int id)
        // {
        //     return "value";
        // }
    }
}

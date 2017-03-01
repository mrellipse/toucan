using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public class SmtpVerificationProvider : IVerificationProvider
    {
        public SmtpVerificationProvider()
        {

        }

        public void Send(IUser recipient, string code)
        {
            
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;

namespace Toucan.Server.Model
{
    public class UpdateUserCultureOptions
    {
        public string CultureName { get; set; }
        public long UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}

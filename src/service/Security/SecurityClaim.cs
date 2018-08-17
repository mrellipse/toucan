using System;
using System.Collections.Generic;

namespace Toucan.Service.Model
{
    public class SecurityClaim
    {
        public SecurityClaim()
        {
        }

        public string SecurityClaimId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Origin { get; set; }
        public string ValidationPattern { get; set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toucan.Contract;

namespace Toucan.Service.Model
{
    public class User : IUserExtended
    {
        public User()
        {
        }

        public string DisplayName { get; set; }

        public string Email
        {
            get
            {
                return this.Username;
            }
        }

        public bool Enabled { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public long UserId { get; set; }

        public string Username { get; set; }

        public bool Verified { get; set; }
    }
}

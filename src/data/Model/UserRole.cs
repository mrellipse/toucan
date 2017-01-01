using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class UserRole
    {
        public UserRole()
        {

        }

        public string RoleId { get; set; }

        public int UserId { get; set; }

        public virtual Role Role { get; set; }
        
        public virtual User User { get; set; }
    }
}

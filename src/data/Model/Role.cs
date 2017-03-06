using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<UserRole>();
        }

        public string RoleId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }

        public virtual User CreatedByUser { get; set; }
    }
}

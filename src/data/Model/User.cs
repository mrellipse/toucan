using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class User
    {
        public User()
        {
            Content = new HashSet<Content>();
            Roles = new HashSet<UserRole>();
            Providers = new HashSet<UserProvider>();
        }

        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public bool Verified { get; set; }

        public virtual ICollection<Content> Content { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<UserProvider> Providers { get; set; }
    }
}

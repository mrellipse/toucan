using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class Provider
    {
        public Provider()
        {
            Users = new HashSet<UserProvider>();
        }

        public string ProviderId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserProvider> Users { get; set; }
    }
}

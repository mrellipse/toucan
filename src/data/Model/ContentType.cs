using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class ContentType
    {
        public ContentType()
        {
            Content = new HashSet<Content>();
        }

        public string ContentTypeId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Content> Content { get; set; }
    }
}

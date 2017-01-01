using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class Content
    {
        public Guid ContentId { get; set; }
        public string Brief { get; set; }
        public byte[] ContentData { get; set; }
        public string ContentTypeId { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }

        public virtual ContentType ContentType { get; set; }
        public virtual User User { get; set; }
    }
}

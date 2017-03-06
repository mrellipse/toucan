using System;

namespace Toucan.Data.Model
{
    public partial class Verification
    {
        public Verification()
        {

        }

        public string Code { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}

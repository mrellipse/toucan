using System;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public interface IAuditable
    {
        long CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        long? LastUpdatedBy { get; set; }
        DateTime? LastUpdatedOn { get; set; }

        User CreatedByUser { get; set; }
        User LastUpdatedByUser { get; set; }
    }
}
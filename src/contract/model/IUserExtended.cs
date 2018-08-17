
using System;
using System.Collections.Generic;

namespace Toucan.Contract
{
    public interface IUserExtended : IUser
    {
        IEnumerable<string> Claims { get; }
        IEnumerable<string> Roles { get; }
        DateTime CreatedOn { get; }
    }
}
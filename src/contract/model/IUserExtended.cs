
using System.Collections.Generic;

namespace Toucan.Contract
{
    public interface IUserExtended : IUser
    {
        IEnumerable<string> Roles { get; }
    }
}
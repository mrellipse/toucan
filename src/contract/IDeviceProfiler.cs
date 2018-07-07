using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface IDeviceProfiler
    {
        string DeriveFingerprint(IUser user);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface IAuditService
    {
        void Record<T>(T data) where T : IAuditEventData;
        void Record<T>(IDomainContext domainContext, T data) where T : IAuditEventData;
    }
}

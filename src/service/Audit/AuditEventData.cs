using Toucan.Contract;

namespace Toucan.Service.Model
{
    public class AuditEventData : IAuditEventData
    {
        public AuditEventData(AuditServiceEventType auditEventType, string message)
        {
            this.AuditEventTypeId = (int)auditEventType;
            this.Message = message;
        }

        public int AuditEventTypeId { get; private set; }
        public string Message { get; private set; }
    }
}
using Toucan.Contract;

namespace Toucan.Service
{
    public static partial class Extensions
    {
        public static void TokenCreatedEvent(this Toucan.Contract.IAuditService audit, string userName, string issuer, int? exp)
        {
            var eventTypeId = Toucan.Service.AuditServiceEventType.TokenCreated;
            IAuditEventData data = new Toucan.Service.Model.AuditEventData(eventTypeId, $"Token Create. Issuer: {issuer}. User Name: {userName}. Exp: {exp}");
            audit.Record(data);
        }
    }
}
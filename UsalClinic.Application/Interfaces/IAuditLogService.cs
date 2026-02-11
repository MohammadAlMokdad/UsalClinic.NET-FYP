using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;

namespace UsalClinic.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLog> LogCreateActionAsync(string entityName, string entityId, string performedBy, string details);
        Task<AuditLog> LogUpdateActionAsync(string entityName, string entityId, string performedBy, string details);
        Task<AuditLog> LogDeleteActionAsync(string entityName, string entityId, string performedBy, string details);
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityName, string entityId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(string performedBy);
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();
    }
}

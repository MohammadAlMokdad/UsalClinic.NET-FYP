using System;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;

namespace UsalClinic.Application.Services
{
    public class AuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(string action, string entityName, string entityId, string performedBy, string? details = null)
        {
            var log = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details ?? string.Empty,
                PerformedBy = performedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(log);
        }
    }
}

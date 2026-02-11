using Moq;
using System;
using System.Threading.Tasks;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class AuditLogServiceTests
    {
        private readonly Mock<IAuditLogRepository> _mockRepo;
        private readonly AuditLogService _service;

        public AuditLogServiceTests()
        {
            _mockRepo = new Mock<IAuditLogRepository>();
            _service = new AuditLogService(_mockRepo.Object);
        }

        [Fact]
        public async Task LogAsync_ValidInput_CreatesAuditLog()
        {
            // Arrange
            string action = "Create";
            string entityName = "Patient";
            string entityId = "123";
            string performedBy = "admin";
            string details = "Created patient record.";

            AuditLog? capturedLog = null;

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                     .Callback<AuditLog>(log => capturedLog = log)
                    .ReturnsAsync((AuditLog log) => log);

            // Act
            await _service.LogAsync(action, entityName, entityId, performedBy, details);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(action, capturedLog.Action);
            Assert.Equal(entityName, capturedLog.EntityName);
            Assert.Equal(entityId, capturedLog.EntityId);
            Assert.Equal(performedBy, capturedLog.PerformedBy);
            Assert.Equal(details, capturedLog.Details);
            Assert.True((DateTime.UtcNow - capturedLog.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public async Task LogAsync_WithoutDetails_SetsEmptyDetails()
        {
            // Arrange
            string action = "Delete";
            string entityName = "Doctor";
            string entityId = "456";
            string performedBy = "system";

            AuditLog? capturedLog = null;

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                     .Callback<AuditLog>(log => capturedLog = log)
                    .ReturnsAsync((AuditLog log) => log);

            // Act
            await _service.LogAsync(action, entityName, entityId, performedBy);

            // Assert
            Assert.NotNull(capturedLog);
            Assert.Equal(string.Empty, capturedLog.Details);
        }
    }
}

namespace Application.DTOs.Responses.AuditLogResponse
{
    public class AuditLogResponse
    {
        public long? UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;

        public long? EntityId { get; set; }

        public string? Description { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

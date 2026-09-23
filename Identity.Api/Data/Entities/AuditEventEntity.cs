namespace Identity.Api.Data.Entities;
public class AuditEventEntity
{
    public long Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public long? SessionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Description { get; set; }
    public string? RemoteIp { get; set; }
    public string? UserAgent { get; set; }
    public string? CorrelationId { get; set; }
    public bool Success { get; set; }
    public string? AdditionalData { get; set; }
}

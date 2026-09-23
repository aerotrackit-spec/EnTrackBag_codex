namespace Identity.Api.Data.Entities;
public class EnTrackBagExceptionEntity
{
    public long ExceptionID { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? CorrelationId { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? QueryString { get; set; }
    public int? StatusCode { get; set; }
    public string? ExceptionType { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
    public string? UserName { get; set; }
    public string? RemoteIp { get; set; }
    public string? UserAgent { get; set; }
    public string? Source { get; set; }
    public bool IsHandled { get; set; }
    public string? AdditionalData { get; set; }
}

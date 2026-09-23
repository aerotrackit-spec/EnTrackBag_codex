namespace EnTrackBag.Api.DTOs;
public record DeviceDetailDto(string Category,string Name,string? Location,string Status,string? IpAddress,string? LastError,DateTime? LastConnected,DateTime? LastDisconnected);

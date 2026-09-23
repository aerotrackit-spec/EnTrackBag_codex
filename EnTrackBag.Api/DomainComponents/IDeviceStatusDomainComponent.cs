using EnTrackBag.Api.DTOs;
namespace EnTrackBag.Api.DomainComponents;
public interface IDeviceStatusDomainComponent { Task<DeviceSummaryDto> GetSummaryAsync(CancellationToken ct); Task<IReadOnlyList<DeviceDetailDto>> GetDetailsAsync(string? category,CancellationToken ct); }

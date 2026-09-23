using EnTrackBag.Api.DTOs;
namespace EnTrackBag.Api.DomainComponents;
public interface IDashboardDomainComponent
{
    Task<DashboardKpiDto> GetKpisAsync(CancellationToken ct);
    Task<BagHistoryDto?> GetHistoryAsync(string bagId, CancellationToken ct);
}

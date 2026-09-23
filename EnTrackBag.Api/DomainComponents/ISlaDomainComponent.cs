using EnTrackBag.Api.DTOs;
namespace EnTrackBag.Api.DomainComponents;
public interface ISlaDomainComponent
{
    Task<SlaBagDto[]> GetSlaAsync(CancellationToken ct);
}

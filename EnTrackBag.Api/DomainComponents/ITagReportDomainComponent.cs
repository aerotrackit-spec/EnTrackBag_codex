using EnTrackBag.Api.DTOs;

namespace EnTrackBag.Api.DomainComponents;

public interface ITagReportDomainComponent
{
    Task<byte[]> ExportAsync(TagReportSearchDto request, CancellationToken ct);
    Task<TagReportPageDto> SearchAsync(TagReportSearchDto request, CancellationToken ct);
    Task<TagHistoryDto?> GetHistoryAsync(string tagId, CancellationToken ct);
}

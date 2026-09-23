using EnTrackBag.Api.Data.Entities;
using EnTrackBag.Api.DTOs;

namespace EnTrackBag.Api.Data.Repositories;

public interface ITagReportRepository
{
    Task<TagViewEntity[]> SearchAsync(TagReportSearchDto request, CancellationToken ct);
    Task<TagViewEntity[]> GetHistoryAsync(string tagId, CancellationToken ct);
}

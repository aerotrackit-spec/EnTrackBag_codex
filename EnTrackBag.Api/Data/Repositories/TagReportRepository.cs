using EnTrackBag.Api.Data.Entities;
using EnTrackBag.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EnTrackBag.Api.Data.Repositories;

public class TagReportRepository : ITagReportRepository
{
    public const int EventLimit = 20000;
    private readonly BltsmftDbContext _db;
    public TagReportRepository(BltsmftDbContext db) { _db = db; }

    // DateTime.Now translates to SQL GETDATE(), matching the legacy query.
    private IQueryable<TagViewEntity> Eligible() => _db.Tags.AsNoTracking()
        .Where(x => x.IsCancelled == false && x.AlarmTime < DateTime.Now && x.TagID != null);

    public Task<TagViewEntity[]> SearchAsync(TagReportSearchDto request, CancellationToken ct)
    {
        var query = Eligible().Where(x => x.TStamp >= request.From && x.TStamp <= request.To);
        var tagId = request.TagId?.Trim();
        if (!string.IsNullOrEmpty(tagId)) query = query.Where(x => x.TagID == tagId);
        return ReadAsync(query, ct);
    }

    public Task<TagViewEntity[]> GetHistoryAsync(string tagId, CancellationToken ct) =>
        ReadAsync(Eligible().Where(x => x.TagID == tagId), ct);

    private static Task<TagViewEntity[]> ReadAsync(IQueryable<TagViewEntity> query, CancellationToken ct) =>
        query.OrderByDescending(x => x.TStamp).ThenByDescending(x => x.AlarmID)
            .Take(EventLimit + 1).ToArrayAsync(ct);
}

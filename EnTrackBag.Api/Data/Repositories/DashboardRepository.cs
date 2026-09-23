using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace EnTrackBag.Api.Data.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly BltsmftDbContext _db; public DashboardRepository(BltsmftDbContext db)
    {
        _db = db;
    }
    public Task<List<SuspectBagEntity>> GetTodayBagsAsync(DateTime from, DateTime to, CancellationToken ct) =>
        _db.SuspectBags.AsNoTracking().Where(x => x.TStamp >= from && x.TStamp < to).ToListAsync(ct);

    public Task<SuspectBagEntity?> GetBagAsync(string bagId, CancellationToken ct) =>
        _db.SuspectBags.AsNoTracking().SingleOrDefaultAsync(x => x.TagID == bagId, ct);
}

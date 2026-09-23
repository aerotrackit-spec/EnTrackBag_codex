using EnTrackBag.Api.Data.Entities; using Microsoft.EntityFrameworkCore;
namespace EnTrackBag.Api.Data.Repositories;
public class SlaRepository : ISlaRepository
{ private readonly BltsmftDbContext _db; public SlaRepository(BltsmftDbContext db){_db=db;} public Task<List<SuspectBagEntity>> GetActiveBagsAsync(CancellationToken ct)=>_db.SuspectBags.AsNoTracking().Where(x=>x.LastSeenTime!=null).OrderBy(x=>x.LastSeenTime).Take(500).ToListAsync(ct); public Task<SystemSettingEntity?> GetSettingAsync(string settingName,CancellationToken ct)=>_db.SystemSettings.AsNoTracking().SingleOrDefaultAsync(x=>x.SettingName==settingName,ct); }

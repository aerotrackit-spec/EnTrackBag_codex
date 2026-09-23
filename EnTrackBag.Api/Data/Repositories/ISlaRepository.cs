using EnTrackBag.Api.Data.Entities;
namespace EnTrackBag.Api.Data.Repositories;
public interface ISlaRepository { Task<List<SuspectBagEntity>> GetActiveBagsAsync(CancellationToken ct); Task<SystemSettingEntity?> GetSettingAsync(string settingName,CancellationToken ct); }

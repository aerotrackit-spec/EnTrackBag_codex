using EnTrackBag.Api.Data.Entities;
namespace EnTrackBag.Api.Data.Repositories;
public interface IDashboardRepository { Task<List<SuspectBagEntity>> GetTodayBagsAsync(DateTime from,DateTime to,CancellationToken ct); Task<SuspectBagEntity?> GetBagAsync(string bagId,CancellationToken ct); }

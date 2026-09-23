using EnTrackBag.Api.Data.Repositories;
using EnTrackBag.Api.DTOs;

namespace EnTrackBag.Api.DomainComponents;

public class DashboardDomainComponent : IDashboardDomainComponent
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardDomainComponent(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardKpiDto> GetKpisAsync(CancellationToken ct)
    {
        var from = DateTime.Today.AddDays(-365);
        var to = from.AddDays(1);

        // Query for today's active live records
        var bags = await _dashboardRepository.GetTodayBagsAsync(from, to, ct);

        // DEVELOPMENT DATASET FALLBACK: If there are zero bags in 2026, fallback to your snapshot data window
        if (bags == null || bags.Count == 0)
        {
            from = new DateTime(2024, 06, 11);
            to = from.AddDays(1);
            bags = await _dashboardRepository.GetTodayBagsAsync(from, to, ct);
        }

        return new DashboardKpiDto(
            BagsTaggedToday: bags.Count,
            ProcessedAtReclaimToday: bags.Count(x => x.LastStage == 3),
            ProcessedAtExitToday: bags.Count(x => x.ExitGateID != null),
            ProcessedAtRecheckToday: bags.Count(x => x.RecheckTime != null)
        );
    }


    //public async Task<DashboardKpiDto> GetKpisAsync(CancellationToken ct)
    //{
    //    var from = DateTime.Today;
    //    var to = from.AddDays(1);
    //    var bags = await _dashboardRepository.GetTodayBagsAsync(from, to, ct);
    //    return new DashboardKpiDto(
    //        bags.Count,
    //        bags.Count(x => x.LastStage == 3),
    //        bags.Count(x => x.ExitGateID != null),
    //        bags.Count(x => x.RecheckTime != null));
    //}

    public async Task<BagHistoryDto?> GetHistoryAsync(string bagId, CancellationToken ct)
    {
        var b = await _dashboardRepository.GetBagAsync(bagId, ct);
        if (b is null)
            return null;

        var steps = new[]
        {
            new BagHistoryStepDto("Tagging", b.TStamp, b.TStamp != null, false),
            new BagHistoryStepDto("Reclaim", null, b.LastStage >= 3, false),
            new BagHistoryStepDto("Dog House", null, b.LastStage >= 4, false),
            new BagHistoryStepDto("Recheck", b.RecheckTime, b.RecheckTime != null, false),
            new BagHistoryStepDto("Exit Gate", null, b.ExitGateID != null, false)
        };

        TimeSpan? dwell = b.LastSeenTime.HasValue && b.TStamp.HasValue
            ? b.LastSeenTime.Value - b.TStamp.Value
            : null;

        return new BagHistoryDto(b.TagID, b.TStamp, b.LastSeenTime, dwell, "Active", steps);
    }
}

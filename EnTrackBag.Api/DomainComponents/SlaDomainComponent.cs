using EnTrackBag.Api.Data.Repositories;
using EnTrackBag.Api.DTOs;
namespace EnTrackBag.Api.DomainComponents;
public class SlaDomainComponent : ISlaDomainComponent
{
    private readonly ISlaRepository _slaRepository;
    public SlaDomainComponent(ISlaRepository slaRepository)
    {
        _slaRepository = slaRepository;
    }
    public async Task<SlaBagDto[]> GetSlaAsync(CancellationToken ct)
    {
        var setting = await _slaRepository.GetSettingAsync("SlaThresholdMinutes", ct);
        if (setting is null || !int.TryParse(setting.SettingValue, out var minutes) || minutes <= 0)
            throw new InvalidOperationException("SLA threshold is not configured in BLTSMFT SystemSettings.");

        var threshold = TimeSpan.FromMinutes(minutes);
        var now = DateTime.Now;
        var rows = await _slaRepository.GetActiveBagsAsync(ct);
        return rows.Select(x =>
        {
            var dwell = x.LastSeenTime.HasValue ? now - x.LastSeenTime.Value : (TimeSpan?)null;
            var breach = dwell.HasValue && dwell.Value > threshold;
            return new SlaBagDto(x.TagID, x.TStamp, x.LastStage, x.LastSeenTime, dwell, threshold, breach, breach ? "Critical" : "Normal");
        }).ToArray();
    }
}

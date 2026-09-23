using EnTrackBag.Api.Data.Repositories;
using EnTrackBag.Api.DTOs;
using System.Globalization;
using System.Text;

namespace EnTrackBag.Api.DomainComponents;

public class TagReportDomainComponent : ITagReportDomainComponent
{
    private readonly ITagReportRepository _repository;
    public TagReportDomainComponent(ITagReportRepository repository) { _repository = repository; }

    public async Task<TagReportPageDto> SearchAsync(TagReportSearchDto request, CancellationToken ct)
    {
        var results = await GetMatchingRowsAsync(request, ct);
        var offset = ((long)request.Page - 1) * request.PageSize;
        var items = offset >= results.Count ? Array.Empty<TagReportRowDto>()
            : results.Skip((int)offset).Take(request.PageSize).ToArray();
        return new(items, results.Count, request.Page, request.PageSize);
    }

    public async Task<byte[]> ExportAsync(TagReportSearchDto request, CancellationToken ct)
    {
        var rows = await GetMatchingRowsAsync(request, ct);
        var csv = new StringBuilder("Tag ID,Threat,Alarm Type,Location,Last Seen,Count\r\n");
        foreach (var row in rows)
        {
            ct.ThrowIfCancellationRequested();
            csv.AppendJoin(",", CsvCell(row.TagId), CsvCell(row.Threat), CsvCell(row.AlarmType),
                CsvCell(row.Location), CsvCell(row.LastSeen?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
                row.Count.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
        }
        // UTF-8 BOM supports Arabic and other Unicode text in spreadsheet applications.
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
    }

    private static string CsvCell(string? value)
    {
        value ??= string.Empty;
        // Neutralize spreadsheet formulas, including those preceded by whitespace.
        var trimmed = value.TrimStart();
        if (trimmed.Length > 0 && "=+-@".Contains(trimmed[0])
            || value.Length > 0 && (char.IsControl(value[0]) || value[0] == '\uFEFF'))
            value = "'" + value;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private async Task<List<TagReportRowDto>> GetMatchingRowsAsync(TagReportSearchDto request, CancellationToken ct)
    {
        if (!request.From.HasValue || !request.To.HasValue || request.From > request.To
            || request.From < new DateTime(1753, 1, 1) || request.From > DateTime.Now)
            throw new ArgumentException("Enter a valid database-local date range; From cannot be in the future.");
        if (!(request.Normal || request.Prohibited || request.DelayProperPath || request.DelayImproperPath || request.MissedLuggage))
            throw new ArgumentException("Select at least one bag or alarm type.");
        var events = await _repository.SearchAsync(request, ct);
        if (events.Length > TagReportRepository.EventLimit)
            throw new ArgumentException("Search exceeds 20,000 events. Narrow the date range or enter an exact Tag ID; no partial results were returned.");

        var results = new List<TagReportRowDto>();
        foreach (var group in events.GroupBy(x => x.TagID!))
        {
            var latest = group.First();
            var threat = ThreatName(latest.ScanResult);
            var suspect = group.FirstOrDefault(x => x.AlarmType == "40" || x.AlarmType == "41");
            string alarm;
            bool include;
            if (threat == "Drugs") { alarm = "Prohibited Luggage"; include = request.Prohibited; }
            else if (suspect is not null)
            {
                // Legacy chooses the newest 40/41 event to select the suspect subtype.
                if (suspect.AlarmType == "41") { alarm = "Delay Improper Path"; include = request.DelayImproperPath; }
                else if (suspect.IsDelayed == true) { alarm = "Missed Luggage"; include = request.MissedLuggage; }
                else if (suspect.IsDelayed == false) { alarm = "Delay Proper Path"; include = request.DelayProperPath; }
                else { alarm = "Suspect Luggage"; include = request.DelayProperPath || request.DelayImproperPath || request.MissedLuggage; }
            }
            else { alarm = "Normal Event"; include = request.Normal; }
            if (include) results.Add(new(group.Key, threat, alarm, latest.CurrentAlarmLocation,
                latest.LastSeenTime, group.Count()));
        }
        return results;
    }

    public async Task<TagHistoryDto?> GetHistoryAsync(string tagId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tagId) || tagId.Trim().Length > 50)
            throw new ArgumentException("A Tag ID of at most 50 characters is required.");
        var events = await _repository.GetHistoryAsync(tagId.Trim(), ct);
        if (events.Length > TagReportRepository.EventLimit)
            throw new ArgumentException("This tag exceeds the 20,000-event history limit; no partial history was returned.");
        if (events.Length == 0) return null;
        var latest = events[0];
        return new(latest.TagID!, ThreatName(latest.ScanResult), latest.GlobalID, latest.IATACode,
            latest.LastStage, latest.LastSeenTime, latest.LogicalDeviceCode,
            events.Select(x => new TagHistoryEntryDto(x.TStamp, x.AlarmType, x.AlarmDesc,
                x.CurrentAlarmLocation, x.IsReported)).ToArray());
    }

    private static string? ThreatName(string? value) => value?.Trim() switch
    {
        "0" => "Unknown", "1" => "Drugs", "2" => "Food", "3" => "Alcohol",
        "4" => "CD_DVD", "5" => "TimeOut", _ => value
    };
}

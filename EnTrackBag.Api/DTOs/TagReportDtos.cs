using System.ComponentModel.DataAnnotations;

namespace EnTrackBag.Api.DTOs;

public class TagReportSearchDto
{
    [StringLength(50)] public string? TagId { get; set; }
    // Legacy operational timestamps are database-local wall times, not UTC.
    [Required] public DateTime? From { get; set; }
    [Required] public DateTime? To { get; set; }
    public bool Prohibited { get; set; }
    public bool Normal { get; set; }
    public bool DelayProperPath { get; set; }
    public bool DelayImproperPath { get; set; }
    public bool MissedLuggage { get; set; }
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)] public int PageSize { get; set; } = 50;
}

public record TagReportRowDto(string TagId, string? Threat, string AlarmType,
    string? Location, DateTime? LastSeen, int Count);
public record TagReportPageDto(TagReportRowDto[] Items, int Total, int Page, int PageSize);
public record TagHistoryEntryDto(DateTime? Time, string? AlarmType, string? Description,
    string? Location, bool? Smis);
public record TagHistoryDto(string TagId, string? Threat, string? GlobalId, string? IataCode,
    int? LastStage, DateTime? LastSeen, string? LastSeenLocation, TagHistoryEntryDto[] Entries);

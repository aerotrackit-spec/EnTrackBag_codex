namespace EnTrackBag.Api.DTOs;
public record BagHistoryStepDto(string Stage,DateTime? Timestamp,bool Completed,bool Delayed);
public record BagHistoryDto(string? BagId,DateTime? TaggedAt,DateTime? LastSeen,TimeSpan? DwellTime,string Status,IReadOnlyList<BagHistoryStepDto> Steps);

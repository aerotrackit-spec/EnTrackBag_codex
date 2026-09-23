namespace EnTrackBag.Api.DTOs;
public record SlaBagDto(string? BagId,DateTime? TaggedAt,int? LastStage,DateTime? LastSeen,TimeSpan? DwellTime,TimeSpan SlaThreshold,bool SlaBreach,string Status);

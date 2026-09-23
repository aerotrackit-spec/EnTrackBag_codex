namespace EnTrackBag.Api.DTOs;
public record DeviceCountDto(int Total,int Online,int Offline);
public record DeviceSummaryDto(DeviceCountDto TaggingStations,DeviceCountDto Readers,DeviceCountDto Antennas,DeviceCountDto Controllers);

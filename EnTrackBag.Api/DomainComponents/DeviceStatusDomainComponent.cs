using EnTrackBag.Api.Data.Repositories;
using EnTrackBag.Api.DTOs;
namespace EnTrackBag.Api.DomainComponents;

public class DeviceStatusDomainComponent : IDeviceStatusDomainComponent
{
    private readonly IReaderRepository _readerRepository;

    public DeviceStatusDomainComponent(IReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public async Task<DeviceSummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        var readers = await _readerRepository.GetReadersAsync(ct);
        var ants = await _readerRepository.GetAntennasAsync(ct);
        var ctrls = await _readerRepository.GetControllersAsync(ct);
        var stations = (await GetLogicalDetailsAsync(ct)).Where(x => x.DeviceType == 1).ToList();
        return new DeviceSummaryDto(new DeviceCountDto(stations.Count,
            stations.Count(x => x.Status == "Online"), stations.Count(x => x.Status == "Offline")),
            Count(readers.Select(x => x.Status), readers.Select(x => x.LastError)),
            Count(ants.Select(x => x.Status), ants.Select(x => x.LastError)),
            Count(ctrls.Select(x => x.Status), ctrls.Select(x => x.LastError))
        );
    }

    public async Task<IReadOnlyList<DeviceDetailDto>> GetDetailsAsync(string? category, CancellationToken ct)
    {
        var result = new List<DeviceDetailDto>();
        var logicalDetails = await GetLogicalDetailsAsync(ct);
        result.AddRange(logicalDetails.Where(x => category is null || x.Category == category));
        if (category is null or "Reader")
        {
            foreach (var x in await _readerRepository.GetReadersAsync(ct))
                result.Add(new DeviceDetailDto("Reader",
                    x.ReaderCode ?? $"Reader {x.ID}",
                    x.ReaderLocation,
                    Normalize(x.Status, x.LastError),
                    x.ReaderIP,
                    x.LastError,
                    x.LastConnected,
                    x.LastDisconnected)
                );
        }
        if (category is null or "Antenna")
        {
            foreach (var x in await _readerRepository.GetAntennasAsync(ct))
                result.Add(new DeviceDetailDto("Antenna",
                    x.AntennaCode ?? $"Antenna {x.ID}",
                    x.AntennaLocation,
                    Normalize(x.Status, x.LastError),
                    null, x.LastError,
                    x.LastConnected,
                    x.LastDisconnected)
                );
        }
        if (category is null or "Controller")
        {
            foreach (var x in await _readerRepository.GetControllersAsync(ct))
                result.Add(new DeviceDetailDto("Controller",
                    x.ControllerName ?? $"Controller {x.ID}",
                    null,
                    Normalize(x.Status, x.LastError),
                    x.ControllerIP,
                    x.LastError,
                    x.LastConnected,
                    x.LastDisconnected)
                );
        }
        return result;
    }

    // IDs and heartbeat semantics from legacy mdGlobal.vb / frmDeviceStatusv2.vb.
    private static readonly Dictionary<int, string> DeviceTypes = new()
    {
        [1] = "Tagging Station",
        [2] = "Tagging Read Point",
        [3] = "Dog House Airside",
        [4] = "Dog House Landside",
        [5] = "Exit Gate",
        [6] = "Inside Lounge",
        [7] = "Exit Lounge",
        [9] = "BHS Return Feed",
        [8] = "Recheck Station"
    };

    private async Task<List<DeviceDetailDto>> GetLogicalDetailsAsync(CancellationToken ct)
    {
        var devices = await _readerRepository.GetLogicalDeviceStatusAsync(ct);
        var antennas = (await _readerRepository.GetAntennasAsync(ct)).ToLookup(x => x.ReaderID);
        // Legacy operational datetime columns store local wall-clock time (not UTC).
        var now = DateTime.Now;
        return devices.Where(x => x.DeviceType.HasValue && DeviceTypes.ContainsKey(x.DeviceType.Value))
            .OrderBy(x => x.ID).ThenBy(x => x.ReaderID).Select(x =>
            {
                var pc = x.DeviceType is 1 or 8;
                var connected = pc ? x.LastConnected : x.ReaderLastConnected;
                var online = connected.HasValue && now - connected.Value <= TimeSpan.FromMinutes(1);
                var ports = Enumerable.Range(1, 8).Select(port =>
                {
                    if (pc || x.ReaderID is null)
                        return "—";
                    var matches = antennas[x.ReaderID].Where(a => a.AntennaPort == port).ToList();
                    if (matches.Count == 0)
                        return "X";
                    return online && matches.Any(a => a.Status == "Connected") ? "Y" : "N";
                }).ToArray();
                return new DeviceDetailDto(DeviceTypes[x.DeviceType!.Value],
                    x.Code ?? x.Name ?? $"Device {x.ID}", null, online ? "Online" : "Offline",
                    pc ? x.Ip : x.ReaderIp, x.LastError, connected,
                    pc ? x.LastDisconnected : x.ReaderLastDisconnected, x.ID, x.DeviceType, ports);
            }).ToList();
    }

    private static DeviceCountDto Count(IEnumerable<string?> statuses, IEnumerable<string?> errors)
    {
        var items = statuses.Zip(errors, (status, error) => new { status, error }).ToList();
        return new DeviceCountDto(items.Count,
            items.Count(x => Normalize(x.status, x.error) == "Online"),
            items.Count(x => Normalize(x.status, x.error) == "Offline"));
    }

    private static string Normalize(string? status, string? error)
    {
        // Connectivity is binary; diagnostic errors remain in LastError, not a third status.
        if (status is "Connected" or "Online")
            return "Online";
        return "Offline";
    }
}

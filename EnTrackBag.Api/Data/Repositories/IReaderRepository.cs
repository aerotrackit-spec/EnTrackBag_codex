using EnTrackBag.Api.Data.Entities;
namespace EnTrackBag.Api.Data.Repositories;
public interface IReaderRepository { Task<List<ReaderEntity>> GetReadersAsync(CancellationToken ct); Task<List<AntennaEntity>> GetAntennasAsync(CancellationToken ct); Task<List<ControllerEntity>> GetControllersAsync(CancellationToken ct);
    Task<List<LogicalDeviceStatusRecord>> GetLogicalDeviceStatusAsync(CancellationToken ct);
}
public record LogicalDeviceStatusRecord(int ID, int? DeviceType, string? Code, string? Name,
    string? Ip, DateTime? LastConnected, DateTime? LastDisconnected, string? LastError,
    int? ReaderID, string? ReaderIp, DateTime? ReaderLastConnected, DateTime? ReaderLastDisconnected);

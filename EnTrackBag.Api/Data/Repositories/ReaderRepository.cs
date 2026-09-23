using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace EnTrackBag.Api.Data.Repositories;

public class ReaderRepository : IReaderRepository
{
    private readonly BltsmftDbContext _db;

    public ReaderRepository(BltsmftDbContext db)
    {
        _db = db;
    }

    public Task<List<ReaderEntity>> GetReadersAsync(CancellationToken ct) =>
        _db.Readers.AsNoTracking().Where(x => x.IsActive != false).ToListAsync(ct);

    public Task<List<AntennaEntity>> GetAntennasAsync(CancellationToken ct) =>
        _db.Antennas.AsNoTracking().Where(x => x.IsActive != false).ToListAsync(ct);

    public Task<List<ControllerEntity>> GetControllersAsync(CancellationToken ct) =>
        _db.Controllers.AsNoTracking().Where(x => x.IsActive != false).ToListAsync(ct);

    public Task<List<LogicalDeviceStatusRecord>> GetLogicalDeviceStatusAsync(CancellationToken ct)
    {
        // Match the legacy DISTINCT logical-device/reader join, including unmapped stations.
        var mappedReaders = from map in _db.LogicalDeviceMaps.AsNoTracking()
                            join reader in _db.Readers.AsNoTracking() on map.ReaderID equals reader.ID
                            select new
                            {
                                map.LogicalDeviceID,
                                ReaderID = (int?)reader.ID,
                                reader.ReaderIP,
                                reader.LastConnected,
                                reader.LastDisconnected
                            };
        return (from device in _db.LogicalDevices.AsNoTracking()
                join reader in mappedReaders on device.ID equals reader.LogicalDeviceID into readers
                from reader in readers.DefaultIfEmpty()
                select new LogicalDeviceStatusRecord(device.ID, device.DeviceType,
                    device.LogicalDeviceCode, device.DeviceName, device.IPAddress,
                    device.LastConnected, device.LastDisconnected, device.LastError,
                    reader.ReaderID, reader.ReaderIP, (DateTime?)reader.LastConnected,
                    (DateTime?)reader.LastDisconnected)).Distinct().ToListAsync(ct);
    }
}

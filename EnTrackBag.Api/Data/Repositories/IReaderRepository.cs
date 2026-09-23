using EnTrackBag.Api.Data.Entities;
namespace EnTrackBag.Api.Data.Repositories;
public interface IReaderRepository { Task<List<ReaderEntity>> GetReadersAsync(CancellationToken ct); Task<List<AntennaEntity>> GetAntennasAsync(CancellationToken ct); Task<List<ControllerEntity>> GetControllersAsync(CancellationToken ct); }

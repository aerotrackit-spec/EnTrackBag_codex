using System.Text.Json;
using EnTrackBag.Api.Data;
using EnTrackBag.Api.Data.Entities;
using EnTrackBag.Api.Data.Repositories;
using EnTrackBag.Api.DomainComponents;
using Microsoft.EntityFrameworkCore;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine($"PASS: {message}");
}

var domain = new DeviceStatusDomainComponent(new SampleRepository());
var details = await domain.GetDetailsAsync(null, default);
var logical = details.Where(x => x.DeviceType != null).ToList();
Check(logical.Count == 9 && logical.Select(x => x.Category).Distinct().Count() == 9, "All nine legacy device types");
Check(logical.Single(x => x.DeviceType == 1).Status == "Online", "Station uses its own heartbeat");
Check(logical.Single(x => x.DeviceType == 2).Status == "Online", "Read point uses reader heartbeat");
Check(logical.Single(x => x.DeviceType == 3).Status == "Offline", "Old reader heartbeat expires");
Check(logical.Single(x => x.DeviceType == 4).Status == "Offline", "Missing heartbeat is offline");
Check(logical.Single(x => x.DeviceType == 8).Status == "Offline", "Recheck ignores fresh reader heartbeat");
Check(logical.Single(x => x.DeviceType == 2).AntennaPorts!.SequenceEqual(new[] { "Y", "N", "X", "X", "X", "X", "X", "X" }), "Eight antenna ports, including unconfigured ports");
Check(logical.Single(x => x.DeviceType == 3).AntennaPorts![0] == "N", "Offline reader overrides connected antenna");
Check(logical.Single(x => x.DeviceType == 1).AntennaPorts!.All(x => x == "—"), "PC station antenna ports not applicable");
Check((await domain.GetDetailsAsync("Exit Lounge", default)).All(x => x.Category == "Exit Lounge"), "Category filter");
var summary = await domain.GetSummaryAsync(default);
Check(summary.TaggingStations.Total == 1 && summary.TaggingStations.Online == 1, "Tagging station card uses real logical count");

if (args.Length == 1)
{
    using var config = JsonDocument.Parse(File.ReadAllText(args[0]));
    var connection = config.RootElement.GetProperty("ConnectionStrings").GetProperty("BLTSMFT").GetString()!;
    await using var db = new BltsmftDbContext(new DbContextOptionsBuilder<BltsmftDbContext>().UseSqlServer(connection).Options);
    var repository = new ReaderRepository(db);
    var rows = await repository.GetLogicalDeviceStatusAsync(default);
    Check(rows.Count == rows.Distinct().Count(), "Live EF query executes and deduplicates mappings");
    var live = await new DeviceStatusDomainComponent(repository).GetDetailsAsync(null, default);
    Check(live.Where(x => x.DeviceType != null).All(x => x.AntennaPorts?.Count == 8), "Live logical devices have eight port states");
    foreach (var group in live.Where(x => x.DeviceType != null).GroupBy(x => x.Category))
        Console.WriteLine($"{group.Key}: {group.Count()} total, {group.Count(x => x.Status == "Online")} online");
}

sealed class SampleRepository : IReaderRepository
{
    public Task<List<LogicalDeviceStatusRecord>> GetLogicalDeviceStatusAsync(CancellationToken ct)
    {
        var now = DateTime.Now;
        return Task.FromResult(Enumerable.Range(1, 9).Select(id => new LogicalDeviceStatusRecord(
            id, id, $"TEST-{id}", null, null, id == 1 ? now : now.AddMinutes(-2), null, null,
            id, null, id == 3 ? now.AddMinutes(-2) : id == 4 ? null : now, null)).ToList());
    }
    public Task<List<AntennaEntity>> GetAntennasAsync(CancellationToken ct) => Task.FromResult(new List<AntennaEntity>
    {
        new() { ReaderID = 2, AntennaPort = 1, Status = "Connected" },
        new() { ReaderID = 2, AntennaPort = 2, Status = "Disconnected" },
        new() { ReaderID = 3, AntennaPort = 1, Status = "Connected" }
    });
    public Task<List<ReaderEntity>> GetReadersAsync(CancellationToken ct) => Task.FromResult(new List<ReaderEntity>());
    public Task<List<ControllerEntity>> GetControllersAsync(CancellationToken ct) => Task.FromResult(new List<ControllerEntity>());
}

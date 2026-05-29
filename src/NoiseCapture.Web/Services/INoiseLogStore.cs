using NoiseCapture.Web.Models;

namespace NoiseCapture.Web.Services;

public interface INoiseLogStore
{
    Task<NoiseLogEntry?> GetLastEntryAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<NoiseLogEntry>> GetEntriesAsync(CancellationToken cancellationToken, int? take = null);

    Task AddEntryAsync(NoiseLogEntry entry, CancellationToken cancellationToken);
}

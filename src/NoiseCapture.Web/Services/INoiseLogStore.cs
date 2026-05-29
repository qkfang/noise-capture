using NoiseCapture.Web.Models;

namespace NoiseCapture.Web.Services;

public interface INoiseLogStore
{
    Task<NoiseLogEntry?> GetLastEntryAsync(CancellationToken cancellationToken);

    Task AddEntryAsync(NoiseLogEntry entry, CancellationToken cancellationToken);
}

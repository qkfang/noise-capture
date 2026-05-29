using System.Text.Json;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Options;

namespace NoiseCapture.Web.Services;

public sealed class NoiseLogStore : INoiseLogStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<NoiseLogStore> _logger;
    private readonly LocalDataOptions _localDataOptions;
    private readonly NoiseStorageOptions _storageOptions;

    public NoiseLogStore(
        IOptions<LocalDataOptions> localDataOptions,
        IOptions<NoiseStorageOptions> storageOptions,
        ILogger<NoiseLogStore> logger)
    {
        _localDataOptions = localDataOptions.Value;
        _storageOptions = storageOptions.Value;
        _logger = logger;
    }

    public async Task<NoiseLogEntry?> GetLastEntryAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            return entries.LastOrDefault();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<NoiseLogEntry>> GetEntriesAsync(CancellationToken cancellationToken, int? take = null)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            IEnumerable<NoiseLogEntry> ordered = entries.OrderByDescending(e => e.RecordedAtSydney);

            if (take.HasValue)
            {
                ordered = ordered.Take(take.Value);
            }

            return ordered.ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddEntryAsync(NoiseLogEntry entry, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            entries.Add(entry);

            var json = JsonSerializer.Serialize(entries, JsonOptions);
            var dataPath = GetLocalDataPath();
            await File.WriteAllTextAsync(dataPath, json, cancellationToken);

            await UploadToBlobAsync(dataPath, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<NoiseLogEntry?> GetEntryAsync(DateTimeOffset recordedAtSydney, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            return entries.FirstOrDefault(e => e.RecordedAtSydney == recordedAtSydney);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateEntryAsync(DateTimeOffset originalRecordedAtSydney, NoiseLogEntry updated, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            var index = entries.FindIndex(e => e.RecordedAtSydney == originalRecordedAtSydney);

            if (index < 0)
            {
                return false;
            }

            entries[index] = updated;

            var json = JsonSerializer.Serialize(entries, JsonOptions);
            var dataPath = GetLocalDataPath();
            await File.WriteAllTextAsync(dataPath, json, cancellationToken);

            await UploadToBlobAsync(dataPath, cancellationToken);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteEntryAsync(DateTimeOffset recordedAtSydney, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var entries = await ReadEntriesAsync(cancellationToken);
            var removed = entries.RemoveAll(e => e.RecordedAtSydney == recordedAtSydney);

            if (removed == 0)
            {
                return false;
            }

            var json = JsonSerializer.Serialize(entries, JsonOptions);
            var dataPath = GetLocalDataPath();
            await File.WriteAllTextAsync(dataPath, json, cancellationToken);

            await UploadToBlobAsync(dataPath, cancellationToken);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<NoiseLogEntry>> ReadEntriesAsync(CancellationToken cancellationToken)
    {
        var dataPath = GetLocalDataPath();

        if (!File.Exists(dataPath))
        {
            return [];
        }

        var json = await File.ReadAllTextAsync(dataPath, cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<NoiseLogEntry>>(json, JsonOptions) ?? [];
    }

    private string GetLocalDataPath()
    {
        var folder = _localDataOptions.FolderPath;

        if (!Path.IsPathRooted(folder))
        {
            folder = Path.Combine(AppContext.BaseDirectory, folder);
        }

        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "noise-log.json");
    }

    private async Task UploadToBlobAsync(string localDataPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_storageOptions.AccountUrl))
        {
            _logger.LogInformation("NoiseStorage AccountUrl not configured. Skipping blob upload.");
            return;
        }

        var blobServiceClient = new BlobServiceClient(new Uri(_storageOptions.AccountUrl), new DefaultAzureCredential());
        var containerClient = blobServiceClient.GetBlobContainerClient(_storageOptions.ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(_storageOptions.BlobName);
        await using var stream = File.OpenRead(localDataPath);
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
    }
}

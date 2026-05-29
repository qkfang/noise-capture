using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NoiseCapture.Web.Data;
using NoiseCapture.Web.Models;

namespace NoiseCapture.Web.Services;

public sealed class NoiseLogStore(NoiseCaptureDbContext dbContext) : INoiseLogStore
{
    public async Task<NoiseLogEntry?> GetLastEntryAsync(CancellationToken cancellationToken)
    {
        return await dbContext.NoiseLogEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.Id)
            .Select(ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NoiseLogEntry>> GetEntriesAsync(CancellationToken cancellationToken, int? take = null)
    {
        var query = dbContext.NoiseLogEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.RecordedAtSydney)
            .Select(ProjectToModel());

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddEntryAsync(NoiseLogEntry entry, CancellationToken cancellationToken)
    {
        await dbContext.NoiseLogEntries.AddAsync(MapToEntity(entry), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<NoiseLogEntry?> GetEntryAsync(DateTimeOffset recordedAtSydney, CancellationToken cancellationToken)
    {
        return await dbContext.NoiseLogEntries
            .AsNoTracking()
            .Where(entry => entry.RecordedAtSydney == recordedAtSydney)
            .Select(ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> UpdateEntryAsync(DateTimeOffset originalRecordedAtSydney, NoiseLogEntry updated, CancellationToken cancellationToken)
    {
        var existing = await dbContext.NoiseLogEntries
            .Include(entry => entry.NoiseSources)
            .Include(entry => entry.Locations)
            .FirstOrDefaultAsync(entry => entry.RecordedAtSydney == originalRecordedAtSydney, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.RecordedAtSydney = updated.RecordedAtSydney;
        existing.Intensity = updated.Intensity;
        existing.Loudness = updated.Loudness;
        existing.Tone = updated.Tone;
        existing.Note = updated.Note;
        existing.Weather = updated.Weather;
        existing.ContinuedFromLast = updated.ContinuedFromLast;

        existing.NoiseSources.Clear();
        foreach (var (value, index) in updated.NoiseSources.Select((value, index) => (value, index)))
        {
            existing.NoiseSources.Add(new NoiseLogEntryNoiseSourceEntity
            {
                NoiseLogEntryId = existing.Id,
                SortOrder = index,
                Value = value
            });
        }

        existing.Locations.Clear();
        foreach (var (value, index) in updated.Locations.Select((value, index) => (value, index)))
        {
            existing.Locations.Add(new NoiseLogEntryLocationEntity
            {
                NoiseLogEntryId = existing.Id,
                SortOrder = index,
                Value = value
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteEntryAsync(DateTimeOffset recordedAtSydney, CancellationToken cancellationToken)
    {
        var existing = await dbContext.NoiseLogEntries
            .FirstOrDefaultAsync(entry => entry.RecordedAtSydney == recordedAtSydney, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        dbContext.NoiseLogEntries.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Expression<Func<NoiseLogEntryEntity, NoiseLogEntry>> ProjectToModel()
    {
        return entry => new NoiseLogEntry
        {
            RecordedAtSydney = entry.RecordedAtSydney,
            NoiseSources = entry.NoiseSources
                .OrderBy(noiseSource => noiseSource.SortOrder)
                .Select(noiseSource => noiseSource.Value)
                .ToList(),
            Intensity = entry.Intensity,
            Loudness = entry.Loudness,
            Tone = entry.Tone,
            Locations = entry.Locations
                .OrderBy(location => location.SortOrder)
                .Select(location => location.Value)
                .ToList(),
            Note = entry.Note,
            Weather = entry.Weather,
            ContinuedFromLast = entry.ContinuedFromLast
        };
    }

    private static NoiseLogEntryEntity MapToEntity(NoiseLogEntry entry)
    {
        return new NoiseLogEntryEntity
        {
            RecordedAtSydney = entry.RecordedAtSydney,
            Intensity = entry.Intensity,
            Loudness = entry.Loudness,
            Tone = entry.Tone,
            Note = entry.Note,
            Weather = entry.Weather,
            ContinuedFromLast = entry.ContinuedFromLast,
            NoiseSources = entry.NoiseSources
                .Select((value, index) => new NoiseLogEntryNoiseSourceEntity
                {
                    SortOrder = index,
                    Value = value
                })
                .ToList(),
            Locations = entry.Locations
                .Select((value, index) => new NoiseLogEntryLocationEntity
                {
                    SortOrder = index,
                    Value = value
                })
                .ToList()
        };
    }
}

using Microsoft.EntityFrameworkCore;

namespace NoiseCapture.Web.Data;

public sealed class NoiseCaptureDbContext(DbContextOptions<NoiseCaptureDbContext> options) : DbContext(options)
{
    public DbSet<NoiseLogEntryEntity> NoiseLogEntries => Set<NoiseLogEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entry = modelBuilder.Entity<NoiseLogEntryEntity>();
        entry.ToTable("NoiseLogEntries");
        entry.HasKey(item => item.Id);
        entry.HasIndex(item => item.RecordedAtSydney).IsUnique();
        entry.Property(item => item.RecordedAtSydney).IsRequired();
        entry.Property(item => item.Intensity).HasMaxLength(32).IsRequired();
        entry.Property(item => item.Loudness).HasMaxLength(32).IsRequired();
        entry.Property(item => item.Tone).HasMaxLength(32).IsRequired();
        entry.Property(item => item.Note).HasMaxLength(2000).IsRequired();

        entry.HasMany(item => item.NoiseSources)
            .WithOne(item => item.NoiseLogEntry)
            .HasForeignKey(item => item.NoiseLogEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        entry.HasMany(item => item.Locations)
            .WithOne(item => item.NoiseLogEntry)
            .HasForeignKey(item => item.NoiseLogEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        var noiseSource = modelBuilder.Entity<NoiseLogEntryNoiseSourceEntity>();
        noiseSource.ToTable("NoiseLogEntryNoiseSources");
        noiseSource.HasKey(item => new { item.NoiseLogEntryId, item.SortOrder });
        noiseSource.Property(item => item.Value).HasMaxLength(128).IsRequired();

        var location = modelBuilder.Entity<NoiseLogEntryLocationEntity>();
        location.ToTable("NoiseLogEntryLocations");
        location.HasKey(item => new { item.NoiseLogEntryId, item.SortOrder });
        location.Property(item => item.Value).HasMaxLength(128).IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NoiseCapture.Web.Data;

public sealed class NoiseCaptureDbContextFactory : IDesignTimeDbContextFactory<NoiseCaptureDbContext>
{
    public NoiseCaptureDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NoiseCaptureDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__NoiseCaptureDatabase")
            ?? "Server=(localdb)\\mssqllocaldb;Database=NoiseCapture;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new NoiseCaptureDbContext(optionsBuilder.Options);
    }
}

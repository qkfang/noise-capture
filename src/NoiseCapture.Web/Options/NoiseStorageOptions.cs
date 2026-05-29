namespace NoiseCapture.Web.Options;

public sealed class NoiseStorageOptions
{
    public const string SectionName = "NoiseStorage";

    public string? AccountUrl { get; set; }

    public string ContainerName { get; set; } = "noise-logs";

    public string BlobName { get; set; } = "noise-log.json";

    public string? TenantId { get; set; }
}

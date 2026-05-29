namespace NoiseCapture.Web.Data;

public sealed class NoiseLogEntryNoiseSourceEntity
{
    public int NoiseLogEntryId { get; set; }

    public int SortOrder { get; set; }

    public string Value { get; set; } = string.Empty;

    public NoiseLogEntryEntity NoiseLogEntry { get; set; } = null!;
}

namespace NoiseCapture.Web.Data;

public sealed class NoiseLogEntryEntity
{
    public int Id { get; set; }

    public DateTimeOffset RecordedDateTime { get; set; }

    public DateTimeOffset CreateDateTime { get; set; }

    public string Intensity { get; set; } = string.Empty;

    public string Loudness { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string? Weather { get; set; }

    public bool ContinuedFromLast { get; set; }

    public List<NoiseLogEntryNoiseSourceEntity> NoiseSources { get; set; } = [];

    public List<NoiseLogEntryLocationEntity> Locations { get; set; } = [];
}

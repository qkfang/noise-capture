namespace NoiseCapture.Web.Models;

public sealed class NoiseLogEntry
{
    public int Id { get; init; }

    public DateTimeOffset RecordedDateTime { get; init; }

    public DateTimeOffset CreateDateTime { get; init; }

    public IReadOnlyList<string> NoiseSources { get; init; } = [];

    public string Intensity { get; init; } = string.Empty;

    public string Loudness { get; init; } = string.Empty;

    public string Tone { get; init; } = string.Empty;

    public IReadOnlyList<string> Locations { get; init; } = [];

    public string? Note { get; init; }

    public string? Weather { get; init; }

    public bool ContinuedFromLast { get; init; }
}

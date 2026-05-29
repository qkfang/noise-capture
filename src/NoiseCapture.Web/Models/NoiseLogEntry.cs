namespace NoiseCapture.Web.Models;

public sealed class NoiseLogEntry
{
    public DateTimeOffset RecordedAtSydney { get; init; }

    public IReadOnlyList<string> NoiseSources { get; init; } = [];

    public string Intensity { get; init; } = string.Empty;

    public string Feeling { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string Note { get; init; } = string.Empty;
}

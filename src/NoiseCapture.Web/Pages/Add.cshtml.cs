using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class AddModel(INoiseLogStore noiseLogStore) : PageModel
{
    private static readonly string[] NoiseSources =
    [
        "Club roof vent",
        "A/C units",
        "Roof vent",
        "Wall vent"
    ];

    private static readonly string[] IntensityLevels = ["High", "Moderate", "Low", "Very Low"];
    private static readonly string[] LoudnessLevels = ["Nothing", "Unpleasant", "Annoying", "Can't stand"];
    private static readonly string[] ToneOptions = ["Continuous", "Intermittent", "Impulsive", "Low"];
    private static readonly string[] Locations = ["Living room", "Bedroom"];

    [BindProperty]
    public NoiseLogInput Input { get; set; } = new();

    public IReadOnlyList<string> NoiseSourceOptions => NoiseSources;

    public IReadOnlyList<string> IntensityOptions => IntensityLevels;

    public IReadOnlyList<string> LoudnessOptions => LoudnessLevels;

    public IReadOnlyList<string> ToneSelectableOptions => ToneOptions;

    public IReadOnlyList<string> LocationOptions => Locations;

    public bool Saved { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await PrefillAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!IsValidOptionSet(Input.NoiseSources, NoiseSources))
        {
            ModelState.AddModelError(nameof(Input.NoiseSources), "Invalid noise source selected.");
        }

        if (!IntensityLevels.Contains(Input.Intensity))
        {
            ModelState.AddModelError(nameof(Input.Intensity), "Invalid intensity value.");
        }

        if (!LoudnessLevels.Contains(Input.Loudness))
        {
            ModelState.AddModelError(nameof(Input.Loudness), "Invalid loudness value.");
        }

        if (!ToneOptions.Contains(Input.Tone))
        {
            ModelState.AddModelError(nameof(Input.Tone), "Invalid tone value.");
        }

        if (!IsValidOptionSet(Input.Locations, Locations))
        {
            ModelState.AddModelError(nameof(Input.Locations), "Invalid location selected.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!DateTime.TryParseExact(
                Input.RecordedDateTimeLocal,
                ["yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var localDateTime))
        {
            ModelState.AddModelError(nameof(Input.RecordedDateTimeLocal), "Use a valid date and time.");
            return Page();
        }

        var offset = SydneyTime.TimeZone.GetUtcOffset(localDateTime);

        var entry = new NoiseLogEntry
        {
            RecordedDateTime = new DateTimeOffset(localDateTime, offset),
            NoiseSources = Input.NoiseSources,
            Intensity = Input.Intensity,
            Loudness = Input.Loudness,
            Tone = Input.Tone,
            Locations = Input.Locations,
            Note = string.IsNullOrWhiteSpace(Input.Note) ? null : Input.Note.Trim(),
            Weather = string.IsNullOrWhiteSpace(Input.Weather) ? null : Input.Weather.Trim(),
            ContinuedFromLast = Input.ContinuedFromLast
        };

        await noiseLogStore.AddEntryAsync(entry, cancellationToken);

        return RedirectToPage("/List");
    }

    private async Task PrefillAsync(CancellationToken cancellationToken)
    {
        var lastEntry = await noiseLogStore.GetLastEntryAsync(cancellationToken);

        if (lastEntry is null)
        {
            Input = new NoiseLogInput
            {
                RecordedDateTimeLocal = ToLocalDateTimeValue(ToSydneyNow()),
                NoiseSources = [NoiseSources[0]],
                Intensity = IntensityLevels[1],
                Loudness = LoudnessLevels[1],
                Tone = ToneOptions[0],
                Locations = [Locations[0]],
                Note = null,
                Weather = null,
                ContinuedFromLast = false
            };

            return;
        }

        Input = new NoiseLogInput
        {
            RecordedDateTimeLocal = ToLocalDateTimeValue(ToSydneyNow()),
            NoiseSources = [.. lastEntry.NoiseSources],
            Intensity = IntensityLevels.Contains(lastEntry.Intensity) ? lastEntry.Intensity : IntensityLevels[1],
            Loudness = LoudnessLevels.Contains(lastEntry.Loudness) ? lastEntry.Loudness : LoudnessLevels[1],
            Tone = ToneOptions.Contains(lastEntry.Tone) ? lastEntry.Tone : ToneOptions[0],
            Locations = [.. lastEntry.Locations],
            Note = lastEntry.Note,
            Weather = lastEntry.Weather,
            ContinuedFromLast = false
        };
    }

    private static DateTimeOffset ToSydneyNow() =>
        SydneyTime.Convert(DateTimeOffset.UtcNow);

    private static bool IsValidOptionSet(IEnumerable<string> selectedValues, IReadOnlyCollection<string> validValues)
    {
        return selectedValues.All(validValues.Contains);
    }

    private static string ToLocalDateTimeValue(DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }
}

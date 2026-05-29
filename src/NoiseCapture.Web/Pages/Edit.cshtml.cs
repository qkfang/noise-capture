using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class EditModel(INoiseLogStore noiseLogStore) : PageModel
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

    [BindProperty]
    public string OriginalRecordedAt { get; set; } = string.Empty;

    public IReadOnlyList<string> NoiseSourceOptions => NoiseSources;
    public IReadOnlyList<string> IntensityOptions => IntensityLevels;
    public IReadOnlyList<string> LoudnessOptions => LoudnessLevels;
    public IReadOnlyList<string> ToneSelectableOptions => ToneOptions;
    public IReadOnlyList<string> LocationOptions => Locations;

    public async Task<IActionResult> OnGetAsync(string recordedAt, CancellationToken cancellationToken)
    {
        if (!TryParseRecordedAt(recordedAt, out var parsed))
        {
            TempData["StatusMessage"] = "Invalid entry identifier.";
            return RedirectToPage("/List");
        }

        var entry = await noiseLogStore.GetEntryAsync(parsed, cancellationToken);

        if (entry is null)
        {
            TempData["StatusMessage"] = "Entry not found.";
            return RedirectToPage("/List");
        }

        OriginalRecordedAt = entry.RecordedAtSydney.ToString("o", CultureInfo.InvariantCulture);
        Input = new NoiseLogInput
        {
            RecordedAtSydneyLocal = entry.RecordedAtSydney.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
            NoiseSources = [.. entry.NoiseSources],
            Intensity = entry.Intensity,
            Loudness = entry.Loudness,
            Tone = entry.Tone,
            Locations = [.. entry.Locations],
            Note = entry.Note,
            ContinuedFromLast = entry.ContinuedFromLast
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!TryParseRecordedAt(OriginalRecordedAt, out var originalParsed))
        {
            ModelState.AddModelError(string.Empty, "Invalid entry identifier.");
            return Page();
        }

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
                Input.RecordedAtSydneyLocal,
                "yyyy-MM-ddTHH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var localDateTime))
        {
            ModelState.AddModelError(nameof(Input.RecordedAtSydneyLocal), "Use a valid date and time.");
            return Page();
        }

        var sydneyTimeZone = ResolveSydneyTimeZone();
        var offset = sydneyTimeZone.GetUtcOffset(localDateTime);

        var updated = new NoiseLogEntry
        {
            RecordedAtSydney = new DateTimeOffset(localDateTime, offset),
            NoiseSources = Input.NoiseSources,
            Intensity = Input.Intensity,
            Loudness = Input.Loudness,
            Tone = Input.Tone,
            Locations = Input.Locations,
            Note = Input.Note.Trim(),
            ContinuedFromLast = Input.ContinuedFromLast
        };

        var ok = await noiseLogStore.UpdateEntryAsync(originalParsed, updated, cancellationToken);
        TempData["StatusMessage"] = ok ? "Entry updated." : "Entry not found.";

        return RedirectToPage("/List");
    }

    private static bool TryParseRecordedAt(string value, out DateTimeOffset parsed)
    {
        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsed);
    }

    private static bool IsValidOptionSet(IEnumerable<string> selectedValues, IReadOnlyCollection<string> validValues)
    {
        return selectedValues.All(validValues.Contains);
    }

    private static TimeZoneInfo ResolveSydneyTimeZone()
    {
        return TimeZoneInfo.TryFindSystemTimeZoneById("Australia/Sydney", out var sydney)
            ? sydney
            : TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
    }
}

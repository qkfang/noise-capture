using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class IndexModel(INoiseLogStore noiseLogStore) : PageModel
{
    private static readonly string[] NoiseSources =
    [
        "Club roof vent",
        "A/C units",
        "Roof vent",
        "Wall vent"
    ];

    private static readonly string[] Levels = ["Extreme", "High", "Medium", "Low"];
    private static readonly string[] Locations = ["Living room", "Bedroom"];

    [BindProperty]
    public NoiseLogInput Input { get; set; } = new();

    public IReadOnlyList<string> NoiseSourceOptions => NoiseSources;

    public IReadOnlyList<string> LevelOptions => Levels;

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

        if (!Levels.Contains(Input.Intensity))
        {
            ModelState.AddModelError(nameof(Input.Intensity), "Invalid intensity value.");
        }

        if (!Levels.Contains(Input.Feeling))
        {
            ModelState.AddModelError(nameof(Input.Feeling), "Invalid feeling value.");
        }

        if (!Locations.Contains(Input.Location))
        {
            ModelState.AddModelError(nameof(Input.Location), "Invalid location value.");
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

        var entry = new NoiseLogEntry
        {
            RecordedAtSydney = new DateTimeOffset(localDateTime, offset),
            NoiseSources = Input.NoiseSources,
            Intensity = Input.Intensity,
            Feeling = Input.Feeling,
            Location = Input.Location,
            Note = Input.Note.Trim()
        };

        await noiseLogStore.AddEntryAsync(entry, cancellationToken);
        Saved = true;

        await PrefillAsync(cancellationToken);

        return Page();
    }

    private async Task PrefillAsync(CancellationToken cancellationToken)
    {
        var lastEntry = await noiseLogStore.GetLastEntryAsync(cancellationToken);

        if (lastEntry is null)
        {
            Input = new NoiseLogInput
            {
                RecordedAtSydneyLocal = ToLocalDateTimeValue(ToSydneyNow()),
                NoiseSources = [NoiseSources[0]],
                Intensity = Levels[1],
                Feeling = Levels[1],
                Location = Locations[0],
                Note = string.Empty
            };

            return;
        }

        Input = new NoiseLogInput
        {
            RecordedAtSydneyLocal = ToLocalDateTimeValue(lastEntry.RecordedAtSydney),
            NoiseSources = [.. lastEntry.NoiseSources],
            Intensity = lastEntry.Intensity,
            Feeling = lastEntry.Feeling,
            Location = lastEntry.Location,
            Note = lastEntry.Note
        };
    }

    private static DateTimeOffset ToSydneyNow()
    {
        var sydneyTimeZone = ResolveSydneyTimeZone();
        return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, sydneyTimeZone);
    }

    private static bool IsValidOptionSet(IEnumerable<string> selectedValues, IReadOnlyCollection<string> validValues)
    {
        return selectedValues.All(validValues.Contains);
    }

    private static string ToLocalDateTimeValue(DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
    }

    private static TimeZoneInfo ResolveSydneyTimeZone()
    {
        return TimeZoneInfo.TryFindSystemTimeZoneById("Australia/Sydney", out var sydney)
            ? sydney
            : TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
    }
}

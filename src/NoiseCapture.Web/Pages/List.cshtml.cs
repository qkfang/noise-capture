using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class ListModel(INoiseLogStore noiseLogStore) : PageModel
{
    private const int DisplayLimit = 100;

    private static readonly JsonSerializerOptions DownloadJsonOptions =
        new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public IReadOnlyList<NoiseLogEntry> Entries { get; private set; } = [];

    public int TotalCount { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var all = await noiseLogStore.GetEntriesAsync(cancellationToken);
        TotalCount = all.Count;
        Entries = all.Take(DisplayLimit).ToList();
    }

    public async Task<IActionResult> OnGetDownloadAsync(CancellationToken cancellationToken)
    {
        var all = await noiseLogStore.GetEntriesAsync(cancellationToken);
        var json = JsonSerializer.Serialize(all, DownloadJsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", "noise-log.json");
    }

    public async Task<IActionResult> OnPostDeleteAsync(string recordedAt, CancellationToken cancellationToken)
    {
        if (DateTimeOffset.TryParse(recordedAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
        {
            var removed = await noiseLogStore.DeleteEntryAsync(parsed, cancellationToken);
            TempData["StatusMessage"] = removed ? "Entry deleted." : "Entry not found.";
        }
        else
        {
            TempData["StatusMessage"] = "Invalid entry identifier.";
        }

        return RedirectToPage();
    }
}

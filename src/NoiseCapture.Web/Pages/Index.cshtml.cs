using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class IndexModel(INoiseLogStore noiseLogStore) : PageModel
{
    public IReadOnlyList<NoiseLogEntry> Entries { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Entries = await noiseLogStore.GetEntriesAsync(cancellationToken, take: 20);
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

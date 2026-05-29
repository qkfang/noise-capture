using Microsoft.AspNetCore.Mvc.RazorPages;
using NoiseCapture.Web.Models;
using NoiseCapture.Web.Services;

namespace NoiseCapture.Web.Pages;

public sealed class ListModel(INoiseLogStore noiseLogStore) : PageModel
{
    public IReadOnlyList<NoiseLogEntry> Entries { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Entries = await noiseLogStore.GetEntriesAsync(cancellationToken);
    }
}

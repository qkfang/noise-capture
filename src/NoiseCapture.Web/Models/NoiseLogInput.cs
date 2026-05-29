using System.ComponentModel.DataAnnotations;

namespace NoiseCapture.Web.Models;

public sealed class NoiseLogInput
{
    [Display(Name = "Date and time (Sydney)")]
    [Required]
    public string RecordedAtSydneyLocal { get; set; } = string.Empty;

    [Display(Name = "Noise source")]
    [MinLength(1, ErrorMessage = "Select at least one noise source.")]
    public List<string> NoiseSources { get; set; } = [];

    [Required]
    public string Intensity { get; set; } = string.Empty;

    [Required]
    public string Feeling { get; set; } = string.Empty;

    [Display(Name = "Location")]
    [MinLength(1, ErrorMessage = "Select at least one location.")]
    public List<string> Locations { get; set; } = [];

    [StringLength(2000)]
    public string Note { get; set; } = string.Empty;
}

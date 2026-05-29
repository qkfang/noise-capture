using System.ComponentModel.DataAnnotations;

namespace NoiseCapture.Web.Models;

public sealed class NoiseLogInput
{
    [Display(Name = "Date and time (Sydney)")]
    [Required]
    public string RecordedDateTimeLocal { get; set; } = string.Empty;

    [Display(Name = "Noise source")]
    [MinLength(1, ErrorMessage = "Select at least one noise source.")]
    public List<string> NoiseSources { get; set; } = [];

    [Required]
    public string Intensity { get; set; } = string.Empty;

    [Required]
    public string Loudness { get; set; } = string.Empty;

    [Required]
    public string Tone { get; set; } = string.Empty;

    [Display(Name = "Location")]
    [MinLength(1, ErrorMessage = "Select at least one location.")]
    public List<string> Locations { get; set; } = [];

    [Display(Name = "How it impacts you")]
    [StringLength(2000)]
    public string? Note { get; set; }

    [Display(Name = "Weather")]
    [StringLength(200)]
    public string? Weather { get; set; }

    [Display(Name = "Continued from last entry")]
    public bool ContinuedFromLast { get; set; }
}

namespace NoiseCapture.Web.Options;

public sealed class LocalDataOptions
{
    public const string SectionName = "LocalData";

    public string FolderPath { get; set; } = "Data";
}

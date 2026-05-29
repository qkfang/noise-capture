namespace NoiseCapture.Web;

public static class SydneyTime
{
    public static readonly TimeZoneInfo TimeZone =
        TimeZoneInfo.TryFindSystemTimeZoneById("Australia/Sydney", out var sydney)
            ? sydney
            : TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

    public static DateTimeOffset Convert(DateTimeOffset value) =>
        TimeZoneInfo.ConvertTime(value, TimeZone);
}

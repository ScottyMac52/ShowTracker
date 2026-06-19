namespace ShowTracker.Providers.Trakt;

public sealed class TraktOptions
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;  // Not needed yet for search

    public string UserAgent { get; set; } = "ShowTracker/0.1";

    public Uri BaseAddress { get; set; } = new("https://api.trakt.tv/");

    public int UpcomingReleaseDays { get; set; } = 30;
}
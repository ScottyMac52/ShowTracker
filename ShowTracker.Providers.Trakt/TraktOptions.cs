namespace ShowTracker.Providers.Trakt;

public sealed class TraktOptions
{
    public string ClientId { get; set; } = "bf928c789cc6ccb47e80bbbf8f3575fd9e86a9ba946e13b34ca1a2c187c91e32";

    public string ClientSecret { get; set; } = "4fb30d61ce1e67c6c02a98a5a2b1f0b7f272e672b4a3973fbe93e79f2225433a";

    public string UserAgent { get; set; } = "ShowTracker/0.1";

    public Uri BaseAddress { get; set; } =
        new("https://api.trakt.tv/");
}
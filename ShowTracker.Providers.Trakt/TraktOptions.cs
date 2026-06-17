namespace ShowTracker.Providers.Trakt;

/// <summary>
/// Stores options for the Trakt provider.
/// </summary>
public sealed class TraktOptions
{
    /// <summary>
    /// Base address for the Trakt API. Defaults to "https://api.trakt.tv/".
    /// </summary>
    public Uri BaseAddress { get; init; } = new("https://api.trakt.tv/");

    /// <summary>
    /// ClientId for authenticating with the Trakt API. Must be set to a valid client ID for the provider to work.  
    /// </summary>
    public string ClientId { get; init; } = "";
}
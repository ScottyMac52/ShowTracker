namespace ShowTracker.Domain.Models;

/// <summary>
/// Defines a record type representing the next episode of a TV show that the user should watch, based on their watch progress. This record includes the provider ID for the episode, the title of the show, the season and episode numbers, and an optional episode title. This information can be used to display the next episode to watch in the user interface or to provide recommendations for what to watch next.
/// </summary>
/// <param name="ProviderId">Provider to use</param>
/// <param name="ShowTitle">Title of the show</param>
/// <param name="SeasonNumber">Season number</param>
/// <param name="EpisodeNumber">Episode number</param>
/// <param name="EpisodeTitle">Title of the Episode</param>
public sealed record NextEpisodeToWatch(
    string ProviderId,
    string ShowTitle,
    int SeasonNumber,
    int EpisodeNumber,
    string? EpisodeTitle);

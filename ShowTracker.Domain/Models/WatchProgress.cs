namespace ShowTracker.Domain.Models;

/// <summary>
/// Record type representing the watch progress of a TV show, including the provider ID, show title, last watched season and episode information, and next season and episode information. This record can be used to represent the watch progress of a TV show from various title tracking providers, allowing for a consistent representation of watch progress across different implementations of the ITitleTrackingProvider interface. The last watched season and episode information includes the season number, episode number, and optional episode title, while the next season and episode information includes the season number, episode number, and optional episode title for the next episode to watch. This record is specifically designed for TV shows, as movies typically do not have multiple seasons or episodes to track separately.
/// </summary>
/// <param name="ProviderId">Provider</param>
/// <param name="ShowTitle">Show title</param>
/// <param name="LastWatchedSeason">Season last watched</param>
/// <param name="LastWatchedEpisode">Last watched episode</param>
/// <param name="LastWatchedEpisodeTitle">Title of last episode watched</param>
/// <param name="NextSeason">Next season</param>
/// <param name="NextEpisode">Next episode</param>
/// <param name="NextEpisodeTitle">Next epsiode title</param>
public sealed record WatchProgress(
    string ProviderId,
    string ShowTitle,
    int? LastWatchedSeason,
    int? LastWatchedEpisode,
    string? LastWatchedEpisodeTitle,
    int? NextSeason,
    int? NextEpisode,
    string? NextEpisodeTitle);

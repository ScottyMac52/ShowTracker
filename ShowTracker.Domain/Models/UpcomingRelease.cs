namespace ShowTracker.Domain.Models;

/// <summary>
/// Record type representing an upcoming release of a show or movie, including the provider ID, title, type (show or movie), release date, and optional season and episode information for TV shows. This record can be used to represent upcoming releases from various title tracking providers, allowing for a consistent representation of upcoming releases across different implementations of the ITitleTrackingProvider interface. The release date is represented as a DateOnly value to indicate the date of the release without including time information, which is appropriate for tracking upcoming releases that may not have a specific time associated with them.
/// </summary>
/// <param name="ProviderId">Provider</param>
/// <param name="Title">Title of the show or movie</param>
/// <param name="Type">Is it a show or a movie</param>
/// <param name="ReleaseDate">Release date</param>
/// <param name="SeasonNumber">Season (for shows)</param>
/// <param name="EpisodeNumber">Episode (for shows)</param>
/// <param name="EpisodeTitle">Title of the episode</param>
/// <param name="Platform">Platform</param>
public sealed record UpcomingRelease(
    string ProviderId,
    string Title,
    TrackedTitleType Type,
    DateOnly ReleaseDate,
    int? SeasonNumber = null,
    int? EpisodeNumber = null,
    string? EpisodeTitle = null,
    string? Platform = null);

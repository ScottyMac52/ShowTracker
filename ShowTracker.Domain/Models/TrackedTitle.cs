namespace ShowTracker.Domain.Models;

/// <summary>
/// Record type representing a title that is being tracked by the user, including the provider ID, title, type (show or movie), and optional platform information. This record can be used to represent titles that the user has chosen to track, allowing for a consistent representation of tracked titles across different implementations of the ITitleTrackingProvider interface.
/// </summary>
/// <param name="ProviderId">Provider</param>
/// <param name="Title">Title of the show or movie</param>
/// <param name="Type">Is it a show or a movie?</param>
/// <param name="Platform">Platform</param>
public sealed record TrackedTitle(
    string ProviderId,
    string Title,
    TrackedTitleType Type,
    string? Platform = null);

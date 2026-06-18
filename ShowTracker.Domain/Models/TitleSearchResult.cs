namespace ShowTracker.Domain.Models;

/// <summary>
/// Defines a record type representing the result of a title search, including the provider ID, title, type (show or movie), and optional year and platform information. This record can be used to represent search results from various title tracking providers, allowing for a consistent representation of search results across different implementations of the ITitleTrackingProvider interface.
/// </summary>
/// <param name="ProviderId">Provider used</param>
/// <param name="Title">Title of the show or movie</param>
/// <param name="Type">Is it a Show or Movie?</param>
/// <param name="Year">Year</param>
/// <param name="Platform">Platform</param>
/// <param name="Score">How relevant is the result row to what was searched for?</param>
public sealed record TitleSearchResult(
    string ProviderId,
    string Title,
    TrackedTitleType Type,
    int? Year = null,
    string? Platform = null,
    double? Score = null);
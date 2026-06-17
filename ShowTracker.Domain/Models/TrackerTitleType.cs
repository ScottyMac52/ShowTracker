namespace ShowTracker.Domain.Models;

/// <summary>
/// Title type for tracking purposes, indicating whether the title is a movie or a TV show. This enumeration is used in various parts of the domain model to differentiate between movies and TV shows when tracking watch progress, searching for titles, and managing tracked titles. The values are assigned explicitly to ensure consistent representation across different implementations of the ITitleTrackingProvider interface.
/// </summary>
public enum TrackedTitleType
{
    /// <summary>
    /// Movie type, representing a film or cinematic work that can be tracked for watch progress. This value is used to indicate that the title being tracked is a movie, as opposed to a TV show, which may have multiple episodes and seasons to track separately.
    /// </summary>
    Movie = 0,

    /// <summary>
    /// Show type, representing a TV show or series that can be tracked for watch progress. This value is used to indicate that the title being tracked is a TV show, which may have multiple episodes and seasons to track separately, as opposed to a movie, which is typically a single work with a defined runtime.
    /// </summary>
    Show = 1
}

using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for untracking a title. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and remove a title from the list of tracked titles for the user. The service validates the input title and checks if the title is being tracked, and if so, it removes the title from the tracking list.
/// </summary>
/// <remarks>
/// Constructor for the UntrackTitleService class, which initializes the service with a specified title tracking provider. The provider is used to access the title tracking data and remove a title from the list of tracked titles for the user.
/// </remarks>
/// <param name="titleTrackingProvider"></param>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> is required</exception>
public sealed class UntrackTitleService(ITitleTrackingProvider titleTrackingProvider)
{
    /// <summary>
    /// Title Tracking Provider used to access the title tracking data and remove a title from the list of tracked titles for the user. This provider is responsible for accessing the underlying data source and providing the necessary functionality to untrack a title.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

    /// <summary>
    /// Untracks a title for the user. This method validates the input title and checks if the title is being tracked, and if so, it removes the title from the tracking list. If the title is null or whitespace, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Title is required</exception>
    public Task UntrackAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        return _titleTrackingProvider.UntrackAsync(
            title.Trim(),
            cancellationToken);
    }
}
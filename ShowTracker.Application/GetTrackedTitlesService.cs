using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for retrieving the list of tracked titles for the user. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and retrieve the list of titles that the user is currently tracking.
/// </summary>
/// <remarks>
/// Constructor for the GetTrackedTitlesService class, which initializes the service with a specified title tracking provider. The provider is used to retrieve title tracking data for shows and retrieve the list of titles that the user is currently tracking.
/// </remarks>
/// <param name="titleTrackingProvider"></param>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> is required</exception>
public sealed class GetTrackedTitlesService(ITitleTrackingProvider titleTrackingProvider)
{
    /// <summary>
    /// Tracking provider used to retrieve title tracking data for shows. This provider is responsible for accessing the underlying data source and providing the necessary information to retrieve the list of tracked titles for the user.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

    public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
        CancellationToken cancellationToken = default)
    {
        return _titleTrackingProvider.GetTrackedTitlesAsync(cancellationToken);
    }
}
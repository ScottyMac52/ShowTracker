using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for retrieving the next upcoming release for a given show title. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and determine the next release date for the specified show.
/// </summary>
/// <remarks>
/// Constructor for the GetNextReleaseService class, which initializes the service with a specified title tracking provider. The provider is used to retrieve title tracking data for shows and determine the next upcoming release.
/// </remarks>
/// <param name="titleTrackingProvider">Tracking provider</param>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> is required</exception>
public sealed class GetNextReleaseService(ITitleTrackingProvider titleTrackingProvider)
{
    /// <summary>
    /// Tracking provider used to retrieve title tracking data for shows. This provider is responsible for accessing the underlying data source and providing the necessary information to determine the next upcoming release.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

    /// <summary>
    /// Gets the next upcoming release for a given show title. The method takes a show title as input and returns an <see cref="UpcomingRelease"/> object containing information about the next release, or null if no upcoming release is found. The method uses the provided <see cref="ITitleTrackingProvider"/> to access the title tracking data and determine the next release date for the specified show.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Show title is required</exception>
    public Task<UpcomingRelease?> GetNextReleaseAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        return _titleTrackingProvider.GetNextReleaseAsync(
            showTitle.Trim(),
            cancellationToken);
    }
}
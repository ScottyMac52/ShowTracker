using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for retrieving the list of upcoming releases for the user. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and retrieve the list of upcoming releases that the user is currently tracking.
/// </summary>
/// <remarks>
/// Constructor for the GetUpcomingReleasesService class, which initializes the service with a specified title tracking provider. The provider is used to retrieve title tracking data for shows and retrieve the list of upcoming releases that the user is currently tracking.
/// </remarks>
/// <param name="titleTrackingProvider">Title tracking provider</param>
/// <exception cref="ArgumentNullException"></exception>
public sealed class GetUpcomingReleasesService(ITitleTrackingProvider titleTrackingProvider)
{
    /// <summary>
    /// Title Tracking Provider used to access the title tracking data and retrieve the list of upcoming releases that the user is currently tracking.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));

    /// <summary>
    /// Gets the list of upcoming releases for the user. It uses the <see cref="ITitleTrackingProvider"/> to access the title tracking data and retrieve the list of upcoming releases that the user is currently tracking.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
        CancellationToken cancellationToken = default)
    {
        return _titleTrackingProvider.GetUpcomingReleasesAsync(cancellationToken);
    }
}
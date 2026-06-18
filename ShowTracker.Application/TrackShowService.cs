using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for tracking a show title. It uses an <see cref="ITitleTrackingProvider"/> to access the title tracking data and add a new show title to the list of tracked titles for the user. The service validates the input show title and platform, checks if the show is already being tracked, and if not, it adds the show to the tracking list and returns the tracked title information.
/// </summary>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> is required</exception>
/// <remarks>
/// Constructor for the TrackShowService class, which initializes the service with a specified title tracking provider. The provider is used to access the title tracking data and add a new show title to the list of tracked titles for the user.
/// </remarks>
/// <param name="titleTrackingProvider"><see cref="ITitleTrackingProvider"/></param>
/// <param name="trackedTitleRepository"><see cref="ITrackedTitleRepository"/></param>
/// <exception cref="ArgumentNullException"><see cref="ITitleTrackingProvider"/> and <see cref="ITrackedTitleRepository"/> are required</exception>
public sealed class TrackShowService(
    ITitleTrackingProvider titleTrackingProvider,
    ITrackedTitleRepository trackedTitleRepository)
{
    /// <summary>
    /// Title Tracking Provider used to access the title tracking data and add a new show title to the list of tracked titles for the user. This provider is responsible for accessing the underlying data source and providing the necessary functionality to track a show title.
    /// </summary>
    private readonly ITitleTrackingProvider _titleTrackingProvider = titleTrackingProvider
            ?? throw new ArgumentNullException(nameof(titleTrackingProvider));
    private readonly ITrackedTitleRepository _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));

    /// <summary>
    /// Tracks a show title for the user. This method validates the input show title and platform, checks if the show is already being tracked, and if not, it adds the show to the tracking list and returns the tracked title information. If the show title is invalid or already being tracked, it throws an appropriate exception.
    /// </summary>
    /// <param name="showTitle"></param>
    /// <param name="platform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"Show title is required</exception>
    /// <exception cref="InvalidOperationException">Show is already being tracked!</exception>
    public async Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));


        var normalizedTitle = showTitle.Trim();

        var existing = await _titleTrackingProvider.FindTrackedTitleAsync(normalizedTitle, cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"'{normalizedTitle}' is already being tracked.");
        }

        var normalizedPlatform = string.IsNullOrWhiteSpace(platform)
            ? null
            : platform.Trim();

        var trackedTitle = await _titleTrackingProvider.TrackShowAsync(
            normalizedTitle,
            normalizedPlatform,
            cancellationToken);

        await _trackedTitleRepository.AddAsync(
            trackedTitle,
            cancellationToken);

        return trackedTitle;
    }
}

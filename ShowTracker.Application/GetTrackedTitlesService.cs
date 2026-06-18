using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for retrieving the list of tracked titles for the user. It uses an <see cref="ITrackedTitleRepository"/> to access the stored tracked titles and retrieve the list of titles that the user is currently tracking. The service provides a method to get all tracked titles, which returns a read-only list of <see cref="TrackedTitle"/> objects representing the shows and movies that the user is tracking.
/// </summary>
/// <remarks>
/// Constructor for the GetTrackedTitlesService class, which initializes the service with a specified title tracking provider. The provider is used to retrieve title tracking data for shows and retrieve the list of titles that the user is currently tracking.
/// </remarks>
/// <param name="trackedTitleRepository"></param>
/// <exception cref="ArgumentNullException"><see cref="ITrackedTitleRepository"/> is required</exception>
public sealed class GetTrackedTitlesService(ITrackedTitleRepository trackedTitleRepository)
{

    private readonly ITrackedTitleRepository _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));

    public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
            CancellationToken cancellationToken = default)
    {
        return _trackedTitleRepository.GetAllAsync(cancellationToken);
    }
}
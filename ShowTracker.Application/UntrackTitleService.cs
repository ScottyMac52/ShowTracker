using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service for untracking a title. It uses an <see cref="ITrackedTitleRepository"/> to access the tracked titles data and remove a title from the list of tracked titles for the user. The service validates the input provider ID and checks if the title is being tracked, and if so, it removes the title from the tracking list. If the provider ID is invalid or if the title is not being tracked, it throws an appropriate exception.
/// </summary>
public sealed class UntrackTitleService
{
    private readonly ITrackedTitleRepository _trackedTitleRepository;

    /// <summary>
    /// Constructor for the UntrackTitleService class, which initializes the service with a specified tracked title repository. The repository is used to access the tracked titles data and remove a title from the list of tracked titles for the user.
    /// </summary>
    /// <param name="trackedTitleRepository"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UntrackTitleService(ITrackedTitleRepository trackedTitleRepository)
    {
        _trackedTitleRepository = trackedTitleRepository
            ?? throw new ArgumentNullException(nameof(trackedTitleRepository));
    }

    /// <summary>
    /// Untracks a title for the user. This method validates the input provider ID and checks if the title is being tracked, and if so, it removes the title from the tracking list. The method throws an exception if the input provider ID is invalid or if the title is not being tracked.
    /// </summary>
    /// <param name="providerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Task UntrackAsync(
        string providerId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider id is required.", nameof(providerId));

        return _trackedTitleRepository.RemoveAsync(
            providerId.Trim(),
            cancellationToken);
    }
}
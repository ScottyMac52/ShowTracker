using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

/// <summary>
/// Service for untracking a title. It uses an <see cref="ITrackedTitleRepository"/> to access the tracked titles data and remove a title from the list of tracked titles for the user. The service validates the input provider ID or title and removes the matching tracked title. If multiple tracked titles have the same title, the caller must use the provider ID.
/// </summary>
public sealed class UntrackTitleService : IUntrackTitleService
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
    /// Untracks a title for the user. The input may be either a provider ID or an exact tracked title name.
    /// </summary>
    /// <param name="titleOrProviderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task UntrackAsync(
        string titleOrProviderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(titleOrProviderId))
            throw new ArgumentException("Title or provider id is required.", nameof(titleOrProviderId));

        var normalizedTarget = titleOrProviderId.Trim();

        var trackedTitles = await _trackedTitleRepository.GetAllAsync(
            cancellationToken);

        var providerIdMatch = trackedTitles.FirstOrDefault(title =>
            string.Equals(
                title.ProviderId,
                normalizedTarget,
                StringComparison.OrdinalIgnoreCase));

        if (providerIdMatch is not null)
        {
            await _trackedTitleRepository.RemoveAsync(
                providerIdMatch.ProviderId,
                cancellationToken);

            return;
        }

        var titleMatches = trackedTitles
            .Where(title =>
                string.Equals(
                    title.Title,
                    normalizedTarget,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (titleMatches.Count == 1)
        {
            await _trackedTitleRepository.RemoveAsync(
                titleMatches[0].ProviderId,
                cancellationToken);

            return;
        }

        if (titleMatches.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple tracked titles match '{normalizedTarget}'. Use the provider id shown by the tracked command.");
        }

        await _trackedTitleRepository.RemoveAsync(
            normalizedTarget,
            cancellationToken);
    }
}
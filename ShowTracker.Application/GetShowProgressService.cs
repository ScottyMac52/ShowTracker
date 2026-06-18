using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

public sealed class GetShowProgressService
{
    private readonly IWatchProgressRepository _watchProgressRepository;

    public GetShowProgressService(IWatchProgressRepository watchProgressRepository)
    {
        _watchProgressRepository = watchProgressRepository
            ?? throw new ArgumentNullException(nameof(watchProgressRepository));
    }

    public Task<WatchProgress?> GetShowProgressAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        return _watchProgressRepository.GetAsync(
            showTitle.Trim(),
            cancellationToken);
    }
}
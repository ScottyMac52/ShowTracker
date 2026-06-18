using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application;

public sealed class GetNextEpisodeToWatchService
{
    private readonly IWatchProgressRepository _watchProgressRepository;

    public GetNextEpisodeToWatchService(IWatchProgressRepository watchProgressRepository)
    {
        _watchProgressRepository = watchProgressRepository
            ?? throw new ArgumentNullException(nameof(watchProgressRepository));
    }

    public async Task<NextEpisodeToWatch?> GetNextEpisodeToWatchAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(showTitle))
            throw new ArgumentException("Show title is required.", nameof(showTitle));

        var progress = await _watchProgressRepository.GetAsync(
            showTitle.Trim(),
            cancellationToken);

        if (progress?.NextSeason is null || progress.NextEpisode is null)
            return null;

        return new NextEpisodeToWatch(
            ProviderId: progress.ProviderId,
            ShowTitle: progress.ShowTitle,
            SeasonNumber: progress.NextSeason.Value,
            EpisodeNumber: progress.NextEpisode.Value,
            EpisodeTitle: progress.NextEpisodeTitle);
    }
}
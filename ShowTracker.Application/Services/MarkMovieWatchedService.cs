using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Application.Services;

public sealed class MarkMovieWatchedService : IMarkMovieWatchedService
{
    private readonly IWatchProgressRepository _watchProgressRepository;

    public MarkMovieWatchedService(IWatchProgressRepository watchProgressRepository)
    {
        _watchProgressRepository = watchProgressRepository
            ?? throw new ArgumentNullException(nameof(watchProgressRepository));
    }

    public Task MarkMovieWatchedAsync(
        string movieTitle,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title is required.", nameof(movieTitle));

        var progress = new WatchProgress(
            ProviderId: string.Empty,
            ShowTitle: movieTitle.Trim(),
            LastWatchedSeason: null,
            LastWatchedEpisode: null,
            LastWatchedEpisodeTitle: null,
            NextSeason: null,
            NextEpisode: null,
            NextEpisodeTitle: null);

        return _watchProgressRepository.SaveAsync(
            progress,
            cancellationToken);
    }
}
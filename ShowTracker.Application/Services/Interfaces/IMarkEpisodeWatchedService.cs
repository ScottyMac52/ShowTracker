namespace ShowTracker.Application.Services.Interfaces
{
    public interface IMarkEpisodeWatchedService
    {
        Task MarkEpisodeWatchedAsync(string showTitle, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default);
    }
}
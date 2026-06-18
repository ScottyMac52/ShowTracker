namespace ShowTracker.Application.Services.Interfaces
{
    public interface IMarkMovieWatchedService
    {
        Task MarkMovieWatchedAsync(string movieTitle, CancellationToken cancellationToken = default);
    }
}
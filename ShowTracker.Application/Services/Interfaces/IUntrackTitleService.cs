namespace ShowTracker.Application.Services.Interfaces
{
    public interface IUntrackTitleService
    {
        Task UntrackAsync(string providerId, CancellationToken cancellationToken = default);
    }
}
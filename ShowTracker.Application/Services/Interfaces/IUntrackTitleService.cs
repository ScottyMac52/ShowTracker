namespace ShowTracker.Application.Services.Interfaces
{
    /// <summary>
    /// Interface for the UntrackTitleService, which defines a method for untracking a title for the user. The input may be either a provider ID or an exact tracked title name. The service validates the input and removes the matching tracked title from the list of tracked titles for the user. If multiple tracked titles have the same title, the caller must use the provider ID to untrack the correct title.
    /// </summary>
    public interface IUntrackTitleService
    {
        /// <summary>
        /// Untracks a title for the user. The input may be either a provider ID or an exact tracked title name. The service validates the input and removes the matching tracked title from the list of tracked titles for the user. If multiple tracked titles have the same title, the caller must use the provider ID to untrack the correct title.
        /// </summary>
        /// <param name="titleOrProviderId">The id of the title</param>
        /// <param name="cancellationToken">Token</param>
        /// <returns></returns>
        Task UntrackAsync(string titleOrProviderId, CancellationToken cancellationToken = default);
    }
}
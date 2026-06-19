using ShowTracker.Domain.Models;

namespace ShowTracker.Providers.Trakt
{
    /// <summary>
    /// Interface for a client that retrieves release information from the Trakt API. It provides methods to get a list of upcoming releases and to get the next release for a specific title. The client abstracts the details of making API calls to Trakt and processing the responses, allowing other parts of the application to easily access release information without needing to know about the underlying API interactions.
    /// </summary>
    public interface ITraktReleaseClient
    {
        /// <summary>
        /// Gets a list of upcoming releases from the Trakt API. This method retrieves information about upcoming movies and TV shows, including their release dates, titles, and other relevant details. The results are returned as a read-only list of UpcomingRelease objects, which can be used by other parts of the application to display or process the release information as needed.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the next release for a specific title from the Trakt API. This method takes a title as input and retrieves information about the next release date for that title, if available. The result is returned as an UpcomingRelease object, which contains details about the release such as the release date, title, type (movie or TV show), and other relevant information. If no upcoming release is found for the given title, the method returns null.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UpcomingRelease?> GetNextReleaseAsync(
            string title,
            CancellationToken cancellationToken = default);
    }
}

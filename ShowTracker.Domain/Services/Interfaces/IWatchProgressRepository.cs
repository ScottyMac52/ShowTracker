using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Services.Interfaces;

/// <summary>
/// Interface for a repository that manages watch progress data. This repository provides methods for saving and retrieving watch progress information for a specific show title. The implementation of this interface is responsible for handling the underlying data storage mechanism, which could be an in-memory collection, a database, or any other form of persistent storage. The repository abstracts the details of how the watch progress data is stored and retrieved, allowing the rest of the application to interact with it without needing to know about the specific storage implementation.
/// </summary>
public interface IWatchProgressRepository
{
    /// <summary>
    /// Saves the watch progress for a specific show title. This method takes a <see cref="WatchProgress"/> object as input and saves it to the underlying data storage. The method may perform any necessary validations or transformations on the input data before saving it to the repository. If the save operation is successful, the method completes without returning a value; otherwise, it may throw an exception if there are issues with the input data or if there is a problem with the underlying data storage.
    /// </summary>
    /// <param name="progress"><see cref="WatchProgress"/></param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task SaveAsync(
        WatchProgress progress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the watch progress for a specific show title. This method takes a show title as input and retrieves the corresponding <see cref="WatchProgress"/> from the underlying data storage. If a matching watch progress is found, it is returned; otherwise, the method returns null. The method may also handle any necessary data transformations or validations related to the show title before performing the retrieval.
    /// </summary>
    /// <param name="showTitle">Show title</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns><see cref="WatchProgress"/></returns>
    Task<WatchProgress?> GetAsync(
        string showTitle,
        CancellationToken cancellationToken = default);
}
using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Services.Interfaces;

/// <summary>
/// Interface for a repository that manages tracked titles. This repository provides methods for adding, finding, retrieving, and removing tracked titles. It abstracts the underlying data storage mechanism and allows for different implementations (e.g., in-memory, database) to be used without affecting the rest of the application. The repository is responsible for ensuring that the tracked titles are stored and retrieved correctly, and it may also handle any necessary data transformations or validations related to the tracked titles.
/// </summary>
public interface ITrackedTitleRepository
{
    /// <summary>
    /// Adds a new tracked title to the repository. This method takes a <see cref="TrackedTitle"/> object as input and adds it to the underlying data storage. The method may perform any necessary validations or transformations on the input data before adding it to the repository. If the addition is successful, the method completes without returning a value; otherwise, it may throw an exception if there are issues with the input data or if there is a problem with the underlying data storage.
    /// </summary>
    /// <param name="trackedTitle">Show or movie metadata</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task AddAsync(
        TrackedTitle trackedTitle,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a tracked title in the repository by its provider ID. This method takes a provider ID as input and searches the underlying data storage for a <see cref="TrackedTitle"/> that matches the given provider ID. If a matching tracked title is found, it is returned; otherwise, the method returns null. The method may also handle any necessary data transformations or validations related to the provider ID before performing the search.
    /// </summary>
    /// <param name="providerId">Provider Id</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task<TrackedTitle?> FindByProviderIdAsync(
        string providerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tracked titles from the repository. This method retrieves all <see cref="TrackedTitle"/> objects from the underlying data storage and returns them as a read-only list. The method may perform any necessary data transformations or validations before returning the list of tracked titles. If there are no tracked titles in the repository, it may return an empty list.
    /// </summary>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task<IReadOnlyList<TrackedTitle>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a tracked title from the repository by its provider ID. This method takes a provider ID as input and searches the underlying data storage for a <see cref="TrackedTitle"/> that matches the given provider ID. If a matching tracked title is found, it is removed from the repository; otherwise, the method may complete without making any changes. The method may also handle any necessary data transformations or validations related to the provider ID before performing the removal.
    /// </summary>
    /// <param name="providerId">Provider Id</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    Task RemoveAsync(
        string providerId,
        CancellationToken cancellationToken = default);
}
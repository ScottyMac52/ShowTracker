using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Testing;

/// <summary>
/// Implements <see cref="ITrackedTitleRepository"/> for testing purposes. This class allows you to set custom handlers for each method, enabling you to simulate different scenarios and behaviors when testing components that depend on the <see cref="ITrackedTitleRepository"/>. Each method checks if the corresponding handler is set and invokes it; if a handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario. This approach provides flexibility in testing various conditions without needing to create multiple concrete implementations of the repository.
/// </summary>
public sealed class TestTrackedTitleRepository : ITrackedTitleRepository
{
    /// <summary>
    /// Handler for the AddAsync method. This allows you to define custom behavior for adding a tracked title during testing. You can set this property to a function that takes a <see cref="TrackedTitle"/> and a <see cref="CancellationToken"/> and returns a <see cref="Task"/>. If this handler is not set when the AddAsync method is called, it will throw a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario.
    /// </summary>
    public Func<TrackedTitle, CancellationToken, Task>? AddAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the FindByProviderIdAsync method. This allows you to define custom behavior for finding a tracked title by its provider ID during testing. You can set this property to a function that takes a provider ID as a string and a <see cref="CancellationToken"/> and returns a <see cref="Task{TrackedTitle?}"/>. If this handler is not set when the FindByProviderIdAsync method is called, it will throw a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario.
    /// </summary>
    public Func<string, CancellationToken, Task<TrackedTitle?>>? FindByProviderIdAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the GetAllAsync method. This allows you to define custom behavior for retrieving all tracked titles during testing. You can set this property to a function that takes a <see cref="CancellationToken"/> and returns a <see cref="Task{IReadOnlyList{TrackedTitle}}"/>. If this handler is not set when the GetAllAsync method is called, it will throw a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario.
    /// </summary>
    public Func<CancellationToken, Task<IReadOnlyList<TrackedTitle>>>? GetAllAsyncHandler { get; set; }

    /// <summary>
    /// Handler for the RemoveAsync method. This allows you to define custom behavior for removing a tracked title by its provider ID during testing. You can set this property to a function that takes a provider ID as a string and a <see cref="CancellationToken"/> and returns a <see cref="Task"/>. If this handler is not set when the RemoveAsync method is called, it will throw a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario.
    /// </summary>
    public Func<string, CancellationToken, Task>? RemoveAsyncHandler { get; set; }

    /// <summary>
    /// Adds a new tracked title to the repository. This method checks if the AddAsyncHandler is set and invokes it with the provided <see cref="TrackedTitle"/> and <see cref="CancellationToken"/>. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario. This allows you to define custom behavior for adding a tracked title during testing, simulating different scenarios and outcomes as needed.
    /// </summary>
    /// <param name="trackedTitle"><see cref="TrackedTitle"/> metadata</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task AddAsync(TrackedTitle trackedTitle, CancellationToken cancellationToken = default)
    {
        if (AddAsyncHandler is null)
            throw new NotImplementedException();

        return AddAsyncHandler(trackedTitle, cancellationToken);
    }

    /// <summary>
    /// Find a tracked title in the repository by its provider ID. This method checks if the FindByProviderIdAsyncHandler is set and invokes it with the provided provider ID and <see cref="CancellationToken"/>. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario. This allows you to define custom behavior for finding a tracked title by its provider ID during testing, simulating different scenarios and outcomes as needed.
    /// </summary>
    /// <param name="providerId">Provider Id</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<TrackedTitle?> FindByProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (FindByProviderIdAsyncHandler is null)
            throw new NotImplementedException();

        return FindByProviderIdAsyncHandler(providerId, cancellationToken);
    }

    /// <summary>
    /// Gets all tracked titles from the repository. This method checks if the GetAllAsyncHandler is set and invokes it with the provided <see cref="CancellationToken"/>. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario. This allows you to define custom behavior for retrieving all tracked titles during testing, simulating different scenarios and outcomes as needed.
    /// </summary>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<IReadOnlyList<TrackedTitle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (GetAllAsyncHandler is null)
            throw new NotImplementedException();

        return GetAllAsyncHandler(cancellationToken);
    }

    /// <summary>
    /// Removes a tracked title from the repository by its provider ID. This method checks if the RemoveAsyncHandler is set and invokes it with the provided provider ID and <see cref="CancellationToken"/>. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the method has not been implemented for the test scenario. This allows you to define custom behavior for removing a tracked title by its provider ID during testing, simulating different scenarios and outcomes as needed.
    /// </summary>
    /// <param name="providerId">Provider Id</param>
    /// <param name="cancellationToken">Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task RemoveAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (RemoveAsyncHandler is null)
            throw new NotImplementedException();

        return RemoveAsyncHandler(providerId, cancellationToken);
    }
}
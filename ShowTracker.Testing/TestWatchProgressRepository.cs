using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ShowTracker.Testing;

/// <summary>
/// Implements <see cref="IWatchProgressRepository"/> for testing purposes. This test repository allows for the simulation of saving and retrieving watch progress data without relying on an actual data storage mechanism. It provides handlers that can be set to define the behavior of the save and get operations, making it useful for unit testing scenarios where the watch progress repository is a dependency. The test repository can be used to verify that the application logic interacts correctly with the watch progress repository and handles the expected data as intended.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class TestWatchProgressRepository : IWatchProgressRepository
{
    /// <summary>
    /// Handler for simulating the save operation of watch progress data. This handler can be set to define the behavior of the save operation, allowing for testing scenarios where the application logic interacts with the watch progress repository. The handler takes a <see cref="WatchProgress"/> object and a <see cref="CancellationToken"/> as parameters and returns a <see cref="Task"/> representing the asynchronous operation of saving the watch progress data.
    /// </summary>
    public Func<WatchProgress, CancellationToken, Task>? SaveAsyncHandler { get; set; }

    /// <summary>
    /// Handler for simulating the retrieval of watch progress data. This handler can be set to define the behavior of the get operation, allowing for testing scenarios where the application logic interacts with the watch progress repository. The handler takes a show title as a string and a <see cref="CancellationToken"/> as parameters and returns a <see cref="Task{WatchProgress?}"/> representing the asynchronous operation of retrieving the watch progress data for the specified show title. The returned value can be used to verify that the application logic correctly handles the retrieved watch progress data as intended.
    /// </summary>
    public Func<string, CancellationToken, Task<WatchProgress?>>? GetAsyncHandler { get; set; }

    /// <summary>
    /// Saves the watch progress data using the defined save handler. This method simulates the saving of watch progress data by invoking the <see cref="SaveAsyncHandler"/> if it is set. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the save operation is not implemented in the test repository. The method takes a <see cref="WatchProgress"/> object and a <see cref="CancellationToken"/> as parameters and returns a <see cref="Task"/> representing the asynchronous operation of saving the watch progress data.
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SaveAsync(
        WatchProgress progress,
        CancellationToken cancellationToken = default)
    {
        if (SaveAsyncHandler is null)
            throw new NotImplementedException();

        return SaveAsyncHandler(progress, cancellationToken);
    }

    /// <summary>
    /// Gets the watch progress data for a specified show title using the defined get handler. This method simulates the retrieval of watch progress data by invoking the <see cref="GetAsyncHandler"/> if it is set. If the handler is not set, it throws a <see cref="NotImplementedException"/> to indicate that the get operation is not implemented in the test repository. The method takes a show title as a string and a <see cref="CancellationToken"/> as parameters and returns a <see cref="Task{WatchProgress?}"/> representing the asynchronous operation of retrieving the watch progress data for the specified show title. The returned value can be used to verify that the application logic correctly handles the retrieved watch progress data as intended.
    /// </summary>
    /// <param name="showTitle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<WatchProgress?> GetAsync(
        string showTitle,
        CancellationToken cancellationToken = default)
    {
        if (GetAsyncHandler is null)
            throw new NotImplementedException();

        return GetAsyncHandler(showTitle, cancellationToken);
    }
}
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

internal sealed class TestTrackShowService : ITrackShowService
{
    public Func<string, string?, CancellationToken, Task<TrackedTitle>>?
        TrackShowAsyncHandler
    { get; set; }

    public Task<TrackedTitle> TrackShowAsync(
        string showTitle,
        string? platform = null,
        CancellationToken cancellationToken = default)
    {
        if (TrackShowAsyncHandler is null)
            throw new NotImplementedException();

        return TrackShowAsyncHandler(
            showTitle,
            platform,
            cancellationToken);
    }
}
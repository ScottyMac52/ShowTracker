using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class GetTrackedTitlesCommandTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_Tracked_Titles()
    {
        var service = new TestGetTrackedTitlesService
        {
            GetTrackedTitlesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new("trakt:show:12345", "Andor", TrackedTitleType.Show, "Disney Plus"),
                    new("trakt:movie:654321", "Dune: Part Two", TrackedTitleType.Movie, "Max")
                ])
        };

        var command = new GetTrackedTitlesCommand(service);

        var output = await command.ExecuteAsync(["tracked"]);

        Assert.Contains("Andor", output);
        Assert.Contains("Show", output);
        Assert.Contains("trakt:show:12345", output);
        Assert.Contains("Dune: Part Two", output);
        Assert.Contains("Movie", output);
        Assert.Contains("trakt:movie:654321", output);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Message_When_No_Titles_Are_Tracked()
    {
        var service = new TestGetTrackedTitlesService
        {
            GetTrackedTitlesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>([])
        };

        var command = new GetTrackedTitlesCommand(service);

        var output = await command.ExecuteAsync(["tracked"]);

        Assert.Equal("No tracked titles.", output);
    }

    private sealed class TestGetTrackedTitlesService : IGetTrackedTitlesService
    {
        public Func<CancellationToken, Task<IReadOnlyList<TrackedTitle>>>? GetTrackedTitlesAsyncHandler { get; set; }

        public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
            CancellationToken cancellationToken = default)
        {
            if (GetTrackedTitlesAsyncHandler is null)
                throw new NotImplementedException();

            return GetTrackedTitlesAsyncHandler(cancellationToken);
        }
    }
}
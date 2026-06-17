namespace ShowTracker.Application.Tests;

public sealed class UntrackTitleServiceTests
{
    [Fact]
    public async Task UntrackAsync_Untracks_Title_Through_Provider()
    {
        string? untrackedTitle = null;

        var provider = new TestTitleTrackingProvider
        {
            UntrackAsyncHandler = (title, _) =>
            {
                untrackedTitle = title;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(provider);

        await service.UntrackAsync("Andor");

        Assert.Equal("Andor", untrackedTitle);
    }

    [Fact]
    public async Task UntrackAsync_Rejects_Blank_Title()
    {
        var provider = new TestTitleTrackingProvider();
        var service = new UntrackTitleService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UntrackAsync("   "));
    }
}

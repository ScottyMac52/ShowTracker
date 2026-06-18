using ShowTracker.Application.Services;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class UntrackTitleServiceTests
{
    [Fact]
    public async Task UntrackAsync_Removes_Title_From_Repository()
    {
        string? removedProviderId = null;

        var repository = new TestTrackedTitleRepository
        {
            RemoveAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(repository);

        await service.UntrackAsync("trakt:show:12345");

        Assert.Equal("trakt:show:12345", removedProviderId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UntrackAsync_Rejects_Blank_Provider_Id(string providerId)
    {
        var repository = new TestTrackedTitleRepository();
        var service = new UntrackTitleService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UntrackAsync(providerId));
    }
}
using ShowTracker.Application.Services;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class UntrackTitleServiceTests
{
    [Fact]
    public async Task UntrackAsync_Removes_Title_From_Repository_When_Input_Is_Provider_Id()
    {
        string? removedProviderId = null;

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new TrackedTitle(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show)
                ]),
            RemoveAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(repository);

        await service.UntrackAsync("139960");

        Assert.Equal("139960", removedProviderId);
    }

    [Fact]
    public async Task UntrackAsync_Removes_Title_From_Repository_When_Input_Is_Title()
    {
        string? removedProviderId = null;

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new TrackedTitle(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show)
                ]),
            RemoveAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(repository);

        await service.UntrackAsync("The Boys");

        Assert.Equal("139960", removedProviderId);
    }

    [Fact]
    public async Task UntrackAsync_Removes_Title_From_Repository_When_Input_Title_Differs_By_Case()
    {
        string? removedProviderId = null;

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new TrackedTitle(
                        ProviderId: "139960",
                        Title: "The Boys",
                        Type: TrackedTitleType.Show)
                ]),
            RemoveAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(repository);

        await service.UntrackAsync("the boys");

        Assert.Equal("139960", removedProviderId);
    }

    [Fact]
    public async Task UntrackAsync_Rejects_Ambiguous_Title_Matches()
    {
        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>(
                [
                    new TrackedTitle(
                        ProviderId: "123",
                        Title: "Cape Fear",
                        Type: TrackedTitleType.Show),
                    new TrackedTitle(
                        ProviderId: "456",
                        Title: "Cape Fear",
                        Type: TrackedTitleType.Movie)
                ])
        };

        var service = new UntrackTitleService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UntrackAsync("Cape Fear"));

        Assert.Contains("Multiple tracked titles match", exception.Message);
        Assert.Contains("provider id", exception.Message);
    }

    [Fact]
    public async Task UntrackAsync_Falls_Back_To_Removing_Input_When_No_Tracked_Title_Matches()
    {
        string? removedProviderId = null;

        var repository = new TestTrackedTitleRepository
        {
            GetAllAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<TrackedTitle>>([]),
            RemoveAsyncHandler = (providerId, _) =>
            {
                removedProviderId = providerId;
                return Task.CompletedTask;
            }
        };

        var service = new UntrackTitleService(repository);

        await service.UntrackAsync("unknown-id");

        Assert.Equal("unknown-id", removedProviderId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UntrackAsync_Rejects_Blank_Title_Or_Provider_Id(string titleOrProviderId)
    {
        var repository = new TestTrackedTitleRepository();
        var service = new UntrackTitleService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UntrackAsync(titleOrProviderId));
    }
}
using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application.Services;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Domain.Models;
using ShowTracker.Domain.Services.Interfaces;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class ApplicationServiceRegistrationTests
{
    [Fact]
    public void AddShowTrackerApplication_Registers_SearchTitlesService()
    {
        using var provider = CreateServiceProvider();

        var service = provider.GetRequiredService<ISearchTitlesService>();

        Assert.IsType<SearchTitlesService>(service);
    }

    [Fact]
    public void AddShowTrackerApplication_Registers_Title_Tracking_Services()
    {
        using var provider = CreateServiceProvider();

        Assert.IsType<TrackShowService>(provider.GetRequiredService<ITrackShowService>());
        Assert.IsType<TrackMovieService>(provider.GetRequiredService<ITrackMovieService>());
        Assert.IsType<UntrackTitleService>(provider.GetRequiredService<IUntrackTitleService>());
        Assert.IsType<GetTrackedTitlesService>(provider.GetRequiredService<IGetTrackedTitlesService>());
    }

    [Fact]
    public void AddShowTrackerApplication_Registers_Watch_Progress_Services()
    {
        using var provider = CreateServiceProvider();

        Assert.IsType<MarkEpisodeWatchedService>(provider.GetRequiredService<IMarkEpisodeWatchedService>());
        Assert.IsType<MarkMovieWatchedService>(provider.GetRequiredService<IMarkMovieWatchedService>());
        Assert.IsType<GetShowProgressService>(provider.GetRequiredService<IGetShowProgressService>());
        Assert.IsType<GetNextEpisodeToWatchService>(provider.GetRequiredService<IGetNextEpisodeToWatchService>());
    }

    [Fact]
    public void AddShowTrackerApplication_Registers_Release_Services()
    {
        using var provider = CreateServiceProvider();

        Assert.IsType<GetUpcomingReleasesService>(provider.GetRequiredService<IGetUpcomingReleasesService>());
        Assert.IsType<GetNextReleaseService>(provider.GetRequiredService<IGetNextReleaseService>());
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ITitleTrackingProvider>(new TestTitleTrackingProvider
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult<IReadOnlyList<TitleSearchResult>>([]),

            TrackShowAsyncHandler = (_, _, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "test-show",
                    Title: "Test Show",
                    Type: TrackedTitleType.Show)),

            TrackMovieAsyncHandler = (_, _, _) =>
                Task.FromResult(new TrackedTitle(
                    ProviderId: "test-movie",
                    Title: "Test Movie",
                    Type: TrackedTitleType.Movie)),

            GetUpcomingReleasesAsyncHandler = _ =>
                Task.FromResult<IReadOnlyList<UpcomingRelease>>([]),

            GetNextReleaseAsyncHandler = (_, _) =>
                Task.FromResult<UpcomingRelease?>(null)
        });

        services.AddSingleton<ITrackedTitleRepository>(new TestTrackedTitleRepository
        {
            AddAsyncHandler = (_, _) => Task.CompletedTask,
            RemoveAsyncHandler = (_, _) => Task.CompletedTask,
            GetAllAsyncHandler = _ => Task.FromResult<IReadOnlyList<TrackedTitle>>([])
        });

        services.AddSingleton<IWatchProgressRepository>(new TestWatchProgressRepository
        {
            SaveAsyncHandler = (_, _) => Task.CompletedTask,
            GetAsyncHandler = (_, _) => Task.FromResult<WatchProgress?>(null)
        });

        services.AddShowTrackerApplication();

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }
}
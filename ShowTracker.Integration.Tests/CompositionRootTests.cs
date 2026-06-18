using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Persistence;
using ShowTracker.Providers.Trakt;

namespace ShowTracker.Integration.Tests;

public sealed class CompositionRootTests
{
    [Fact]
    public void Can_Build_Complete_ServiceProvider()
    {
        using var provider = CreateProvider();

        Assert.NotNull(provider);
    }

    [Fact]
    public void Can_Resolve_Title_Application_Services()
    {
        using var provider = CreateProvider();

        Assert.NotNull(provider.GetRequiredService<ISearchTitlesService>());
        Assert.NotNull(provider.GetRequiredService<ITrackShowService>());
        Assert.NotNull(provider.GetRequiredService<ITrackMovieService>());
        Assert.NotNull(provider.GetRequiredService<IUntrackTitleService>());
        Assert.NotNull(provider.GetRequiredService<IGetTrackedTitlesService>());
    }

    [Fact]
    public void Can_Resolve_Watch_Progress_Application_Services()
    {
        using var provider = CreateProvider();

        Assert.NotNull(provider.GetRequiredService<IMarkEpisodeWatchedService>());
        Assert.NotNull(provider.GetRequiredService<IMarkMovieWatchedService>());
        Assert.NotNull(provider.GetRequiredService<IGetShowProgressService>());
        Assert.NotNull(provider.GetRequiredService<IGetNextEpisodeToWatchService>());
    }

    [Fact]
    public void Can_Resolve_Release_Application_Services()
    {
        using var provider = CreateProvider();

        Assert.NotNull(provider.GetRequiredService<IGetUpcomingReleasesService>());
        Assert.NotNull(provider.GetRequiredService<IGetNextReleaseService>());
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();

        services.AddShowTrackerApplication();

        services.AddShowTrackerPersistence(
            "Data Source=:memory:");

        services.AddShowTrackerTrakt(options =>
        {
            options.ClientId = "test-client-id";
        });

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }
}
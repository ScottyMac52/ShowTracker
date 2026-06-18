using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console;
using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class ProgramBootstrapTests
{
    [Fact]
    public void AddShowTrackerConsole_Registers_IConsoleApplication()
    {
        using var provider = CreateServiceProvider();

        var app = provider.GetRequiredService<IConsoleApplication>();

        Assert.NotNull(app);
        Assert.IsType<ConsoleApplication>(app);
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_CommandRouter()
    {
        using var provider = CreateServiceProvider();

        var router = provider.GetRequiredService<CommandRouter>();

        Assert.NotNull(router);
    }

    [Fact]
    public void AddShowTrackerConsole_Can_Resolve_All_Console_Commands()
    {
        using var provider = CreateServiceProvider();

        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<SearchCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<TrackShowCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<TrackMovieCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetTrackedTitlesCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<UntrackCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<MarkEpisodeWatchedCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<MarkMovieWatchedCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetNextEpisodeCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetNextReleaseCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetUpcomingReleasesCommand>());
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISearchTitlesService, TestSearchTitlesService>();
        services.AddSingleton<ITrackShowService, TestTrackShowService>();
        services.AddSingleton<ITrackMovieService, TestTrackMovieService>();
        services.AddSingleton<IGetTrackedTitlesService, TestGetTrackedTitlesService>();
        services.AddSingleton<IUntrackTitleService, TestUntrackTitleService>();
        services.AddSingleton<IMarkEpisodeWatchedService, TestMarkEpisodeWatchedService>();
        services.AddSingleton<IMarkMovieWatchedService, TestMarkMovieWatchedService>();
        services.AddSingleton<IGetNextEpisodeToWatchService, TestGetNextEpisodeToWatchService>();
        services.AddSingleton<IGetNextReleaseService, TestGetNextReleaseService>();
        services.AddSingleton<IGetUpcomingReleasesService, TestGetUpcomingReleasesService>();

        services.AddShowTrackerConsole();

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }

    private sealed class TestSearchTitlesService : ISearchTitlesService
    {
        public Task<IReadOnlyList<TitleSearchResult>> SearchTitlesAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
        }
    }

    private sealed class TestTrackShowService : ITrackShowService
    {
        public Task<TrackedTitle> TrackShowAsync(
            string showTitle,
            string? platform = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new TrackedTitle(
                    ProviderId: "test-show",
                    Title: showTitle,
                    Type: TrackedTitleType.Show,
                    Platform: platform));
        }
    }

    private sealed class TestTrackMovieService : ITrackMovieService
    {
        public Task<TrackedTitle> TrackMovieAsync(
            string movieTitle,
            string? platform = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new TrackedTitle(
                    ProviderId: "test-movie",
                    Title: movieTitle,
                    Type: TrackedTitleType.Movie,
                    Platform: platform));
        }
    }

    private sealed class TestGetTrackedTitlesService : IGetTrackedTitlesService
    {
        public Task<IReadOnlyList<TrackedTitle>> GetTrackedTitlesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TrackedTitle>>([]);
        }
    }

    private sealed class TestUntrackTitleService : IUntrackTitleService
    {
        public Task UntrackAsync(
            string providerId,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TestMarkEpisodeWatchedService : IMarkEpisodeWatchedService
    {
        public Task MarkEpisodeWatchedAsync(
            string showTitle,
            int seasonNumber,
            int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TestMarkMovieWatchedService : IMarkMovieWatchedService
    {
        public Task MarkMovieWatchedAsync(
            string movieTitle,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TestGetNextEpisodeToWatchService : IGetNextEpisodeToWatchService
    {
        public Task<NextEpisodeToWatch?> GetNextEpisodeToWatchAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<NextEpisodeToWatch?>(null);
        }
    }

    private sealed class TestGetNextReleaseService : IGetNextReleaseService
    {
        public Task<UpcomingRelease?> GetNextReleaseAsync(
            string showTitle,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UpcomingRelease?>(null);
        }
    }

    private sealed class TestGetUpcomingReleasesService : IGetUpcomingReleasesService
    {
        public Task<IReadOnlyList<UpcomingRelease>> GetUpcomingReleasesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<UpcomingRelease>>([]);
        }
    }
}
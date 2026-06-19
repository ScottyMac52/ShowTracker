using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console;
using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Tests;

public sealed class ConsoleServiceRegistrationTests
{
    [Fact]
    public void AddShowTrackerConsole_Registers_CommandRouter()
    {
        using var provider = CreateServiceProvider();

        var router = provider.GetRequiredService<CommandRouter>();

        Assert.NotNull(router);
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_SearchCommand()
    {
        using var provider = CreateServiceProvider();

        var command = provider.GetRequiredService<SearchCommand>();

        Assert.NotNull(command);
        Assert.IsAssignableFrom<IConsoleCommand>(command);
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_Track_Commands()
    {
        using var provider = CreateServiceProvider();

        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<TrackShowCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<TrackMovieCommand>());
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_Title_List_Commands()
    {
        using var provider = CreateServiceProvider();

        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetTrackedTitlesCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<UntrackCommand>());
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_Watch_Progress_Commands()
    {
        using var provider = CreateServiceProvider();

        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<MarkEpisodeWatchedCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<MarkMovieWatchedCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetNextEpisodeCommand>());
    }

    [Fact]
    public void AddShowTrackerConsole_Registers_Release_Commands()
    {
        using var provider = CreateServiceProvider();

        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetNextReleaseCommand>());
        Assert.IsAssignableFrom<IConsoleCommand>(provider.GetRequiredService<GetUpcomingReleasesCommand>());
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISearchTitlesService>(new TestSearchTitlesService());
        services.AddSingleton<ITrackShowService>(new TestTrackShowService());
        services.AddSingleton<ITrackMovieService>(new TestTrackMovieService());
        services.AddSingleton<IGetTrackedTitlesService>(new TestGetTrackedTitlesService());
        services.AddSingleton<IUntrackTitleService>(new TestUntrackTitleService());
        services.AddSingleton<IMarkEpisodeWatchedService>(new TestMarkEpisodeWatchedService());
        services.AddSingleton<IMarkMovieWatchedService>(new TestMarkMovieWatchedService());
        services.AddSingleton<IGetNextEpisodeToWatchService>(new TestGetNextEpisodeToWatchService());
        services.AddSingleton<IGetNextReleaseService>(new TestGetNextReleaseService());
        services.AddSingleton<IGetUpcomingReleasesService>(new TestGetUpcomingReleasesService());

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

        public Task<IReadOnlyList<TitleSearchResult>> SearchShowsAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
        }

        public Task<IReadOnlyList<TitleSearchResult>> SearchMoviesAsync(
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
                new TrackedTitle("test-show", showTitle, TrackedTitleType.Show, platform));
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
                new TrackedTitle("test-movie", movieTitle, TrackedTitleType.Movie, platform));
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
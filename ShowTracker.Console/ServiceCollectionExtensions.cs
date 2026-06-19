using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Console.Commands;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShowTrackerConsole(
        this IServiceCollection services)
    {
        services.AddTransient<SearchCommand>();
        services.AddTransient<TrackShowCommand>();
        services.AddTransient<TrackMovieCommand>();
        services.AddTransient<GetTrackedTitlesCommand>();
        services.AddTransient<UntrackCommand>();
        services.AddTransient<MarkEpisodeWatchedCommand>();
        services.AddTransient<MarkMovieWatchedCommand>();
        services.AddTransient<GetNextEpisodeCommand>();
        services.AddTransient<GetNextReleaseCommand>();
        services.AddTransient<GetUpcomingReleasesCommand>();
        services.AddTransient<SearchShowCommand>();
        services.AddTransient<SearchMovieCommand>();
        services.AddTransient<ConsoleApplication>();
        services.AddTransient<IConsoleApplication, ConsoleApplication>();

        services.AddTransient(provider =>
            new CommandRouter(
                new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase)
                {
                    ["search"] = provider.GetRequiredService<SearchCommand>(),
                    ["track-show"] = provider.GetRequiredService<TrackShowCommand>(),
                    ["track-movie"] = provider.GetRequiredService<TrackMovieCommand>(),
                    ["tracked"] = provider.GetRequiredService<GetTrackedTitlesCommand>(),
                    ["untrack"] = provider.GetRequiredService<UntrackCommand>(),
                    ["watched-episode"] = provider.GetRequiredService<MarkEpisodeWatchedCommand>(),
                    ["watched-movie"] = provider.GetRequiredService<MarkMovieWatchedCommand>(),
                    ["next-episode"] = provider.GetRequiredService<GetNextEpisodeCommand>(),
                    ["next-release"] = provider.GetRequiredService<GetNextReleaseCommand>(),
                    ["releases"] = provider.GetRequiredService<GetUpcomingReleasesCommand>(),
                    ["search-show"] = provider.GetRequiredService<SearchShowCommand>(),
                    ["search-movie"] = provider.GetRequiredService<SearchMovieCommand>(),

                }));

        return services;
    }
}
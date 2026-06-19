using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application.Services;
using ShowTracker.Application.Services.Interfaces;

namespace ShowTracker.Application;

/// <summary>
/// Service collection extensions for the ShowTracker application. This class provides extension methods for registering the application services with the dependency injection container. The services include functionality for searching titles, tracking shows and movies, untracking titles, retrieving tracked titles, marking episodes and movies as watched, getting show progress, getting the next episode to watch, and getting upcoming releases. By using these extension methods, you can easily add the necessary services to your application's dependency injection container and ensure that they are available for use throughout the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ShowTracker application services to the specified service collection. This method registers all the necessary services for the ShowTracker application, including services for searching titles, tracking shows and movies, untracking titles, retrieving tracked titles, marking episodes and movies as watched, getting show progress, getting the next episode to watch, and getting upcoming releases. By calling this method, you can easily add all the required services to your application's dependency injection container and ensure that they are available for use throughout the application.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddShowTrackerApplication(
        this IServiceCollection services)
    {
        services.AddTransient<ISearchTitlesService, SearchTitlesService>();
        services.AddTransient<ITrackShowService, TrackShowService>();
        services.AddTransient<ITrackMovieService, TrackMovieService>();
        services.AddTransient<IUntrackTitleService, UntrackTitleService>();
        services.AddTransient<IGetTrackedTitlesService, GetTrackedTitlesService>();
        services.AddTransient<IMarkEpisodeWatchedService, MarkEpisodeWatchedService>();
        services.AddTransient<IMarkMovieWatchedService, MarkMovieWatchedService>();
        services.AddTransient<IGetShowProgressService, GetShowProgressService>();
        services.AddTransient<IGetNextEpisodeToWatchService, GetNextEpisodeToWatchService>();
        services.AddTransient<IGetUpcomingReleasesService, GetUpcomingReleasesService>();
        services.AddTransient<IGetNextReleaseService, GetNextReleaseService>();

        return services;
    }
}
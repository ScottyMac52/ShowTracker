using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Providers.Trakt;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShowTrackerTrakt(
        this IServiceCollection services,
        Action<TraktOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new TraktOptions();

        configure(options);

        services.AddSingleton(options);

        services.AddHttpClient<ITraktTitleSearchClient, TraktTitleSearchClient>(
            client =>
            {
                client.BaseAddress = options.BaseAddress;
            });

        services.AddSingleton<ITitleTrackingProvider, TraktTitleTrackingProvider>();

        return services;
    }
}
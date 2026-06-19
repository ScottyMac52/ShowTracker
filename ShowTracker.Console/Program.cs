using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application;
using ShowTracker.Console.Commands.Interfaces;
using ShowTracker.Persistence;
using ShowTracker.Providers.Trakt;

namespace ShowTracker.Console;

public static class Program
{

    public static async Task<int> Main(string[] args)
    {
        using var provider = BuildServiceProvider();

        var application = provider.GetRequiredService<IConsoleApplication>();

        return await application.RunAsync(args);
    }

    internal static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddShowTrackerApplication();

        services.AddShowTrackerPersistence(
            "Data Source=showtracker.db");

        services.AddShowTrackerTrakt(options =>
        {
            var clientId = Environment.GetEnvironmentVariable("TRAKT_CLIENT_ID");
            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("TRAKT_CLIENT_ID environment variable is required. Set it in VS project properties or your terminal.");

            options.ClientId = clientId;
            options.UserAgent = "ShowTracker/0.1";
        });

        services.AddShowTrackerConsole();

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }
}
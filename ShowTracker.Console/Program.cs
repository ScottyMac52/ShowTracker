using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Application;
using ShowTracker.Console.Commands.Interfaces;
using ShowTracker.Persistence;
using ShowTracker.Providers.Trakt;

namespace ShowTracker.Console;

public static class Program
{
    private const string ClientId = "bf928c789cc6ccb47e80bbbf8f3575fd9e86a9ba946e13b34ca1a2c187c91e32";

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
            options.ClientId =
                Environment.GetEnvironmentVariable("TRAKT_CLIENT_ID") ?? ClientId ?? throw new InvalidOperationException("TRAKT_CLIENT_ID is not configured.");

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
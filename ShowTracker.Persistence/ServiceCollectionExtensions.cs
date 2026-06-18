using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShowTrackerPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is required.", nameof(connectionString));

        services.AddSingleton(_ => new SqliteConnection(connectionString));

        services.AddSingleton<ITrackedTitleRepository, SqliteTrackedTitleRepository>();
        services.AddSingleton<IWatchProgressRepository, SqliteWatchProgressRepository>();

        return services;
    }
}
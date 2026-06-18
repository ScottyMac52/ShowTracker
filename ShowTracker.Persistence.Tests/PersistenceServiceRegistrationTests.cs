using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Persistence.Tests;

public sealed class PersistenceServiceRegistrationTests
{
    [Fact]
    public void AddShowTrackerPersistence_Registers_ITrackedTitleRepository()
    {
        using var provider = CreateServiceProvider();

        var repository = provider.GetRequiredService<ITrackedTitleRepository>();

        Assert.IsType<SqliteTrackedTitleRepository>(repository);
    }

    [Fact]
    public void AddShowTrackerPersistence_Registers_IWatchProgressRepository()
    {
        using var provider = CreateServiceProvider();

        var repository = provider.GetRequiredService<IWatchProgressRepository>();

        Assert.IsType<SqliteWatchProgressRepository>(repository);
    }

    [Fact]
    public void AddShowTrackerPersistence_Registers_Shared_SqliteConnection()
    {
        using var provider = CreateServiceProvider();

        var connection = provider.GetRequiredService<SqliteConnection>();

        Assert.Equal("Data Source=:memory:", connection.ConnectionString);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddShowTrackerPersistence(
            "Data Source=:memory:");

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }
}
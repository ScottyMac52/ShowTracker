using Microsoft.Extensions.DependencyInjection;
using ShowTracker.Domain.Services.Interfaces;

namespace ShowTracker.Providers.Trakt.Tests;

public sealed class TraktServiceRegistrationTests
{
    [Fact]
    public void AddShowTrackerTrakt_Registers_ITitleTrackingProvider()
    {
        using var provider = CreateServiceProvider();

        var service = provider.GetRequiredService<ITitleTrackingProvider>();

        Assert.IsType<TraktTitleTrackingProvider>(service);
    }

    [Fact]
    public void AddShowTrackerTrakt_Registers_ITraktTitleSearchClient()
    {
        using var provider = CreateServiceProvider();

        var service = provider.GetRequiredService<ITraktTitleSearchClient>();

        Assert.IsType<TraktTitleSearchClient>(service);
    }

    [Fact]
    public void AddShowTrackerTrakt_Registers_TraktOptions()
    {
        using var provider = CreateServiceProvider();

        var options = provider.GetRequiredService<TraktOptions>();

        Assert.Equal("test-client-id", options.ClientId);
        Assert.Equal(new Uri("https://api.trakt.tv/"), options.BaseAddress);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

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
using ShowTracker.Application;
using ShowTracker.Domain.Models;
using ShowTracker.Testing;

namespace ShowTracker.Application.Tests;

public sealed class SearchTitlesServiceTests
{
    [Fact]
    public async Task SearchTitlesAsync_Returns_Provider_Results()
    {
        IReadOnlyList<TitleSearchResult> expected =
        [
            new(
                ProviderId: "trakt:show:12345",
                Title: "Andor",
                Type: TrackedTitleType.Show,
                Year: 2022),

            new(
                ProviderId: "trakt:movie:654321",
                Title: "Andor: A Disney+ Day Special Look",
                Type: TrackedTitleType.Movie,
                Year: 2022)
        ];

        var provider = new TestTitleTrackingProvider
        {
            SearchTitlesAsyncHandler = (_, _) =>
                Task.FromResult(expected)
        };

        var service = new SearchTitlesService(provider);

        var results = await service.SearchTitlesAsync("Andor");

        Assert.Equal(expected, results);
    }

    [Fact]
    public async Task SearchTitlesAsync_Trims_Query()
    {
        string? requestedQuery = null;

        var provider = new TestTitleTrackingProvider
        {
            SearchTitlesAsyncHandler = (query, _) =>
            {
                requestedQuery = query;
                return Task.FromResult<IReadOnlyList<TitleSearchResult>>([]);
            }
        };

        var service = new SearchTitlesService(provider);

        await service.SearchTitlesAsync("  Andor  ");

        Assert.Equal("Andor", requestedQuery);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SearchTitlesAsync_Rejects_Blank_Query(string query)
    {
        var provider = new TestTitleTrackingProvider();
        var service = new SearchTitlesService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.SearchTitlesAsync(query));
    }
}
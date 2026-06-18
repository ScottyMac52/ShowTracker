using Microsoft.Data.Sqlite;
using ShowTracker.Domain.Models;

namespace ShowTracker.Persistence.Tests;

public sealed class SqliteTrackedTitleRepositoryTests
{
    [Fact]
    public async Task AddAsync_Saves_Tracked_Title()
    {
        var repository = CreateRepository();

        var title = new TrackedTitle(
            ProviderId: "trakt:show:12345",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Platform: "Disney Plus");

        await repository.AddAsync(title);

        var result = await repository.FindByProviderIdAsync("trakt:show:12345");

        Assert.NotNull(result);
        Assert.Equal(title, result);
    }

    [Fact]
    public async Task GetAllAsync_Returns_All_Tracked_Titles()
    {
        var repository = CreateRepository();

        await repository.AddAsync(new TrackedTitle(
            ProviderId: "trakt:show:12345",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Platform: "Disney Plus"));

        await repository.AddAsync(new TrackedTitle(
            ProviderId: "trakt:movie:654321",
            Title: "Dune: Part Two",
            Type: TrackedTitleType.Movie,
            Platform: "Max"));

        var results = await repository.GetAllAsync();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, t => t.Title == "Andor");
        Assert.Contains(results, t => t.Title == "Dune: Part Two");
    }

    [Fact]
    public async Task RemoveAsync_Removes_Tracked_Title()
    {
        var repository = CreateRepository();

        await repository.AddAsync(new TrackedTitle(
            ProviderId: "trakt:show:12345",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Platform: "Disney Plus"));

        await repository.RemoveAsync("trakt:show:12345");

        var result = await repository.FindByProviderIdAsync("trakt:show:12345");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByProviderIdAsync_Returns_Null_When_Missing()
    {
        var repository = CreateRepository();

        var result = await repository.FindByProviderIdAsync("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_Rejects_Null_Title()
    {
        var repository = CreateRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repository.AddAsync(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RemoveAsync_Rejects_Blank_Provider_Id(string providerId)
    {
        var repository = CreateRepository();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            repository.RemoveAsync(providerId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task FindByProviderIdAsync_Rejects_Blank_Provider_Id(string providerId)
    {
        var repository = CreateRepository();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            repository.FindByProviderIdAsync(providerId));
    }

    [Fact]
    public async Task GetAllAsync_Returns_Empty_List_When_None_Exist()
    {
        var repository = CreateRepository();

        var results = await repository.GetAllAsync();

        Assert.Empty(results);
    }

    private static SqliteTrackedTitleRepository CreateRepository()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        return new SqliteTrackedTitleRepository(connection);
    }
}
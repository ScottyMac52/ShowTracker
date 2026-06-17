using ShowTracker.Domain.Models;

namespace ShowTracker.Domain.Tests;

public sealed class TitleSearchResultTests
{
    [Fact]
    public void TitleSearchResult_Can_Represent_Search_Match()
    {
        var result = new TitleSearchResult(
            ProviderId: "trakt:show:andor",
            Title: "Andor",
            Type: TrackedTitleType.Show,
            Year: 2022,
            Platform: "Disney Plus");

        Assert.Equal("trakt:show:andor", result.ProviderId);
        Assert.Equal("Andor", result.Title);
        Assert.Equal(TrackedTitleType.Show, result.Type);
        Assert.Equal(2022, result.Year);
        Assert.Equal("Disney Plus", result.Platform);
    }
}
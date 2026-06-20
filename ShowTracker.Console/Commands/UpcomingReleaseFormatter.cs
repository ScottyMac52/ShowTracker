using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Commands;

internal static class UpcomingReleaseFormatter
{
    public static string Format(
        UpcomingRelease release)
    {
        ArgumentNullException.ThrowIfNull(release);

        var releaseDate = release.ReleaseDate.ToString("yyyy-MM-dd");

        if (release.Type == TrackedTitleType.Show)
            return FormatShowRelease(releaseDate, release);

        return FormatMovieRelease(releaseDate, release);
    }

    private static string FormatShowRelease(
        string releaseDate,
        UpcomingRelease release)
    {
        var episodeNumber = FormatEpisodeNumber(
            release.SeasonNumber,
            release.EpisodeNumber);

        var episodeTitle = string.IsNullOrWhiteSpace(release.EpisodeTitle)
            ? null
            : release.EpisodeTitle.Trim();

        if (episodeNumber is not null && episodeTitle is not null)
            return $"{releaseDate}: {release.Title} - {episodeNumber} - {episodeTitle}";

        if (episodeNumber is not null)
            return $"{releaseDate}: {release.Title} - {episodeNumber}";

        if (episodeTitle is not null)
            return $"{releaseDate}: {release.Title} - {episodeTitle}";

        return $"{releaseDate}: {release.Title}";
    }

    private static string FormatMovieRelease(
        string releaseDate,
        UpcomingRelease release)
    {
        return $"{releaseDate}: {release.Title}";
    }

    private static string? FormatEpisodeNumber(
        int? seasonNumber,
        int? episodeNumber)
    {
        if (seasonNumber is not null && episodeNumber is not null)
            return $"S{seasonNumber:00}E{episodeNumber:00}";

        if (episodeNumber is not null)
            return $"E{episodeNumber:00}";

        return null;
    }
}
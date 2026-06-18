namespace ShowTracker.Console.Commands;

internal static class CommandArgumentParser
{
    public static string RequireTextAfterCommand(string[] args, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length < 2)
            throw new ArgumentException(errorMessage, nameof(args));

        var value = string.Join(' ', args.Skip(1)).Trim();

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(errorMessage, nameof(args));

        return value;
    }

    public static (string Title, int Season, int Episode) RequireEpisodeArguments(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length < 4)
            throw new ArgumentException("Show title, season number, and episode number are required.", nameof(args));

        if (!int.TryParse(args[^2], out var seasonNumber))
            throw new ArgumentException("Season number is required.", nameof(args));

        if (!int.TryParse(args[^1], out var episodeNumber))
            throw new ArgumentException("Episode number is required.", nameof(args));

        var title = string.Join(' ', args.Skip(1).Take(args.Length - 3)).Trim();

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Show title is required.", nameof(args));

        return (title, seasonNumber, episodeNumber);
    }
}
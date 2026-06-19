namespace ShowTracker.Console.Commands;

internal static class TrackCommandArgumentParser
{
    public static (string Title, string? Platform) Parse(
        string[] args,
        string missingTitleMessage)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length < 2)
            throw new ArgumentException(missingTitleMessage, nameof(args));

        var platformIndex = Array.FindIndex(
            args,
            argument => string.Equals(argument, "--platform", StringComparison.OrdinalIgnoreCase));

        if (platformIndex < 0)
            return (string.Join(" ", args.Skip(1)).Trim(), null);

        if (platformIndex == args.Length - 1)
            throw new ArgumentException("Platform value is required.", nameof(args));

        var title = string.Join(" ", args.Skip(1).Take(platformIndex - 1)).Trim();
        var platform = string.Join(" ", args.Skip(platformIndex + 1)).Trim();

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(missingTitleMessage, nameof(args));

        return (title, platform);
    }
}
using System.Text;
using ShowTracker.Application.Services.Interfaces;
using ShowTracker.Console.Commands.Interfaces;

namespace ShowTracker.Console.Commands;

public sealed class GetUpcomingReleasesCommand : IConsoleCommand
{
    private readonly IGetUpcomingReleasesService _getUpcomingReleasesService;

    public GetUpcomingReleasesCommand(IGetUpcomingReleasesService getUpcomingReleasesService)
    {
        _getUpcomingReleasesService = getUpcomingReleasesService ?? throw new ArgumentNullException(nameof(getUpcomingReleasesService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var releases = await _getUpcomingReleasesService.GetUpcomingReleasesAsync(cancellationToken);

        if (releases.Count == 0)
            return "No upcoming releases.";

        var builder = new StringBuilder();

        foreach (var release in releases)
            builder.AppendLine($"{release.ReleaseDate:yyyy-MM-dd}: {release.Title}");

        return builder.ToString().TrimEnd();
    }
}
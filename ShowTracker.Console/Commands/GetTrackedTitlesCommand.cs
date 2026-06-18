using ShowTracker.Application.Services.Interfaces;
using System.Text;

namespace ShowTracker.Console.Commands;

public sealed class GetTrackedTitlesCommand : IConsoleCommand
{
    private readonly IGetTrackedTitlesService _getTrackedTitlesService;

    public GetTrackedTitlesCommand(IGetTrackedTitlesService getTrackedTitlesService)
    {
        _getTrackedTitlesService = getTrackedTitlesService ?? throw new ArgumentNullException(nameof(getTrackedTitlesService));
    }

    public async Task<string> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var titles = await _getTrackedTitlesService.GetTrackedTitlesAsync(cancellationToken);

        if (titles.Count == 0)
            return "No tracked titles.";

        var builder = new StringBuilder();

        foreach (var title in titles)
            builder.AppendLine($"{title.Type}: {title.Title} [{title.ProviderId}]");

        return builder.ToString().TrimEnd();
    }
}
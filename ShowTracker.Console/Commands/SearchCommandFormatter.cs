using System.Text;
using ShowTracker.Domain.Models;

namespace ShowTracker.Console.Commands;

internal static class SearchCommandFormatter
{
    public static string Format(
        IReadOnlyList<TitleSearchResult> results)
    {
        if (results.Count == 0)
            return "No titles found.";

        var builder = new StringBuilder();

        foreach (var result in results)
        {
            builder.Append(result.Type);
            builder.Append(": ");
            builder.Append(result.Title);

            if (result.Year is not null)
            {
                builder.Append(" (");
                builder.Append(result.Year);
                builder.Append(')');
            }

            builder.Append(" [");
            builder.Append(result.ProviderId);
            builder.AppendLine("]");
        }

        return builder.ToString().TrimEnd();
    }
}
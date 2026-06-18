public interface IConsoleCommand
{
    Task<string> ExecuteAsync(
        string[] args,
        CancellationToken cancellationToken = default);
}
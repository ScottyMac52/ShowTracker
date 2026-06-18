namespace ShowTracker.Console.Commands.Interfaces;

public interface IConsoleApplication
{
    Task<int> RunAsync(
        string[] args,
        CancellationToken cancellationToken = default);
}
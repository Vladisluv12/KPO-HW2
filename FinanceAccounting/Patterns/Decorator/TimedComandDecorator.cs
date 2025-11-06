using KPO_DZ2.Patterns.Command;

namespace KPO_DZ2.Patterns.Decorator;

using System.Diagnostics;

public class TimedCommandDecorator<T>(ICommand<T> decoratedCommand) : ICommand<T>
{
    public T Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        var result = decoratedCommand.Execute();
        stopwatch.Stop();
        
        Console.WriteLine($"Command executed in {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }
}
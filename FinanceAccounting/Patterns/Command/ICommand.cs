namespace KPO_DZ2.Patterns.Command;

public interface ICommand
{
    void Execute();
}

public interface ICommand<out T>
{
    T Execute();
}
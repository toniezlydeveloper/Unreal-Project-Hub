using System.Windows.Input;

namespace UnrealProjectHub;

public class SimpleCommand(Action execute) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? _) => true;

    public void Execute(object? _)
    {
        execute();
    }
}
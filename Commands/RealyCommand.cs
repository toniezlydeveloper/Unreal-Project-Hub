using System.Windows.Input;
using UnrealProjectHub.Models;

namespace UnrealProjectHub.Commands;

public class RelayCommand(Action<ProjectEntry> execute, Func<ProjectEntry, bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter is not ProjectEntry entry)
        {
            return true;
        }
        
        return canExecute?.Invoke(entry) ?? true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not ProjectEntry entry)
        {
            return;
        }
        
        execute(entry);
    }
}
using System.Windows.Input;

namespace EvalProUI.model;

/// <summary>
/// Simple ICommand implementation for MVVM data binding.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>Creates a new command with a parameterized execute action.</summary>
    /// <param name="execute">Action to execute.</param>
    /// <param name="canExecute">Optional predicate controlling whether the command can execute.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>Creates a new command with a parameterless execute action.</summary>
    /// <param name="execute">Action to execute.</param>
    /// <param name="canExecute">Optional predicate controlling whether the command can execute.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
        : this(_ => execute(), canExecute != null ? _ => canExecute() : null)
    {
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc />
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <inheritdoc />
    public void Execute(object? parameter) => _execute(parameter);
}

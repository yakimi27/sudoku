using System.Windows.Input;

namespace Sudoku.Presentation.ViewModels.Base;

/// <summary>
/// Implements ICommand to provide a simple command binding mechanism for ViewModels.
/// Enables binding user actions to ViewModel methods without code-behind.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    /// <summary>
    /// Initializes a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">The action to execute when the command is invoked.</param>
    /// <param name="canExecute">Optional predicate to determine if command can execute.</param>
    /// <exception cref="ArgumentNullException">Thrown when execute is null.</exception>
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Event raised when the ability to execute the command changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines if the command can be executed with the given parameter.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>True if the command can execute, false otherwise.</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// Executes the command with the given parameter.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to notify UI of command state change.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// Generic variant of RelayCommand with typed command parameter.
/// </summary>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    /// <summary>
    /// Initializes a new instance of the RelayCommand&lt;T&gt; class.
    /// </summary>
    /// <param name="execute">The action to execute when the command is invoked.</param>
    /// <param name="canExecute">Optional predicate to determine if command can execute.</param>
    /// <exception cref="ArgumentNullException">Thrown when execute is null.</exception>
    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Event raised when the ability to execute the command changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines if the command can be executed with the given parameter.
    /// </summary>
    /// <param name="parameter">The command parameter (will be cast to T).</param>
    /// <returns>True if the command can execute, false otherwise.</returns>
    public bool CanExecute(object? parameter)
    {
        try
        {
            var typedParameter = (T?)parameter;
            return _canExecute?.Invoke(typedParameter) ?? true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Executes the command with the given parameter.
    /// </summary>
    /// <param name="parameter">The command parameter (will be cast to T).</param>
    public void Execute(object? parameter)
    {
        _execute((T?)parameter);
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to notify UI of command state change.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

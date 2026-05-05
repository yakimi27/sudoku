using Sudoku.Core.Models;

namespace Sudoku.Core.Commands;

/// <summary>
/// Manages command history for undo/redo functionality.
/// Maintains two stacks: one for undo and one for redo.
/// Implements the Command pattern with stack-based navigation.
/// </summary>
public sealed class CommandHistory
{
    /// <summary>
    /// Stack of commands that can be undone (executing them would reach current state).
    /// </summary>
    private readonly Stack<IGameCommand> _undoStack;

    /// <summary>
    /// Stack of commands that can be redone (undoing them reached current state).
    /// </summary>
    private readonly Stack<IGameCommand> _redoStack;

    /// <summary>
    /// Gets a value indicating whether an undo operation is available.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether a redo operation is available.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Gets the number of commands in the undo history.
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    /// Gets the number of commands in the redo history.
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Initializes a new instance of the CommandHistory class.
    /// </summary>
    public CommandHistory()
    {
        _undoStack = new Stack<IGameCommand>();
        _redoStack = new Stack<IGameCommand>();
    }

    /// <summary>
    /// Executes a command on the board and adds it to the undo history.
    /// Clears the redo stack since a new branch was created.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="board">The board to execute the command on.</param>
    /// <exception cref="ArgumentNullException">Thrown when command or board is null.</exception>
    public void Execute(IGameCommand command, SudokuBoard board)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        command.Execute(board);
        _undoStack.Push(command);
        _redoStack.Clear(); // Branching: clear redo history
    }

    /// <summary>
    /// Undoes the last command by popping from undo stack and executing its Undo method.
    /// The command is pushed onto the redo stack.
    /// </summary>
    /// <param name="board">The board to undo on.</param>
    /// <returns>True if an undo was performed, false if undo stack was empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public bool Undo(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        if (!CanUndo)
            return false;

        var command = _undoStack.Pop();
        command.Undo(board);
        _redoStack.Push(command);
        return true;
    }

    /// <summary>
    /// Redoes the last undone command by popping from redo stack and executing it.
    /// The command is pushed onto the undo stack.
    /// </summary>
    /// <param name="board">The board to redo on.</param>
    /// <returns>True if a redo was performed, false if redo stack was empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public bool Redo(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        if (!CanRedo)
            return false;

        var command = _redoStack.Pop();
        command.Execute(board);
        _undoStack.Push(command);
        return true;
    }

    /// <summary>
    /// Gets the complete command history as a read-only list.
    /// Most recent commands appear last in the list.
    /// </summary>
    /// <returns>A read-only list of all commands in undo history.</returns>
    public IReadOnlyList<IGameCommand> GetHistory()
    {
        return _undoStack.Reverse().ToList().AsReadOnly();
    }

    /// <summary>
    /// Clears both undo and redo stacks.
    /// Call this when starting a new game or resetting.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    /// <summary>
    /// Gets the description of the last undoable command without executing it.
    /// Useful for UI "Undo X" buttons.
    /// </summary>
    /// <returns>The description of the last command, or null if no undo available.</returns>
    public string? GetUndoDescription()
    {
        if (!CanUndo)
            return null;

        return _undoStack.Peek().Description;
    }

    /// <summary>
    /// Gets the description of the last redoable command without executing it.
    /// Useful for UI "Redo X" buttons.
    /// </summary>
    /// <returns>The description of the last command, or null if no redo available.</returns>
    public string? GetRedoDescription()
    {
        if (!CanRedo)
            return null;

        return _redoStack.Peek().Description;
    }
}

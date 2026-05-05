using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.Commands;

/// <summary>
/// Command for setting a cell's value in a Sudoku puzzle.
/// Stores the old and new values along with cell states for proper undo.
/// </summary>
public sealed class SetValueCommand : IGameCommand
{
    /// <summary>
    /// The row of the cell.
    /// </summary>
    private readonly int _row;

    /// <summary>
    /// The column of the cell.
    /// </summary>
    private readonly int _column;

    /// <summary>
    /// The value before the command was executed.
    /// </summary>
    private readonly int _oldValue;

    /// <summary>
    /// The value after the command is executed.
    /// </summary>
    private readonly int _newValue;

    /// <summary>
    /// The cell state before the command was executed.
    /// </summary>
    private readonly CellState _oldState;

    /// <summary>
    /// Gets the timestamp when this command was executed.
    /// </summary>
    public DateTime ExecutedAt { get; }

    /// <summary>
    /// Gets a human-readable description of this command.
    /// </summary>
    public string Description => $"Set cell ({_row}, {_column}) to {_newValue}";

    /// <summary>
    /// Initializes a new instance of the SetValueCommand class.
    /// </summary>
    /// <param name="row">The row of the cell (0-8 or 0-5 for mini).</param>
    /// <param name="column">The column of the cell (0-8 or 0-5 for mini).</param>
    /// <param name="oldValue">The previous value in the cell.</param>
    /// <param name="newValue">The new value to set.</param>
    /// <param name="oldState">The previous state of the cell.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is out of range.</exception>
    public SetValueCommand(int row, int column, int oldValue, int newValue, CellState oldState)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0-8.");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be 0-8.");

        _row = row;
        _column = column;
        _oldValue = oldValue;
        _newValue = newValue;
        _oldState = oldState;
        ExecutedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Executes the command by setting the cell to the new value.
    /// </summary>
    /// <param name="board">The board to execute on.</param>
    public void Execute(SudokuBoard board)
    {
        var newCell = new Cell(_row, _column, value: _newValue, state: CellState.Filled);
        board.SetCell(_row, _column, newCell);
    }

    /// <summary>
    /// Undoes the command by restoring the cell to its old value and state.
    /// </summary>
    /// <param name="board">The board to undo on.</param>
    public void Undo(SudokuBoard board)
    {
        var oldCell = new Cell(_row, _column, value: _oldValue, state: _oldState);
        board.SetCell(_row, _column, oldCell);
    }
}

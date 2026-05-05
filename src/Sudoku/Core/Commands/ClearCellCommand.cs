using Sudoku.Core.Models;

namespace Sudoku.Core.Commands;

/// <summary>
/// Command for clearing a cell (removing both value and notes).
/// Stores a complete snapshot of the cell for full undo capability.
/// </summary>
public sealed class ClearCellCommand : IGameCommand
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
    /// The complete cell snapshot before the command was executed.
    /// Stores value, state, and notes for complete undo.
    /// </summary>
    private readonly Cell _snapshotBefore;

    /// <summary>
    /// Gets the timestamp when this command was executed.
    /// </summary>
    public DateTime ExecutedAt { get; }

    /// <summary>
    /// Gets a human-readable description of this command.
    /// </summary>
    public string Description => $"Clear cell ({_row}, {_column})";

    /// <summary>
    /// Initializes a new instance of the ClearCellCommand class.
    /// </summary>
    /// <param name="row">The row of the cell (0-8).</param>
    /// <param name="column">The column of the cell (0-8).</param>
    /// <param name="cellBefore">A snapshot of the cell before clearing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is out of range.</exception>
    /// <exception cref="ArgumentNullException">Thrown when cellBefore is null.</exception>
    public ClearCellCommand(int row, int column, Cell cellBefore)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0-8.");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be 0-8.");
        if (cellBefore == null)
            throw new ArgumentNullException(nameof(cellBefore));

        _row = row;
        _column = column;
        _snapshotBefore = cellBefore;
        ExecutedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Executes the command by clearing the cell (value = 0, empty notes).
    /// </summary>
    /// <param name="board">The board to execute on.</param>
    public void Execute(SudokuBoard board)
    {
        var clearedCell = new Cell(_row, _column, value: 0);
        board.SetCell(_row, _column, clearedCell);
    }

    /// <summary>
    /// Undoes the command by restoring the cell to its snapshot state.
    /// </summary>
    /// <param name="board">The board to undo on.</param>
    public void Undo(SudokuBoard board)
    {
        board.SetCell(_row, _column, _snapshotBefore);
    }
}

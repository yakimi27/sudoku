using Sudoku.Core.Models;

namespace Sudoku.Core.Commands;

/// <summary>
/// Command for toggling a note digit on a cell.
/// Notes are candidate digits that might go in the cell.
/// </summary>
public sealed class SetNoteCommand : IGameCommand
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
    /// The note digit being toggled (1-9).
    /// </summary>
    private readonly int _digit;

    /// <summary>
    /// Whether the note was present before the command.
    /// Used to determine if we're adding or removing the note on undo.
    /// </summary>
    private readonly bool _wasPresent;

    /// <summary>
    /// Gets the timestamp when this command was executed.
    /// </summary>
    public DateTime ExecutedAt { get; }

    /// <summary>
    /// Gets a human-readable description of this command.
    /// </summary>
    public string Description => _wasPresent 
        ? $"Remove note {_digit} from cell ({_row}, {_column})" 
        : $"Add note {_digit} to cell ({_row}, {_column})";

    /// <summary>
    /// Initializes a new instance of the SetNoteCommand class.
    /// </summary>
    /// <param name="row">The row of the cell (0-8).</param>
    /// <param name="column">The column of the cell (0-8).</param>
    /// <param name="digit">The note digit (1-9).</param>
    /// <param name="wasPresent">Whether the note existed before this command.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of range.</exception>
    public SetNoteCommand(int row, int column, int digit, bool wasPresent)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0-8.");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be 0-8.");
        if (digit < 1 || digit > 9)
            throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be 1-9.");

        _row = row;
        _column = column;
        _digit = digit;
        _wasPresent = wasPresent;
        ExecutedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Executes the command by toggling the note (removes if present, adds if absent).
    /// </summary>
    /// <param name="board">The board to execute on.</param>
    public void Execute(SudokuBoard board)
    {
        var cell = board.GetCell(_row, _column);
        var newNotes = new List<int>(cell.Notes);

        if (_wasPresent)
        {
            // Remove the note
            newNotes.Remove(_digit);
        }
        else
        {
            // Add the note
            if (!newNotes.Contains(_digit))
                newNotes.Add(_digit);
        }

        var updatedCell = cell.WithNotes(newNotes);
        board.SetCell(_row, _column, updatedCell);
    }

    /// <summary>
    /// Undoes the command by restoring the note to its previous state.
    /// </summary>
    /// <param name="board">The board to undo on.</param>
    public void Undo(SudokuBoard board)
    {
        var cell = board.GetCell(_row, _column);
        var newNotes = new List<int>(cell.Notes);

        if (_wasPresent)
        {
            // Undo removal: add note back
            if (!newNotes.Contains(_digit))
                newNotes.Add(_digit);
        }
        else
        {
            // Undo addition: remove note
            newNotes.Remove(_digit);
        }

        var updatedCell = cell.WithNotes(newNotes);
        board.SetCell(_row, _column, updatedCell);
    }
}

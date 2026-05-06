using System.Collections.Immutable;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for a single cell in the Sudoku board.
/// Provides Observable properties for binding: Value, Notes, State, IsSelected, IsHighlighted, IsError.
/// </summary>
public class CellViewModel : ObservableObject
{
    private int _value;
    private ImmutableHashSet<int> _notes;
    private CellState _state;
    private bool _isSelected;
    private bool _isHighlighted;
    private bool _isError;

    /// <summary>
    /// Initializes a new instance of the CellViewModel class.
    /// </summary>
    /// <param name="cell">The underlying Cell model.</param>
    /// <exception cref="ArgumentNullException">Thrown when cell is null.</exception>
    public CellViewModel(Cell cell)
    {
        if (cell == null)
            throw new ArgumentNullException(nameof(cell));

        Row = cell.Row;
        Column = cell.Column;
        _value = cell.Value;
        _notes = cell.Notes;
        _state = cell.State;
        _isSelected = false;
        _isHighlighted = false;
        _isError = false;
    }

    /// <summary>
    /// Gets the row index of this cell (0-8).
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the column index of this cell (0-8).
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets or sets the current value in the cell (1-9 for filled, 0 for empty).
    /// </summary>
    public int Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    /// <summary>
    /// Gets or sets the candidate notes for this cell (1-9).
    /// </summary>
    public ImmutableHashSet<int> Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    /// <summary>
    /// Gets or sets the state of this cell (Empty, Given, Filled, Error).
    /// </summary>
    public CellState State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this cell is currently selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this cell is highlighted.
    /// True when it shares the same value, row, column, or block as the selected cell.
    /// </summary>
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this cell is in an error state.
    /// </summary>
    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    /// <summary>
    /// Synchronizes this ViewModel with the underlying Cell model.
    /// </summary>
    /// <param name="cell">The Cell model to sync with.</param>
    /// <exception cref="ArgumentNullException">Thrown when cell is null.</exception>
    public void SyncWithModel(Cell cell)
    {
        if (cell == null)
            throw new ArgumentNullException(nameof(cell));

        Value = cell.Value;
        Notes = cell.Notes;
        State = cell.State;
    }

    /// <summary>
    /// Determines whether this cell is in the same 3x3 block as another cell.
    /// </summary>
    /// <param name="otherRow">The row index of the other cell.</param>
    /// <param name="otherColumn">The column index of the other cell.</param>
    /// <returns>True if both cells are in the same block, false otherwise.</returns>
    public bool IsInSameBlock(int otherRow, int otherColumn)
    {
        int blockRow = Row / 3;
        int blockCol = Column / 3;
        int otherBlockRow = otherRow / 3;
        int otherBlockCol = otherColumn / 3;

        return blockRow == otherBlockRow && blockCol == otherBlockCol;
    }

    /// <summary>
    /// Determines whether this cell is in the same row as another cell.
    /// </summary>
    /// <param name="otherRow">The row index to check.</param>
    /// <returns>True if the row matches, false otherwise.</returns>
    public bool IsInSameRow(int otherRow) => Row == otherRow;

    /// <summary>
    /// Determines whether this cell is in the same column as another cell.
    /// </summary>
    /// <param name="otherColumn">The column index to check.</param>
    /// <returns>True if the column matches, false otherwise.</returns>
    public bool IsInSameColumn(int otherColumn) => Column == otherColumn;
}

namespace Sudoku.Infrastructure.Storage.Dtos;

/// <summary>
/// Data Transfer Object for a Sudoku cell.
/// Plain data class used for serialization, with no domain logic.
/// </summary>
public class CellDto
{
    /// <summary>
    /// Gets or sets the row index of this cell (0-8).
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Gets or sets the column index of this cell (0-8).
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Gets or sets the current value in this cell (0 means empty).
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a given (clue) cell.
    /// </summary>
    public bool IsGiven { get; set; }

    /// <summary>
    /// Gets or sets the current state of this cell (Empty, Given, Filled, etc.).
    /// </summary>
    public string CellState { get; set; } = "Empty";

    /// <summary>
    /// Gets or sets the list of candidate values for this cell.
    /// </summary>
    public List<int> Candidates { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the CellDto class.
    /// </summary>
    public CellDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the CellDto class with the specified values.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    /// <param name="value">The cell value.</param>
    /// <param name="isGiven">Whether this is a given cell.</param>
    /// <param name="cellState">The cell state.</param>
    public CellDto(int row, int column, int value, bool isGiven, string cellState = "Empty")
    {
        Row = row;
        Column = column;
        Value = value;
        IsGiven = isGiven;
        CellState = cellState;
    }
}

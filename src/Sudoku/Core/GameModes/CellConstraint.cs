namespace Sudoku.Core.GameModes;

/// <summary>
/// Represents a constraint on a cell for a specific game mode.
/// Used by UI to highlight and display cell constraints visually.
/// </summary>
public class CellConstraint
{
    /// <summary>
    /// Gets the row of the cell.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the column of the cell.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the type of constraint.
    /// </summary>
    public ConstraintType Type { get; }

    /// <summary>
    /// Gets additional constraint data (e.g., cage sum, thermometer ID).
    /// </summary>
    public string? Data { get; }

    /// <summary>
    /// Initializes a new instance of the CellConstraint class.
    /// </summary>
    /// <param name="row">The row of the cell.</param>
    /// <param name="column">The column of the cell.</param>
    /// <param name="type">The type of constraint.</param>
    /// <param name="data">Optional constraint data.</param>
    public CellConstraint(int row, int column, ConstraintType type, string? data = null)
    {
        Row = row;
        Column = column;
        Type = type;
        Data = data;
    }

    /// <summary>
    /// Equality comparison based on position, type, and data.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not CellConstraint other)
            return false;

        return Row == other.Row && Column == other.Column && Type == other.Type && Data == other.Data;
    }

    /// <summary>
    /// Hash code based on position, type, and data.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column, Type, Data);
    }
}

/// <summary>
/// Types of constraints that can apply to a cell.
/// </summary>
public enum ConstraintType
{
    /// <summary>
    /// No specific constraint.
    /// </summary>
    None = 0,

    /// <summary>
    /// Cell is part of a thermometer (ordered sequence).
    /// </summary>
    Thermometer = 1,

    /// <summary>
    /// Cell is part of a killer cage with sum constraint.
    /// </summary>
    KillerCage = 2,

    /// <summary>
    /// Cell is given/clue.
    /// </summary>
    Given = 3
}

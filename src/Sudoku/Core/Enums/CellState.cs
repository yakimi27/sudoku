namespace Sudoku.Core.Enums;

/// <summary>
/// Defines the possible states of a Sudoku cell.
/// </summary>
public enum CellState
{
    /// <summary>
    /// Cell is empty and has not been filled.
    /// </summary>
    Empty = 0,

    /// <summary>
    /// Cell is given (part of initial puzzle).
    /// </summary>
    Given = 1,

    /// <summary>
    /// Cell has been filled by the player.
    /// </summary>
    Filled = 2,

    /// <summary>
    /// Cell contains candidate numbers (pencil marks).
    /// </summary>
    Candidates = 3,

    /// <summary>
    /// Cell is selected by the player.
    /// </summary>
    Selected = 4,

    /// <summary>
    /// Cell contains an error (conflicts with rules).
    /// </summary>
    Error = 5
}

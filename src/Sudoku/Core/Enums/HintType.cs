namespace Sudoku.Core.Enums;

/// <summary>
/// Defines the types of hints available to the player.
/// </summary>
public enum HintType
{
    /// <summary>
    /// Reveal a single cell value.
    /// </summary>
    RevealCell = 0,

    /// <summary>
    /// Show candidate numbers for a cell.
    /// </summary>
    ShowCandidates = 1,

    /// <summary>
    /// Eliminate incorrect candidates.
    /// </summary>
    EliminateCandidates = 2,

    /// <summary>
    /// Highlight related cells (same row, column, or box).
    /// </summary>
    HighlightRelated = 3,

    /// <summary>
    /// Solve the entire puzzle.
    /// </summary>
    SolveAll = 4
}

namespace Sudoku.Core.Enums;

/// <summary>
/// Defines the available game modes in Sudoku.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Standard 9x9 Sudoku with 3x3 boxes.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Timed mode with countdown timer.
    /// </summary>
    Timed = 1,

    /// <summary>
    /// Puzzle mode with limited hints.
    /// </summary>
    Puzzle = 2,

    /// <summary>
    /// Campaign mode with multiple levels.
    /// </summary>
    Campaign = 3
}

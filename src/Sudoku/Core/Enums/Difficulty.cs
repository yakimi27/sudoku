namespace Sudoku.Core.Enums;

/// <summary>
/// Defines the difficulty levels for Sudoku puzzles.
/// </summary>
public enum Difficulty
{
    /// <summary>
    /// Easy puzzle with many given cells.
    /// </summary>
    Easy = 0,

    /// <summary>
    /// Medium puzzle with moderate number of given cells.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Hard puzzle with few given cells.
    /// </summary>
    Hard = 2,

    /// <summary>
    /// Expert puzzle with minimal given cells.
    /// </summary>
    Expert = 3
}

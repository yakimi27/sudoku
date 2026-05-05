using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Classic 9x9 Sudoku with no additional rules.
/// Implements the Strategy pattern as the base case.
/// </summary>
public sealed class ClassicMode : IGameMode
{
    /// <summary>
    /// Valid digits for classic Sudoku (1-9).
    /// </summary>
    private static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// Gets the game mode type.
    /// </summary>
    public GameMode ModeType => GameMode.Standard;

    /// <summary>
    /// Gets the localization key for display name.
    /// </summary>
    public string DisplayNameKey => "mode.classic";

    /// <summary>
    /// Gets the number of rows (9 for classic).
    /// </summary>
    public int BoardRows => 9;

    /// <summary>
    /// Gets the number of columns (9 for classic).
    /// </summary>
    public int BoardColumns => 9;

    /// <summary>
    /// Gets the block height (3x3 for classic).
    /// </summary>
    public int BlockHeight => 3;

    /// <summary>
    /// Gets the block width (3x3 for classic).
    /// </summary>
    public int BlockWidth => 3;

    /// <summary>
    /// Classic mode has no extra rules beyond standard Sudoku.
    /// </summary>
    /// <param name="board">The board state (unused).</param>
    /// <param name="row">The row (unused).</param>
    /// <param name="column">The column (unused).</param>
    /// <returns>Always true, as there are no extra rules.</returns>
    public bool ValidateExtraRules(SudokuBoard board, int row, int column)
    {
        return true;
    }

    /// <summary>
    /// Classic mode has no special constraints to return.
    /// </summary>
    /// <param name="board">The board (unused).</param>
    /// <returns>An empty list of constraints.</returns>
    public IReadOnlyList<CellConstraint> GetConstraints(SudokuBoard board)
    {
        return Array.Empty<CellConstraint>();
    }

    /// <summary>
    /// Returns valid digits for classic Sudoku (1-9).
    /// </summary>
    /// <returns>An array containing digits 1 through 9.</returns>
    public int[] GetValidDigits()
    {
        return ValidDigits;
    }
}

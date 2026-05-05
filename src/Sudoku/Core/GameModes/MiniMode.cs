using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Mini Mode: 6x6 Sudoku with 2x3 blocks.
/// Uses digits 1-6 instead of 1-9.
/// </summary>
public sealed class MiniMode : IGameMode
{
    /// <summary>
    /// Valid digits for mini Sudoku (1-6).
    /// </summary>
    private static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6 };

    /// <summary>
    /// Gets the game mode type.
    /// </summary>
    public GameMode ModeType => GameMode.Mini;

    /// <summary>
    /// Gets the localization key for display name.
    /// </summary>
    public string DisplayNameKey => "mode.mini";

    /// <summary>
    /// Gets the number of rows (6 for mini board).
    /// </summary>
    public int BoardRows => 6;

    /// <summary>
    /// Gets the number of columns (6 for mini board).
    /// </summary>
    public int BoardColumns => 6;

    /// <summary>
    /// Gets the block height (2 for 2x3 blocks).
    /// </summary>
    public int BlockHeight => 2;

    /// <summary>
    /// Gets the block width (3 for 2x3 blocks).
    /// </summary>
    public int BlockWidth => 3;

    /// <summary>
    /// Mini mode has no extra rules beyond standard Sudoku rules.
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
    /// Mini mode has no special constraints to return.
    /// </summary>
    /// <param name="board">The board (unused).</param>
    /// <returns>An empty list of constraints.</returns>
    public IReadOnlyList<CellConstraint> GetConstraints(SudokuBoard board)
    {
        return Array.Empty<CellConstraint>();
    }

    /// <summary>
    /// Returns valid digits for mini Sudoku (1-6).
    /// </summary>
    /// <returns>An array containing digits 1 through 6.</returns>
    public int[] GetValidDigits()
    {
        return ValidDigits;
    }
}

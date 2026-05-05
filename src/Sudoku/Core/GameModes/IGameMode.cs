using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Defines the contract for a Sudoku game mode.
/// Each mode can have additional rules beyond standard Sudoku.
/// Implements the Strategy pattern for interchangeable game behaviors.
/// </summary>
public interface IGameMode
{
    /// <summary>
    /// Gets the game mode type.
    /// </summary>
    GameMode ModeType { get; }

    /// <summary>
    /// Gets the localization key for the mode name.
    /// The UI will use this key to fetch the translated display name.
    /// </summary>
    string DisplayNameKey { get; }

    /// <summary>
    /// Gets the number of rows in this game mode's board.
    /// </summary>
    int BoardRows { get; }

    /// <summary>
    /// Gets the number of columns in this game mode's board.
    /// </summary>
    int BoardColumns { get; }

    /// <summary>
    /// Gets the height of a block (box) in this game mode.
    /// </summary>
    int BlockHeight { get; }

    /// <summary>
    /// Gets the width of a block (box) in this game mode.
    /// </summary>
    int BlockWidth { get; }

    /// <summary>
    /// Validates mode-specific rules in addition to standard Sudoku rules.
    /// </summary>
    /// <param name="board">The board state to validate.</param>
    /// <param name="row">The row of the cell being validated.</param>
    /// <param name="column">The column of the cell being validated.</param>
    /// <returns>True if the cell placement satisfies mode-specific rules, false otherwise.</returns>
    bool ValidateExtraRules(SudokuBoard board, int row, int column);

    /// <summary>
    /// Gets all constraints applicable to cells in this board.
    /// Constraints are used for visualization and rule checking.
    /// </summary>
    /// <param name="board">The board to analyze.</param>
    /// <returns>A read-only list of cell constraints for this mode.</returns>
    IReadOnlyList<CellConstraint> GetConstraints(SudokuBoard board);

    /// <summary>
    /// Creates a valid digit set for this game mode.
    /// Standard mode uses 1-9; Mini uses 1-6.
    /// </summary>
    /// <returns>An array of valid digits for this mode.</returns>
    int[] GetValidDigits();
}

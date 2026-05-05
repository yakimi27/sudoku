using Sudoku.Core.Models;

namespace Sudoku.Core.Commands;

/// <summary>
/// Defines the contract for a game command that can be executed and undone.
/// Implements the Command pattern for full undo/redo support and session replay.
/// </summary>
public interface IGameCommand
{
    /// <summary>
    /// Gets the timestamp when this command was executed.
    /// </summary>
    DateTime ExecutedAt { get; }

    /// <summary>
    /// Gets a human-readable description of this command.
    /// Used for displaying the command in history/undo UI.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes this command on the specified board.
    /// Modifies the board state according to the command's logic.
    /// </summary>
    /// <param name="board">The board to execute the command on.</param>
    void Execute(SudokuBoard board);

    /// <summary>
    /// Undoes this command on the specified board.
    /// Restores the board to its state before the command was executed.
    /// </summary>
    /// <param name="board">The board to undo the command on.</param>
    void Undo(SudokuBoard board);
}

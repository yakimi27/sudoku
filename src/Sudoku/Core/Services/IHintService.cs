namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for hint generation and management.
/// </summary>
public interface IHintService
{
    /// <summary>
    /// Gets the number of remaining hints available to the player.
    /// </summary>
    int RemainingHints { get; }

    /// <summary>
    /// Gets the maximum number of hints allowed per game.
    /// </summary>
    int MaxHints { get; }

    /// <summary>
    /// Generates a hint for the current game state.
    /// </summary>
    /// <param name="hintType">The type of hint requested.</param>
    /// <param name="gameState">Current game board state (serialized).</param>
    /// <returns>Hint data containing the recommended action.</returns>
    Task<string> GenerateHintAsync(Enums.HintType hintType, string gameState);

    /// <summary>
    /// Consumes a hint, decrementing the available hint count.
    /// </summary>
    /// <param name="hintType">The type of hint being used.</param>
    /// <returns>True if hint was consumed successfully, false if no hints remaining.</returns>
    bool UseHint(Enums.HintType hintType);

    /// <summary>
    /// Resets the hint count to maximum.
    /// </summary>
    void ResetHints();

    /// <summary>
    /// Sets the maximum number of hints for a game.
    /// </summary>
    /// <param name="maxHints">The maximum hint count.</param>
    void SetMaxHints(int maxHints);
}

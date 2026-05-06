using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Services;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements hint service functionality using the HintEngine.
/// Provides hint generation, hint count management, and hint type handling.
/// </summary>
public class HintService : IHintService
{
    private readonly HintEngine _hintEngine;
    private int _maxHints;

    /// <summary>
    /// Gets the number of remaining hints.
    /// </summary>
    public int RemainingHints => _hintEngine.RemainingHints;

    /// <summary>
    /// Gets the maximum number of hints allowed per game.
    /// </summary>
    public int MaxHints
    {
        get => _maxHints;
        private set => _maxHints = value;
    }

    /// <summary>
    /// Initializes a new instance of the HintService class.
    /// </summary>
    /// <param name="maxHints">Maximum hints allowed per game. Defaults to 5.</param>
    public HintService(int maxHints = 5)
    {
        MaxHints = maxHints;
        _hintEngine = new HintEngine(maxHints);
    }

    /// <summary>
    /// Generates a hint for the current game state asynchronously.
    /// </summary>
    /// <param name="hintType">The type of hint requested.</param>
    /// <param name="gameState">Serialized current game board state (currently unused).</param>
    /// <returns>A task resolving to hint message or empty string if no hint available.</returns>
    public Task<string> GenerateHintAsync(HintType hintType, string gameState)
    {
        // For now, return a placeholder message
        // In a real implementation, this would deserialize gameState and generate hints
        var message = hintType switch
        {
            HintType.RevealCell => "Hint: Reveal one cell",
            HintType.ShowCandidates => "Hint: Show candidates",
            HintType.EliminateCandidates => "Hint: Eliminate candidates",
            HintType.HighlightRelated => "Hint: Highlight related cells",
            HintType.SolveAll => "Hint: Solve all",
            _ => "Hint available"
        };

        return Task.FromResult(message);
    }

    /// <summary>
    /// Attempts to consume a hint, decrementing the remaining hint count.
    /// </summary>
    /// <param name="hintType">The type of hint being used (for future extension).</param>
    /// <returns>True if a hint was successfully consumed, false if no hints remain.</returns>
    public bool UseHint(HintType hintType)
    {
        return _hintEngine.UseHint();
    }

    /// <summary>
    /// Resets the hint count to the maximum.
    /// </summary>
    public void ResetHints()
    {
        _hintEngine.ResetHints();
    }

    /// <summary>
    /// Sets the maximum number of hints for the game.
    /// </summary>
    /// <param name="maxHints">The new maximum hint count.</param>
    public void SetMaxHints(int maxHints)
    {
        MaxHints = maxHints;
        _hintEngine.SetMaxHints(maxHints);
    }

    /// <summary>
    /// Gets a hint result for a specific board and hint type.
    /// This is an extension method not in the original interface.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="hintType">The type of hint to generate.</param>
    /// <returns>A HintResult containing the hint information, or null if unable to generate.</returns>
    public HintResult? GetHintResult(SudokuBoard board, HintType hintType)
    {
        return _hintEngine.GetHint(board, hintType);
    }

    /// <summary>
    /// Disposes the hint service resources.
    /// </summary>
    public void Dispose()
    {
        _hintEngine?.Dispose();
        GC.SuppressFinalize(this);
    }
}

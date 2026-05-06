using Sudoku.Core.Models;

namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for automatic session persistence.
/// Handles saving game state with debouncing to avoid excessive writes.
/// </summary>
public interface IAutoSaveService
{
    /// <summary>
    /// Asynchronously saves a game session with debouncing.
    /// If another save is requested within the debounce window, the timer resets.
    /// </summary>
    /// <param name="session">The game session to save.</param>
    /// <returns>A task that completes when the save is queued.</returns>
    Task SaveSessionAsync(GameSession session);

    /// <summary>
    /// Flushes any pending autosave immediately without waiting for debounce.
    /// Call this when the app is about to suspend or close.
    /// </summary>
    /// <returns>A task that completes when the save is finished.</returns>
    Task FlushAsync();

    /// <summary>
    /// Clears any pending autosave without writing to disk.
    /// </summary>
    void Cancel();
}

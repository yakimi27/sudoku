using Sudoku.Core.Models;

namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for app-level session lifecycle management.
/// Handles startup restoration and graceful shutdown/suspension.
/// </summary>
public interface ISessionLifecycleService
{
    /// <summary>
    /// Attempts to load a previously saved session on app startup.
    /// </summary>
    /// <returns>A task that completes with the loaded session, or null if none is saved.</returns>
    Task<GameSession?> LoadPreviousSessionAsync();

    /// <summary>
    /// Clears the saved session when the user declines to restore.
    /// </summary>
    /// <returns>A task that completes when the session is cleared.</returns>
    Task ClearSessionAsync();

    /// <summary>
    /// Flushes any pending autosave immediately before app suspension or emergency shutdown.
    /// </summary>
    /// <returns>A task that completes when the flush is finished.</returns>
    Task FlushPendingAutoSaveAsync();
}

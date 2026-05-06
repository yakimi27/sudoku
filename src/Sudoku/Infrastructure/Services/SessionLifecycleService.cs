using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Infrastructure.Repositories;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements app-level session lifecycle management.
/// Coordinates session loading, clearing, and emergency flushing with the autosave service.
/// </summary>
public class SessionLifecycleService : ISessionLifecycleService
{
    private readonly SessionRepository _sessionRepository;
    private readonly IAutoSaveService _autoSaveService;

    /// <summary>
    /// Initializes a new instance of the SessionLifecycleService class.
    /// </summary>
    /// <param name="sessionRepository">The session persistence repository.</param>
    /// <param name="autoSaveService">The autosave service for flushing pending saves.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public SessionLifecycleService(
        SessionRepository sessionRepository,
        IAutoSaveService autoSaveService)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _autoSaveService = autoSaveService ?? throw new ArgumentNullException(nameof(autoSaveService));
    }

    /// <summary>
    /// Attempts to load a previously saved session on app startup.
    /// </summary>
    /// <returns>A task that completes with the loaded session, or null if none is saved.</returns>
    public async Task<GameSession?> LoadPreviousSessionAsync()
    {
        try
        {
            return await _sessionRepository.LoadSessionAsync();
        }
        catch (Exception ex)
        {
            // Log but don't throw - app should continue even if session load fails
            System.Diagnostics.Debug.WriteLine($"Failed to load previous session: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears the saved session when the user declines to restore.
    /// </summary>
    /// <returns>A task that completes when the session is cleared.</returns>
    public async Task ClearSessionAsync()
    {
        try
        {
            await _sessionRepository.ClearSessionAsync();
        }
        catch (Exception ex)
        {
            // Log but don't throw - app should continue even if clear fails
            System.Diagnostics.Debug.WriteLine($"Failed to clear session: {ex.Message}");
        }
    }

    /// <summary>
    /// Flushes any pending autosave immediately before app suspension or emergency shutdown.
    /// </summary>
    /// <returns>A task that completes when the flush is finished.</returns>
    public async Task FlushPendingAutoSaveAsync()
    {
        try
        {
            await _autoSaveService.FlushAsync();
        }
        catch (Exception ex)
        {
            // Log but don't throw - critical autosave flush shouldn't crash app
            System.Diagnostics.Debug.WriteLine($"Failed to flush autosave during app suspension: {ex.Message}");
        }
    }
}

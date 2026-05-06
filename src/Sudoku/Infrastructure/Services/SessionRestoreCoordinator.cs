using Sudoku.Core.Models;
using Sudoku.Core.Services;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Helper service for coordinating app startup session restore flow.
/// Delegates to ISessionLifecycleService but provides a single-method interface
/// for app startup that coordinates loading and user decision handling.
/// </summary>
public class SessionRestoreCoordinator
{
    private readonly ISessionLifecycleService _lifecycleService;

    /// <summary>
    /// Delegate for presenting a restore confirmation dialog to the user.
    /// Returns true if the user chooses to restore, false otherwise.
    /// </summary>
    /// <param name="session">The session that would be restored.</param>
    /// <returns>True to restore, false to start fresh.</returns>
    public delegate Task<bool> RestoreConfirmationDelegate(GameSession session);

    /// <summary>
    /// Initializes a new instance of the SessionRestoreCoordinator class.
    /// </summary>
    /// <param name="lifecycleService">The session lifecycle service.</param>
    /// <exception cref="ArgumentNullException">Thrown when lifecycleService is null.</exception>
    public SessionRestoreCoordinator(ISessionLifecycleService lifecycleService)
    {
        _lifecycleService = lifecycleService ?? throw new ArgumentNullException(nameof(lifecycleService));
    }

    /// <summary>
    /// Attempts to load a previous session and asks the user for confirmation.
    /// If the user declines, the session is cleared.
    /// </summary>
    /// <param name="confirmationDelegate">Delegate for presenting the restore confirmation dialog.</param>
    /// <returns>
    /// The loaded GameSession if the user confirms restoration, or null if no session exists
    /// or the user declines to restore.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when confirmationDelegate is null.</exception>
    public async Task<GameSession?> TryRestoreSessionWithUserConfirmAsync(
        RestoreConfirmationDelegate confirmationDelegate)
    {
        if (confirmationDelegate == null)
            throw new ArgumentNullException(nameof(confirmationDelegate));

        // Try to load the previous session
        var session = await _lifecycleService.LoadPreviousSessionAsync();
        if (session == null)
        {
            // No session to restore
            return null;
        }

        // Ask the user if they want to restore
        bool userConfirmed = await confirmationDelegate(session);
        if (userConfirmed)
        {
            // User wants to restore
            return session;
        }

        // User declined - clear the session
        await _lifecycleService.ClearSessionAsync();
        return null;
    }

    /// <summary>
    /// Flushes any pending autosave immediately (for app suspension or shutdown).
    /// </summary>
    /// <returns>A task that completes when the flush is finished.</returns>
    public async Task FlushPendingAutoSaveAsync()
    {
        await _lifecycleService.FlushPendingAutoSaveAsync();
    }
}

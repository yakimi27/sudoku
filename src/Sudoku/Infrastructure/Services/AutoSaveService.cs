using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Storage;
using Sudoku.Infrastructure.Storage.Dtos;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements automatic session persistence with debouncing.
/// Converts game sessions to DTOs and saves them via SessionRepository.
/// Debounces rapid saves to prevent excessive disk writes (500ms default).
/// </summary>
public class AutoSaveService : IAutoSaveService
{
    /// <summary>
    /// The debounce delay in milliseconds before writing to disk.
    /// </summary>
    private const int DebounceDelayMs = 500;

    private readonly SessionRepository _sessionRepository;
    private CancellationTokenSource? _debounceCts;
    private GameSession? _pendingSession;
    private Task _pendingTask = Task.CompletedTask;

    /// <summary>
    /// Initializes a new instance of the AutoSaveService class.
    /// </summary>
    /// <param name="sessionRepository">The repository for persisting sessions.</param>
    /// <exception cref="ArgumentNullException">Thrown when sessionRepository is null.</exception>
    public AutoSaveService(SessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    /// <summary>
    /// Asynchronously saves a game session with debouncing.
    /// If another save is requested within the debounce window, the timer resets.
    /// </summary>
    /// <param name="session">The game session to save.</param>
    /// <returns>A task that completes when the save is queued.</returns>
    public async Task SaveSessionAsync(GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        _pendingSession = session;

        // Cancel the previous debounce timer
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();

        try
        {
            // Wait for debounce period or cancellation
            await Task.Delay(DebounceDelayMs, _debounceCts.Token);

            // Only save if not cancelled
            if (!_debounceCts.Token.IsCancellationRequested && _pendingSession != null)
            {
                await PerformSaveAsync(_pendingSession);
            }
        }
        catch (OperationCanceledException)
        {
            // Debounce was cancelled due to another save request - that's expected
        }
    }

    /// <summary>
    /// Flushes any pending autosave immediately without waiting for debounce.
    /// Call this when the app is about to suspend or close.
    /// </summary>
    /// <returns>A task that completes when the save is finished.</returns>
    public async Task FlushAsync()
    {
        // Cancel the pending debounce
        _debounceCts?.Cancel();

        // Save immediately if there's a pending session
        if (_pendingSession != null)
        {
            await PerformSaveAsync(_pendingSession);
        }

        // Wait for any pending save to complete
        await _pendingTask;
    }

    /// <summary>
    /// Clears any pending autosave without writing to disk.
    /// </summary>
    public void Cancel()
    {
        _debounceCts?.Cancel();
        _pendingSession = null;
    }

    /// <summary>
    /// Performs the actual save operation by converting the session to a DTO and storing it.
    /// </summary>
    private async Task PerformSaveAsync(GameSession session)
    {
        try
        {
            _pendingTask = _sessionRepository.SaveSessionAsync(session);
            await _pendingTask;
        }
        catch (Exception ex)
        {
            // Log but don't throw - autosave shouldn't crash the game
            System.Diagnostics.Debug.WriteLine($"AutoSave failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Converts a GameSession domain model to a SessionDto for persistence.
    /// Note: This method is currently unused as SessionRepository handles conversion.
    /// Kept for future extension if DTO mapping is needed separately.
    /// </summary>
    private static SessionDto ConvertToDto(GameSession session)
    {
        var dto = new SessionDto
        {
            SessionId = session.SessionId,
            GameMode = session.GameMode.ToString(),
            Difficulty = session.Difficulty.ToString(),
            ElapsedTimeMs = (long)session.ElapsedTime.TotalMilliseconds,
            HintsUsed = session.HintsUsed,
            IsCompleted = session.IsCompleted,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            LastSavedAt = DateTime.UtcNow
        };

        // Convert board cells to DTOs
        foreach (var cell in session.Board.GetAllCells())
        {
            var cellDto = new CellDto(
                row: cell.Row,
                column: cell.Column,
                value: cell.Value,
                isGiven: cell.IsGiven,
                cellState: cell.State.ToString()
            )
            {
                Candidates = cell.Notes.ToList()
            };

            dto.Cells.Add(cellDto);
        }

        // Note: Commands are not serialized in this basic implementation
        // because IGameCommand implementations may not be easily serializable.
        // A full implementation would need to serialize each command type's data.

        return dto;
    }
}

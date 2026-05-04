namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for game timing functionality.
/// Implements Singleton pattern - single instance throughout application lifecycle.
/// </summary>
public interface ITimerService
{
    /// <summary>
    /// Gets the elapsed time since the game started.
    /// </summary>
    TimeSpan ElapsedTime { get; }

    /// <summary>
    /// Gets a value indicating whether the timer is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Event raised when the elapsed time changes.
    /// </summary>
    event EventHandler<TimeSpan>? TimeChanged;

    /// <summary>
    /// Event raised when the timer reaches zero (in timed mode).
    /// </summary>
    event EventHandler? TimerExpired;

    /// <summary>
    /// Starts the timer.
    /// </summary>
    void Start();

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the timer from a paused state.
    /// </summary>
    void Resume();

    /// <summary>
    /// Stops and resets the timer to zero.
    /// </summary>
    void Stop();

    /// <summary>
    /// Sets the countdown duration for timed mode.
    /// </summary>
    /// <param name="duration">The countdown duration.</param>
    void SetCountdown(TimeSpan duration);
}

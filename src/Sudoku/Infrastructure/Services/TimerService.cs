using Sudoku.Core.Services;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements timer functionality for game timing.
/// Provides elapsed time tracking, pause/resume, and countdown mode.
/// Uses System.Timers.Timer for timing operations.
/// </summary>
public sealed class TimerService : ITimerService, IDisposable
{
    private readonly System.Timers.Timer _timer;
    private TimeSpan _elapsedTime;
    private TimeSpan _countdownDuration;
    private bool _isRunning;
    private bool _isDisposed;

    /// <summary>
    /// Gets the elapsed time since the game started.
    /// </summary>
    public TimeSpan ElapsedTime
    {
        get => _elapsedTime;
        private set => _elapsedTime = value;
    }

    /// <summary>
    /// Gets a value indicating whether the timer is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Event raised when the elapsed time changes.
    /// </summary>
    public event EventHandler<TimeSpan>? TimeChanged;

    /// <summary>
    /// Event raised when the timer reaches zero (in timed mode).
    /// </summary>
    public event EventHandler? TimerExpired;

    /// <summary>
    /// Initializes a new instance of the TimerService class.
    /// </summary>
    public TimerService()
    {
        _elapsedTime = TimeSpan.Zero;
        _countdownDuration = TimeSpan.Zero;
        _isRunning = false;
        _isDisposed = false;

        // Create timer that fires every second
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Stop();
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TimerService));

        if (!_isRunning)
        {
            _isRunning = true;
            _timer.Start();
        }
    }

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void Pause()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TimerService));

        if (_isRunning)
        {
            _isRunning = false;
            _timer.Stop();
        }
    }

    /// <summary>
    /// Resumes the timer from a paused state.
    /// </summary>
    public void Resume()
    {
        Start();
    }

    /// <summary>
    /// Stops and resets the timer to zero.
    /// </summary>
    public void Stop()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TimerService));

        _isRunning = false;
        _timer.Stop();
        _elapsedTime = TimeSpan.Zero;
        _countdownDuration = TimeSpan.Zero;
    }

    /// <summary>
    /// Sets the countdown duration for timed mode.
    /// </summary>
    /// <param name="duration">The countdown duration.</param>
    public void SetCountdown(TimeSpan duration)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TimerService));

        _countdownDuration = duration;
    }

    /// <summary>
    /// Handles the timer elapsed event.
    /// Increments elapsed time and checks for countdown expiration.
    /// </summary>
    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

        // Notify listeners of time change
        TimeChanged?.Invoke(this, _elapsedTime);

        // Check if countdown has expired
        if (_countdownDuration > TimeSpan.Zero && _elapsedTime >= _countdownDuration)
        {
            _isRunning = false;
            _timer.Stop();
            TimerExpired?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Disposes the timer service and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isRunning = false;
        _timer?.Stop();
        _timer?.Dispose();
        _isDisposed = true;
    }
}

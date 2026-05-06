using Sudoku.Core.Models;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main menu screen.
/// Manages commands for starting games, accessing statistics, settings, and profile.
/// Displays daily challenge and session continuation options.
/// </summary>
public class MenuViewModel : ObservableObject
{
    private DailyChallenge? _dailyChallenge;
    private bool _hasLastSession;

    /// <summary>
    /// Event raised when the user initiates starting a new game.
    /// </summary>
    public event EventHandler? StartGameRequested;

    /// <summary>
    /// Event raised when the user wants to continue a previous session.
    /// </summary>
    public event EventHandler? ContinueGameRequested;

    /// <summary>
    /// Event raised when the user wants to view statistics.
    /// </summary>
    public event EventHandler? StatisticsRequested;

    /// <summary>
    /// Event raised when the user wants to open settings.
    /// </summary>
    public event EventHandler? SettingsRequested;

    /// <summary>
    /// Event raised when the user wants to view their profile.
    /// </summary>
    public event EventHandler? ProfileRequested;

    /// <summary>
    /// Initializes a new instance of the MenuViewModel class.
    /// </summary>
    public MenuViewModel()
    {
        _dailyChallenge = null;
        _hasLastSession = false;

        StartGameCommand = new RelayCommand(_ => OnStartGame());
        ContinueGameCommand = new RelayCommand(_ => OnContinueGame(), _ => HasLastSession);
        ViewStatisticsCommand = new RelayCommand(_ => OnStatisticsRequested());
        OpenSettingsCommand = new RelayCommand(_ => OnSettingsRequested());
        OpenProfileCommand = new RelayCommand(_ => OnProfileRequested());
    }

    /// <summary>
    /// Gets or sets the daily challenge information.
    /// </summary>
    public DailyChallenge? DailyChallenge
    {
        get => _dailyChallenge;
        set => SetProperty(ref _dailyChallenge, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a previous session is available to continue.
    /// </summary>
    public bool HasLastSession
    {
        get => _hasLastSession;
        set
        {
            if (SetProperty(ref _hasLastSession, value))
            {
                // Update CanExecute for ContinueGameCommand
                (ContinueGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets the command to start a new game.
    /// </summary>
    public RelayCommand StartGameCommand { get; }

    /// <summary>
    /// Gets the command to continue the last game session.
    /// </summary>
    public RelayCommand ContinueGameCommand { get; }

    /// <summary>
    /// Gets the command to view game statistics.
    /// </summary>
    public RelayCommand ViewStatisticsCommand { get; }

    /// <summary>
    /// Gets the command to open application settings.
    /// </summary>
    public RelayCommand OpenSettingsCommand { get; }

    /// <summary>
    /// Gets the command to open the player profile.
    /// </summary>
    public RelayCommand OpenProfileCommand { get; }

    /// <summary>
    /// Loads daily challenge information.
    /// </summary>
    /// <param name="dailyChallenge">The daily challenge for today.</param>
    /// <exception cref="ArgumentNullException">Thrown when dailyChallenge is null.</exception>
    public void LoadDailyChallenge(DailyChallenge dailyChallenge)
    {
        if (dailyChallenge == null)
            throw new ArgumentNullException(nameof(dailyChallenge));

        DailyChallenge = dailyChallenge;
    }

    /// <summary>
    /// Loads information about whether a previous session exists.
    /// </summary>
    /// <param name="hasSession">True if a session can be continued, false otherwise.</param>
    public void LoadSessionInfo(bool hasSession)
    {
        HasLastSession = hasSession;
    }

    /// <summary>
    /// Raises the StartGameRequested event.
    /// </summary>
    private void OnStartGame()
    {
        StartGameRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the ContinueGameRequested event.
    /// </summary>
    private void OnContinueGame()
    {
        ContinueGameRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the StatisticsRequested event.
    /// </summary>
    private void OnStatisticsRequested()
    {
        StatisticsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the SettingsRequested event.
    /// </summary>
    private void OnSettingsRequested()
    {
        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the ProfileRequested event.
    /// </summary>
    private void OnProfileRequested()
    {
        ProfileRequested?.Invoke(this, EventArgs.Empty);
    }
}

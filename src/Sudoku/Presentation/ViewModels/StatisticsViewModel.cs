using Sudoku.Core.Services;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying player statistics.
/// Loads and displays game statistics grouped by mode and difficulty.
/// </summary>
public class StatisticsViewModel : ObservableObject
{
    private readonly IStatisticsService _statisticsService;
    private string _statisticsData;
    private int _totalGamesCompleted;
    private bool _isLoading;
    private bool _showResetConfirmation;

    /// <summary>
    /// Event raised when statistics are loaded successfully.
    /// </summary>
    public event EventHandler? StatisticsLoaded;

    /// <summary>
    /// Event raised when statistics are reset successfully.
    /// </summary>
    public event EventHandler? StatisticsReset;

    /// <summary>
    /// Initializes a new instance of the StatisticsViewModel class.
    /// </summary>
    /// <param name="statisticsService">The statistics service for loading statistics data.</param>
    /// <exception cref="ArgumentNullException">Thrown when statisticsService is null.</exception>
    public StatisticsViewModel(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _statisticsData = string.Empty;
        _totalGamesCompleted = 0;
        _isLoading = false;
        _showResetConfirmation = false;

        LoadStatisticsCommand = new RelayCommand(_ => LoadStatisticsAsync());
        ResetStatisticsCommand = new RelayCommand(_ => ResetStatisticsAsync(), _ => !IsLoading);
        ConfirmResetCommand = new RelayCommand(_ => ConfirmResetAsync());
        CancelResetCommand = new RelayCommand(_ => CancelReset());
    }

    /// <summary>
    /// Gets the statistics data as a formatted string.
    /// </summary>
    public string StatisticsData
    {
        get => _statisticsData;
        private set => SetProperty(ref _statisticsData, value);
    }

    /// <summary>
    /// Gets the total number of games completed.
    /// </summary>
    public int TotalGamesCompleted
    {
        get => _totalGamesCompleted;
        private set => SetProperty(ref _totalGamesCompleted, value);
    }

    /// <summary>
    /// Gets a value indicating whether statistics are currently being loaded.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                (ResetStatisticsCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the reset confirmation dialog is shown.
    /// </summary>
    public bool ShowResetConfirmation
    {
        get => _showResetConfirmation;
        set => SetProperty(ref _showResetConfirmation, value);
    }

    /// <summary>
    /// Gets the command to load statistics.
    /// </summary>
    public RelayCommand LoadStatisticsCommand { get; }

    /// <summary>
    /// Gets the command to reset statistics.
    /// </summary>
    public RelayCommand ResetStatisticsCommand { get; }

    /// <summary>
    /// Gets the command to confirm reset after showing confirmation dialog.
    /// </summary>
    public RelayCommand ConfirmResetCommand { get; }

    /// <summary>
    /// Gets the command to cancel reset operation.
    /// </summary>
    public RelayCommand CancelResetCommand { get; }

    /// <summary>
    /// Loads statistics asynchronously.
    /// </summary>
    private async void LoadStatisticsAsync()
    {
        IsLoading = true;

        try
        {
            var allStats = await _statisticsService.GetAllStatisticsAsync();
            StatisticsData = allStats;

            var totalCompleted = await _statisticsService.GetTotalGamesCompletedAsync();
            TotalGamesCompleted = totalCompleted;

            StatisticsLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            // Log or handle load error
            StatisticsData = "Error loading statistics.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Displays the reset confirmation dialog.
    /// </summary>
    private void ResetStatisticsAsync()
    {
        ShowResetConfirmation = true;
    }

    /// <summary>
    /// Confirms and executes the reset statistics operation.
    /// </summary>
    private async void ConfirmResetAsync()
    {
        ShowResetConfirmation = false;
        IsLoading = true;

        try
        {
            await _statisticsService.ClearStatisticsAsync();
            TotalGamesCompleted = 0;
            StatisticsData = "Statistics have been reset.";
            StatisticsReset?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            // Log or handle reset error
            StatisticsData = "Error resetting statistics.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Cancels the reset operation.
    /// </summary>
    private void CancelReset()
    {
        ShowResetConfirmation = false;
    }
}

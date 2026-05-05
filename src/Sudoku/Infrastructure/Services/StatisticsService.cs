using System.Text.Json;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Infrastructure.Repositories;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements statistics tracking and recording for completed games.
/// Provides aggregated game statistics and stores them persistently.
/// </summary>
public sealed class StatisticsService : IStatisticsService
{
    private readonly StatisticsRepository _repository;
    private Statistics _statistics;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the StatisticsService class.
    /// </summary>
    /// <param name="repository">The repository for persisting statistics.</param>
    /// <exception cref="ArgumentNullException">Thrown when repository is null.</exception>
    public StatisticsService(StatisticsRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _statistics = new Statistics();
        _isInitialized = false;
    }

    /// <summary>
    /// Initializes the service by loading statistics from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        _statistics = await _repository.LoadStatisticsAsync();
        _isInitialized = true;
    }

    /// <summary>
    /// Records a completed game with its difficulty and elapsed time.
    /// Updates statistics atomically.
    /// </summary>
    /// <param name="difficulty">The difficulty level of the completed game.</param>
    /// <param name="elapsedTime">Time taken to complete the game.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordGameCompletionAsync(Difficulty difficulty, TimeSpan elapsedTime)
    {
        await InitializeAsync();

        // For now, record against Standard mode
        // In future, this should include the game mode parameter
        var stats = _statistics.GetStats(GameMode.Standard, difficulty);
        stats.RecordGame(elapsedTime, wasCompleted: true);

        await _repository.SaveStatisticsAsync(_statistics);
    }

    /// <summary>
    /// Gets the total number of games completed.
    /// </summary>
    /// <returns>Total game completions count.</returns>
    public async Task<int> GetTotalGamesCompletedAsync()
    {
        await InitializeAsync();

        int total = 0;
        foreach (var gameMode in Enum.GetValues<GameMode>())
        {
            foreach (var difficulty in Enum.GetValues<Difficulty>())
            {
                total += _statistics.GetStats(gameMode, difficulty).GamesCompleted;
            }
        }

        return total;
    }

    /// <summary>
    /// Gets the average completion time for a specific difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty level to query.</param>
    /// <returns>Average time span for completions, or TimeSpan.Zero if no games completed.</returns>
    public async Task<TimeSpan> GetAverageCompletionTimeAsync(Difficulty difficulty)
    {
        await InitializeAsync();

        TimeSpan totalTime = TimeSpan.Zero;
        int totalCompleted = 0;

        foreach (var gameMode in Enum.GetValues<GameMode>())
        {
            var stats = _statistics.GetStats(gameMode, difficulty);
            if (stats.AverageTime.HasValue)
            {
                totalTime += stats.AverageTime.Value;
                totalCompleted++;
            }
        }

        if (totalCompleted == 0)
            return TimeSpan.Zero;

        return TimeSpan.FromSeconds(totalTime.TotalSeconds / totalCompleted);
    }

    /// <summary>
    /// Gets all statistics data as JSON string.
    /// </summary>
    /// <returns>Serialized statistics data.</returns>
    public async Task<string> GetAllStatisticsAsync()
    {
        await InitializeAsync();

        return JsonSerializer.Serialize(_statistics, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Clears all recorded statistics.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ClearStatisticsAsync()
    {
        await InitializeAsync();

        _statistics = new Statistics();
        await _repository.SaveStatisticsAsync(_statistics);
    }
}

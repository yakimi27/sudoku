namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for tracking and managing player statistics.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Records a completed game.
    /// </summary>
    /// <param name="difficulty">The difficulty level of the completed game.</param>
    /// <param name="elapsedTime">Time taken to complete the game.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordGameCompletionAsync(Enums.Difficulty difficulty, TimeSpan elapsedTime);

    /// <summary>
    /// Gets the total number of games completed.
    /// </summary>
    /// <returns>Total game completions count.</returns>
    Task<int> GetTotalGamesCompletedAsync();

    /// <summary>
    /// Gets the average completion time for a specific difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty level to query.</param>
    /// <returns>Average time span for completions, or TimeSpan.Zero if no games completed.</returns>
    Task<TimeSpan> GetAverageCompletionTimeAsync(Enums.Difficulty difficulty);

    /// <summary>
    /// Gets all statistics data.
    /// </summary>
    /// <returns>Serialized statistics data.</returns>
    Task<string> GetAllStatisticsAsync();

    /// <summary>
    /// Clears all recorded statistics.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearStatisticsAsync();
}

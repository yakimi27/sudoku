using Sudoku.Core.Models;

namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for daily challenge management.
/// Handles getting today's challenge, marking completion, and tracking streaks.
/// </summary>
public interface IDailyChallengeService
{
    /// <summary>
    /// Gets today's daily challenge asynchronously.
    /// </summary>
    /// <returns>A task that resolves to today's DailyChallenge.</returns>
    Task<DailyChallenge> GetTodaysChallengeAsync();

    /// <summary>
    /// Marks today's challenge as completed with the given time.
    /// </summary>
    /// <param name="completionTime">The time taken to complete the challenge.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkCompletedAsync(TimeSpan completionTime);

    /// <summary>
    /// Gets the current completion streak (consecutive days completed).
    /// </summary>
    /// <returns>The streak count as an integer.</returns>
    int GetStreak();

    /// <summary>
    /// Gets the completion status for a specific date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the challenge for that date was completed, false otherwise.</returns>
    Task<bool> IsCompletedAsync(DateTime date);
}

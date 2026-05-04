namespace Sudoku.Core.Models;

/// <summary>
/// Represents a daily challenge puzzle that resets every day.
/// Uses a date-based seed to ensure the same puzzle appears for all players on the same day.
/// </summary>
public class DailyChallenge
{
    /// <summary>
    /// The number of days since January 1, 2024, used to generate unique puzzles.
    /// </summary>
    private const int SeedEpoch = 19723; // January 1, 2024

    /// <summary>
    /// Gets the date of this daily challenge.
    /// </summary>
    public DateTime ChallengeDate { get; }

    /// <summary>
    /// Gets the random seed derived from the challenge date.
    /// This ensures the same puzzle is generated for all players on the same day.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets a value indicating whether this challenge has been completed today.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the completion time, or null if not yet completed.
    /// </summary>
    public TimeSpan? CompletionTime { get; private set; }

    /// <summary>
    /// Gets the date and time when the challenge was completed, or null if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier for this challenge.
    /// </summary>
    public string ChallengeId { get; }

    /// <summary>
    /// Initializes a new instance of the DailyChallenge class for the specified date.
    /// </summary>
    /// <param name="challengeDate">The date of the challenge. If not provided, uses today.</param>
    public DailyChallenge(DateTime? challengeDate = null)
    {
        ChallengeDate = challengeDate?.Date ?? DateTime.UtcNow.Date;
        Seed = GenerateSeed(ChallengeDate);
        IsCompleted = false;
        CompletionTime = null;
        CompletedAt = null;
        ChallengeId = $"daily_{ChallengeDate:yyyy-MM-dd}";
    }

    /// <summary>
    /// Marks the challenge as completed.
    /// </summary>
    /// <param name="elapsedTime">The time taken to complete the challenge.</param>
    public void MarkCompleted(TimeSpan elapsedTime)
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletionTime = elapsedTime;
            CompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Generates a seed value based on the given date.
    /// The seed is consistent for the same date, ensuring all players get the same puzzle.
    /// </summary>
    /// <param name="date">The date to generate a seed for.</param>
    /// <returns>The seed value for the date.</returns>
    private static int GenerateSeed(DateTime date)
    {
        // Calculate days since epoch (January 1, 2024)
        int daysSinceEpoch = (int)(date.Date - new DateTime(2024, 1, 1)).TotalDays;

        // Use a hash of the days to create a unique seed
        // This ensures the puzzle is deterministic and repeatable
        unchecked
        {
            int seed = daysSinceEpoch * 397 ^ (daysSinceEpoch >> 16);
            return Math.Abs(seed);
        }
    }

    /// <summary>
    /// Gets the daily challenge for today.
    /// </summary>
    /// <returns>A DailyChallenge instance for today's date.</returns>
    public static DailyChallenge GetTodayChallenge()
    {
        return new DailyChallenge(DateTime.UtcNow.Date);
    }

    /// <summary>
    /// Determines if a daily challenge is from today.
    /// </summary>
    /// <param name="challenge">The challenge to check.</param>
    /// <returns>True if the challenge is from today, false otherwise.</returns>
    public static bool IsToday(DailyChallenge challenge)
    {
        return challenge.ChallengeDate.Date == DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Gets the time remaining until the next daily challenge (next midnight UTC).
    /// </summary>
    /// <returns>The time remaining until the next challenge.</returns>
    public static TimeSpan GetTimeUntilNextChallenge()
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        return nextMidnight - now;
    }

    /// <summary>
    /// Gets a summary of the daily challenge.
    /// </summary>
    /// <returns>A formatted string describing the challenge.</returns>
    public string GetSummary()
    {
        return IsCompleted
            ? $"Daily Challenge ({ChallengeDate:yyyy-MM-dd}) - Completed in {CompletionTime:mm\\:ss}"
            : $"Daily Challenge ({ChallengeDate:yyyy-MM-dd}) - Not completed";
    }
}

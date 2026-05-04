using Sudoku.Core.Enums;

namespace Sudoku.Core.Models;

/// <summary>
/// An immutable record of a completed game.
/// Represents a snapshot of game history for persistence and analysis.
/// Uses C# 12 record type for value equality and immutability.
/// </summary>
public sealed record GameRecord(
    string RecordId,
    GameMode GameMode,
    Difficulty Difficulty,
    TimeSpan CompletionTime,
    int HintsUsed,
    DateTime CompletedAt,
    string PlayerName,
    int Score
)
{
    /// <summary>
    /// Creates a new game record from a completed game session.
    /// </summary>
    /// <param name="session">The completed game session.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="score">The score earned for the completion.</param>
    /// <returns>A new GameRecord instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when session or playerName is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the session is not completed.</exception>
    public static GameRecord CreateFromSession(GameSession session, string playerName, int score)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));
        if (string.IsNullOrWhiteSpace(playerName))
            throw new ArgumentException("Player name cannot be empty.", nameof(playerName));
        if (!session.IsCompleted)
            throw new ArgumentException("Cannot create a record from an incomplete session.", nameof(session));

        return new GameRecord(
            RecordId: Guid.NewGuid().ToString(),
            GameMode: session.GameMode,
            Difficulty: session.Difficulty,
            CompletionTime: session.ElapsedTime,
            HintsUsed: session.HintsUsed,
            CompletedAt: session.CompletedAt ?? DateTime.UtcNow,
            PlayerName: playerName,
            Score: score
        );
    }

    /// <summary>
    /// Gets a description of the game record for display purposes.
    /// </summary>
    /// <returns>A formatted string describing the game record.</returns>
    public string GetDescription()
    {
        return $"{GameMode} ({Difficulty}) - {CompletionTime:mm\\:ss} - Score: {Score}";
    }

    /// <summary>
    /// Calculates the score based on difficulty and completion time.
    /// Higher scores for faster completions on harder difficulties.
    /// </summary>
    /// <param name="difficulty">The game difficulty.</param>
    /// <param name="completionTime">The time taken to complete.</param>
    /// <returns>The calculated score.</returns>
    public static int CalculateScore(Difficulty difficulty, TimeSpan completionTime)
    {
        int baseScore = difficulty switch
        {
            Difficulty.Easy => 100,
            Difficulty.Medium => 250,
            Difficulty.Hard => 500,
            Difficulty.Expert => 1000,
            _ => 0
        };

        // Time bonus: reduce score slightly for each minute over base time
        int baseTimeMinutes = difficulty switch
        {
            Difficulty.Easy => 5,
            Difficulty.Medium => 10,
            Difficulty.Hard => 20,
            Difficulty.Expert => 30,
            _ => 0
        };

        int excessMinutes = Math.Max(0, (int)completionTime.TotalMinutes - baseTimeMinutes);
        int penalty = excessMinutes * 10;

        return Math.Max(0, baseScore - penalty);
    }
}

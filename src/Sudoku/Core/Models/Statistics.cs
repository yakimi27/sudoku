using Sudoku.Core.Enums;

namespace Sudoku.Core.Models;

/// <summary>
/// Represents statistics for a specific game mode and difficulty combination.
/// </summary>
public class GameModeStats
{
    /// <summary>
    /// Gets or sets the total number of games played.
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Gets or sets the total number of games completed successfully.
    /// </summary>
    public int GamesCompleted { get; set; }

    /// <summary>
    /// Gets or sets the best (fastest) completion time.
    /// </summary>
    public TimeSpan? BestTime { get; set; }

    /// <summary>
    /// Gets or sets the average completion time across all completed games.
    /// </summary>
    public TimeSpan? AverageTime { get; set; }

    /// <summary>
    /// Gets or sets the total time spent playing.
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// Gets the win rate as a percentage (0-100).
    /// </summary>
    public double WinRate => GamesPlayed > 0 ? (GamesCompleted * 100.0) / GamesPlayed : 0.0;

    /// <summary>
    /// Records a game result (whether completed or not).
    /// </summary>
    /// <param name="elapsedTime">The time spent on the game.</param>
    /// <param name="wasCompleted">Whether the game was completed.</param>
    public void RecordGame(TimeSpan elapsedTime, bool wasCompleted)
    {
        GamesPlayed++;
        TotalTime += elapsedTime;

        if (wasCompleted)
        {
            GamesCompleted++;

            // Update best time
            if (BestTime == null || elapsedTime < BestTime)
            {
                BestTime = elapsedTime;
            }

            // Update average time
            AverageTime = TimeSpan.FromSeconds(TotalTime.TotalSeconds / GamesCompleted);
        }
    }

    /// <summary>
    /// Resets all statistics.
    /// </summary>
    public void Reset()
    {
        GamesPlayed = 0;
        GamesCompleted = 0;
        BestTime = null;
        AverageTime = null;
        TotalTime = TimeSpan.Zero;
    }
}

/// <summary>
/// Represents comprehensive game statistics across all game modes and difficulties.
/// </summary>
public class Statistics
{
    private readonly Dictionary<(GameMode, Difficulty), GameModeStats> _stats;

    /// <summary>
    /// Initializes a new instance of the Statistics class.
    /// </summary>
    public Statistics()
    {
        _stats = new Dictionary<(GameMode, Difficulty), GameModeStats>();
        InitializeAllStats();
    }

    /// <summary>
    /// Gets statistics for a specific game mode and difficulty.
    /// </summary>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="difficulty">The difficulty level.</param>
    /// <returns>The GameModeStats for the specified combination.</returns>
    public GameModeStats GetStats(GameMode gameMode, Difficulty difficulty)
    {
        var key = (gameMode, difficulty);
        if (!_stats.ContainsKey(key))
        {
            _stats[key] = new GameModeStats();
        }
        return _stats[key];
    }

    /// <summary>
    /// Records a completed game in the statistics.
    /// </summary>
    /// <param name="gameMode">The game mode played.</param>
    /// <param name="difficulty">The difficulty level.</param>
    /// <param name="elapsedTime">The time taken to complete.</param>
    /// <param name="wasCompleted">Whether the game was successfully completed.</param>
    public void RecordGame(GameMode gameMode, Difficulty difficulty, TimeSpan elapsedTime, bool wasCompleted)
    {
        var stats = GetStats(gameMode, difficulty);
        stats.RecordGame(elapsedTime, wasCompleted);
    }

    /// <summary>
    /// Gets total games played across all modes and difficulties.
    /// </summary>
    /// <returns>The total game count.</returns>
    public int GetTotalGamesPlayed()
    {
        return _stats.Values.Sum(s => s.GamesPlayed);
    }

    /// <summary>
    /// Gets total games completed across all modes and difficulties.
    /// </summary>
    /// <returns>The total completed game count.</returns>
    public int GetTotalGamesCompleted()
    {
        return _stats.Values.Sum(s => s.GamesCompleted);
    }

    /// <summary>
    /// Gets the overall win rate across all games.
    /// </summary>
    /// <returns>Win rate as a percentage (0-100).</returns>
    public double GetOverallWinRate()
    {
        int totalPlayed = GetTotalGamesPlayed();
        if (totalPlayed == 0)
            return 0.0;

        int totalCompleted = GetTotalGamesCompleted();
        return (totalCompleted * 100.0) / totalPlayed;
    }

    /// <summary>
    /// Gets the total time spent playing across all games.
    /// </summary>
    /// <returns>The total time span.</returns>
    public TimeSpan GetTotalTimeSpent()
    {
        return TimeSpan.FromSeconds(_stats.Values.Sum(s => s.TotalTime.TotalSeconds));
    }

    /// <summary>
    /// Gets the personal best (fastest) completion time for a specific mode and difficulty.
    /// </summary>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="difficulty">The difficulty level.</param>
    /// <returns>The best time, or null if no completed games exist.</returns>
    public TimeSpan? GetBestTime(GameMode gameMode, Difficulty difficulty)
    {
        return GetStats(gameMode, difficulty).BestTime;
    }

    /// <summary>
    /// Gets the average completion time for a specific mode and difficulty.
    /// </summary>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="difficulty">The difficulty level.</param>
    /// <returns>The average time, or null if no completed games exist.</returns>
    public TimeSpan? GetAverageTime(GameMode gameMode, Difficulty difficulty)
    {
        return GetStats(gameMode, difficulty).AverageTime;
    }

    /// <summary>
    /// Gets a dictionary of all statistics for serialization purposes.
    /// </summary>
    /// <returns>A dictionary of (GameMode, Difficulty) to GameModeStats.</returns>
    public IReadOnlyDictionary<(GameMode, Difficulty), GameModeStats> GetAllStats()
    {
        return _stats.AsReadOnly();
    }

    /// <summary>
    /// Resets all statistics.
    /// </summary>
    public void ResetAll()
    {
        foreach (var stats in _stats.Values)
        {
            stats.Reset();
        }
    }

    /// <summary>
    /// Initializes the statistics dictionary with entries for all mode/difficulty combinations.
    /// </summary>
    private void InitializeAllStats()
    {
        foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
        {
            foreach (Difficulty difficulty in Enum.GetValues(typeof(Difficulty)))
            {
                _stats[(gameMode, difficulty)] = new GameModeStats();
            }
        }
    }
}

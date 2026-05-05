using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the Statistics model.
/// </summary>
public class StatisticsTests
{
    [Fact]
    public void Statistics_Initialized_HasAllGameModeDifficultyCombinations()
    {
        // Act
        var stats = new Statistics();

        // Assert
        var allStats = stats.GetAllStats();
        // 4 game modes * 4 difficulties = 16 combinations
        Assert.Equal(16, allStats.Count);
    }

    [Fact]
    public void RecordGame_IncrementGamesPlayed()
    {
        // Arrange
        var stats = new Statistics();

        // Act
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(300), wasCompleted: false);

        // Assert
        var easyStats = stats.GetStats(GameMode.Standard, Difficulty.Easy);
        Assert.Equal(1, easyStats.GamesPlayed);
        Assert.Equal(0, easyStats.GamesCompleted);
    }

    [Fact]
    public void RecordGame_Completed_IncrementsBothCounters()
    {
        // Arrange
        var stats = new Statistics();

        // Act
        stats.RecordGame(GameMode.Standard, Difficulty.Medium, TimeSpan.FromSeconds(600), wasCompleted: true);

        // Assert
        var mediumStats = stats.GetStats(GameMode.Standard, Difficulty.Medium);
        Assert.Equal(1, mediumStats.GamesPlayed);
        Assert.Equal(1, mediumStats.GamesCompleted);
    }

    [Fact]
    public void RecordGame_UpdatesBestTime()
    {
        // Arrange
        var stats = new Statistics();
        var time1 = TimeSpan.FromSeconds(300);
        var time2 = TimeSpan.FromSeconds(250);
        var time3 = TimeSpan.FromSeconds(400);

        // Act
        stats.RecordGame(GameMode.Standard, Difficulty.Hard, time1, wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Hard, time2, wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Hard, time3, wasCompleted: true);

        // Assert
        var hardStats = stats.GetStats(GameMode.Standard, Difficulty.Hard);
        Assert.Equal(time2, hardStats.BestTime);
    }

    [Fact]
    public void RecordGame_CalculatesAverageTime()
    {
        // Arrange
        var stats = new Statistics();
        var time1 = TimeSpan.FromSeconds(300);
        var time2 = TimeSpan.FromSeconds(400);

        // Act
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, time1, wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, time2, wasCompleted: true);

        // Assert
        var easyStats = stats.GetStats(GameMode.Standard, Difficulty.Easy);
        var expectedAverage = TimeSpan.FromSeconds(350);
        Assert.Equal(expectedAverage, easyStats.AverageTime);
    }

    [Fact]
    public void GetTotalGamesPlayed_SumsAllModes()
    {
        // Arrange
        var stats = new Statistics();
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(100), wasCompleted: false);
        stats.RecordGame(GameMode.Thermometer, Difficulty.Medium, TimeSpan.FromSeconds(200), wasCompleted: false);
        stats.RecordGame(GameMode.Killer, Difficulty.Hard, TimeSpan.FromSeconds(300), wasCompleted: false);

        // Act
        var total = stats.GetTotalGamesPlayed();

        // Assert
        Assert.Equal(3, total);
    }

    [Fact]
    public void GetTotalGamesCompleted_SumsCompletedOnly()
    {
        // Arrange
        var stats = new Statistics();
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Medium, TimeSpan.FromSeconds(200), wasCompleted: false);
        stats.RecordGame(GameMode.Standard, Difficulty.Hard, TimeSpan.FromSeconds(300), wasCompleted: true);

        // Act
        var completed = stats.GetTotalGamesCompleted();

        // Assert
        Assert.Equal(2, completed);
    }

    [Fact]
    public void GetOverallWinRate_CalculatesCorrectly()
    {
        // Arrange
        var stats = new Statistics();
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(150), wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(200), wasCompleted: false);

        // Act
        var winRate = stats.GetOverallWinRate();

        // Assert
        Assert.InRange(winRate, 66.6, 66.7);
    }

    [Fact]
    public void GetTotalTimeSpent_SumsAllTimes()
    {
        // Arrange
        var stats = new Statistics();
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(200), wasCompleted: false);
        stats.RecordGame(GameMode.Thermometer, Difficulty.Medium, TimeSpan.FromSeconds(150), wasCompleted: true);

        // Act
        var totalTime = stats.GetTotalTimeSpent();

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(450), totalTime);
    }

    [Fact]
    public void GameModeStats_WinRate_Calculated()
    {
        // Arrange
        var stats = new GameModeStats();

        // Act
        stats.RecordGame(TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(TimeSpan.FromSeconds(200), wasCompleted: true);
        stats.RecordGame(TimeSpan.FromSeconds(300), wasCompleted: false);

        // Assert
        Assert.InRange(stats.WinRate, 66.6, 66.7);
    }

    [Fact]
    public void GameModeStats_Reset_ClearsAllData()
    {
        // Arrange
        var stats = new GameModeStats();
        stats.RecordGame(TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(TimeSpan.FromSeconds(200), wasCompleted: true);

        // Act
        stats.Reset();

        // Assert
        Assert.Equal(0, stats.GamesPlayed);
        Assert.Equal(0, stats.GamesCompleted);
        Assert.Null(stats.BestTime);
        Assert.Null(stats.AverageTime);
        Assert.Equal(TimeSpan.Zero, stats.TotalTime);
    }

    [Fact]
    public void ResetAll_ClearsAllStatistics()
    {
        // Arrange
        var stats = new Statistics();
        stats.RecordGame(GameMode.Standard, Difficulty.Easy, TimeSpan.FromSeconds(100), wasCompleted: true);
        stats.RecordGame(GameMode.Killer, Difficulty.Hard, TimeSpan.FromSeconds(500), wasCompleted: true);

        // Act
        stats.ResetAll();

        // Assert
        Assert.Equal(0, stats.GetTotalGamesPlayed());
        Assert.Equal(0, stats.GetTotalGamesCompleted());
    }

    [Fact]
    public void GetStats_NonExistentCombination_ReturnsEmptyStats()
    {
        // Arrange
        var stats = new Statistics();

        // Act
        var hardStats = stats.GetStats(GameMode.Mini, Difficulty.Expert);

        // Assert
        Assert.Equal(0, hardStats.GamesPlayed);
        Assert.Null(hardStats.BestTime);
    }
}

using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the GameRecord immutable record type.
/// </summary>
public class GameRecordTests
{
    [Fact]
    public void GameRecord_WithValidArguments_CreatesSuccessfully()
    {
        // Act
        var record = new GameRecord(
            RecordId: "test-001",
            GameMode: GameMode.Standard,
            Difficulty: Difficulty.Medium,
            CompletionTime: TimeSpan.FromSeconds(300),
            HintsUsed: 2,
            CompletedAt: DateTime.UtcNow,
            PlayerName: "TestPlayer",
            Score: 500
        );

        // Assert
        Assert.Equal("test-001", record.RecordId);
        Assert.Equal(GameMode.Standard, record.GameMode);
        Assert.Equal(Difficulty.Medium, record.Difficulty);
        Assert.Equal(2, record.HintsUsed);
        Assert.Equal("TestPlayer", record.PlayerName);
        Assert.Equal(500, record.Score);
    }

    [Fact]
    public void CreateFromSession_CreatesRecordSuccessfully()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Hard);
        session.UpdateElapsedTime(TimeSpan.FromSeconds(450));
        session.UseHint();
        session.UseHint();
        session.Complete();

        // Act
        var record = GameRecord.CreateFromSession(session, "TestPlayer", 750);

        // Assert
        Assert.NotNull(record.RecordId);
        Assert.Equal(GameMode.Standard, record.GameMode);
        Assert.Equal(Difficulty.Hard, record.Difficulty);
        Assert.Equal(TimeSpan.FromSeconds(450), record.CompletionTime);
        Assert.Equal(2, record.HintsUsed);
        Assert.Equal("TestPlayer", record.PlayerName);
        Assert.Equal(750, record.Score);
    }

    [Fact]
    public void CreateFromSession_IncompleteSession_ThrowsException()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            GameRecord.CreateFromSession(session, "TestPlayer", 100));
    }

    [Fact]
    public void CreateFromSession_NullSession_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            GameRecord.CreateFromSession(null!, "TestPlayer", 100));
    }

    [Fact]
    public void CreateFromSession_EmptyPlayerName_ThrowsException()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        session.Complete();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            GameRecord.CreateFromSession(session, string.Empty, 100));
    }

    [Fact]
    public void GameRecord_Records_AreEqualWhenValuesEqual()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var record1 = new GameRecord("id-1", GameMode.Standard, Difficulty.Easy, 
            TimeSpan.FromSeconds(300), 1, now, "Player", 100);
        var record2 = new GameRecord("id-1", GameMode.Standard, Difficulty.Easy, 
            TimeSpan.FromSeconds(300), 1, now, "Player", 100);

        // Act & Assert
        Assert.Equal(record1, record2);
    }

    [Fact]
    public void GameRecord_Records_AreNotEqualWhenDifferent()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var record1 = new GameRecord("id-1", GameMode.Standard, Difficulty.Easy, 
            TimeSpan.FromSeconds(300), 1, now, "Player1", 100);
        var record2 = new GameRecord("id-2", GameMode.Standard, Difficulty.Easy, 
            TimeSpan.FromSeconds(300), 1, now, "Player2", 100);

        // Act & Assert
        Assert.NotEqual(record1, record2);
    }

    [Fact]
    public void GetDescription_FormatsCorrectly()
    {
        // Arrange
        var record = new GameRecord("id-1", GameMode.Standard, Difficulty.Hard,
            TimeSpan.FromSeconds(600), 3, DateTime.UtcNow, "TestPlayer", 450);

        // Act
        var description = record.GetDescription();

        // Assert
        Assert.Contains("Standard", description);
        Assert.Contains("Hard", description);
        Assert.Contains("10:00", description); // 600 seconds = 10 minutes
        Assert.Contains("450", description);
    }

    [Fact]
    public void CalculateScore_Easy_ReturnsBaseScore()
    {
        // Act
        var score = GameRecord.CalculateScore(Difficulty.Easy, TimeSpan.FromSeconds(300));

        // Assert
        Assert.Equal(100, score);
    }

    [Fact]
    public void CalculateScore_Medium_ReturnsHigherScore()
    {
        // Act
        var score = GameRecord.CalculateScore(Difficulty.Medium, TimeSpan.FromSeconds(600));

        // Assert
        Assert.Equal(250, score);
    }

    [Fact]
    public void CalculateScore_Hard_ReturnsEvenHigherScore()
    {
        // Act
        var score = GameRecord.CalculateScore(Difficulty.Hard, TimeSpan.FromSeconds(1200));

        // Assert
        Assert.Equal(500, score);
    }

    [Fact]
    public void CalculateScore_Expert_ReturnsMaxScore()
    {
        // Act
        var score = GameRecord.CalculateScore(Difficulty.Expert, TimeSpan.FromSeconds(1800));

        // Assert
        Assert.Equal(1000, score);
    }

    [Fact]
    public void CalculateScore_ExcessTime_ReducesScore()
    {
        // Act
        var scoreOnTime = GameRecord.CalculateScore(Difficulty.Medium, TimeSpan.FromSeconds(600)); // 10 min
        var scoreWithExcess = GameRecord.CalculateScore(Difficulty.Medium, TimeSpan.FromSeconds(1200)); // 20 min, 10 min excess

        // Assert
        Assert.True(scoreOnTime > scoreWithExcess);
    }

    [Fact]
    public void CalculateScore_NegativeScore_ReturnsZero()
    {
        // Act
        var score = GameRecord.CalculateScore(Difficulty.Easy, TimeSpan.FromSeconds(3600)); // 1 hour

        // Assert
        Assert.Equal(0, score);
    }
}

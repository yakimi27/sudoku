using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the GameSession model.
/// </summary>
public class GameSessionTests
{
    private SudokuBoard CreateTestBoard()
    {
        return SudokuBoard.CreateEmpty();
    }

    [Fact]
    public void GameSession_WithValidArguments_CreatesSuccessfully()
    {
        // Arrange
        var board = CreateTestBoard();

        // Act
        var session = new GameSession(board, GameMode.Standard, Difficulty.Medium);

        // Assert
        Assert.Equal(board, session.Board);
        Assert.Equal(GameMode.Standard, session.GameMode);
        Assert.Equal(Difficulty.Medium, session.Difficulty);
        Assert.Equal(TimeSpan.Zero, session.ElapsedTime);
        Assert.Equal(0, session.HintsUsed);
        Assert.False(session.IsCompleted);
        Assert.Null(session.CompletedAt);
    }

    [Fact]
    public void GameSession_GeneratesUniqueSessionId()
    {
        // Arrange
        var board = CreateTestBoard();

        // Act
        var session1 = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        var session2 = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Assert
        Assert.NotEqual(session1.SessionId, session2.SessionId);
    }

    [Fact]
    public void UpdateElapsedTime_UpdatesTimeCorrectly()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        var newTime = TimeSpan.FromSeconds(150);

        // Act
        session.UpdateElapsedTime(newTime);

        // Assert
        Assert.Equal(newTime, session.ElapsedTime);
    }

    [Fact]
    public void UpdateElapsedTime_AfterCompletion_DoesNotUpdate()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        session.Complete();
        var completedTime = session.ElapsedTime;

        // Act
        session.UpdateElapsedTime(TimeSpan.FromSeconds(500));

        // Assert
        Assert.Equal(completedTime, session.ElapsedTime);
    }

    [Fact]
    public void UseHint_IncrementsCounter()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Act
        session.UseHint();
        session.UseHint();
        session.UseHint();

        // Assert
        Assert.Equal(3, session.HintsUsed);
    }

    [Fact]
    public void UseHint_AfterCompletion_DoesNotIncrement()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        session.Complete();
        var hintsBeforeCompletion = session.HintsUsed;

        // Act
        session.UseHint();

        // Assert
        Assert.Equal(hintsBeforeCompletion, session.HintsUsed);
    }

    [Fact]
    public void Complete_SetsCompletedFlag()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Act
        session.Complete();

        // Assert
        Assert.True(session.IsCompleted);
        Assert.NotNull(session.CompletedAt);
    }

    [Fact]
    public void Complete_SetsPreciseCompletionTime()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        var beforeCompletion = DateTime.UtcNow;

        // Act
        session.Complete();
        var afterCompletion = DateTime.UtcNow;

        // Assert
        Assert.NotNull(session.CompletedAt);
        Assert.True(session.CompletedAt >= beforeCompletion);
        Assert.True(session.CompletedAt <= afterCompletion);
    }

    [Fact]
    public void Complete_CanOnlyBeCalledOnce()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Act
        session.Complete();
        var firstCompletedAt = session.CompletedAt;
        System.Threading.Thread.Sleep(10);
        session.Complete();
        var secondCompletedAt = session.CompletedAt;

        // Assert
        Assert.Equal(firstCompletedAt, secondCompletedAt);
    }

    [Fact]
    public void IsBoardFilled_ReturnsFalse_WhenEmptyBoardCreated()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);

        // Act
        var isFilled = session.IsBoardFilled();

        // Assert
        Assert.False(isFilled);
    }

    [Fact]
    public void GetSummary_ReturnsSummaryWithAllData()
    {
        // Arrange
        var board = CreateTestBoard();
        var session = new GameSession(board, GameMode.Timed, Difficulty.Hard);
        session.UpdateElapsedTime(TimeSpan.FromSeconds(300));
        session.UseHint();

        // Act
        var summary = session.GetSummary();

        // Assert
        Assert.Equal(session.SessionId, summary.SessionId);
        Assert.Equal(GameMode.Timed, summary.GameMode);
        Assert.Equal(Difficulty.Hard, summary.Difficulty);
        Assert.Equal(TimeSpan.FromSeconds(300), summary.ElapsedTime);
        Assert.Equal(1, summary.HintsUsed);
        Assert.False(summary.IsCompleted);
        Assert.Null(summary.CompletedAt);
    }

    [Fact]
    public void GameSession_NullBoard_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GameSession(null!, GameMode.Standard, Difficulty.Easy));
    }

    [Fact]
    public void GameSession_HasStartedAt()
    {
        // Arrange
        var board = CreateTestBoard();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var session = new GameSession(board, GameMode.Standard, Difficulty.Easy);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(session.StartedAt >= beforeCreation);
        Assert.True(session.StartedAt <= afterCreation);
    }
}

using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the DailyChallenge model.
/// </summary>
public class DailyChallengeTests
{
    [Fact]
    public void DailyChallenge_WithoutDate_UsesToday()
    {
        // Act
        var challenge = new DailyChallenge();

        // Assert
        Assert.Equal(DateTime.UtcNow.Date, challenge.ChallengeDate.Date);
    }

    [Fact]
    public void DailyChallenge_WithSpecificDate_UsesProvidedDate()
    {
        // Arrange
        var specificDate = new DateTime(2024, 6, 15);

        // Act
        var challenge = new DailyChallenge(specificDate);

        // Assert
        Assert.Equal(specificDate.Date, challenge.ChallengeDate.Date);
    }

    [Fact]
    public void DailyChallenge_GeneratesUniqueSeedPerDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 6, 15);
        var date2 = new DateTime(2024, 6, 16);

        // Act
        var challenge1 = new DailyChallenge(date1);
        var challenge2 = new DailyChallenge(date2);

        // Assert
        Assert.NotEqual(challenge1.Seed, challenge2.Seed);
    }

    [Fact]
    public void DailyChallenge_SameDateGeneratesSameSeed()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15);

        // Act
        var challenge1 = new DailyChallenge(date);
        var challenge2 = new DailyChallenge(date);

        // Assert
        Assert.Equal(challenge1.Seed, challenge2.Seed);
    }

    [Fact]
    public void DailyChallenge_NewInstance_IsNotCompleted()
    {
        // Act
        var challenge = new DailyChallenge();

        // Assert
        Assert.False(challenge.IsCompleted);
        Assert.Null(challenge.CompletionTime);
        Assert.Null(challenge.CompletedAt);
    }

    [Fact]
    public void MarkCompleted_SetsCompletionData()
    {
        // Arrange
        var challenge = new DailyChallenge();
        var elapsedTime = TimeSpan.FromSeconds(450);

        // Act
        challenge.MarkCompleted(elapsedTime);

        // Assert
        Assert.True(challenge.IsCompleted);
        Assert.Equal(elapsedTime, challenge.CompletionTime);
        Assert.NotNull(challenge.CompletedAt);
    }

    [Fact]
    public void MarkCompleted_CanOnlyBeCalledOnce()
    {
        // Arrange
        var challenge = new DailyChallenge();
        var time1 = TimeSpan.FromSeconds(300);
        var time2 = TimeSpan.FromSeconds(400);

        // Act
        challenge.MarkCompleted(time1);
        var completedAtFirst = challenge.CompletedAt;
        System.Threading.Thread.Sleep(10);
        challenge.MarkCompleted(time2);
        var completedAtSecond = challenge.CompletedAt;

        // Assert
        Assert.Equal(time1, challenge.CompletionTime);
        Assert.Equal(completedAtFirst, completedAtSecond);
    }

    [Fact]
    public void GetTodayChallenge_ReturnsChallengeForToday()
    {
        // Act
        var challenge = DailyChallenge.GetTodayChallenge();

        // Assert
        Assert.Equal(DateTime.UtcNow.Date, challenge.ChallengeDate.Date);
    }

    [Fact]
    public void IsToday_ReturnsTrueForTodaysChallenge()
    {
        // Arrange
        var challenge = DailyChallenge.GetTodayChallenge();

        // Act & Assert
        Assert.True(DailyChallenge.IsToday(challenge));
    }

    [Fact]
    public void IsToday_ReturnsFalseForOldChallenge()
    {
        // Arrange
        var oldDate = DateTime.UtcNow.AddDays(-1);
        var challenge = new DailyChallenge(oldDate);

        // Act & Assert
        Assert.False(DailyChallenge.IsToday(challenge));
    }

    [Fact]
    public void GetTimeUntilNextChallenge_ReturnsPositiveTimeSpan()
    {
        // Act
        var timeRemaining = DailyChallenge.GetTimeUntilNextChallenge();

        // Assert
        Assert.True(timeRemaining > TimeSpan.Zero);
        Assert.True(timeRemaining <= TimeSpan.FromHours(24));
    }

    [Fact]
    public void DailyChallenge_GeneratesChallengeId()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15);

        // Act
        var challenge = new DailyChallenge(date);

        // Assert
        Assert.Equal("daily_2024-06-15", challenge.ChallengeId);
    }

    [Fact]
    public void GetSummary_BeforeCompletion_ShowsNotCompleted()
    {
        // Arrange
        var challenge = new DailyChallenge();

        // Act
        var summary = challenge.GetSummary();

        // Assert
        Assert.Contains("Not completed", summary);
    }

    [Fact]
    public void GetSummary_AfterCompletion_ShowsCompletedWithTime()
    {
        // Arrange
        var challenge = new DailyChallenge();
        challenge.MarkCompleted(TimeSpan.FromSeconds(600));

        // Act
        var summary = challenge.GetSummary();

        // Assert
        Assert.Contains("Completed", summary);
        Assert.Contains("10:00", summary); // 600 seconds
    }

    [Fact]
    public void DailyChallenge_SeedIsPositive()
    {
        // Arrange
        var challenge = new DailyChallenge();

        // Act & Assert
        Assert.True(challenge.Seed >= 0);
    }

    [Fact]
    public void DailyChallenge_ConsistentSeedsOverTime()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15);
        var challenges = new List<int>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            var challenge = new DailyChallenge(date);
            challenges.Add(challenge.Seed);
        }

        // Assert
        Assert.All(challenges, seed => Assert.Equal(challenges[0], seed));
    }
}

using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Storage;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Repositories;

/// <summary>
/// Unit tests for StatisticsRepository.
/// </summary>
public class StatisticsRepositoryTests
{
    private readonly LocalStorageService _storageService;
    private readonly StatisticsRepository _repository;

    public StatisticsRepositoryTests()
    {
        _storageService = new LocalStorageService();
        _repository = new StatisticsRepository(_storageService);
    }

    /// <summary>
    /// Tests that SaveStatisticsAsync persists statistics data.
    /// </summary>
    [Fact]
    public async Task SaveStatisticsAsync_PersistsStatistics()
    {
        // Arrange
        var stats = new Statistics();
        var gameStat = stats.GetStats(GameMode.Standard, Difficulty.Easy);
        gameStat.RecordGame(TimeSpan.FromMinutes(5), wasCompleted: true);

        // Act
        await _repository.SaveStatisticsAsync(stats);
        var loaded = await _repository.LoadStatisticsAsync();

        // Assert
        Assert.NotNull(loaded);
        var loadedStat = loaded.GetStats(GameMode.Standard, Difficulty.Easy);
        Assert.Equal(1, loadedStat.GamesCompleted);
        Assert.Equal(TimeSpan.FromMinutes(5), loadedStat.BestTime);
    }

    /// <summary>
    /// Tests that LoadStatisticsAsync returns empty statistics when none exist.
    /// </summary>
    [Fact]
    public async Task LoadStatisticsAsync_ReturnsEmptyStatistics_WhenNoneExists()
    {
        // Act
        var stats = await _repository.LoadStatisticsAsync();

        // Assert
        Assert.NotNull(stats);
        var gameStat = stats.GetStats(GameMode.Standard, Difficulty.Easy);
        Assert.Equal(0, gameStat.GamesCompleted);
    }

    /// <summary>
    /// Tests that SaveStatisticsAsync throws when statistics is null.
    /// </summary>
    [Fact]
    public async Task SaveStatisticsAsync_ThrowsArgumentNullException_WhenStatisticsIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.SaveStatisticsAsync(null!));
    }
}

using Sudoku.Core.Enums;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Storage;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for StatisticsService.
/// </summary>
public class StatisticsServiceTests
{
    private readonly LocalStorageService _storageService;
    private readonly StatisticsRepository _repository;
    private readonly StatisticsService _service;

    public StatisticsServiceTests()
    {
        _storageService = new LocalStorageService();
        _repository = new StatisticsRepository(_storageService);
        _service = new StatisticsService(_repository);
    }

    /// <summary>
    /// Tests that RecordGameCompletionAsync increments game count.
    /// </summary>
    [Fact]
    public async Task RecordGameCompletionAsync_IncrementsGameCount()
    {
        // Act
        await _service.RecordGameCompletionAsync(Difficulty.Easy, TimeSpan.FromMinutes(5));
        var totalGames = await _service.GetTotalGamesCompletedAsync();

        // Assert
        Assert.Equal(1, totalGames);
    }

    /// <summary>
    /// Tests that RecordGameCompletionAsync updates best time.
    /// </summary>
    [Fact]
    public async Task RecordGameCompletionAsync_UpdatesBestTime()
    {
        // Act
        await _service.RecordGameCompletionAsync(Difficulty.Medium, TimeSpan.FromMinutes(10));
        await _service.RecordGameCompletionAsync(Difficulty.Medium, TimeSpan.FromMinutes(8));

        var avgTime = await _service.GetAverageCompletionTimeAsync(Difficulty.Medium);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(9), avgTime);
    }

    /// <summary>
    /// Tests that GetTotalGamesCompletedAsync returns zero initially.
    /// </summary>
    [Fact]
    public async Task GetTotalGamesCompletedAsync_ReturnsZeroInitially()
    {
        // Act
        var total = await _service.GetTotalGamesCompletedAsync();

        // Assert
        Assert.Equal(0, total);
    }

    /// <summary>
    /// Tests that ClearStatisticsAsync resets all statistics.
    /// </summary>
    [Fact]
    public async Task ClearStatisticsAsync_ResetsAllStatistics()
    {
        // Arrange
        await _service.RecordGameCompletionAsync(Difficulty.Hard, TimeSpan.FromMinutes(15));

        // Act
        await _service.ClearStatisticsAsync();
        var totalGames = await _service.GetTotalGamesCompletedAsync();

        // Assert
        Assert.Equal(0, totalGames);
    }
}

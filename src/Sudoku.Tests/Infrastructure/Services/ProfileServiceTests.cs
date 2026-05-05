using Sudoku.Core.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Storage;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for ProfileService.
/// </summary>
public class ProfileServiceTests
{
    private readonly LocalStorageService _storageService;
    private readonly ProfileRepository _repository;
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _storageService = new LocalStorageService();
        _repository = new ProfileRepository(_storageService);
        _service = new ProfileService(_repository);
    }

    /// <summary>
    /// Tests that SetPlayerNameAsync changes player name.
    /// </summary>
    [Fact]
    public async Task SetPlayerNameAsync_ChangesPlayerName()
    {
        // Act
        await _service.SetPlayerNameAsync("NewPlayer");
        var name = _service.PlayerName;

        // Assert
        Assert.Equal("NewPlayer", name);
    }

    /// <summary>
    /// Tests that AddExperiencePointsAsync increases experience.
    /// </summary>
    [Fact]
    public async Task AddExperiencePointsAsync_IncreasesExperience()
    {
        // Act
        await _service.AddExperiencePointsAsync(50);
        var xp = _service.ExperiencePoints;

        // Assert
        Assert.Equal(50, xp);
    }

    /// <summary>
    /// Tests that AddExperiencePointsAsync increases level appropriately.
    /// </summary>
    [Fact]
    public async Task AddExperiencePointsAsync_IncreasesLevelAt100XP()
    {
        // Act
        await _service.AddExperiencePointsAsync(100);
        var level = _service.Level;

        // Assert
        Assert.Equal(2, level);
    }

    /// <summary>
    /// Tests that UnlockAchievementAsync increments achievement count.
    /// </summary>
    [Fact]
    public async Task UnlockAchievementAsync_IncrementsAchievementCount()
    {
        // Act
        await _service.UnlockAchievementAsync("achievement_1");
        await _service.UnlockAchievementAsync("achievement_2");
        var count = _service.AchievementsCount;

        // Assert
        Assert.Equal(2, count);
    }

    /// <summary>
    /// Tests that SetPlayerNameAsync throws when name is empty.
    /// </summary>
    [Fact]
    public async Task SetPlayerNameAsync_ThrowsArgumentException_WhenNameIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SetPlayerNameAsync(""));
    }

    /// <summary>
    /// Tests that AddExperiencePointsAsync throws when points are negative.
    /// </summary>
    [Fact]
    public async Task AddExperiencePointsAsync_ThrowsArgumentException_WhenNegative()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddExperiencePointsAsync(-10));
    }

    /// <summary>
    /// Tests that LoadProfileAsync initializes the service.
    /// </summary>
    [Fact]
    public async Task LoadProfileAsync_InitializesService()
    {
        // Act
        await _service.LoadProfileAsync();
        var name = _service.PlayerName;

        // Assert
        Assert.NotNull(name);
    }

    /// <summary>
    /// Tests that SaveProfileAsync persists changes.
    /// </summary>
    [Fact]
    public async Task SaveProfileAsync_PersistsChanges()
    {
        // Arrange
        await _service.SetPlayerNameAsync("PersistentPlayer");

        // Act
        await _service.SaveProfileAsync();
        var newService = new ProfileService(_repository);
        await newService.LoadProfileAsync();
        var name = newService.PlayerName;

        // Assert
        Assert.Equal("PersistentPlayer", name);
    }
}

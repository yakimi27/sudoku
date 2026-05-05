using Sudoku.Core.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Storage;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Repositories;

/// <summary>
/// Unit tests for ProfileRepository.
/// </summary>
public class ProfileRepositoryTests
{
    private readonly LocalStorageService _storageService;
    private readonly ProfileRepository _repository;

    public ProfileRepositoryTests()
    {
        _storageService = new LocalStorageService();
        _repository = new ProfileRepository(_storageService);
    }

    /// <summary>
    /// Tests that SaveProfileAsync persists profile data.
    /// </summary>
    [Fact]
    public async Task SaveProfileAsync_PersistsProfile()
    {
        // Arrange
        var profile = PlayerProfile.Instance;
        profile.Name = "TestPlayer";
        profile.Level = 10;

        // Act
        await _repository.SaveProfileAsync(profile);
        var loaded = await _repository.LoadProfileAsync();

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("TestPlayer", loaded.Name);
        Assert.Equal(10, loaded.Level);
    }

    /// <summary>
    /// Tests that LoadProfileAsync returns default profile when none exists.
    /// </summary>
    [Fact]
    public async Task LoadProfileAsync_ReturnsDefaultProfile_WhenNoneExists()
    {
        // Act
        var profile = await _repository.LoadProfileAsync();

        // Assert
        Assert.NotNull(profile);
        Assert.Equal("Player", profile.Name);
        Assert.Equal(1, profile.Level);
    }

    /// <summary>
    /// Tests that SaveProfileAsync throws when profile is null.
    /// </summary>
    [Fact]
    public async Task SaveProfileAsync_ThrowsArgumentNullException_WhenProfileIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.SaveProfileAsync(null!));
    }
}

using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the PlayerProfile singleton model.
/// </summary>
public class PlayerProfileTests
{
    // Note: PlayerProfile is a Singleton, so tests may affect each other.
    // We reset after tests that modify state.

    private void ResetProfile()
    {
        PlayerProfile.Instance.Reset();
    }

    [Fact]
    public void PlayerProfile_Instance_IsNotNull()
    {
        // Act & Assert
        Assert.NotNull(PlayerProfile.Instance);
    }

    [Fact]
    public void PlayerProfile_Instance_ReturnsSameInstance()
    {
        // Act
        var instance1 = PlayerProfile.Instance;
        var instance2 = PlayerProfile.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void PlayerProfile_DefaultValues_AreSet()
    {
        // Arrange & Act
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Assert
        Assert.Equal("Player", profile.Name);
        Assert.Equal(0, profile.AvatarIndex);
        Assert.Equal("en", profile.PreferredLanguage);
        Assert.Equal("Auto", profile.PreferredTheme);
        Assert.Equal(0, profile.ExperiencePoints);
        Assert.Equal(1, profile.Level);
    }

    [Fact]
    public void PlayerProfile_CanSetName()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act
        profile.Name = "TestPlayer";
        profile.MarkAsModified();

        // Assert
        Assert.Equal("TestPlayer", profile.Name);
    }

    [Fact]
    public void PlayerProfile_CanSetPreferences()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act
        profile.PreferredLanguage = "es";
        profile.PreferredTheme = "Dark";
        profile.AvatarIndex = 3;
        profile.MarkAsModified();

        // Assert
        Assert.Equal("es", profile.PreferredLanguage);
        Assert.Equal("Dark", profile.PreferredTheme);
        Assert.Equal(3, profile.AvatarIndex);
    }

    [Fact]
    public void PlayerProfile_AddExperiencePoints_IncreasesPoints()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act
        profile.AddExperiencePoints(100);
        profile.AddExperiencePoints(50);

        // Assert
        Assert.Equal(150, profile.ExperiencePoints);
    }

    [Fact]
    public void PlayerProfile_AddExperiencePoints_IgnoresNegativePoints()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;
        profile.AddExperiencePoints(100);

        // Act
        profile.AddExperiencePoints(-50);

        // Assert
        Assert.Equal(100, profile.ExperiencePoints);
    }

    [Fact]
    public void PlayerProfile_AddExperiencePoints_IgnoresZero()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act
        profile.AddExperiencePoints(0);

        // Assert
        Assert.Equal(0, profile.ExperiencePoints);
    }

    [Fact]
    public void PlayerProfile_MarkAsModified_UpdatesLastModifiedAt()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;
        var originalModified = profile.LastModifiedAt;

        // Act
        System.Threading.Thread.Sleep(10);
        profile.MarkAsModified();
        var newModified = profile.LastModifiedAt;

        // Assert
        Assert.True(newModified > originalModified);
    }

    [Fact]
    public void PlayerProfile_CreatedAt_IsSet()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act & Assert
        Assert.NotEqual(default(DateTime), profile.CreatedAt);
    }

    [Fact]
    public void PlayerProfile_Reset_RestoresDefaults()
    {
        // Arrange
        var profile = PlayerProfile.Instance;
        profile.Name = "CustomName";
        profile.PreferredLanguage = "fr";
        profile.ExperiencePoints = 999;

        // Act
        profile.Reset();

        // Assert
        Assert.Equal("Player", profile.Name);
        Assert.Equal("en", profile.PreferredLanguage);
        Assert.Equal(0, profile.ExperiencePoints);
        Assert.Equal(1, profile.Level);
    }

    [Fact]
    public void PlayerProfile_GetSummary_ContainsAllData()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;
        profile.Name = "TestPlayer";
        profile.AvatarIndex = 2;
        profile.PreferredLanguage = "de";
        profile.ExperiencePoints = 250;
        profile.Level = 5;
        profile.MarkAsModified();

        // Act
        var summary = profile.GetSummary();

        // Assert
        Assert.Equal("TestPlayer", summary.Name);
        Assert.Equal(2, summary.AvatarIndex);
        Assert.Equal("de", summary.PreferredLanguage);
        Assert.Equal(250, summary.ExperiencePoints);
        Assert.Equal(5, summary.Level);
        Assert.NotEqual(default(DateTime), summary.CreatedAt);
        Assert.NotEqual(default(DateTime), summary.LastModifiedAt);
    }

    [Fact]
    public void PlayerProfile_LastModifiedAt_UpdatesAfterReset()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;
        var initialModified = profile.LastModifiedAt;

        // Act
        System.Threading.Thread.Sleep(10);
        profile.Name = "NewName";
        profile.Reset();
        var afterResetModified = profile.LastModifiedAt;

        // Assert
        Assert.True(afterResetModified > initialModified);
    }

    [Fact]
    public void PlayerProfile_Level_CanBeSet()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;

        // Act
        profile.Level = 10;
        profile.MarkAsModified();

        // Assert
        Assert.Equal(10, profile.Level);
    }

    [Fact]
    public void PlayerProfile_Singleton_SurvivesManyInstances()
    {
        // Arrange
        ResetProfile();
        var profile = PlayerProfile.Instance;
        profile.Name = "Singleton";
        profile.MarkAsModified();

        // Act
        var instances = Enumerable.Range(0, 10).Select(_ => PlayerProfile.Instance).ToList();

        // Assert
        Assert.All(instances, inst => Assert.Equal("Singleton", inst.Name));
        Assert.All(instances, inst => Assert.Same(profile, inst));
    }
}

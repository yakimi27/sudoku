using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Infrastructure.Repositories;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements player profile management with in-memory caching.
/// Provides access to player name, level, experience, and achievements.
/// Implements Singleton pattern - single instance throughout application lifecycle.
/// </summary>
public sealed class ProfileService : IProfileService
{
    private readonly ProfileRepository _repository;
    private PlayerProfile? _cachedProfile;
    private bool _isInitialized;
    private int _unlockedAchievementsCount;

    /// <summary>
    /// Gets the current player's profile name.
    /// </summary>
    public string PlayerName => _cachedProfile?.Name ?? "Player";

    /// <summary>
    /// Gets the player's current level.
    /// </summary>
    public int Level => _cachedProfile?.Level ?? 1;

    /// <summary>
    /// Gets the player's current experience points.
    /// </summary>
    public int ExperiencePoints => _cachedProfile?.ExperiencePoints ?? 0;

    /// <summary>
    /// Gets the total number of achievements earned.
    /// </summary>
    public int AchievementsCount => _unlockedAchievementsCount;

    /// <summary>
    /// Initializes a new instance of the ProfileService class.
    /// </summary>
    /// <param name="repository">The repository for persisting profile data.</param>
    /// <exception cref="ArgumentNullException">Thrown when repository is null.</exception>
    public ProfileService(ProfileRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _isInitialized = false;
        _unlockedAchievementsCount = 0;
    }

    /// <summary>
    /// Initializes the service by loading the profile from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        _cachedProfile = await _repository.LoadProfileAsync();
        _isInitialized = true;
    }

    /// <summary>
    /// Sets the player's profile name.
    /// </summary>
    /// <param name="name">The new player name.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public async Task SetPlayerNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name cannot be null or empty.", nameof(name));

        await InitializeAsync();

        _cachedProfile!.Name = name;
        _cachedProfile.MarkAsModified();
        await _repository.SaveProfileAsync(_cachedProfile);
    }

    /// <summary>
    /// Adds experience points to the player's profile.
    /// </summary>
    /// <param name="points">Number of points to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when points is negative.</exception>
    public async Task AddExperiencePointsAsync(int points)
    {
        if (points < 0)
            throw new ArgumentException("Experience points cannot be negative.", nameof(points));

        await InitializeAsync();

        _cachedProfile!.AddExperiencePoints(points);

        // Simple level calculation: every 100 XP = 1 level
        int newLevel = (_cachedProfile.ExperiencePoints / 100) + 1;
        if (newLevel > _cachedProfile.Level)
        {
            _cachedProfile.Level = newLevel;
        }

        await _repository.SaveProfileAsync(_cachedProfile);
    }

    /// <summary>
    /// Unlocks an achievement for the player.
    /// </summary>
    /// <param name="achievementId">Identifier of the achievement.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when achievementId is null or empty.</exception>
    public async Task UnlockAchievementAsync(string achievementId)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
            throw new ArgumentException("Achievement ID cannot be null or empty.", nameof(achievementId));

        await InitializeAsync();

        // Increment unlocked achievements counter
        _unlockedAchievementsCount++;
        _cachedProfile!.MarkAsModified();
        await _repository.SaveProfileAsync(_cachedProfile);
    }

    /// <summary>
    /// Loads the player's profile from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadProfileAsync()
    {
        _isInitialized = false;
        await InitializeAsync();
    }

    /// <summary>
    /// Saves the player's profile to storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveProfileAsync()
    {
        await InitializeAsync();

        if (_cachedProfile != null)
        {
            await _repository.SaveProfileAsync(_cachedProfile);
        }
    }
}

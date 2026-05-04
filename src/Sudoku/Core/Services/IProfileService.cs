namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for managing player profile information.
/// Implements Singleton pattern - single instance throughout application lifecycle.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Gets the current player's profile name.
    /// </summary>
    string PlayerName { get; }

    /// <summary>
    /// Gets the player's current level.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Gets the player's current experience points.
    /// </summary>
    int ExperiencePoints { get; }

    /// <summary>
    /// Gets the total number of achievements earned.
    /// </summary>
    int AchievementsCount { get; }

    /// <summary>
    /// Sets the player's profile name.
    /// </summary>
    /// <param name="name">The new player name.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetPlayerNameAsync(string name);

    /// <summary>
    /// Adds experience points to the player's profile.
    /// </summary>
    /// <param name="points">Number of points to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddExperiencePointsAsync(int points);

    /// <summary>
    /// Unlocks an achievement for the player.
    /// </summary>
    /// <param name="achievementId">Identifier of the achievement.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnlockAchievementAsync(string achievementId);

    /// <summary>
    /// Loads the player's profile from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LoadProfileAsync();

    /// <summary>
    /// Saves the player's profile to storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveProfileAsync();
}

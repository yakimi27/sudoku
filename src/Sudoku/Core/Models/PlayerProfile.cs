namespace Sudoku.Core.Models;

/// <summary>
/// Represents a player's profile information and preferences.
/// Implements Singleton pattern - single instance throughout application lifecycle.
/// Use PlayerProfile.Instance to access the singleton.
/// </summary>
public sealed class PlayerProfile
{
    private static PlayerProfile? _instance;
    private static readonly object _lockObject = new object();

    /// <summary>
    /// Gets the singleton instance of the PlayerProfile.
    /// </summary>
    public static PlayerProfile Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new PlayerProfile();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets or sets the player's display name.
    /// </summary>
    public string Name { get; set; } = "Player";

    /// <summary>
    /// Gets or sets the index of the player's avatar (for avatar selection).
    /// </summary>
    public int AvatarIndex { get; set; } = 0;

    /// <summary>
    /// Gets or sets the preferred language code (e.g., "en", "es", "fr").
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets the preferred UI theme ("Light", "Dark", "Auto").
    /// </summary>
    public string PreferredTheme { get; set; } = "Auto";

    /// <summary>
    /// Gets or sets the player's total experience points.
    /// </summary>
    public int ExperiencePoints { get; set; } = 0;

    /// <summary>
    /// Gets or sets the player's current level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Gets the date and time when the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the date and time when the profile was last modified.
    /// </summary>
    public DateTime LastModifiedAt { get; private set; }

    /// <summary>
    /// Private constructor to prevent external instantiation.
    /// </summary>
    private PlayerProfile()
    {
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the LastModifiedAt timestamp to the current time.
    /// Call this method after making changes to profile properties.
    /// </summary>
    public void MarkAsModified()
    {
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the profile to default values.
    /// </summary>
    public void Reset()
    {
        Name = "Player";
        AvatarIndex = 0;
        PreferredLanguage = "en";
        PreferredTheme = "Auto";
        ExperiencePoints = 0;
        Level = 1;
        MarkAsModified();
    }

    /// <summary>
    /// Adds experience points to the player's profile.
    /// </summary>
    /// <param name="points">The number of points to add.</param>
    public void AddExperiencePoints(int points)
    {
        if (points > 0)
        {
            ExperiencePoints += points;
            MarkAsModified();
        }
    }

    /// <summary>
    /// Gets a summary of the player's profile for display or serialization.
    /// </summary>
    /// <returns>A ProfileSummary containing key profile information.</returns>
    public ProfileSummary GetSummary()
    {
        return new ProfileSummary
        {
            Name = Name,
            AvatarIndex = AvatarIndex,
            PreferredLanguage = PreferredLanguage,
            PreferredTheme = PreferredTheme,
            ExperiencePoints = ExperiencePoints,
            Level = Level,
            CreatedAt = CreatedAt,
            LastModifiedAt = LastModifiedAt
        };
    }
}

/// <summary>
/// A serialization-friendly summary of player profile data.
/// </summary>
public class ProfileSummary
{
    /// <summary>
    /// Gets or sets the player's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the avatar index.
    /// </summary>
    public int AvatarIndex { get; set; }

    /// <summary>
    /// Gets or sets the preferred language.
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets the preferred theme.
    /// </summary>
    public string PreferredTheme { get; set; } = "Auto";

    /// <summary>
    /// Gets or sets the experience points.
    /// </summary>
    public int ExperiencePoints { get; set; }

    /// <summary>
    /// Gets or sets the player level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the profile creation time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the profile last modification time.
    /// </summary>
    public DateTime LastModifiedAt { get; set; }
}

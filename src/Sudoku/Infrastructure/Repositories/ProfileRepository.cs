using Sudoku.Core.Models;
using Sudoku.Infrastructure.Storage;

namespace Sudoku.Infrastructure.Repositories;

/// <summary>
/// Repository for persisting and retrieving player profile data.
/// Manages profile storage using JSON serialization.
/// </summary>
public sealed class ProfileRepository
{
    private const string ProfileStorageKey = "player_profile";

    private readonly LocalStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the ProfileRepository class.
    /// </summary>
    /// <param name="storageService">The storage service for persisting profile data.</param>
    /// <exception cref="ArgumentNullException">Thrown when storageService is null.</exception>
    public ProfileRepository(LocalStorageService storageService)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    /// <summary>
    /// Saves a player profile to storage.
    /// </summary>
    /// <param name="profile">The player profile to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when profile is null.</exception>
    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        await _storageService.SaveAsync(ProfileStorageKey, profile);
    }

    /// <summary>
    /// Loads a player profile from storage.
    /// Returns a default profile if none is found.
    /// </summary>
    /// <returns>The loaded PlayerProfile, or a new default profile if none exists.</returns>
    public async Task<PlayerProfile> LoadProfileAsync()
    {
        var profile = await _storageService.LoadAsync<PlayerProfile>(ProfileStorageKey);
        return profile ?? PlayerProfile.Instance;
    }
}

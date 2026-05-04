namespace Sudoku.Core.Services;

/// <summary>
/// Defines the contract for persisting and retrieving game data.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Saves a game state to persistent storage.
    /// </summary>
    /// <param name="gameId">Unique identifier for the game.</param>
    /// <param name="gameData">Serialized game data to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveGameAsync(string gameId, string gameData);

    /// <summary>
    /// Loads a saved game state from persistent storage.
    /// </summary>
    /// <param name="gameId">Unique identifier for the game.</param>
    /// <returns>Serialized game data, or null if not found.</returns>
    Task<string?> LoadGameAsync(string gameId);

    /// <summary>
    /// Deletes a saved game from persistent storage.
    /// </summary>
    /// <param name="gameId">Unique identifier for the game to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteGameAsync(string gameId);

    /// <summary>
    /// Retrieves all saved game identifiers.
    /// </summary>
    /// <returns>Collection of game IDs for all saved games.</returns>
    Task<IEnumerable<string>> GetSavedGamesAsync();
}

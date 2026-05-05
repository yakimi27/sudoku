using Sudoku.Core.Enums;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Factory for creating IGameMode instances.
/// Centralizes all game mode creation using the Factory Method pattern.
/// No `new` keyword for game modes should appear outside this factory.
/// </summary>
public static class GameModeFactory
{
    /// <summary>
    /// Creates a game mode instance for the specified type.
    /// This is the single creation point for all game modes.
    /// </summary>
    /// <param name="modeType">The type of game mode to create.</param>
    /// <returns>An IGameMode instance of the requested type.</returns>
    /// <exception cref="ArgumentException">Thrown when mode type is unknown.</exception>
    public static IGameMode Create(GameMode modeType)
    {
        return modeType switch
        {
            GameMode.Standard => new ClassicMode(),
            GameMode.Thermometer => new ThermometerMode(),
            GameMode.Killer => new KillerMode(),
            GameMode.Mini => new MiniMode(),
            _ => throw new ArgumentException($"Unknown game mode: {modeType}", nameof(modeType))
        };
    }

    /// <summary>
    /// Gets the localization key for a game mode.
    /// Useful for UI that needs to display mode names without instantiating modes.
    /// </summary>
    /// <param name="modeType">The type of game mode.</param>
    /// <returns>The localization key for the mode's display name.</returns>
    /// <exception cref="ArgumentException">Thrown when mode type is unknown.</exception>
    public static string GetDisplayNameKey(GameMode modeType)
    {
        return modeType switch
        {
            GameMode.Standard => "mode.classic",
            GameMode.Thermometer => "mode.thermometer",
            GameMode.Killer => "mode.killer",
            GameMode.Mini => "mode.mini",
            _ => throw new ArgumentException($"Unknown game mode: {modeType}", nameof(modeType))
        };
    }

    /// <summary>
    /// Gets board dimensions for a specific game mode.
    /// </summary>
    /// <param name="modeType">The type of game mode.</param>
    /// <returns>A tuple of (rows, columns, blockHeight, blockWidth).</returns>
    /// <exception cref="ArgumentException">Thrown when mode type is unknown.</exception>
    public static (int rows, int cols, int blockHeight, int blockWidth) GetBoardDimensions(GameMode modeType)
    {
        return modeType switch
        {
            GameMode.Standard => (9, 9, 3, 3),
            GameMode.Thermometer => (9, 9, 3, 3),
            GameMode.Killer => (9, 9, 3, 3),
            GameMode.Mini => (6, 6, 2, 3),
            _ => throw new ArgumentException($"Unknown game mode: {modeType}", nameof(modeType))
        };
    }

    /// <summary>
    /// Gets valid digits for a specific game mode.
    /// </summary>
    /// <param name="modeType">The type of game mode.</param>
    /// <returns>An array of valid digits for the mode.</returns>
    /// <exception cref="ArgumentException">Thrown when mode type is unknown.</exception>
    public static int[] GetValidDigits(GameMode modeType)
    {
        return modeType switch
        {
            GameMode.Standard => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            GameMode.Thermometer => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            GameMode.Killer => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            GameMode.Mini => new[] { 1, 2, 3, 4, 5, 6 },
            _ => throw new ArgumentException($"Unknown game mode: {modeType}", nameof(modeType))
        };
    }
}

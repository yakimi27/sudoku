using Sudoku.Core.Models;
using Sudoku.Infrastructure.Storage;

namespace Sudoku.Infrastructure.Repositories;

/// <summary>
/// Repository for persisting and retrieving player statistics data.
/// Manages statistics storage using JSON serialization.
/// </summary>
public sealed class StatisticsRepository
{
    private const string StatisticsStorageKey = "player_statistics";

    private readonly LocalStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the StatisticsRepository class.
    /// </summary>
    /// <param name="storageService">The storage service for persisting statistics data.</param>
    /// <exception cref="ArgumentNullException">Thrown when storageService is null.</exception>
    public StatisticsRepository(LocalStorageService storageService)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    /// <summary>
    /// Saves player statistics to storage.
    /// </summary>
    /// <param name="statistics">The statistics to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistics is null.</exception>
    public async Task SaveStatisticsAsync(Statistics statistics)
    {
        if (statistics == null)
            throw new ArgumentNullException(nameof(statistics));

        await _storageService.SaveAsync(StatisticsStorageKey, statistics);
    }

    /// <summary>
    /// Loads player statistics from storage.
    /// Returns a new empty statistics object if none is found.
    /// </summary>
    /// <returns>The loaded Statistics, or a new Statistics instance if none exists.</returns>
    public async Task<Statistics> LoadStatisticsAsync()
    {
        var statistics = await _storageService.LoadAsync<Statistics>(StatisticsStorageKey);
        return statistics ?? new Statistics();
    }
}

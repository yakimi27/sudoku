using System.Text.Json;

namespace Sudoku.Infrastructure.Storage;

/// <summary>
/// Provides local storage functionality for persisting application data using JSON serialization.
/// Stores all data in the application's local data folder.
/// </summary>
public sealed class LocalStorageService
{
    private readonly string _storagePath;

    /// <summary>
    /// Initializes a new instance of the LocalStorageService class.
    /// </summary>
    public LocalStorageService()
    {
        // Use application data folder in user's local AppData directory
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _storagePath = Path.Combine(appDataPath, "Sudoku", "Data");

        // Ensure the storage directory exists
        Directory.CreateDirectory(_storagePath);
    }

    /// <summary>
    /// Saves data to storage as JSON.
    /// </summary>
    /// <typeparam name="T">The type of data to save.</typeparam>
    /// <param name="key">The storage key (used as filename).</param>
    /// <param name="data">The data to serialize and save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public async Task SaveAsync<T>(string key, T data)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Storage key cannot be null or empty.", nameof(key));

        var fileName = GetSafeFileName(key);
        var filePath = Path.Combine(_storagePath, $"{fileName}.json");

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Loads data from storage and deserializes from JSON.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    /// <param name="key">The storage key (used as filename).</param>
    /// <returns>The deserialized data, or null if the key does not exist.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public async Task<T?> LoadAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Storage key cannot be null or empty.", nameof(key));

        var fileName = GetSafeFileName(key);
        var filePath = Path.Combine(_storagePath, $"{fileName}.json");

        try
        {
            if (!File.Exists(filePath))
                return default;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (IOException)
        {
            return default;
        }
    }

    /// <summary>
    /// Deletes data from storage.
    /// </summary>
    /// <param name="key">The storage key (used as filename).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public async Task DeleteAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Storage key cannot be null or empty.", nameof(key));

        var fileName = GetSafeFileName(key);
        var filePath = Path.Combine(_storagePath, $"{fileName}.json");

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (IOException)
        {
            // File doesn't exist or cannot be deleted, ignore
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Converts a storage key to a safe filename.
    /// Removes invalid filename characters to prevent file system errors.
    /// </summary>
    /// <param name="key">The storage key.</param>
    /// <returns>A filename-safe version of the key.</returns>
    private static string GetSafeFileName(string key)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(key.Where(c => !invalidChars.Contains(c)).ToArray());
    }
}

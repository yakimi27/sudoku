using Sudoku.Infrastructure.Storage;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Storage;

/// <summary>
/// Unit tests for LocalStorageService.
/// </summary>
public class LocalStorageServiceTests
{
    private readonly LocalStorageService _service;

    public LocalStorageServiceTests()
    {
        _service = new LocalStorageService();
    }

    /// <summary>
    /// Tests that SaveAsync can persist data.
    /// </summary>
    [Fact]
    public async Task SaveAsync_PersistsData()
    {
        // Arrange
        var key = "test_data";
        var data = new { Name = "TestPlayer", Level = 5 };

        // Act
        await _service.SaveAsync(key, data);
        var loaded = await _service.LoadAsync<dynamic>(key);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("TestPlayer", (string)loaded.Name);
        Assert.Equal(5, (int)loaded.Level);
    }

    /// <summary>
    /// Tests that LoadAsync returns null for non-existent keys.
    /// </summary>
    [Fact]
    public async Task LoadAsync_ReturnsNull_WhenKeyNotFound()
    {
        // Act
        var result = await _service.LoadAsync<string>("non_existent_key_" + Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that DeleteAsync removes saved data.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_RemovesData()
    {
        // Arrange
        var key = "delete_test_" + Guid.NewGuid();
        var data = new { Value = 42 };
        await _service.SaveAsync(key, data);

        // Act
        await _service.DeleteAsync(key);
        var result = await _service.LoadAsync<dynamic>(key);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that SaveAsync throws when key is null.
    /// </summary>
    [Fact]
    public async Task SaveAsync_ThrowsArgumentException_WhenKeyIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SaveAsync(null!, new { }));
    }

    /// <summary>
    /// Tests that SaveAsync throws when key is empty.
    /// </summary>
    [Fact]
    public async Task SaveAsync_ThrowsArgumentException_WhenKeyIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SaveAsync("", new { }));
    }

    /// <summary>
    /// Tests that LoadAsync can handle complex types.
    /// </summary>
    [Fact]
    public async Task LoadAsync_CanDeserializeComplexTypes()
    {
        // Arrange
        var key = "complex_" + Guid.NewGuid();
        var data = new TestData
        {
            Id = 123,
            Name = "Test",
            Items = new List<int> { 1, 2, 3 }
        };

        // Act
        await _service.SaveAsync(key, data);
        var loaded = await _service.LoadAsync<TestData>(key);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(123, loaded.Id);
        Assert.Equal("Test", loaded.Name);
        Assert.Equal(new[] { 1, 2, 3 }, loaded.Items);
    }

    /// <summary>
    /// Test data class for complex type deserialization.
    /// </summary>
    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> Items { get; set; } = new();
    }
}

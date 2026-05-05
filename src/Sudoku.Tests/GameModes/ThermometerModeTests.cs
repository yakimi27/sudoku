using Sudoku.Core.Enums;
using Sudoku.Core.GameModes;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.GameModes;

/// <summary>
/// Unit tests for ThermometerMode.
/// </summary>
public class ThermometerModeTests
{
    /// <summary>
    /// Tests that ThermometerMode returns correct mode type.
    /// </summary>
    [Fact]
    public void ModeType_ReturnsThermometer()
    {
        // Arrange & Act
        var mode = new ThermometerMode();

        // Assert
        Assert.Equal(GameMode.Thermometer, mode.ModeType);
    }

    /// <summary>
    /// Tests that ThermometerMode returns correct display name key.
    /// </summary>
    [Fact]
    public void DisplayNameKey_ReturnsLocalizedKey()
    {
        // Arrange & Act
        var mode = new ThermometerMode();

        // Assert
        Assert.Equal("mode.thermometer", mode.DisplayNameKey);
    }

    /// <summary>
    /// Tests that ThermometerMode has correct board dimensions.
    /// </summary>
    [Fact]
    public void BoardDimensions_Returns9x9With3x3Blocks()
    {
        // Arrange & Act
        var mode = new ThermometerMode();

        // Assert
        Assert.Equal(9, mode.BoardRows);
        Assert.Equal(9, mode.BoardColumns);
        Assert.Equal(3, mode.BlockHeight);
        Assert.Equal(3, mode.BlockWidth);
    }

    /// <summary>
    /// Tests that adding a valid thermometer succeeds.
    /// </summary>
    [Fact]
    public void AddThermometer_WithValidPath_Succeeds()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };

        // Act & Assert
        mode.AddThermometer(path); // Should not throw
    }

    /// <summary>
    /// Tests that adding an empty thermometer throws.
    /// </summary>
    [Fact]
    public void AddThermometer_WithEmptyPath_Throws()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => mode.AddThermometer(path));
    }

    /// <summary>
    /// Tests that strictly increasing thermometer validates correctly.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithStrictlyIncreasing_ReturnsTrue()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        mode.AddThermometer(path);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));
        board.SetCell(0, 1, new Cell(0, 1, value: 5));
        board.SetCell(0, 2, new Cell(0, 2, value: 9));

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that non-increasing thermometer validates as false.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithNonIncreasing_ReturnsFalse()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        mode.AddThermometer(path);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(0, 1, new Cell(0, 1, value: 3)); // Less than previous

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that equal consecutive values fail validation.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithEqualConsecutive_ReturnsFalse()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddThermometer(path);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(0, 1, new Cell(0, 1, value: 5)); // Equal

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that empty cells pass validation (partial board).
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithEmptyCell_ReturnsTrue()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddThermometer(path);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.True(result); // Empty cell in thermometer is ok
    }

    /// <summary>
    /// Tests that GetConstraints returns thermometer constraints.
    /// </summary>
    [Fact]
    public void GetConstraints_ReturnsThermometerConstraints()
    {
        // Arrange
        var mode = new ThermometerMode();
        var path = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        mode.AddThermometer(path);

        var board = SudokuBoard.CreateEmpty();

        // Act
        var constraints = mode.GetConstraints(board);

        // Assert
        Assert.Equal(3, constraints.Count);
        Assert.All(constraints, c => Assert.Equal(ConstraintType.Thermometer, c.Type));
    }

    /// <summary>
    /// Tests that GetValidDigits returns 1-9.
    /// </summary>
    [Fact]
    public void GetValidDigits_Returns1Through9()
    {
        // Arrange & Act
        var mode = new ThermometerMode();
        var digits = mode.GetValidDigits();

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, digits);
    }
}

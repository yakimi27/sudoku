using Sudoku.Core.Enums;
using Sudoku.Core.GameModes;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.GameModes;

/// <summary>
/// Unit tests for ClassicMode.
/// </summary>
public class ClassicModeTests
{
    /// <summary>
    /// Tests that ClassicMode returns correct mode type.
    /// </summary>
    [Fact]
    public void ModeType_ReturnsStandard()
    {
        // Arrange & Act
        var mode = new ClassicMode();

        // Assert
        Assert.Equal(GameMode.Standard, mode.ModeType);
    }

    /// <summary>
    /// Tests that ClassicMode returns correct display name key.
    /// </summary>
    [Fact]
    public void DisplayNameKey_ReturnsLocalizedKey()
    {
        // Arrange & Act
        var mode = new ClassicMode();

        // Assert
        Assert.Equal("mode.classic", mode.DisplayNameKey);
    }

    /// <summary>
    /// Tests that ClassicMode has correct board dimensions.
    /// </summary>
    [Fact]
    public void BoardDimensions_Returns9x9With3x3Blocks()
    {
        // Arrange & Act
        var mode = new ClassicMode();

        // Assert
        Assert.Equal(9, mode.BoardRows);
        Assert.Equal(9, mode.BoardColumns);
        Assert.Equal(3, mode.BlockHeight);
        Assert.Equal(3, mode.BlockWidth);
    }

    /// <summary>
    /// Tests that ClassicMode has no extra validation rules.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_AlwaysReturnsTrue()
    {
        // Arrange
        var mode = new ClassicMode();
        var board = SudokuBoard.CreateEmpty();

        // Act & Assert
        Assert.True(mode.ValidateExtraRules(board, 0, 0));
        Assert.True(mode.ValidateExtraRules(board, 5, 5));
        Assert.True(mode.ValidateExtraRules(board, 8, 8));
    }

    /// <summary>
    /// Tests that ClassicMode returns empty constraints.
    /// </summary>
    [Fact]
    public void GetConstraints_ReturnsEmptyList()
    {
        // Arrange
        var mode = new ClassicMode();
        var board = SudokuBoard.CreateEmpty();

        // Act
        var constraints = mode.GetConstraints(board);

        // Assert
        Assert.Empty(constraints);
    }

    /// <summary>
    /// Tests that ClassicMode returns digits 1-9.
    /// </summary>
    [Fact]
    public void GetValidDigits_Returns1Through9()
    {
        // Arrange & Act
        var mode = new ClassicMode();
        var digits = mode.GetValidDigits();

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, digits);
    }
}

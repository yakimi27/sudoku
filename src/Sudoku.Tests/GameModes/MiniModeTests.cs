using Sudoku.Core.Enums;
using Sudoku.Core.GameModes;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.GameModes;

/// <summary>
/// Unit tests for MiniMode.
/// </summary>
public class MiniModeTests
{
    /// <summary>
    /// Tests that MiniMode returns correct mode type.
    /// </summary>
    [Fact]
    public void ModeType_ReturnsMini()
    {
        // Arrange & Act
        var mode = new MiniMode();

        // Assert
        Assert.Equal(GameMode.Mini, mode.ModeType);
    }

    /// <summary>
    /// Tests that MiniMode returns correct display name key.
    /// </summary>
    [Fact]
    public void DisplayNameKey_ReturnsLocalizedKey()
    {
        // Arrange & Act
        var mode = new MiniMode();

        // Assert
        Assert.Equal("mode.mini", mode.DisplayNameKey);
    }

    /// <summary>
    /// Tests that MiniMode has correct board dimensions (6x6 with 2x3 blocks).
    /// </summary>
    [Fact]
    public void BoardDimensions_Returns6x6With2x3Blocks()
    {
        // Arrange & Act
        var mode = new MiniMode();

        // Assert
        Assert.Equal(6, mode.BoardRows);
        Assert.Equal(6, mode.BoardColumns);
        Assert.Equal(2, mode.BlockHeight);
        Assert.Equal(3, mode.BlockWidth);
    }

    /// <summary>
    /// Tests that MiniMode has no extra validation rules.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_AlwaysReturnsTrue()
    {
        // Arrange
        var mode = new MiniMode();
        var board = SudokuBoard.CreateEmpty();

        // Act & Assert
        Assert.True(mode.ValidateExtraRules(board, 0, 0));
        Assert.True(mode.ValidateExtraRules(board, 3, 3));
        Assert.True(mode.ValidateExtraRules(board, 5, 5));
    }

    /// <summary>
    /// Tests that MiniMode returns empty constraints.
    /// </summary>
    [Fact]
    public void GetConstraints_ReturnsEmptyList()
    {
        // Arrange
        var mode = new MiniMode();
        var board = SudokuBoard.CreateEmpty();

        // Act
        var constraints = mode.GetConstraints(board);

        // Assert
        Assert.Empty(constraints);
    }

    /// <summary>
    /// Tests that MiniMode returns digits 1-6.
    /// </summary>
    [Fact]
    public void GetValidDigits_Returns1Through6()
    {
        // Arrange & Act
        var mode = new MiniMode();
        var digits = mode.GetValidDigits();

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, digits);
    }
}

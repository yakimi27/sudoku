using Sudoku.Core.Enums;
using Sudoku.Core.GameModes;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.GameModes;

/// <summary>
/// Unit tests for KillerMode.
/// </summary>
public class KillerModeTests
{
    /// <summary>
    /// Tests that KillerMode returns correct mode type.
    /// </summary>
    [Fact]
    public void ModeType_ReturnsKiller()
    {
        // Arrange & Act
        var mode = new KillerMode();

        // Assert
        Assert.Equal(GameMode.Killer, mode.ModeType);
    }

    /// <summary>
    /// Tests that KillerMode returns correct display name key.
    /// </summary>
    [Fact]
    public void DisplayNameKey_ReturnsLocalizedKey()
    {
        // Arrange & Act
        var mode = new KillerMode();

        // Assert
        Assert.Equal("mode.killer", mode.DisplayNameKey);
    }

    /// <summary>
    /// Tests that KillerMode has correct board dimensions.
    /// </summary>
    [Fact]
    public void BoardDimensions_Returns9x9With3x3Blocks()
    {
        // Arrange & Act
        var mode = new KillerMode();

        // Assert
        Assert.Equal(9, mode.BoardRows);
        Assert.Equal(9, mode.BoardColumns);
        Assert.Equal(3, mode.BlockHeight);
        Assert.Equal(3, mode.BlockWidth);
    }

    /// <summary>
    /// Tests that adding a valid cage succeeds.
    /// </summary>
    [Fact]
    public void AddCage_WithValidCage_Succeeds()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };

        // Act & Assert
        mode.AddCage(sum: 5, cells); // Should not throw
    }

    /// <summary>
    /// Tests that adding a cage with empty cells throws.
    /// </summary>
    [Fact]
    public void AddCage_WithEmptyCells_Throws()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => mode.AddCage(5, cells));
    }

    /// <summary>
    /// Tests that adding a cage with non-positive sum throws.
    /// </summary>
    [Fact]
    public void AddCage_WithNonPositiveSum_Throws()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => mode.AddCage(0, cells));
        Assert.Throws<ArgumentException>(() => mode.AddCage(-5, cells));
    }

    /// <summary>
    /// Tests that no duplicate digits in cage validates correctly.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithNoDuplicates_ReturnsTrue()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddCage(sum: 5, cells);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 2));
        board.SetCell(0, 1, new Cell(0, 1, value: 3));

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that duplicate digits in cage fail validation.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithDuplicate_ReturnsFalse()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddCage(sum: 5, cells);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 3));
        board.SetCell(0, 1, new Cell(0, 1, value: 3)); // Duplicate

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that cage sum exceeding target when complete fails.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithIncorrectSum_ReturnsFalse()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddCage(sum: 5, cells);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 3));
        board.SetCell(0, 1, new Cell(0, 1, value: 3)); // Sum is 6, exceeds target

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that partial cage passes validation.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithPartialCage_ReturnsTrue()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1), (0, 2) };
        mode.AddCage(sum: 6, cells);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));
        board.SetCell(0, 1, new Cell(0, 1, value: 2));

        // Act
        var result = mode.ValidateExtraRules(board, 0, 2);

        // Assert
        Assert.True(result); // Partial, sum is 3, under target
    }

    /// <summary>
    /// Tests that exact sum matches when cage complete.
    /// </summary>
    [Fact]
    public void ValidateExtraRules_WithExactSum_ReturnsTrue()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddCage(sum: 5, cells);

        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 2));
        board.SetCell(0, 1, new Cell(0, 1, value: 3));

        // Act
        var result = mode.ValidateExtraRules(board, 0, 1);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that GetConstraints returns killer cage constraints.
    /// </summary>
    [Fact]
    public void GetConstraints_ReturnsCageConstraints()
    {
        // Arrange
        var mode = new KillerMode();
        var cells = new List<(int, int)> { (0, 0), (0, 1) };
        mode.AddCage(sum: 5, cells);

        var board = SudokuBoard.CreateEmpty();

        // Act
        var constraints = mode.GetConstraints(board);

        // Assert
        Assert.Equal(2, constraints.Count);
        Assert.All(constraints, c => Assert.Equal(ConstraintType.KillerCage, c.Type));
    }

    /// <summary>
    /// Tests that GetValidDigits returns 1-9.
    /// </summary>
    [Fact]
    public void GetValidDigits_Returns1Through9()
    {
        // Arrange & Act
        var mode = new KillerMode();
        var digits = mode.GetValidDigits();

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, digits);
    }
}

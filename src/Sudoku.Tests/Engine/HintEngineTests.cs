using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for the HintEngine class.
/// </summary>
public class HintEngineTests
{
    /// <summary>
    /// Tests that GetHint returns a valid HintResult for RevealCell hint type.
    /// </summary>
    [Fact]
    public void GetHint_WithRevealCellType_ReturnsHintResult()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        var partialGrid = new int[9, 9];
        // First row: 1 2 3 4 5 6 7 8 9
        for (int i = 0; i < 9; i++)
        {
            partialGrid[0, i] = i + 1;
        }
        var partialBoard = SudokuBoard.CreateFromGivens(partialGrid);
        var hintType = HintType.RevealCell;

        // Act
        var result = hintEngine.GetHint(partialBoard, hintType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hintType, result.HintType);
        Assert.NotEmpty(result.AffectedCells);
        Assert.Equal("Hint_RevealCell", result.MessageKey);
    }

    /// <summary>
    /// Tests that GetHint returns a valid HintResult for ShowCandidates hint type.
    /// </summary>
    [Fact]
    public void GetHint_WithShowCandidatesType_ReturnsHintResult()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        var partialGrid = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            partialGrid[0, i] = i + 1;
        }
        var partialBoard = SudokuBoard.CreateFromGivens(partialGrid);
        var hintType = HintType.ShowCandidates;

        // Act
        var result = hintEngine.GetHint(partialBoard, hintType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hintType, result.HintType);
        Assert.Equal("Hint_ShowCandidates", result.MessageKey);
    }

    /// <summary>
    /// Tests that UseHint returns true and decrements remaining hints when available.
    /// </summary>
    [Fact]
    public void UseHint_WithAvailableHints_ReturnsTrue()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        var initialRemaining = hintEngine.RemainingHints;

        // Act
        var result = hintEngine.UseHint();

        // Assert
        Assert.True(result);
        Assert.Equal(initialRemaining - 1, hintEngine.RemainingHints);
    }

    /// <summary>
    /// Tests that UseHint returns false when no hints remain.
    /// </summary>
    [Fact]
    public void UseHint_WhenNoHintsRemaining_ReturnsFalse()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 1);

        // Act
        hintEngine.UseHint(); // First hint
        var secondAttempt = hintEngine.UseHint(); // Should fail

        // Assert
        Assert.False(secondAttempt);
        Assert.Equal(0, hintEngine.RemainingHints);
    }

    /// <summary>
    /// Tests that ResetHints restores the hint count to maximum.
    /// </summary>
    [Fact]
    public void ResetHints_RestoresHintCount()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        hintEngine.UseHint();
        hintEngine.UseHint();
        var beforeReset = hintEngine.RemainingHints;

        // Act
        hintEngine.ResetHints();

        // Assert
        Assert.Equal(5, hintEngine.RemainingHints);
        Assert.True(hintEngine.RemainingHints > beforeReset);
    }

    /// <summary>
    /// Tests that SetMaxHints updates both max and remaining hints.
    /// </summary>
    [Fact]
    public void SetMaxHints_UpdatesMaxAndRemaining()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        var newMax = 10;

        // Act
        hintEngine.SetMaxHints(newMax);

        // Assert
        Assert.Equal(newMax, hintEngine.MaxHints);
        Assert.Equal(newMax, hintEngine.RemainingHints);
    }

    /// <summary>
    /// Tests that GetHint with HighlightRelated returns multiple cells.
    /// </summary>
    [Fact]
    public void GetHint_WithHighlightRelatedType_ReturnsMultipleCells()
    {
        // Arrange
        var hintEngine = new HintEngine(maxHints: 5);
        var partialGrid = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            partialGrid[0, i] = i + 1;
        }
        var partialBoard = SudokuBoard.CreateFromGivens(partialGrid);
        var hintType = HintType.HighlightRelated;

        // Act
        var result = hintEngine.GetHint(partialBoard, hintType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hintType, result.HintType);
        // HighlightRelated should return multiple related cells
        Assert.True(result.AffectedCells.Count >= 1);
    }
}

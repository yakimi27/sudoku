using Sudoku.Core.Commands;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Commands;

/// <summary>
/// Unit tests for SetValueCommand.
/// </summary>
public class SetValueCommandTests
{
    /// <summary>
    /// Tests that Execute sets the cell to the new value.
    /// </summary>
    [Fact]
    public void Execute_SetsNewValue()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(0, 0, oldValue: 0, newValue: 5, oldState: CellState.Empty);

        // Act
        command.Execute(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Equal(5, cell.Value);
    }

    /// <summary>
    /// Tests that Undo restores the original cell state.
    /// </summary>
    [Fact]
    public void Undo_RestoresOldValue()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 3));
        var command = new SetValueCommand(0, 0, oldValue: 3, newValue: 7, oldState: CellState.Filled);
        command.Execute(board);

        // Act
        command.Undo(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Equal(3, cell.Value);
    }

    /// <summary>
    /// Tests that command stores execution timestamp.
    /// </summary>
    [Fact]
    public void ExecutedAt_IsSetToCurrentTime()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var command = new SetValueCommand(0, 0, 0, 5, CellState.Empty);
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(command.ExecutedAt >= before && command.ExecutedAt <= after);
    }

    /// <summary>
    /// Tests that Description provides readable command info.
    /// </summary>
    [Fact]
    public void Description_ReturnsReadableText()
    {
        // Arrange & Act
        var command = new SetValueCommand(2, 3, 1, 8, CellState.Empty);

        // Assert
        Assert.Contains("2", command.Description);
        Assert.Contains("3", command.Description);
        Assert.Contains("8", command.Description);
    }

    /// <summary>
    /// Tests that constructor validates row bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidRow_Throws(int invalidRow)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SetValueCommand(invalidRow, 0, 0, 5, CellState.Empty));
    }

    /// <summary>
    /// Tests that constructor validates column bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidColumn_Throws(int invalidCol)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SetValueCommand(0, invalidCol, 0, 5, CellState.Empty));
    }
}

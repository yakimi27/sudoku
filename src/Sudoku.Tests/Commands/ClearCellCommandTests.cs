using System.Collections.Immutable;
using Sudoku.Core.Commands;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Commands;

/// <summary>
/// Unit tests for ClearCellCommand.
/// </summary>
public class ClearCellCommandTests
{
    /// <summary>
    /// Tests that Execute clears the cell completely.
    /// </summary>
    [Fact]
    public void Execute_ClearsCell()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var filledCell = new Cell(0, 0, value: 5, state: CellState.Filled).WithNotes(ImmutableHashSet.Create(1, 3));
        board.SetCell(0, 0, filledCell);

        var command = new ClearCellCommand(0, 0, filledCell);

        // Act
        command.Execute(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Equal(0, cell.Value);
        Assert.Empty(cell.Notes);
    }

    /// <summary>
    /// Tests that Undo restores the complete cell snapshot.
    /// </summary>
    [Fact]
    public void Undo_RestoresFullSnapshot()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var originalCell = new Cell(0, 0, value: 7, state: CellState.Filled).WithNotes(ImmutableHashSet.Create(2, 4, 6));
        board.SetCell(0, 0, originalCell);

        var command = new ClearCellCommand(0, 0, originalCell);
        command.Execute(board);

        // Act
        command.Undo(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Equal(7, cell.Value);
        Assert.Equal(3, cell.Notes.Count);
        Assert.Contains(2, cell.Notes);
        Assert.Contains(4, cell.Notes);
        Assert.Contains(6, cell.Notes);
    }

    /// <summary>
    /// Tests that Description provides clear feedback.
    /// </summary>
    [Fact]
    public void Description_ReturnsReadableText()
    {
        // Arrange
        var cell = new Cell(3, 4, value: 5);
        var command = new ClearCellCommand(3, 4, cell);

        // Assert
        Assert.Contains("Clear", command.Description);
        Assert.Contains("3", command.Description);
        Assert.Contains("4", command.Description);
    }

    /// <summary>
    /// Tests that constructor validates row bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidRow_Throws(int invalidRow)
    {
        // Arrange
        var cell = new Cell(0, 0, value: 5);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ClearCellCommand(invalidRow, 0, cell));
    }

    /// <summary>
    /// Tests that constructor validates column bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidColumn_Throws(int invalidCol)
    {
        // Arrange
        var cell = new Cell(0, 0, value: 5);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ClearCellCommand(0, invalidCol, cell));
    }

    /// <summary>
    /// Tests that constructor throws when cell is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullCell_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ClearCellCommand(0, 0, cellBefore: null!));
    }
}

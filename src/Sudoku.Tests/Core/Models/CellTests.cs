using System.Collections.Immutable;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the Cell model.
/// </summary>
public class CellTests
{
    [Fact]
    public void Cell_WithValidArguments_CreatesSuccessfully()
    {
        // Arrange & Act
        var cell = new Cell(row: 0, column: 0, value: 5, isGiven: false, state: CellState.Filled);

        // Assert
        Assert.Equal(0, cell.Row);
        Assert.Equal(0, cell.Column);
        Assert.Equal(5, cell.Value);
        Assert.False(cell.IsGiven);
        Assert.Equal(CellState.Filled, cell.State);
    }

    [Fact]
    public void Cell_EmptyCell_HasDefaultState()
    {
        // Arrange & Act
        var cell = new Cell(3, 4);

        // Assert
        Assert.Equal(0, cell.Value);
        Assert.Equal(CellState.Empty, cell.State);
        Assert.True(cell.IsEmpty);
        Assert.True(cell.IsEditable);
    }

    [Fact]
    public void Cell_GivenCell_IsNotEditable()
    {
        // Arrange & Act
        var cell = new Cell(2, 2, value: 7, isGiven: true, state: CellState.Given);

        // Assert
        Assert.True(cell.IsGiven);
        Assert.False(cell.IsEditable);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(9, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 9)]
    [InlineData(5, 10)]
    public void Cell_InvalidRowOrColumn_ThrowsException(int row, int column)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(row, column));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void Cell_InvalidValue_ThrowsException(int value)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cell(0, 0, value));
    }

    [Fact]
    public void Cell_WithValue_CreatesNewCellWithUpdatedValue()
    {
        // Arrange
        var originalCell = new Cell(1, 1, value: 0);

        // Act
        var updatedCell = originalCell.WithValue(9);

        // Assert
        Assert.Equal(0, originalCell.Value);
        Assert.Equal(9, updatedCell.Value);
        Assert.Equal(CellState.Filled, updatedCell.State);
        Assert.Equal(1, updatedCell.Row);
        Assert.Equal(1, updatedCell.Column);
    }

    [Fact]
    public void Cell_WithNotes_CreatesNewCellWithCandidates()
    {
        // Arrange
        var originalCell = new Cell(2, 3);
        var notes = ImmutableHashSet.Create(1, 2, 3);

        // Act
        var cellWithNotes = originalCell.WithNotes(notes);

        // Assert
        Assert.Empty(originalCell.Notes);
        Assert.Equal(notes, cellWithNotes.Notes);
        Assert.Equal(CellState.Candidates, cellWithNotes.State);
    }

    [Fact]
    public void Cell_Equality_BasedOnPosition()
    {
        // Arrange
        var cell1 = new Cell(3, 4, value: 5);
        var cell2 = new Cell(3, 4, value: 7);
        var cell3 = new Cell(3, 5, value: 5);

        // Act & Assert
        Assert.Equal(cell1, cell2); // Same position, different values
        Assert.NotEqual(cell1, cell3); // Different positions
    }

    [Fact]
    public void Cell_HashCode_ConsistentWithPosition()
    {
        // Arrange
        var cell1 = new Cell(2, 5, value: 3);
        var cell2 = new Cell(2, 5, value: 9);

        // Act & Assert
        Assert.Equal(cell1.GetHashCode(), cell2.GetHashCode());
    }

    [Fact]
    public void Cell_ToString_ContainsPosition()
    {
        // Arrange
        var cell = new Cell(1, 2, value: 5, state: CellState.Filled);

        // Act
        var str = cell.ToString();

        // Assert
        Assert.Contains("1", str);
        Assert.Contains("2", str);
        Assert.Contains("5", str);
    }
}

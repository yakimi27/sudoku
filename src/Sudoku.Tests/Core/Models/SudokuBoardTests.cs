using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Core.Models;

/// <summary>
/// Unit tests for the SudokuBoard model.
/// </summary>
public class SudokuBoardTests
{
    [Fact]
    public void CreateEmpty_CreatesBoard_WithAllEmptyCells()
    {
        // Act
        var board = SudokuBoard.CreateEmpty();

        // Assert
        Assert.Equal(81, board.GetEmptyCellCount());
        var allCells = board.GetAllCells().ToList();
        Assert.All(allCells, cell => Assert.True(cell.IsEmpty));
    }

    [Fact]
    public void GetCell_ReturnsCorrectCell()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var cell = board.GetCell(0, 0);

        // Act & Assert
        Assert.Equal(0, cell.Row);
        Assert.Equal(0, cell.Column);
        Assert.Equal(CellState.Empty, cell.State);
    }

    [Fact]
    public void SetCell_UpdatesCell()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var newCell = new Cell(2, 3, value: 5, isGiven: false, state: CellState.Filled);

        // Act
        board.SetCell(2, 3, newCell);
        var retrieved = board.GetCell(2, 3);

        // Assert
        Assert.Equal(5, retrieved.Value);
        Assert.Equal(CellState.Filled, retrieved.State);
    }

    [Fact]
    public void GetRow_ReturnsNineCells()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var rowCells = board.GetRow(0).ToList();

        // Assert
        Assert.Equal(9, rowCells.Count);
        Assert.All(rowCells, cell => Assert.Equal(0, cell.Row));
    }

    [Fact]
    public void GetColumn_ReturnsNineCells()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var colCells = board.GetColumn(5).ToList();

        // Assert
        Assert.Equal(9, colCells.Count);
        Assert.All(colCells, cell => Assert.Equal(5, cell.Column));
    }

    [Fact]
    public void GetBox_ReturnsNineCells()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var boxCells = board.GetBox(0).ToList();

        // Assert
        Assert.Equal(9, boxCells.Count);
        // Box 0 contains rows 0-2, cols 0-2
        Assert.All(boxCells, cell =>
        {
            Assert.InRange(cell.Row, 0, 2);
            Assert.InRange(cell.Column, 0, 2);
        });
    }

    [Fact]
    public void GetBoxByPosition_ReturnsCorrectBox()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var boxCells = board.GetBoxByPosition(4, 4).ToList(); // Center cell (Box 4)

        // Assert
        Assert.Equal(9, boxCells.Count);
        // Box 4 (center) contains rows 3-5, cols 3-5
        Assert.All(boxCells, cell =>
        {
            Assert.InRange(cell.Row, 3, 5);
            Assert.InRange(cell.Column, 3, 5);
        });
    }

    [Fact]
    public void CreateFromGivens_CreatesBoard_WithGivenValues()
    {
        // Arrange
        var givens = new int[9, 9];
        givens[0, 0] = 5;
        givens[0, 1] = 3;
        givens[1, 0] = 6;

        // Act
        var board = SudokuBoard.CreateFromGivens(givens);

        // Assert
        Assert.Equal(5, board.GetCell(0, 0).Value);
        Assert.True(board.GetCell(0, 0).IsGiven);
        Assert.Equal(CellState.Given, board.GetCell(0, 0).State);

        Assert.Equal(0, board.GetCell(2, 2).Value);
        Assert.False(board.GetCell(2, 2).IsGiven);
    }

    [Fact]
    public void GetEmptyCellCount_ReturnsCorrectCount()
    {
        // Arrange
        var givens = new int[9, 9];
        for (int i = 0; i < 5; i++)
        {
            givens[i, i] = i + 1;
        }
        var board = SudokuBoard.CreateFromGivens(givens);

        // Act
        var emptyCells = board.GetEmptyCellCount();

        // Assert
        Assert.Equal(76, emptyCells); // 81 - 5 = 76
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = SudokuBoard.CreateEmpty();
        var cell = new Cell(0, 0, value: 9, state: CellState.Filled);
        original.SetCell(0, 0, cell);

        // Act
        var cloned = original.Clone();
        cloned.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Assert
        Assert.Equal(9, original.GetCell(0, 0).Value);
        Assert.Equal(1, cloned.GetCell(0, 0).Value);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(9, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 9)]
    public void GetCell_InvalidPosition_ThrowsException(int row, int column)
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => board.GetCell(row, column));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void GetRow_InvalidRow_ThrowsException(int row)
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => board.GetRow(row).ToList());
    }

    [Fact]
    public void GetAllCells_Returns81Cells()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var allCells = board.GetAllCells().ToList();

        // Assert
        Assert.Equal(81, allCells.Count);
    }

    [Fact]
    public void CreateFromGivens_InvalidArray_ThrowsException()
    {
        // Arrange
        var invalidGivens = new int[8, 8]; // Wrong size

        // Act & Assert
        Assert.Throws<ArgumentException>(() => SudokuBoard.CreateFromGivens(invalidGivens));
    }
}

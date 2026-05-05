using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for BoardSolver.
/// </summary>
public class BoardSolverTests
{
    /// <summary>
    /// Tests that a valid solvable board is correctly identified.
    /// </summary>
    [Fact]
    public void IsSolvable_WithValidBoard_ReturnsTrue()
    {
        // Arrange
        var board = CreateSimpleSolvableBoard();

        // Act
        var result = BoardSolver.IsSolvable(board);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that an unsolvable board is correctly identified.
    /// </summary>
    [Fact]
    public void IsSolvable_WithUnsolvableBoard_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        // Add conflicting values to make it unsolvable
        board.SetCell(0, 0, new Cell(0, 0, value: 1));
        board.SetCell(0, 1, new Cell(0, 1, value: 1)); // Duplicate in row

        // Act
        var result = BoardSolver.IsSolvable(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Solve returns a valid solution for a solvable board.
    /// </summary>
    [Fact]
    public void Solve_WithValidBoard_ReturnsSolvedBoard()
    {
        // Arrange
        var board = CreateSimpleSolvableBoard();

        // Act
        var solved = BoardSolver.Solve(board);

        // Assert
        Assert.NotNull(solved);
        Assert.Equal(0, solved.GetEmptyCellCount());
        Assert.True(BoardValidator.IsSolved(solved));
    }

    /// <summary>
    /// Tests that Solve returns null for an unsolvable board.
    /// </summary>
    [Fact]
    public void Solve_WithUnsolvableBoard_ReturnsNull()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));
        board.SetCell(0, 1, new Cell(0, 1, value: 1));

        // Act
        var result = BoardSolver.Solve(board);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that HasUniqueSolution correctly identifies boards with unique solutions.
    /// </summary>
    [Fact]
    public void HasUniqueSolution_WithUniqueBoard_ReturnsTrue()
    {
        // Arrange
        var board = CreateSimpleSolvableBoard();

        // Act
        var result = BoardSolver.HasUniqueSolution(board);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that HasUniqueSolution correctly identifies boards with multiple solutions.
    /// </summary>
    [Fact]
    public void HasUniqueSolution_WithMultipleSolutions_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        // Add only a few clues to ensure multiple solutions exist
        board.SetCell(0, 0, new Cell(0, 0, value: 1));
        board.SetCell(1, 1, new Cell(1, 1, value: 2));

        // Act
        var result = BoardSolver.HasUniqueSolution(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that IsValidMove correctly validates legal moves.
    /// </summary>
    [Fact]
    public void IsValidMove_WithLegalMove_ReturnsTrue()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardSolver.IsValidMove(board, 0, 1, 2); // Different column, different value

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that IsValidMove correctly rejects duplicate values in row.
    /// </summary>
    [Fact]
    public void IsValidMove_WithDuplicateInRow_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardSolver.IsValidMove(board, 0, 1, 1); // Same value in same row

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that IsValidMove correctly rejects duplicate values in column.
    /// </summary>
    [Fact]
    public void IsValidMove_WithDuplicateInColumn_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardSolver.IsValidMove(board, 1, 0, 1); // Same value in same column

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that IsValidMove correctly rejects duplicate values in 3x3 block.
    /// </summary>
    [Fact]
    public void IsValidMove_WithDuplicateInBlock_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardSolver.IsValidMove(board, 1, 1, 1); // Same value in same 3x3 block

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that GetCandidates returns valid candidates for an empty cell.
    /// </summary>
    [Fact]
    public void GetCandidates_WithEmptyCell_ReturnsCandidates()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var candidates = BoardSolver.GetCandidates(board, 0, 1);

        // Assert
        Assert.NotEmpty(candidates);
        Assert.DoesNotContain(1, candidates); // 1 is already in row
    }

    /// <summary>
    /// Tests that GetCandidates returns empty for filled cell.
    /// </summary>
    [Fact]
    public void GetCandidates_WithFilledCell_ReturnsEmpty()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));

        // Act
        var candidates = BoardSolver.GetCandidates(board, 0, 0);

        // Assert
        Assert.Empty(candidates);
    }

    /// <summary>
    /// Helper: Creates a simple solvable Sudoku board.
    /// </summary>
    private static SudokuBoard CreateSimpleSolvableBoard()
    {
        var board = SudokuBoard.CreateEmpty();

        // Add a few clues to make it solvable with unique solution
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(0, 1, new Cell(0, 1, value: 3));
        board.SetCell(0, 4, new Cell(0, 4, value: 7));
        board.SetCell(1, 0, new Cell(1, 0, value: 6));
        board.SetCell(1, 3, new Cell(1, 3, value: 1));
        board.SetCell(1, 4, new Cell(1, 4, value: 9));
        board.SetCell(1, 5, new Cell(1, 5, value: 5));
        board.SetCell(2, 1, new Cell(2, 1, value: 9));
        board.SetCell(2, 2, new Cell(2, 2, value: 8));
        board.SetCell(2, 5, new Cell(2, 5, value: 6));
        board.SetCell(3, 0, new Cell(3, 0, value: 8));
        board.SetCell(3, 4, new Cell(3, 4, value: 6));
        board.SetCell(3, 8, new Cell(3, 8, value: 3));
        board.SetCell(4, 0, new Cell(4, 0, value: 4));
        board.SetCell(4, 3, new Cell(4, 3, value: 8));
        board.SetCell(4, 5, new Cell(4, 5, value: 3));
        board.SetCell(4, 8, new Cell(4, 8, value: 1));
        board.SetCell(5, 0, new Cell(5, 0, value: 7));
        board.SetCell(5, 4, new Cell(5, 4, value: 2));
        board.SetCell(5, 8, new Cell(5, 8, value: 6));
        board.SetCell(6, 3, new Cell(6, 3, value: 4));
        board.SetCell(6, 6, new Cell(6, 6, value: 7));
        board.SetCell(6, 7, new Cell(6, 7, value: 8));
        board.SetCell(7, 1, new Cell(7, 1, value: 8));
        board.SetCell(7, 3, new Cell(7, 3, value: 6));
        board.SetCell(7, 7, new Cell(7, 7, value: 2));

        return board;
    }
}

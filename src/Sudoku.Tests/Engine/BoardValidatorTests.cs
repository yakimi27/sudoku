using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for BoardValidator.
/// </summary>
public class BoardValidatorTests
{
    /// <summary>
    /// Tests that a valid empty board passes validation.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyBoard_ReturnsValid()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var result = BoardValidator.Validate(board);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(ValidationType.None, result.ErrorType);
        Assert.Empty(result.ErrorCells);
    }

    /// <summary>
    /// Tests that a valid completed board passes validation.
    /// </summary>
    [Fact]
    public void Validate_WithValidCompletedBoard_ReturnsValid()
    {
        // Arrange
        var board = CreateValidCompletedBoard();

        // Act
        var result = BoardValidator.Validate(board);

        // Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Tests that duplicate values in a row are detected.
    /// </summary>
    [Fact]
    public void Validate_WithDuplicateInRow_ReturnsDuplicateInRowError()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(0, 1, new Cell(0, 1, value: 5)); // Duplicate in same row

        // Act
        var result = BoardValidator.Validate(board);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ValidationType.DuplicateInRow, result.ErrorType);
        Assert.NotEmpty(result.ErrorCells);
    }

    /// <summary>
    /// Tests that duplicate values in a column are detected.
    /// </summary>
    [Fact]
    public void Validate_WithDuplicateInColumn_ReturnsDuplicateInColumnError()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(1, 0, new Cell(1, 0, value: 5)); // Duplicate in same column

        // Act
        var result = BoardValidator.Validate(board);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ValidationType.DuplicateInColumn, result.ErrorType);
        Assert.NotEmpty(result.ErrorCells);
    }

    /// <summary>
    /// Tests that duplicate values in a 3x3 block are detected.
    /// </summary>
    [Fact]
    public void Validate_WithDuplicateInBlock_ReturnsDuplicateInBlockError()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));
        board.SetCell(1, 1, new Cell(1, 1, value: 5)); // Same 3x3 block (0)

        // Act
        var result = BoardValidator.Validate(board);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ValidationType.DuplicateInBlock, result.ErrorType);
        Assert.NotEmpty(result.ErrorCells);
    }

    /// <summary>
    /// Tests that IsValidMove returns true for valid moves.
    /// </summary>
    [Fact]
    public void IsValidMove_WithLegalMove_ReturnsTrue()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardValidator.IsValidMove(board, 0, 1, 2);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that IsValidMove returns false for moves that create row conflict.
    /// </summary>
    [Fact]
    public void IsValidMove_WithRowConflict_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1));

        // Act
        var result = BoardValidator.IsValidMove(board, 0, 1, 1);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that IsSolved correctly identifies a completed valid board.
    /// </summary>
    [Fact]
    public void IsSolved_WithValidCompletedBoard_ReturnsTrue()
    {
        // Arrange
        var board = CreateValidCompletedBoard();

        // Act
        var result = BoardValidator.IsSolved(board);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that IsSolved returns false for incomplete boards.
    /// </summary>
    [Fact]
    public void IsSolved_WithIncompleteBoard_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 5));

        // Act
        var result = BoardValidator.IsSolved(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that IsSolved returns false for invalid boards even if complete.
    /// </summary>
    [Fact]
    public void IsSolved_WithInvalidCompletedBoard_ReturnsFalse()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        // Fill first row with all 1s (invalid)
        for (int col = 0; col < 9; col++)
        {
            board.SetCell(0, col, new Cell(0, col, value: 1));
        }

        // Act
        var result = BoardValidator.IsSolved(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that ValidationResult.Success creates valid result.
    /// </summary>
    [Fact]
    public void ValidationResult_Success_CreatesValidResult()
    {
        // Act
        var result = ValidationResult.Success();

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(ValidationType.None, result.ErrorType);
    }

    /// <summary>
    /// Tests that ValidationResult.Failure creates failure result.
    /// </summary>
    [Fact]
    public void ValidationResult_Failure_CreatesFailureResult()
    {
        // Act
        var result = ValidationResult.Failure(ValidationType.DuplicateInRow, "Test error");

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ValidationType.DuplicateInRow, result.ErrorType);
        Assert.Equal("Test error", result.ErrorMessage);
    }

    /// <summary>
    /// Helper: Creates a valid completed Sudoku board.
    /// </summary>
    private static SudokuBoard CreateValidCompletedBoard()
    {
        int[,] gridData = new[,]
        {
            { 5, 3, 4, 6, 7, 8, 9, 1, 2 },
            { 6, 7, 2, 1, 9, 5, 3, 4, 8 },
            { 1, 9, 8, 3, 4, 2, 5, 6, 7 },
            { 8, 5, 9, 7, 6, 1, 4, 2, 3 },
            { 4, 2, 6, 8, 5, 3, 7, 9, 1 },
            { 7, 1, 3, 9, 2, 4, 8, 5, 6 },
            { 9, 6, 1, 5, 3, 7, 2, 8, 4 },
            { 2, 8, 7, 4, 1, 9, 6, 3, 5 },
            { 3, 4, 5, 2, 8, 6, 1, 7, 9 }
        };

        var board = SudokuBoard.CreateEmpty();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = new Cell(row, col, value: gridData[row, col]);
                board.SetCell(row, col, cell);
            }
        }

        return board;
    }
}

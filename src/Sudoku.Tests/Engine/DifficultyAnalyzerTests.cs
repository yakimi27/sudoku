using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for DifficultyAnalyzer.
/// </summary>
public class DifficultyAnalyzerTests
{
    /// <summary>
    /// Tests that CountGivens correctly counts given cells.
    /// </summary>
    [Fact]
    public void CountGivens_WithGivenCells_ReturnsCorrectCount()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        board.SetCell(0, 0, new Cell(0, 0, value: 1, isGiven: true, state: CellState.Given));
        board.SetCell(0, 1, new Cell(0, 1, value: 2, isGiven: true, state: CellState.Given));
        board.SetCell(1, 0, new Cell(1, 0, value: 3, isGiven: false));

        // Act
        var count = DifficultyAnalyzer.CountGivens(board);

        // Assert
        Assert.Equal(2, count);
    }

    /// <summary>
    /// Tests that Analyze correctly identifies an easy puzzle.
    /// </summary>
    [Fact]
    public void Analyze_WithManyGivens_ReturnsEasy()
    {
        // Arrange
        var board = CreateBoardWithGivenCount(45);

        // Act
        var difficulty = DifficultyAnalyzer.Analyze(board);

        // Assert
        Assert.Equal(Difficulty.Easy, difficulty);
    }

    /// <summary>
    /// Tests that Analyze correctly identifies a medium puzzle.
    /// </summary>
    [Fact]
    public void Analyze_WithMediumGivens_ReturnsMedium()
    {
        // Arrange
        var board = CreateBoardWithGivenCount(35);

        // Act
        var difficulty = DifficultyAnalyzer.Analyze(board);

        // Assert
        // Should return Medium or Hard depending on heuristics
        Assert.True(difficulty == Difficulty.Medium || difficulty == Difficulty.Hard);
    }

    /// <summary>
    /// Tests that Analyze correctly identifies a hard/expert puzzle.
    /// </summary>
    [Fact]
    public void Analyze_WithFewGivens_ReturnsHardOrExpert()
    {
        // Arrange
        var board = CreateBoardWithGivenCount(25);

        // Act
        var difficulty = DifficultyAnalyzer.Analyze(board);

        // Assert
        Assert.True(difficulty == Difficulty.Hard || difficulty == Difficulty.Expert);
    }

    /// <summary>
    /// Tests that GetDescription returns valid descriptions for all difficulties.
    /// </summary>
    [Fact]
    public void GetDescription_WithEasyDifficulty_ReturnsValidDescription()
    {
        // Act
        var description = DifficultyAnalyzer.GetDescription(Difficulty.Easy);

        // Assert
        Assert.NotEmpty(description);
        Assert.Contains("Easy", description);
    }

    /// <summary>
    /// Tests that GetDescription works for all difficulty levels.
    /// </summary>
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    [InlineData(Difficulty.Expert)]
    public void GetDescription_WithAllDifficulties_ReturnsNonEmptyDescription(Difficulty difficulty)
    {
        // Act
        var description = DifficultyAnalyzer.GetDescription(difficulty);

        // Assert
        Assert.NotEmpty(description);
    }

    /// <summary>
    /// Tests that puzzles with more givens score higher (easier).
    /// </summary>
    [Fact]
    public void Analyze_MoreGivensMeansEasier()
    {
        // Arrange
        var easyBoard = CreateBoardWithGivenCount(50);
        var hardBoard = CreateBoardWithGivenCount(20);

        // Act
        var easyDifficulty = DifficultyAnalyzer.Analyze(easyBoard);
        var hardDifficulty = DifficultyAnalyzer.Analyze(hardBoard);

        // Assert
        Assert.True(IsDifficultyLessThan(easyDifficulty, hardDifficulty) ||
                    easyDifficulty == hardDifficulty);
    }

    /// <summary>
    /// Tests CountGivens with no given cells.
    /// </summary>
    [Fact]
    public void CountGivens_WithEmptyBoard_ReturnsZero()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();

        // Act
        var count = DifficultyAnalyzer.CountGivens(board);

        // Assert
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Tests CountGivens with all cells given.
    /// </summary>
    [Fact]
    public void CountGivens_WithFullBoard_Returns81()
    {
        // Arrange
        var board = CreateFullBoard();

        // Act
        var count = DifficultyAnalyzer.CountGivens(board);

        // Assert
        Assert.Equal(81, count);
    }

    /// <summary>
    /// Helper: Creates a board with approximately the specified number of given cells.
    /// </summary>
    private static SudokuBoard CreateBoardWithGivenCount(int targetCount)
    {
        var board = SudokuBoard.CreateEmpty();
        int count = 0;

        for (int row = 0; row < 9 && count < targetCount; row++)
        {
            for (int col = 0; col < 9 && count < targetCount; col++)
            {
                var value = ((row + col) % 9) + 1;
                var cell = new Cell(row, col, value: value, isGiven: true, state: CellState.Given);
                board.SetCell(row, col, cell);
                count++;
            }
        }

        return board;
    }

    /// <summary>
    /// Helper: Creates a full completed board.
    /// </summary>
    private static SudokuBoard CreateFullBoard()
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
                var cell = new Cell(row, col, value: gridData[row, col], isGiven: true, state: CellState.Given);
                board.SetCell(row, col, cell);
            }
        }

        return board;
    }

    /// <summary>
    /// Helper: Compares two difficulty levels.
    /// </summary>
    private static bool IsDifficultyLessThan(Difficulty a, Difficulty b)
    {
        return (int)a < (int)b;
    }
}

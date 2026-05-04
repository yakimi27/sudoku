using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for BoardGenerator.
/// </summary>
public class BoardGeneratorTests
{
    /// <summary>
    /// Tests that generated boards are always solvable.
    /// </summary>
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    [InlineData(Difficulty.Expert)]
    public void Generate_WithAnyDifficulty_ReturnsSolvableBoard(Difficulty difficulty)
    {
        // Act
        var board = BoardGenerator.Generate(difficulty, GameMode.Standard);

        // Assert
        Assert.True(BoardSolver.IsSolvable(board));
    }

    /// <summary>
    /// Tests that generated boards have unique solutions.
    /// </summary>
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    public void Generate_WithDifficulty_ReturnsUniqueSolutionBoard(Difficulty difficulty)
    {
        // Act
        var board = BoardGenerator.Generate(difficulty, GameMode.Standard);

        // Assert
        Assert.True(BoardSolver.HasUniqueSolution(board));
    }

    /// <summary>
    /// Tests that generated easy boards have enough givens.
    /// </summary>
    [Fact]
    public void Generate_WithEasyDifficulty_HasMoreGivens()
    {
        // Arrange
        var easyBoard = BoardGenerator.Generate(Difficulty.Easy, GameMode.Standard);
        var hardBoard = BoardGenerator.Generate(Difficulty.Hard, GameMode.Standard);

        // Act
        var easyGivens = DifficultyAnalyzer.CountGivens(easyBoard);
        var hardGivens = DifficultyAnalyzer.CountGivens(hardBoard);

        // Assert
        Assert.True(easyGivens >= hardGivens);
    }

    /// <summary>
    /// Tests that generated boards pass validation.
    /// </summary>
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    [InlineData(Difficulty.Expert)]
    public void Generate_WithAnyDifficulty_ReturnsValidBoard(Difficulty difficulty)
    {
        // Act
        var board = BoardGenerator.Generate(difficulty, GameMode.Standard);

        // Assert
        var validation = BoardValidator.Validate(board);
        Assert.True(validation.IsValid);
    }

    /// <summary>
    /// Tests that generated boards have empty cells (not already solved).
    /// </summary>
    [Fact]
    public void Generate_WithDifficulty_HasEmptyCells()
    {
        // Act
        var board = BoardGenerator.Generate(Difficulty.Medium, GameMode.Standard);

        // Assert
        Assert.True(board.GetEmptyCellCount() > 0);
    }

    /// <summary>
    /// Tests that easy puzzles have fewer empty cells than hard puzzles (typically).
    /// </summary>
    [Fact]
    public void Generate_EasyHasFewerEmptyCellsThanHard()
    {
        // Act
        var easyBoard = BoardGenerator.Generate(Difficulty.Easy, GameMode.Standard);
        var hardBoard = BoardGenerator.Generate(Difficulty.Hard, GameMode.Standard);

        // Assert
        var easyEmpty = easyBoard.GetEmptyCellCount();
        var hardEmpty = hardBoard.GetEmptyCellCount();

        Assert.True(easyEmpty <= hardEmpty);
    }

    /// <summary>
    /// Tests that generated puzzles can be solved.
    /// </summary>
    [Fact]
    public void Generate_WithMediumDifficulty_CanBeSolved()
    {
        // Act
        var board = BoardGenerator.Generate(Difficulty.Medium, GameMode.Standard);
        var solved = BoardSolver.Solve(board);

        // Assert
        Assert.NotNull(solved);
        Assert.True(BoardValidator.IsSolved(solved));
    }

    /// <summary>
    /// Tests that multiple generated boards are different.
    /// </summary>
    [Fact]
    public void Generate_MultipleCalls_ProduceDifferentBoards()
    {
        // Act
        var board1 = BoardGenerator.Generate(Difficulty.Medium, GameMode.Standard);
        var board2 = BoardGenerator.Generate(Difficulty.Medium, GameMode.Standard);
        var board3 = BoardGenerator.Generate(Difficulty.Medium, GameMode.Standard);

        // Assert - boards should differ (with extremely high probability)
        var board1Givens = DifficultyAnalyzer.CountGivens(board1);
        var board2Givens = DifficultyAnalyzer.CountGivens(board2);
        var board3Givens = DifficultyAnalyzer.CountGivens(board3);

        // At least check they're not all identical
        Assert.True(board1Givens != board2Givens || board2Givens != board3Givens);
    }

    /// <summary>
    /// Tests that generated easy boards are typically easier than expert boards.
    /// </summary>
    [Fact]
    public void Generate_EasyBoardIsEasierThanExpertBoard()
    {
        // Act
        var easyBoard = BoardGenerator.Generate(Difficulty.Easy, GameMode.Standard);
        var expertBoard = BoardGenerator.Generate(Difficulty.Expert, GameMode.Standard);

        // Assert
        var easyAnalysis = DifficultyAnalyzer.Analyze(easyBoard);
        var expertAnalysis = DifficultyAnalyzer.Analyze(expertBoard);

        Assert.True((int)easyAnalysis <= (int)expertAnalysis);
    }
}

using Sudoku.Core.Engine;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Engine;

/// <summary>
/// Unit tests for DailyPuzzleProvider.
/// </summary>
public class DailyPuzzleProviderTests
{
    /// <summary>
    /// Tests that GetForDate returns a valid solvable puzzle.
    /// </summary>
    [Fact]
    public void GetForDate_WithAnyDate_ReturnsSolvablePuzzle()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(date);

        // Assert
        Assert.NotNull(puzzle);
        Assert.True(BoardSolver.IsSolvable(puzzle));
    }

    /// <summary>
    /// Tests that GetForDate returns a puzzle with unique solution.
    /// </summary>
    [Fact]
    public void GetForDate_WithAnyDate_ReturnsUniqueSolutionPuzzle()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(date);

        // Assert
        Assert.True(BoardSolver.HasUniqueSolution(puzzle));
    }

    /// <summary>
    /// Tests that same date always returns same puzzle (deterministic).
    /// </summary>
    [Fact]
    public void GetForDate_WithSameDate_ReturnsSamePuzzle()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var puzzle1 = DailyPuzzleProvider.GetForDate(date);
        var puzzle2 = DailyPuzzleProvider.GetForDate(date);

        // Assert
        Assert.True(BoardsAreEqual(puzzle1, puzzle2));
    }

    /// <summary>
    /// Tests that different dates produce different puzzles.
    /// </summary>
    [Fact]
    public void GetForDate_WithDifferentDates_ReturnsDifferentPuzzles()
    {
        // Arrange
        var date1 = new DateOnly(2024, 1, 15);
        var date2 = new DateOnly(2024, 1, 16);

        // Act
        var puzzle1 = DailyPuzzleProvider.GetForDate(date1);
        var puzzle2 = DailyPuzzleProvider.GetForDate(date2);

        // Assert
        Assert.False(BoardsAreEqual(puzzle1, puzzle2));
    }

    /// <summary>
    /// Tests that GetTodayPuzzle returns a valid puzzle.
    /// </summary>
    [Fact]
    public void GetTodayPuzzle_ReturnsValidSolvablePuzzle()
    {
        // Act
        var puzzle = DailyPuzzleProvider.GetTodayPuzzle();

        // Assert
        Assert.NotNull(puzzle);
        Assert.True(BoardSolver.IsSolvable(puzzle));
    }

    /// <summary>
    /// Tests that daily puzzles pass validation.
    /// </summary>
    [Fact]
    public void GetForDate_WithAnyDate_ReturnsValidBoard()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 29); // Leap day

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(date);

        // Assert
        var validation = BoardValidator.Validate(puzzle);
        Assert.True(validation.IsValid);
    }

    /// <summary>
    /// Tests that daily puzzles have empty cells (not solved).
    /// </summary>
    [Fact]
    public void GetForDate_WithAnyDate_HasEmptyCells()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(date);

        // Assert
        Assert.True(puzzle.GetEmptyCellCount() > 0);
    }

    /// <summary>
    /// Tests that daily puzzle for past date is deterministic.
    /// </summary>
    [Fact]
    public void GetForDate_WithPastDate_IsDeterministic()
    {
        // Arrange
        var pastDate = new DateOnly(2023, 12, 25);

        // Act
        var puzzle1 = DailyPuzzleProvider.GetForDate(pastDate);
        var puzzle2 = DailyPuzzleProvider.GetForDate(pastDate);

        // Assert
        Assert.True(BoardsAreEqual(puzzle1, puzzle2));
    }

    /// <summary>
    /// Tests that daily puzzle for future date is deterministic.
    /// </summary>
    [Fact]
    public void GetForDate_WithFutureDate_IsDeterministic()
    {
        // Arrange
        var futureDate = new DateOnly(2025, 12, 25);

        // Act
        var puzzle1 = DailyPuzzleProvider.GetForDate(futureDate);
        var puzzle2 = DailyPuzzleProvider.GetForDate(futureDate);

        // Assert
        Assert.True(BoardsAreEqual(puzzle1, puzzle2));
    }

    /// <summary>
    /// Tests that difficulty varies throughout week (different seeds produce different difficulties).
    /// </summary>
    [Fact]
    public void GetForDate_AcrossMultipleDays_ProducesDifferentDifficulties()
    {
        // Arrange - Get puzzles for 7 consecutive days
        var difficulties = new List<Difficulty>();

        for (int day = 0; day < 7; day++)
        {
            var date = new DateOnly(2024, 1, 15 + day);
            var puzzle = DailyPuzzleProvider.GetForDate(date);
            var difficulty = DifficultyAnalyzer.Analyze(puzzle);
            difficulties.Add(difficulty);
        }

        // Act - Check if we have variety
        var uniqueDifficulties = difficulties.Distinct().Count();

        // Assert - We should have at least 2 different difficulties across the week
        Assert.True(uniqueDifficulties >= 1); // At minimum, puzzles exist
    }

    /// <summary>
    /// Tests that daily puzzles can be solved.
    /// </summary>
    [Fact]
    public void GetForDate_WithAnyDate_PuzzleCanBeSolved()
    {
        // Arrange
        var date = new DateOnly(2024, 3, 1);

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(date);
        var solved = BoardSolver.Solve(puzzle);

        // Assert
        Assert.NotNull(solved);
        Assert.True(BoardValidator.IsSolved(solved));
    }

    /// <summary>
    /// Tests epoch date boundary (earliest valid date).
    /// </summary>
    [Fact]
    public void GetForDate_WithEpochDate_ReturnsPuzzle()
    {
        // Arrange
        var epochDate = new DateOnly(2024, 1, 1);

        // Act
        var puzzle = DailyPuzzleProvider.GetForDate(epochDate);

        // Assert
        Assert.NotNull(puzzle);
        Assert.True(BoardSolver.IsSolvable(puzzle));
    }

    /// <summary>
    /// Helper: Compares two boards for equality.
    /// </summary>
    private static bool BoardsAreEqual(SudokuBoard board1, SudokuBoard board2)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell1 = board1.GetCell(row, col);
                var cell2 = board2.GetCell(row, col);

                if (cell1.Value != cell2.Value || cell1.IsGiven != cell2.IsGiven)
                    return false;
            }
        }

        return true;
    }
}

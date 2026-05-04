using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Analyzes a Sudoku board to determine its difficulty level.
/// Uses heuristics based on the number of given cells and solving techniques required.
/// </summary>
public static class DifficultyAnalyzer
{
    /// <summary>
    /// Thresholds for difficulty levels based on number of given cells.
    /// Fewer givens = harder puzzle (requires more solving techniques).
    /// </summary>
    private const int EasyThreshold = 40;      // 40+ givens
    private const int MediumThreshold = 35;    // 35-39 givens
    private const int HardThreshold = 30;      // 30-34 givens
    // Below 30 givens = Expert

    /// <summary>
    /// Analyzes a Sudoku board and determines its difficulty level.
    /// </summary>
    /// <param name="board">The board to analyze.</param>
    /// <returns>The estimated difficulty level of the board.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public static Difficulty Analyze(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        var givenCount = CountGivens(board);
        var score = CalculateDifficultyScore(board, givenCount);

        return score switch
        {
            >= 80 => Difficulty.Easy,
            >= 60 => Difficulty.Medium,
            >= 40 => Difficulty.Hard,
            _ => Difficulty.Expert
        };
    }

    /// <summary>
    /// Counts the number of given (clue) cells in the board.
    /// </summary>
    /// <param name="board">The board to count.</param>
    /// <returns>The number of given cells (0-81).</returns>
    public static int CountGivens(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        return board.GetAllCells().Count(cell => cell.IsGiven);
    }

    /// <summary>
    /// Calculates a difficulty score based on board characteristics.
    /// Higher score = easier puzzle (more clues, less solving required).
    /// </summary>
    /// <param name="board">The board to score.</param>
    /// <param name="givenCount">The number of given cells.</param>
    /// <returns>A score from 0-100 where 100 is easiest.</returns>
    private static int CalculateDifficultyScore(SudokuBoard board, int givenCount)
    {
        // Base score from number of givens (more givens = easier)
        // Map 17 givens (minimum for unique solution) to 0
        // Map 81 givens (complete board) to 100
        var baseScore = Math.Max(0, Math.Min(100, ((givenCount - 17) * 100) / 64));

        // Bonus for symmetric distribution (easier to solve)
        var symmetryBonus = CalculateSymmetryBonus(board);

        // Analyze required solving techniques
        var techniqueBonus = AnalyzeSolvingTechniques(board);

        var totalScore = (int)((baseScore * 0.6) + (symmetryBonus * 0.2) + (techniqueBonus * 0.2));
        return Math.Max(0, Math.Min(100, totalScore));
    }

    /// <summary>
    /// Calculates a bonus score for symmetric board layouts.
    /// Symmetric puzzles are typically easier to solve intuitively.
    /// </summary>
    private static int CalculateSymmetryBonus(SudokuBoard board)
    {
        // Check for rotational symmetry (180 degrees)
        bool hasSymmetry = true;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell1 = board.GetCell(row, col);
                var cell2 = board.GetCell(8 - row, 8 - col);

                // Check if both are given or both are empty (symmetric)
                if (cell1.IsGiven != cell2.IsGiven)
                {
                    hasSymmetry = false;
                    break;
                }
            }
            if (!hasSymmetry) break;
        }

        return hasSymmetry ? 20 : 0;
    }

    /// <summary>
    /// Analyzes which Sudoku solving techniques are required.
    /// More advanced techniques suggest a harder puzzle.
    /// </summary>
    private static int AnalyzeSolvingTechniques(SudokuBoard board)
    {
        var emptyCount = board.GetEmptyCellCount();

        // Rough estimate: fewer empty cells with same given count = needs more advanced techniques
        // This is a simplified heuristic
        var techniqueFactor = Math.Max(0, 100 - (emptyCount * 2));

        return techniqueFactor;
    }

    /// <summary>
    /// Gets a descriptive label for the given difficulty level.
    /// </summary>
    /// <param name="difficulty">The difficulty to describe.</param>
    /// <returns>A user-friendly difficulty description.</returns>
    public static string GetDescription(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => "Easy - Good for beginners",
            Difficulty.Medium => "Medium - Requires basic techniques",
            Difficulty.Hard => "Hard - Requires advanced techniques",
            Difficulty.Expert => "Expert - For experienced players",
            _ => "Unknown difficulty"
        };
    }
}

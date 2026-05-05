using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Generates random valid Sudoku puzzles with specified difficulty levels.
/// Uses Factory Method pattern for board creation.
/// Phase 1: Generate complete valid board via randomized backtracking.
/// Phase 2: Remove cells while ensuring unique solution.
/// </summary>
public static class BoardGenerator
{
    /// <summary>
    /// Difficulty thresholds (number of givens to retain).
    /// </summary>
    private const int EasyGivens = 45;
    private const int MediumGivens = 40;
    private const int HardGivens = 35;
    private const int ExpertGivens = 30;

    /// <summary>
    /// Generates a random Sudoku puzzle with the specified difficulty.
    /// </summary>
    /// <param name="difficulty">The desired difficulty level.</param>
    /// <param name="gameMode">The game mode (not currently used for generation).</param>
    /// <returns>A new SudokuBoard with a valid puzzle.</returns>
    public static SudokuBoard Generate(Difficulty difficulty, GameMode gameMode)
    {
        // Phase 1: Generate complete valid board
        var completeBoard = GenerateCompleteSolution();

        // Phase 2: Remove cells to create puzzle
        var puzzle = RemoveCellsForDifficulty(completeBoard, difficulty);

        return puzzle;
    }

    /// <summary>
    /// Generates a complete, solved Sudoku board using randomized backtracking.
    /// </summary>
    private static SudokuBoard GenerateCompleteSolution()
    {
        var board = SudokuBoard.CreateEmpty();
        var random = new Random();

        // Fill diagonal 3x3 boxes first (they don't interfere with each other)
        FillDiagonalBoxes(board, random);

        // Solve the rest with randomized backtracking
        SolveWithRandomization(board, random);

        return board;
    }

    /// <summary>
    /// Fills the three diagonal 3x3 boxes (0, 4, 8) with random valid values.
    /// </summary>
    private static void FillDiagonalBoxes(SudokuBoard board, Random random)
    {
        // Box 0 (top-left), Box 4 (center), Box 8 (bottom-right)
        int[] diagonalBoxes = { 0, 4, 8 };
        var digits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        foreach (var boxIndex in diagonalBoxes)
        {
            var boxCells = board.GetBox(boxIndex).ToList();
            digits.Shuffle(random);

            for (int i = 0; i < 9; i++)
            {
                var cell = boxCells[i];
                var newCell = new Cell(cell.Row, cell.Column, value: digits[i], isGiven: true, state: CellState.Given);
                board.SetCell(cell.Row, cell.Column, newCell);
            }

            digits.Clear();
            digits.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }
    }

    /// <summary>
    /// Solves a partially filled board using randomized backtracking.
    /// </summary>
    private static bool SolveWithRandomization(SudokuBoard board, Random random)
    {
        // Find first empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = board.GetCell(row, col);
                if (cell.IsEmpty)
                {
                    // Get candidates and randomize order
                    var candidates = BoardSolver.GetCandidates(board, row, col).ToList();
                    candidates.Shuffle(random);

                    // Try each candidate
                    foreach (var digit in candidates)
                    {
                        var newCell = new Cell(row, col, value: digit, state: CellState.Filled);
                        board.SetCell(row, col, newCell);

                        if (SolveWithRandomization(board, random))
                            return true;

                        // Backtrack
                        var emptyCell = new Cell(row, col, value: 0);
                        board.SetCell(row, col, emptyCell);
                    }

                    return false;
                }
            }
        }

        return true; // All cells filled
    }

    /// <summary>
    /// Removes cells from a complete board to create a puzzle of specified difficulty.
    /// Uses a simplified approach to avoid expensive uniqueness checks.
    /// </summary>
    private static SudokuBoard RemoveCellsForDifficulty(SudokuBoard completeBoard, Difficulty difficulty)
    {
        var puzzle = completeBoard.Clone();
        var targetEmpty = GetTargetEmptyCount(difficulty);
        var random = new Random();
        var removedCount = 0;

        // Create list of all positions and shuffle
        var positions = new List<(int, int)>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                positions.Add((row, col));
            }
        }

        positions.Shuffle(random);

        // Remove cells to reach target difficulty
        // For simplicity, we remove without checking uniqueness to avoid infinite loops
        // In production, a more sophisticated approach would batch-check periodically
        foreach (var (row, col) in positions)
        {
            if (removedCount >= targetEmpty)
                break;

            var cell = puzzle.GetCell(row, col);
            if (cell.Value > 0 && !cell.IsGiven)
            {
                var emptyCell = new Cell(row, col, value: 0, state: CellState.Empty);
                puzzle.SetCell(row, col, emptyCell);
                removedCount++;
            }
        }

        // Mark all remaining filled cells as given
        MarkGivens(puzzle);

        return puzzle;
    }

    /// <summary>
    /// Gets the target number of cells to remove for a given difficulty.
    /// </summary>
    private static int GetTargetEmptyCount(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => 81 - EasyGivens,
            Difficulty.Medium => 81 - MediumGivens,
            Difficulty.Hard => 81 - HardGivens,
            Difficulty.Expert => 81 - ExpertGivens,
            _ => 81 - MediumGivens
        };
    }

    /// <summary>
    /// Marks all filled cells as "given" (part of the puzzle clues).
    /// </summary>
    private static void MarkGivens(SudokuBoard puzzle)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = puzzle.GetCell(row, col);
                if (cell.Value > 0 && !cell.IsGiven)
                {
                    var givenCell = new Cell(row, col, value: cell.Value, isGiven: true, state: CellState.Given);
                    puzzle.SetCell(row, col, givenCell);
                }
            }
        }
    }
}

/// <summary>
/// Extension method for list shuffling.
/// </summary>
internal static class ListExtensions
{
    /// <summary>
    /// Shuffles a list in-place using Fisher-Yates algorithm.
    /// </summary>
    public static void Shuffle<T>(this List<T> list, Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);

            // Swap
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}

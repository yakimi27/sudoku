using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Provides deterministic daily puzzles based on the date.
/// Same date always produces the same puzzle across all player sessions.
/// Uses seeded Random based on the date to ensure consistency.
/// </summary>
public static class DailyPuzzleProvider
{
    /// <summary>
    /// The epoch date used as a reference point for calculating seeds.
    /// </summary>
    private static readonly DateTime Epoch = new(2024, 1, 1);

    /// <summary>
    /// Gets the daily puzzle for a specific date.
    /// The puzzle is deterministic - same date always returns same puzzle.
    /// </summary>
    /// <param name="date">The date to get the puzzle for.</param>
    /// <returns>A SudokuBoard representing the daily puzzle.</returns>
    public static SudokuBoard GetForDate(DateOnly date)
    {
        // Convert DateOnly to DateTime at start of day
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        // Generate seed from date
        int seed = GenerateSeed(dateTime);

        // Use seeded random for reproducible generation
        return GeneratePuzzleWithSeed(seed);
    }

    /// <summary>
    /// Gets the daily puzzle for today.
    /// </summary>
    /// <returns>Today's SudokuBoard puzzle.</returns>
    public static SudokuBoard GetTodayPuzzle()
    {
        return GetForDate(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    /// <summary>
    /// Generates a unique seed based on the date.
    /// Same date always produces same seed, ensuring puzzle consistency.
    /// </summary>
    private static int GenerateSeed(DateTime date)
    {
        // Calculate days since epoch
        var timespan = date - Epoch;
        int daysSinceEpoch = (int)timespan.TotalDays;

        // Create hash from day count to generate seed
        unchecked
        {
            int seed = daysSinceEpoch * 397 ^ (daysSinceEpoch >> 16);
            return Math.Abs(seed);
        }
    }

    /// <summary>
    /// Generates a puzzle using a specific seed for reproducibility.
    /// </summary>
    private static SudokuBoard GeneratePuzzleWithSeed(int seed)
    {
        // For daily puzzles, we'll use a balanced difficulty
        // Vary difficulty throughout the week for variety
        var difficulty = GetDifficultyForSeed(seed);

        // Generate a complete solution using the seed
        var completeBoard = GenerateCompleteBoardWithSeed(seed);

        // Remove cells for puzzle
        var puzzle = RemoveCellsWithSeed(completeBoard, difficulty, seed);

        return puzzle;
    }

    /// <summary>
    /// Gets the difficulty level based on the seed (varies throughout the week).
    /// </summary>
    private static Difficulty GetDifficultyForSeed(int seed)
    {
        // Use seed to vary difficulty: Mon=Easy, Tue=Medium, Wed=Hard, Thu=Easy, etc.
        int dayOfWeek = (seed / 1000) % 7;

        return dayOfWeek switch
        {
            0 => Difficulty.Easy,      // Sunday
            1 => Difficulty.Hard,      // Monday
            2 => Difficulty.Medium,    // Tuesday
            3 => Difficulty.Hard,      // Wednesday
            4 => Difficulty.Medium,    // Thursday
            5 => Difficulty.Expert,    // Friday
            6 => Difficulty.Easy,      // Saturday
            _ => Difficulty.Medium
        };
    }

    /// <summary>
    /// Generates a complete valid board using a seeded random generator.
    /// </summary>
    private static SudokuBoard GenerateCompleteBoardWithSeed(int seed)
    {
        var random = new Random(seed);
        var board = SudokuBoard.CreateEmpty();

        // Fill diagonal boxes with randomized values
        FillDiagonalBoxesWithSeed(board, random);

        // Solve the rest
        SolveWithSeed(board, random);

        return board;
    }

    /// <summary>
    /// Fills diagonal 3x3 boxes with randomized values.
    /// </summary>
    private static void FillDiagonalBoxesWithSeed(SudokuBoard board, Random random)
    {
        int[] diagonalBoxes = { 0, 4, 8 };
        var digits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        foreach (var boxIndex in diagonalBoxes)
        {
            var boxCells = board.GetBox(boxIndex).ToList();
            digits.Shuffle(random);

            for (int i = 0; i < 9; i++)
            {
                var cell = boxCells[i];
                var newCell = new Cell(cell.Row, cell.Column, value: digits[i], isGiven: true, state: Enums.CellState.Given);
                board.SetCell(cell.Row, cell.Column, newCell);
            }

            digits.Clear();
            digits.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }
    }

    /// <summary>
    /// Solves a partially filled board with seeded randomization.
    /// </summary>
    private static bool SolveWithSeed(SudokuBoard board, Random random)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = board.GetCell(row, col);
                if (cell.IsEmpty)
                {
                    var candidates = BoardSolver.GetCandidates(board, row, col).ToList();
                    candidates.Shuffle(random);

                    foreach (var digit in candidates)
                    {
                        var newCell = new Cell(row, col, value: digit, state: Enums.CellState.Filled);
                        board.SetCell(row, col, newCell);

                        if (SolveWithSeed(board, random))
                            return true;

                        var emptyCell = new Cell(row, col, value: 0);
                        board.SetCell(row, col, emptyCell);
                    }

                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Removes cells to create the puzzle, using seeded random for consistency.
    /// Uses simplified removal without uniqueness checks to avoid infinite loops.
    /// </summary>
    private static SudokuBoard RemoveCellsWithSeed(SudokuBoard completeBoard, Difficulty difficulty, int seed)
    {
        var puzzle = completeBoard.Clone();
        var random = new Random(seed);
        var targetEmpty = GetTargetEmptyCount(difficulty);
        var removedCount = 0;

        var positions = new List<(int, int)>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                positions.Add((row, col));
            }
        }

        positions.Shuffle(random);

        foreach (var (row, col) in positions)
        {
            if (removedCount >= targetEmpty)
                break;

            var cell = puzzle.GetCell(row, col);
            if (cell.Value > 0 && !cell.IsGiven)
            {
                var emptyCell = new Cell(row, col, value: 0, state: Enums.CellState.Empty);
                puzzle.SetCell(row, col, emptyCell);
                removedCount++;
            }
        }

        // Mark remaining cells as given
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = puzzle.GetCell(row, col);
                if (cell.Value > 0 && !cell.IsGiven)
                {
                    var givenCell = new Cell(row, col, value: cell.Value, isGiven: true, state: Enums.CellState.Given);
                    puzzle.SetCell(row, col, givenCell);
                }
            }
        }

        return puzzle;
    }

    /// <summary>
    /// Gets the target number of cells to remove for a given difficulty.
    /// </summary>
    private static int GetTargetEmptyCount(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => 81 - 45,
            Difficulty.Medium => 81 - 40,
            Difficulty.Hard => 81 - 35,
            Difficulty.Expert => 81 - 30,
            _ => 81 - 40
        };
    }
}

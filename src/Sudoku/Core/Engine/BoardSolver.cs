using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Provides Sudoku board solving algorithms using backtracking.
/// All methods are stateless and can be used in parallel contexts.
/// </summary>
public static class BoardSolver
{
    /// <summary>
    /// The set of valid Sudoku digits (1-9).
    /// </summary>
    private static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// Determines if a given board configuration is solvable.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if the board has at least one valid solution, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public static bool IsSolvable(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        return Solve(board) != null;
    }

    /// <summary>
    /// Attempts to solve a Sudoku board using backtracking algorithm.
    /// Returns null if no solution exists.
    /// </summary>
    /// <param name="board">The board to solve.</param>
    /// <returns>A solved SudokuBoard, or null if no solution exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public static SudokuBoard? Solve(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        var workingBoard = board.Clone();
        return SolveInternal(workingBoard) ? workingBoard : null;
    }

    /// <summary>
    /// Determines if a board has exactly one unique solution.
    /// Stops searching after finding 2 solutions for efficiency.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if exactly one solution exists, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public static bool HasUniqueSolution(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        var workingBoard = board.Clone();
        var solutionCount = 0;
        CountSolutions(workingBoard, ref solutionCount);
        return solutionCount == 1;
    }

    /// <summary>
    /// Validates that a move is legal without solving the entire board.
    /// Checks if placing a value at a position violates Sudoku rules.
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="row">The row of the cell.</param>
    /// <param name="column">The column of the cell.</param>
    /// <param name="value">The value to validate (1-9).</param>
    /// <returns>True if the move is valid, false otherwise.</returns>
    public static bool IsValidMove(SudokuBoard board, int row, int column, int value)
    {
        // Check row
        foreach (var cell in board.GetRow(row))
        {
            if (cell.Value == value)
                return false;
        }

        // Check column
        foreach (var cell in board.GetColumn(column))
        {
            if (cell.Value == value)
                return false;
        }

        // Check 3x3 box
        foreach (var cell in board.GetBoxByPosition(row, column))
        {
            if (cell.Value == value)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets all valid candidates for a specific cell.
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="row">The row of the cell.</param>
    /// <param name="column">The column of the cell.</param>
    /// <returns>A collection of valid digits (1-9) that can be placed in the cell.</returns>
    public static IEnumerable<int> GetCandidates(SudokuBoard board, int row, int column)
    {
        var cell = board.GetCell(row, column);
        if (!cell.IsEmpty)
            return Enumerable.Empty<int>();

        return ValidDigits.Where(digit => IsValidMove(board, row, column, digit));
    }

    /// <summary>
    /// Internal backtracking solver implementation.
    /// </summary>
    private static bool SolveInternal(SudokuBoard board)
    {
        // Find first empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = board.GetCell(row, col);
                if (cell.IsEmpty)
                {
                    // Try each digit
                    foreach (var digit in ValidDigits)
                    {
                        if (IsValidMove(board, row, col, digit))
                        {
                            var newCell = new Cell(row, col, value: digit, state: Enums.CellState.Filled);
                            board.SetCell(row, col, newCell);

                            if (SolveInternal(board))
                                return true;

                            // Backtrack
                            var emptyCell = new Cell(row, col, value: 0);
                            board.SetCell(row, col, emptyCell);
                        }
                    }

                    return false; // No valid digit found
                }
            }
        }

        return true; // All cells filled successfully
    }

    /// <summary>
    /// Counts the number of solutions, stopping after finding 2 for efficiency.
    /// </summary>
    private static void CountSolutions(SudokuBoard board, ref int count)
    {
        if (count > 1)
            return; // Stop counting after 2 solutions found

        // Find first empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = board.GetCell(row, col);
                if (cell.IsEmpty)
                {
                    // Try each digit
                    foreach (var digit in ValidDigits)
                    {
                        if (IsValidMove(board, row, col, digit))
                        {
                            var newCell = new Cell(row, col, value: digit, state: Enums.CellState.Filled);
                            board.SetCell(row, col, newCell);

                            CountSolutions(board, ref count);

                            // Backtrack
                            var emptyCell = new Cell(row, col, value: 0);
                            board.SetCell(row, col, emptyCell);
                        }
                    }

                    return; // Tried all digits, return
                }
            }
        }

        // No empty cell found - found a complete solution
        count++;
    }
}

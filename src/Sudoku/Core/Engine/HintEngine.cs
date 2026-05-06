using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Implements hint generation and management for Sudoku puzzles.
/// Provides different types of hints: reveal cell, highlight errors, explain techniques.
/// Tracks remaining hints per session.
/// </summary>
public class HintEngine : IDisposable
{
    private const int DefaultMaxHints = 5;
    private int _remainingHints;

    /// <summary>
    /// Gets the number of hints remaining in the current session.
    /// </summary>
    public int RemainingHints => _remainingHints;

    /// <summary>
    /// Gets the maximum number of hints allowed per game session.
    /// </summary>
    public int MaxHints { get; private set; }

    /// <summary>
    /// Initializes a new instance of the HintEngine class.
    /// </summary>
    /// <param name="maxHints">Maximum hints available. Defaults to 5.</param>
    public HintEngine(int maxHints = DefaultMaxHints)
    {
        MaxHints = maxHints > 0 ? maxHints : DefaultMaxHints;
        _remainingHints = MaxHints;
    }

    /// <summary>
    /// Generates a hint for the given board state based on the hint type.
    /// </summary>
    /// <param name="board">The current Sudoku board.</param>
    /// <param name="hintType">The type of hint requested.</param>
    /// <returns>A HintResult containing the hint information, or null if no hint can be generated.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public HintResult? GetHint(SudokuBoard board, HintType hintType)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        return hintType switch
        {
            HintType.RevealCell => GenerateRevealCellHint(board),
            HintType.ShowCandidates => GenerateShowCandidatesHint(board),
            HintType.EliminateCandidates => GenerateEliminateCandidatesHint(board),
            HintType.HighlightRelated => GenerateHighlightRelatedHint(board),
            HintType.SolveAll => GenerateSolveAllHint(board),
            _ => null
        };
    }

    /// <summary>
    /// Attempts to use a hint, decrementing the remaining hint count.
    /// </summary>
    /// <returns>True if a hint was successfully consumed, false if no hints remain.</returns>
    public bool UseHint()
    {
        if (_remainingHints > 0)
        {
            _remainingHints--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets the hint count to the maximum.
    /// </summary>
    public void ResetHints()
    {
        _remainingHints = MaxHints;
    }

    /// <summary>
    /// Sets the maximum number of hints for the current session.
    /// </summary>
    /// <param name="maxHints">The new maximum hint count.</param>
    public void SetMaxHints(int maxHints)
    {
        MaxHints = maxHints > 0 ? maxHints : DefaultMaxHints;
        _remainingHints = MaxHints;
    }

    /// <summary>
    /// Generates a reveal cell hint - finds the cell that, when revealed, reduces ambiguity the most.
    /// </summary>
    private HintResult GenerateRevealCellHint(SudokuBoard board)
    {
        // Find all empty cells
        var emptyCells = new List<(int Row, int Column)>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value == 0)
                {
                    emptyCells.Add((row, col));
                }
            }
        }

        if (emptyCells.Count == 0)
            return null; // Puzzle is solved

        // For simplicity, reveal the cell with fewest candidates (highest constraint)
        var bestCell = emptyCells[0];
        int minCandidates = GetCandidateCount(board, bestCell.Row, bestCell.Column);

        foreach (var (row, col) in emptyCells.Skip(1))
        {
            int candidates = GetCandidateCount(board, row, col);
            if (candidates < minCandidates)
            {
                minCandidates = candidates;
                bestCell = (row, col);
            }
        }

        return new HintResult(
            new[] { bestCell },
            "Hint_RevealCell",
            HintType.RevealCell,
            "Direct Reveal"
        );
    }

    /// <summary>
    /// Generates a show candidates hint - highlights possible values for an empty cell.
    /// </summary>
    private HintResult GenerateShowCandidatesHint(SudokuBoard board)
    {
        // Find first empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value == 0)
                {
                    return new HintResult(
                        new[] { (row, col) },
                        "Hint_ShowCandidates",
                        HintType.ShowCandidates,
                        "Candidates"
                    );
                }
            }
        }

        return null; // Puzzle is solved
    }

    /// <summary>
    /// Generates an eliminate candidates hint - removes impossible candidates from cells.
    /// </summary>
    private HintResult GenerateEliminateCandidatesHint(SudokuBoard board)
    {
        // Find a cell with multiple candidates where we can eliminate some
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value == 0)
                {
                    var candidates = GetCandidates(board, row, col);
                    if (candidates.Count > 1)
                    {
                        // This cell has candidates that can be eliminated
                        return new HintResult(
                            new[] { (row, col) },
                            "Hint_EliminateCandidates",
                            HintType.EliminateCandidates,
                            "Elimination"
                        );
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Generates a highlight related hint - shows cells that share row, column, or box.
    /// </summary>
    private HintResult GenerateHighlightRelatedHint(SudokuBoard board)
    {
        // Find first filled cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value != 0)
                {
                    // Highlight all related cells
                    var relatedCells = new List<(int, int)> { (row, col) };

                    // Same row
                    for (int c = 0; c < 9; c++)
                    {
                        if (c != col)
                            relatedCells.Add((row, c));
                    }

                    // Same column
                    for (int r = 0; r < 9; r++)
                    {
                        if (r != row)
                            relatedCells.Add((r, col));
                    }

                    // Same 3x3 box
                    int boxRow = (row / 3) * 3;
                    int boxCol = (col / 3) * 3;
                    for (int r = boxRow; r < boxRow + 3; r++)
                    {
                        for (int c = boxCol; c < boxCol + 3; c++)
                        {
                            if ((r, c) != (row, col) && !relatedCells.Contains((r, c)))
                                relatedCells.Add((r, c));
                        }
                    }

                    return new HintResult(
                        relatedCells.AsReadOnly(),
                        "Hint_HighlightRelated",
                        HintType.HighlightRelated,
                        "Constraint Analysis"
                    );
                }
            }
        }

        // If no filled cells, find first empty cell
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value == 0)
                {
                    var relatedCells = new List<(int, int)> { (row, col) };

                    // Same row
                    for (int c = 0; c < 9; c++)
                    {
                        if (c != col)
                            relatedCells.Add((row, c));
                    }

                    // Same column
                    for (int r = 0; r < 9; r++)
                    {
                        if (r != row)
                            relatedCells.Add((r, col));
                    }

                    // Same 3x3 box
                    int boxRow = (row / 3) * 3;
                    int boxCol = (col / 3) * 3;
                    for (int r = boxRow; r < boxRow + 3; r++)
                    {
                        for (int c = boxCol; c < boxCol + 3; c++)
                        {
                            if ((r, c) != (row, col) && !relatedCells.Contains((r, c)))
                                relatedCells.Add((r, c));
                        }
                    }

                    return new HintResult(
                        relatedCells.AsReadOnly(),
                        "Hint_HighlightRelated",
                        HintType.HighlightRelated,
                        "Constraint Analysis"
                    );
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Generates a solve all hint - returns all remaining empty cells as the affected set.
    /// </summary>
    private HintResult GenerateSolveAllHint(SudokuBoard board)
    {
        var allEmptyCells = new List<(int, int)>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board.GetCell(row, col).Value == 0)
                {
                    allEmptyCells.Add((row, col));
                }
            }
        }

        if (allEmptyCells.Count == 0)
            return null; // Already solved

        return new HintResult(
            allEmptyCells.AsReadOnly(),
            "Hint_SolveAll",
            HintType.SolveAll,
            "Complete Solution"
        );
    }

    /// <summary>
    /// Gets all valid candidate numbers for a cell.
    /// </summary>
    private HashSet<int> GetCandidates(SudokuBoard board, int row, int col)
    {
        var candidates = new HashSet<int>();
        for (int num = 1; num <= 9; num++)
        {
            if (BoardSolver.IsValidMove(board, row, col, num))
            {
                candidates.Add(num);
            }
        }
        return candidates;
    }

    /// <summary>
    /// Gets the count of valid candidates for a cell.
    /// </summary>
    private int GetCandidateCount(SudokuBoard board, int row, int col)
    {
        return GetCandidates(board, row, col).Count;
    }

    /// <summary>
    /// Disposes the hint engine resources.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose currently, but provided for future expansion
        GC.SuppressFinalize(this);
    }
}

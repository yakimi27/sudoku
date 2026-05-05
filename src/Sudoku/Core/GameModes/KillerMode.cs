using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Killer Mode: 9x9 Sudoku where cells are grouped into cages,
/// each with a target sum. No digit repeats within a cage.
/// </summary>
public sealed class KillerMode : IGameMode
{
    /// <summary>
    /// Valid digits for killer Sudoku (1-9).
    /// </summary>
    private static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// Represents a killer cage with target sum and cell positions.
    /// </summary>
    private class Cage
    {
        public int Sum { get; }
        public List<(int row, int col)> Cells { get; }

        public Cage(int sum, List<(int row, int col)> cells)
        {
            Sum = sum;
            Cells = new List<(int row, int col)>(cells);
        }
    }

    /// <summary>
    /// List of cages in this killer puzzle.
    /// </summary>
    private readonly List<Cage> _cages;

    /// <summary>
    /// Map from cell position to cage index for quick lookup.
    /// </summary>
    private readonly Dictionary<(int row, int col), int> _cellToCageIndex;

    /// <summary>
    /// Initializes a new instance of KillerMode.
    /// </summary>
    public KillerMode()
    {
        _cages = new List<Cage>();
        _cellToCageIndex = new Dictionary<(int row, int col), int>();
    }

    /// <summary>
    /// Gets the game mode type.
    /// </summary>
    public GameMode ModeType => GameMode.Killer;

    /// <summary>
    /// Gets the localization key for display name.
    /// </summary>
    public string DisplayNameKey => "mode.killer";

    /// <summary>
    /// Gets the number of rows (9 for standard board).
    /// </summary>
    public int BoardRows => 9;

    /// <summary>
    /// Gets the number of columns (9 for standard board).
    /// </summary>
    public int BoardColumns => 9;

    /// <summary>
    /// Gets the block height (3x3).
    /// </summary>
    public int BlockHeight => 3;

    /// <summary>
    /// Gets the block width (3x3).
    /// </summary>
    public int BlockWidth => 3;

    /// <summary>
    /// Adds a killer cage to this mode.
    /// </summary>
    /// <param name="sum">The target sum for the cage.</param>
    /// <param name="cells">List of (row, col) cells in the cage.</param>
    public void AddCage(int sum, List<(int row, int col)> cells)
    {
        if (cells == null || cells.Count == 0)
            throw new ArgumentException("Cage cells cannot be null or empty.");

        if (sum <= 0)
            throw new ArgumentException("Cage sum must be positive.");

        var cage = new Cage(sum, cells);
        var cageIndex = _cages.Count;
        _cages.Add(cage);

        foreach (var (row, col) in cells)
        {
            _cellToCageIndex[(row, col)] = cageIndex;
        }
    }

    /// <summary>
    /// Validates that the cell placement respects killer cage rules:
    /// - No digit repeats within a cage
    /// - Cage sum is correct when complete
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="row">The row of the cell being validated.</param>
    /// <param name="column">The column of the cell being validated.</param>
    /// <returns>True if the cell satisfies all killer constraints, false otherwise.</returns>
    public bool ValidateExtraRules(SudokuBoard board, int row, int column)
    {
        var cellKey = (row, column);
        if (!_cellToCageIndex.TryGetValue(cellKey, out var cageIndex))
            return true; // Cell not in any cage (shouldn't happen in valid setup)

        var cage = _cages[cageIndex];
        var cell = board.GetCell(row, column);

        if (cell.IsEmpty)
            return true;

        // Check no digit repeats within cage
        foreach (var (cageRow, cageCol) in cage.Cells)
        {
            if (cageRow == row && cageCol == column)
                continue; // Skip the cell we're validating

            var otherCell = board.GetCell(cageRow, cageCol);
            if (!otherCell.IsEmpty && otherCell.Value == cell.Value)
                return false; // Duplicate in cage
        }

        // Check cage sum when complete
        var sum = 0;
        var allFilled = true;

        foreach (var (cageRow, cageCol) in cage.Cells)
        {
            var cageCell = board.GetCell(cageRow, cageCol);
            if (cageCell.IsEmpty)
            {
                allFilled = false;
            }
            else
            {
                sum += cageCell.Value;
            }
        }

        // If all cells filled, sum must match
        if (allFilled && sum != cage.Sum)
            return false;

        // If not all filled, sum must not exceed target
        if (!allFilled && sum >= cage.Sum)
            return false;

        return true;
    }

    /// <summary>
    /// Gets constraints for all cells in killer cages.
    /// </summary>
    /// <param name="board">The board (unused).</param>
    /// <returns>A list of constraints for each cell in a cage.</returns>
    public IReadOnlyList<CellConstraint> GetConstraints(SudokuBoard board)
    {
        var constraints = new List<CellConstraint>();

        for (int cageIndex = 0; cageIndex < _cages.Count; cageIndex++)
        {
            var cage = _cages[cageIndex];
            foreach (var (row, col) in cage.Cells)
            {
                var data = $"cage:{cageIndex}:sum={cage.Sum}";
                constraints.Add(new CellConstraint(row, col, ConstraintType.KillerCage, data));
            }
        }

        return constraints;
    }

    /// <summary>
    /// Returns valid digits for killer Sudoku (1-9).
    /// </summary>
    /// <returns>An array containing digits 1 through 9.</returns>
    public int[] GetValidDigits()
    {
        return ValidDigits;
    }
}

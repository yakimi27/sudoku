using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Core.GameModes;

/// <summary>
/// Thermometer Mode: 9x9 Sudoku where values along thermometer paths
/// must strictly increase from the bulb to the tip.
/// </summary>
public sealed class ThermometerMode : IGameMode
{
    /// <summary>
    /// Valid digits for thermometer Sudoku (1-9).
    /// </summary>
    private static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// List of thermometer paths. Each path is a list of (row, col) coordinates
    /// representing cells that must have strictly increasing values.
    /// </summary>
    private readonly List<List<(int row, int col)>> _thermometers;

    /// <summary>
    /// Initializes a new instance of ThermometerMode.
    /// </summary>
    public ThermometerMode()
    {
        _thermometers = new List<List<(int row, int col)>>();
    }

    /// <summary>
    /// Gets the game mode type.
    /// </summary>
    public GameMode ModeType => GameMode.Thermometer;

    /// <summary>
    /// Gets the localization key for display name.
    /// </summary>
    public string DisplayNameKey => "mode.thermometer";

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
    /// Adds a thermometer path to this mode.
    /// </summary>
    /// <param name="path">A list of (row, col) coordinates representing the thermometer.</param>
    public void AddThermometer(List<(int row, int col)> path)
    {
        if (path == null || path.Count == 0)
            throw new ArgumentException("Thermometer path cannot be null or empty.");

        _thermometers.Add(new List<(int row, int col)>(path));
    }

    /// <summary>
    /// Validates that the cell placement respects thermometer ordering rules.
    /// For any thermometer containing the cell, values must be strictly increasing.
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="row">The row of the cell being validated.</param>
    /// <param name="column">The column of the cell being validated.</param>
    /// <returns>True if the cell satisfies all thermometer constraints, false otherwise.</returns>
    public bool ValidateExtraRules(SudokuBoard board, int row, int column)
    {
        var cell = board.GetCell(row, column);
        if (cell.IsEmpty)
            return true;

        // Check each thermometer containing this cell
        foreach (var thermo in _thermometers)
        {
            var index = thermo.FindIndex(pos => pos.row == row && pos.col == column);
            if (index == -1)
                continue; // Cell not in this thermometer

            // Check value is greater than previous cell (if exists)
            if (index > 0)
            {
                var prevCell = board.GetCell(thermo[index - 1].row, thermo[index - 1].col);
                if (!prevCell.IsEmpty && prevCell.Value >= cell.Value)
                    return false;
            }

            // Check value is less than next cell (if exists)
            if (index < thermo.Count - 1)
            {
                var nextCell = board.GetCell(thermo[index + 1].row, thermo[index + 1].col);
                if (!nextCell.IsEmpty && nextCell.Value <= cell.Value)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets constraints for all cells in thermometers.
    /// </summary>
    /// <param name="board">The board (unused).</param>
    /// <returns>A list of constraints for each cell in a thermometer.</returns>
    public IReadOnlyList<CellConstraint> GetConstraints(SudokuBoard board)
    {
        var constraints = new List<CellConstraint>();

        for (int thermoIndex = 0; thermoIndex < _thermometers.Count; thermoIndex++)
        {
            var thermo = _thermometers[thermoIndex];
            foreach (var (row, col) in thermo)
            {
                var data = $"thermo:{thermoIndex}";
                constraints.Add(new CellConstraint(row, col, ConstraintType.Thermometer, data));
            }
        }

        return constraints;
    }

    /// <summary>
    /// Returns valid digits for thermometer Sudoku (1-9).
    /// </summary>
    /// <returns>An array containing digits 1 through 9.</returns>
    public int[] GetValidDigits()
    {
        return ValidDigits;
    }
}

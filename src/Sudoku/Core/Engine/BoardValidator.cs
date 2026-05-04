using Sudoku.Core.Models;

namespace Sudoku.Core.Engine;

/// <summary>
/// Validation result containing the outcome of board validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the board is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets the type of error found, if any.
    /// </summary>
    public ValidationType ErrorType { get; set; }

    /// <summary>
    /// Gets the list of cell coordinates that contain errors.
    /// Format: list of (row, column) tuples.
    /// </summary>
    public List<(int Row, int Column)> ErrorCells { get; set; } = new();

    /// <summary>
    /// Gets an optional error message describing the validation failure.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Returns a successful validation result.
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true, ErrorType = ValidationType.None };

    /// <summary>
    /// Returns a failed validation result with error details.
    /// </summary>
    public static ValidationResult Failure(ValidationType errorType, string message = "")
    {
        return new()
        {
            IsValid = false,
            ErrorType = errorType,
            ErrorMessage = message
        };
    }
}

/// <summary>
/// Enumeration of validation error types.
/// </summary>
public enum ValidationType
{
    /// <summary>
    /// No error - board is valid.
    /// </summary>
    None = 0,

    /// <summary>
    /// Duplicate value found in a row.
    /// </summary>
    DuplicateInRow = 1,

    /// <summary>
    /// Duplicate value found in a column.
    /// </summary>
    DuplicateInColumn = 2,

    /// <summary>
    /// Duplicate value found in a 3x3 block.
    /// </summary>
    DuplicateInBlock = 3,

    /// <summary>
    /// A given cell was modified.
    /// </summary>
    GivenCellModified = 4,

    /// <summary>
    /// Game mode specific rule violation.
    /// </summary>
    GameModeRuleViolation = 5
}

/// <summary>
/// Validates Sudoku board states for correctness and rule compliance.
/// All methods are stateless and can be used in parallel contexts.
/// </summary>
public static class BoardValidator
{
    /// <summary>
    /// Validates a board according to standard Sudoku rules.
    /// </summary>
    /// <param name="board">The board to validate.</param>
    /// <returns>A ValidationResult indicating success or failure with error details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public static ValidationResult Validate(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        // Check for duplicate values in rows
        var rowValidation = ValidateRows(board);
        if (!rowValidation.IsValid)
            return rowValidation;

        // Check for duplicate values in columns
        var columnValidation = ValidateColumns(board);
        if (!columnValidation.IsValid)
            return columnValidation;

        // Check for duplicate values in 3x3 blocks
        var blockValidation = ValidateBlocks(board);
        if (!blockValidation.IsValid)
            return blockValidation;

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates that rows contain no duplicate values.
    /// </summary>
    private static ValidationResult ValidateRows(SudokuBoard board)
    {
        for (int row = 0; row < 9; row++)
        {
            var seenValues = new HashSet<int>();
            var rowCells = board.GetRow(row).ToList();

            foreach (var cell in rowCells)
            {
                if (cell.Value > 0)
                {
                    if (seenValues.Contains(cell.Value))
                    {
                        var result = ValidationResult.Failure(
                            ValidationType.DuplicateInRow,
                            $"Duplicate value {cell.Value} in row {row}"
                        );
                        // Add all cells with this value to error list
                        foreach (var errorCell in rowCells.Where(c => c.Value == cell.Value))
                        {
                            result.ErrorCells.Add((errorCell.Row, errorCell.Column));
                        }
                        return result;
                    }
                    seenValues.Add(cell.Value);
                }
            }
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates that columns contain no duplicate values.
    /// </summary>
    private static ValidationResult ValidateColumns(SudokuBoard board)
    {
        for (int col = 0; col < 9; col++)
        {
            var seenValues = new HashSet<int>();
            var colCells = board.GetColumn(col).ToList();

            foreach (var cell in colCells)
            {
                if (cell.Value > 0)
                {
                    if (seenValues.Contains(cell.Value))
                    {
                        var result = ValidationResult.Failure(
                            ValidationType.DuplicateInColumn,
                            $"Duplicate value {cell.Value} in column {col}"
                        );
                        foreach (var errorCell in colCells.Where(c => c.Value == cell.Value))
                        {
                            result.ErrorCells.Add((errorCell.Row, errorCell.Column));
                        }
                        return result;
                    }
                    seenValues.Add(cell.Value);
                }
            }
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates that 3x3 blocks contain no duplicate values.
    /// </summary>
    private static ValidationResult ValidateBlocks(SudokuBoard board)
    {
        for (int boxIndex = 0; boxIndex < 9; boxIndex++)
        {
            var seenValues = new HashSet<int>();
            var blockCells = board.GetBox(boxIndex).ToList();

            foreach (var cell in blockCells)
            {
                if (cell.Value > 0)
                {
                    if (seenValues.Contains(cell.Value))
                    {
                        var result = ValidationResult.Failure(
                            ValidationType.DuplicateInBlock,
                            $"Duplicate value {cell.Value} in block {boxIndex}"
                        );
                        foreach (var errorCell in blockCells.Where(c => c.Value == cell.Value))
                        {
                            result.ErrorCells.Add((errorCell.Row, errorCell.Column));
                        }
                        return result;
                    }
                    seenValues.Add(cell.Value);
                }
            }
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Checks if a move is valid (doesn't violate Sudoku rules).
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="row">The row of the move.</param>
    /// <param name="column">The column of the move.</param>
    /// <param name="value">The value being placed (1-9).</param>
    /// <returns>True if the move is valid, false otherwise.</returns>
    public static bool IsValidMove(SudokuBoard board, int row, int column, int value)
    {
        return BoardSolver.IsValidMove(board, row, column, value);
    }

    /// <summary>
    /// Checks if the board is completely solved (all cells filled with valid values).
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if the board is solved, false otherwise.</returns>
    public static bool IsSolved(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        return board.GetEmptyCellCount() == 0 && Validate(board).IsValid;
    }
}

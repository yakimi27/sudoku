namespace Sudoku.Core.Models;

/// <summary>
/// Represents a 9x9 Sudoku board containing 81 cells.
/// The board uses row-major indexing (row 0-8, column 0-8).
/// Provides methods to access rows, columns, and 3x3 blocks.
/// </summary>
public sealed class SudokuBoard
{
    /// <summary>
    /// The total number of rows and columns in a standard Sudoku board.
    /// </summary>
    public const int BoardSize = 9;

    /// <summary>
    /// The size of each 3x3 box within the board.
    /// </summary>
    public const int BoxSize = 3;

    /// <summary>
    /// The total number of cells in a Sudoku board.
    /// </summary>
    public const int TotalCells = BoardSize * BoardSize;

    private readonly Cell[,] _cells;

    /// <summary>
    /// Initializes a new instance of the SudokuBoard class with the provided cells.
    /// </summary>
    /// <param name="cells">A 9x9 array of Cell objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when cells is null.</exception>
    /// <exception cref="ArgumentException">Thrown when cells is not a 9x9 array.</exception>
    private SudokuBoard(Cell[,] cells)
    {
        if (cells == null)
            throw new ArgumentNullException(nameof(cells));
        if (cells.GetLength(0) != BoardSize || cells.GetLength(1) != BoardSize)
            throw new ArgumentException("Cells must be a 9x9 array.", nameof(cells));

        _cells = (Cell[,])cells.Clone();
    }

    /// <summary>
    /// Gets a cell at the specified row and column.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <param name="column">The column index (0-8).</param>
    /// <returns>The Cell at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is out of range.</exception>
    public Cell GetCell(int row, int column)
    {
        if (row < 0 || row >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");
        if (column < 0 || column >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");

        return _cells[row, column];
    }

    /// <summary>
    /// Sets a cell at the specified row and column.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <param name="column">The column index (0-8).</param>
    /// <param name="cell">The new Cell to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is out of range.</exception>
    /// <exception cref="ArgumentNullException">Thrown when cell is null.</exception>
    public void SetCell(int row, int column, Cell cell)
    {
        if (row < 0 || row >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");
        if (column < 0 || column >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");
        if (cell == null)
            throw new ArgumentNullException(nameof(cell));

        _cells[row, column] = cell;
    }

    /// <summary>
    /// Gets all cells in the specified row.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <returns>An enumeration of cells in the row.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row is out of range.</exception>
    public IEnumerable<Cell> GetRow(int row)
    {
        if (row < 0 || row >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");

        for (int col = 0; col < BoardSize; col++)
        {
            yield return _cells[row, col];
        }
    }

    /// <summary>
    /// Gets all cells in the specified column.
    /// </summary>
    /// <param name="column">The column index (0-8).</param>
    /// <returns>An enumeration of cells in the column.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when column is out of range.</exception>
    public IEnumerable<Cell> GetColumn(int column)
    {
        if (column < 0 || column >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");

        for (int row = 0; row < BoardSize; row++)
        {
            yield return _cells[row, column];
        }
    }

    /// <summary>
    /// Gets all cells in the 3x3 box at the specified box coordinates.
    /// Box indices are 0-8, where box 0 is top-left, box 8 is bottom-right.
    /// </summary>
    /// <param name="boxIndex">The box index (0-8).</param>
    /// <returns>An enumeration of cells in the 3x3 box.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when boxIndex is out of range.</exception>
    public IEnumerable<Cell> GetBox(int boxIndex)
    {
        if (boxIndex < 0 || boxIndex >= 9)
            throw new ArgumentOutOfRangeException(nameof(boxIndex), "Box index must be between 0 and 8.");

        int boxRow = (boxIndex / BoxSize) * BoxSize;
        int boxCol = (boxIndex % BoxSize) * BoxSize;

        for (int row = boxRow; row < boxRow + BoxSize; row++)
        {
            for (int col = boxCol; col < boxCol + BoxSize; col++)
            {
                yield return _cells[row, col];
            }
        }
    }

    /// <summary>
    /// Gets all cells in the 3x3 box containing the specified cell position.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <param name="column">The column index (0-8).</param>
    /// <returns>An enumeration of cells in the 3x3 box.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is out of range.</exception>
    public IEnumerable<Cell> GetBoxByPosition(int row, int column)
    {
        if (row < 0 || row >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");
        if (column < 0 || column >= BoardSize)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");

        int boxRow = (row / BoxSize) * BoxSize;
        int boxCol = (column / BoxSize) * BoxSize;

        for (int r = boxRow; r < boxRow + BoxSize; r++)
        {
            for (int c = boxCol; c < boxCol + BoxSize; c++)
            {
                yield return _cells[r, c];
            }
        }
    }

    /// <summary>
    /// Gets all cells on the board.
    /// </summary>
    /// <returns>An enumeration of all cells.</returns>
    public IEnumerable<Cell> GetAllCells()
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                yield return _cells[row, col];
            }
        }
    }

    /// <summary>
    /// Creates an empty board with no givens.
    /// </summary>
    /// <returns>A new SudokuBoard with all cells empty.</returns>
    public static SudokuBoard CreateEmpty()
    {
        var cells = new Cell[BoardSize, BoardSize];
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                cells[row, col] = new Cell(row, col, value: 0, isGiven: false);
            }
        }
        return new SudokuBoard(cells);
    }

    /// <summary>
    /// Creates a board from an array of given values.
    /// </summary>
    /// <param name="givens">A 9x9 array where 0 means empty, 1-9 are given values.</param>
    /// <returns>A new SudokuBoard with the specified givens.</returns>
    /// <exception cref="ArgumentException">Thrown when givens is not a 9x9 array or contains invalid values.</exception>
    public static SudokuBoard CreateFromGivens(int[,] givens)
    {
        if (givens == null)
            throw new ArgumentNullException(nameof(givens));
        if (givens.GetLength(0) != BoardSize || givens.GetLength(1) != BoardSize)
            throw new ArgumentException("Givens must be a 9x9 array.", nameof(givens));

        var cells = new Cell[BoardSize, BoardSize];
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                int value = givens[row, col];
                if (value < 0 || value > 9)
                    throw new ArgumentException($"Invalid value {value} at position ({row},{col}). Values must be 0-9.", nameof(givens));

                bool isGiven = value != 0;
                var state = isGiven ? Enums.CellState.Given : Enums.CellState.Empty;
                cells[row, col] = new Cell(row, col, value, isGiven, state);
            }
        }
        return new SudokuBoard(cells);
    }

    /// <summary>
    /// Gets the count of empty cells on the board.
    /// </summary>
    /// <returns>The number of cells with value 0.</returns>
    public int GetEmptyCellCount()
    {
        int count = 0;
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (_cells[row, col].IsEmpty)
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Creates a deep copy of this board.
    /// </summary>
    /// <returns>A new SudokuBoard with copies of all cells.</returns>
    public SudokuBoard Clone()
    {
        var cellsCopy = new Cell[BoardSize, BoardSize];
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                cellsCopy[row, col] = _cells[row, col];
            }
        }
        return new SudokuBoard(cellsCopy);
    }
}

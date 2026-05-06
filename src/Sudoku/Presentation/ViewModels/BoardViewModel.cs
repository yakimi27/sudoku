using System.Collections.ObjectModel;
using Sudoku.Core.Models;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Sudoku board, managing all 81 cell ViewModels.
/// Provides observable collection and selection management.
/// </summary>
public class BoardViewModel : ObservableObject
{
    private CellViewModel? _selectedCell;

    /// <summary>
    /// Initializes a new instance of the BoardViewModel class.
    /// </summary>
    /// <param name="board">The underlying SudokuBoard model.</param>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public BoardViewModel(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        Cells = new ObservableCollection<CellViewModel>();
        InitializeCells(board);
    }

    /// <summary>
    /// Gets the observable collection of all 81 cell ViewModels.
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; }

    /// <summary>
    /// Gets or sets the currently selected cell.
    /// </summary>
    public CellViewModel? SelectedCell
    {
        get => _selectedCell;
        set
        {
            if (SetProperty(ref _selectedCell, value))
            {
                UpdateHighlighting();
            }
        }
    }

    /// <summary>
    /// Initializes the cell collection from the board model.
    /// </summary>
    private void InitializeCells(SudokuBoard board)
    {
        Cells.Clear();

        for (int row = 0; row < SudokuBoard.BoardSize; row++)
        {
            for (int col = 0; col < SudokuBoard.BoardSize; col++)
            {
                var cell = board.GetCell(row, col);
                var cellViewModel = new CellViewModel(cell);
                Cells.Add(cellViewModel);
            }
        }
    }

    /// <summary>
    /// Updates cell highlighting based on the selected cell.
    /// Highlights cells in the same row, column, and 3x3 block, as well as cells with the same value.
    /// </summary>
    private void UpdateHighlighting()
    {
        if (SelectedCell == null)
        {
            foreach (var cell in Cells)
            {
                cell.IsHighlighted = false;
            }
            return;
        }

        int selectedRow = SelectedCell.Row;
        int selectedCol = SelectedCell.Column;
        int selectedValue = SelectedCell.Value;

        foreach (var cell in Cells)
        {
            bool shouldHighlight = (cell.Row == selectedRow) ||
                                   (cell.Column == selectedCol) ||
                                   (cell.IsInSameBlock(selectedRow, selectedCol)) ||
                                   (selectedValue > 0 && cell.Value == selectedValue);

            cell.IsHighlighted = shouldHighlight;
        }
    }

    /// <summary>
    /// Refreshes the board state from an updated board model.
    /// Syncs all cells with their corresponding board cells.
    /// </summary>
    /// <param name="board">The updated board model.</param>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public void RefreshFromBoard(SudokuBoard board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        if (Cells.Count != SudokuBoard.TotalCells)
        {
            InitializeCells(board);
            return;
        }

        for (int i = 0; i < Cells.Count; i++)
        {
            int row = i / SudokuBoard.BoardSize;
            int col = i % SudokuBoard.BoardSize;
            var boardCell = board.GetCell(row, col);
            Cells[i].SyncWithModel(boardCell);
        }

        UpdateHighlighting();
    }

    /// <summary>
    /// Gets a cell ViewModel by its row and column indices.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <param name="column">The column index (0-8).</param>
    /// <returns>The CellViewModel at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when indices are out of range.</exception>
    public CellViewModel GetCell(int row, int column)
    {
        if (row < 0 || row >= SudokuBoard.BoardSize || column < 0 || column >= SudokuBoard.BoardSize)
            throw new ArgumentOutOfRangeException($"Cell indices out of range: ({row}, {column})");

        int index = row * SudokuBoard.BoardSize + column;
        return Cells[index];
    }

    /// <summary>
    /// Clears the selection and highlighting.
    /// </summary>
    public void ClearSelection()
    {
        SelectedCell = null;
    }
}

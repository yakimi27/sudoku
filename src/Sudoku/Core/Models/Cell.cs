using System.Collections.Immutable;
using Sudoku.Core.Enums;

namespace Sudoku.Core.Models;

/// <summary>
/// Represents a single cell in a Sudoku board.
/// Cells are immutable once created; state changes require creating new Cell instances.
/// </summary>
public sealed class Cell : IEquatable<Cell>
{
    /// <summary>
    /// The row index of this cell (0-8).
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// The column index of this cell (0-8).
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// The current value in the cell (1-9 for filled, 0 for empty).
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Indicates whether this cell is part of the puzzle's initial givens.
    /// Given cells cannot be changed during gameplay.
    /// </summary>
    public bool IsGiven { get; }

    /// <summary>
    /// The current state of the cell (Empty, Given, Filled, etc.).
    /// </summary>
    public CellState State { get; }

    /// <summary>
    /// The candidate numbers for this cell (1-9), shown as pencil marks.
    /// An empty set means no candidates are shown.
    /// </summary>
    public ImmutableHashSet<int> Notes { get; }

    /// <summary>
    /// Initializes a new instance of the Cell class.
    /// </summary>
    /// <param name="row">The row index (0-8).</param>
    /// <param name="column">The column index (0-8).</param>
    /// <param name="value">The cell value (0-9, where 0 means empty).</param>
    /// <param name="isGiven">Whether this cell is a puzzle given.</param>
    /// <param name="state">The current cell state.</param>
    /// <param name="notes">Optional candidate numbers.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row, column, or value are out of valid range.</exception>
    public Cell(int row, int column, int value = 0, bool isGiven = false, 
        CellState state = CellState.Empty, ImmutableHashSet<int>? notes = null)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");
        if (value < 0 || value > 9)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 9.");

        Row = row;
        Column = column;
        Value = value;
        IsGiven = isGiven;
        State = state;
        Notes = notes ?? ImmutableHashSet<int>.Empty;
    }

    /// <summary>
    /// Creates a new Cell with the specified value, preserving other properties.
    /// </summary>
    /// <param name="newValue">The new value to set (0-9).</param>
    /// <returns>A new Cell instance with the updated value.</returns>
    public Cell WithValue(int newValue)
    {
        return new Cell(Row, Column, newValue, IsGiven, 
            newValue == 0 ? CellState.Empty : CellState.Filled, ImmutableHashSet<int>.Empty);
    }

    /// <summary>
    /// Creates a new Cell with the specified notes, preserving other properties.
    /// </summary>
    /// <param name="newNotes">The new candidate numbers.</param>
    /// <returns>A new Cell instance with the updated notes.</returns>
    public Cell WithNotes(ImmutableHashSet<int> newNotes)
    {
        return new Cell(Row, Column, Value, IsGiven, CellState.Candidates, newNotes);
    }

    /// <summary>
    /// Creates a new Cell with the specified state, preserving other properties.
    /// </summary>
    /// <param name="newState">The new cell state.</param>
    /// <returns>A new Cell instance with the updated state.</returns>
    public Cell WithState(CellState newState)
    {
        return new Cell(Row, Column, Value, IsGiven, newState, Notes);
    }

    /// <summary>
    /// Determines whether the specified cell is equal to this cell.
    /// Two cells are equal if they have the same row and column indices.
    /// </summary>
    public bool Equals(Cell? other)
    {
        return other != null && Row == other.Row && Column == other.Column;
    }

    /// <summary>
    /// Determines whether the specified object is equal to this cell.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Cell);
    }

    /// <summary>
    /// Returns the hash code for this cell, based on its position.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    /// <summary>
    /// Returns a string representation of the cell.
    /// </summary>
    public override string ToString()
    {
        return $"Cell({Row},{Column})={Value} {State}";
    }

    /// <summary>
    /// Checks if this cell is empty (value is 0).
    /// </summary>
    public bool IsEmpty => Value == 0;

    /// <summary>
    /// Checks if this cell can be edited (is not a given).
    /// </summary>
    public bool IsEditable => !IsGiven;
}

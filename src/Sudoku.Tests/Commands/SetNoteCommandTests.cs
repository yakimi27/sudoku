using System.Collections.Immutable;
using Sudoku.Core.Commands;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Commands;

/// <summary>
/// Unit tests for SetNoteCommand.
/// </summary>
public class SetNoteCommandTests
{
    /// <summary>
    /// Tests that Execute removes a note that was present.
    /// </summary>
    [Fact]
    public void Execute_RemovesNote_WhenWasPresent()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var cellBefore = board.GetCell(0, 0).WithNotes(ImmutableHashSet.Create(1, 3, 5));
        board.SetCell(0, 0, cellBefore);

        var command = new SetNoteCommand(0, 0, digit: 3, wasPresent: true);

        // Act
        command.Execute(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.DoesNotContain(3, cell.Notes);
        Assert.Contains(1, cell.Notes);
        Assert.Contains(5, cell.Notes);
    }

    /// <summary>
    /// Tests that Execute adds a note that was not present.
    /// </summary>
    [Fact]
    public void Execute_AddsNote_WhenWasNotPresent()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var cellBefore = board.GetCell(0, 0).WithNotes(ImmutableHashSet.Create(1, 5));
        board.SetCell(0, 0, cellBefore);

        var command = new SetNoteCommand(0, 0, digit: 3, wasPresent: false);

        // Act
        command.Execute(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Contains(3, cell.Notes);
    }

    /// <summary>
    /// Tests that Undo restores the note if it was present before.
    /// </summary>
    [Fact]
    public void Undo_RestoresNote_WhenWasPresent()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var cellBefore = board.GetCell(0, 0).WithNotes(ImmutableHashSet.Create(3));
        board.SetCell(0, 0, cellBefore);

        var command = new SetNoteCommand(0, 0, digit: 3, wasPresent: true);
        command.Execute(board);

        // Act
        command.Undo(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.Contains(3, cell.Notes);
    }

    /// <summary>
    /// Tests that Undo removes the note if it was not present before.
    /// </summary>
    [Fact]
    public void Undo_RemovesNote_WhenWasNotPresent()
    {
        // Arrange
        var board = SudokuBoard.CreateEmpty();
        var command = new SetNoteCommand(0, 0, digit: 5, wasPresent: false);
        command.Execute(board);

        // Act
        command.Undo(board);

        // Assert
        var cell = board.GetCell(0, 0);
        Assert.DoesNotContain(5, cell.Notes);
    }

    /// <summary>
    /// Tests that Description reflects the action.
    /// </summary>
    [Theory]
    [InlineData(true, "Remove")]
    [InlineData(false, "Add")]
    public void Description_ReflectsAction(bool wasPresent, string expectedAction)
    {
        // Arrange & Act
        var command = new SetNoteCommand(1, 2, digit: 7, wasPresent: wasPresent);

        // Assert
        Assert.Contains(expectedAction, command.Description);
        Assert.Contains("7", command.Description);
    }

    /// <summary>
    /// Tests that constructor validates row bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidRow_Throws(int invalidRow)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SetNoteCommand(invalidRow, 0, 5, wasPresent: false));
    }

    /// <summary>
    /// Tests that constructor validates column bounds.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Constructor_WithInvalidColumn_Throws(int invalidCol)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SetNoteCommand(0, invalidCol, 5, wasPresent: false));
    }

    /// <summary>
    /// Tests that constructor validates digit bounds.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Constructor_WithInvalidDigit_Throws(int invalidDigit)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SetNoteCommand(0, 0, invalidDigit, wasPresent: false));
    }
}

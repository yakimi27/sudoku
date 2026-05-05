using Sudoku.Core.Commands;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Xunit;

namespace Sudoku.Tests.Commands;

/// <summary>
/// Unit tests for CommandHistory with full undo/redo workflow.
/// </summary>
public class CommandHistoryTests
{
    /// <summary>
    /// Tests that Execute adds command to undo stack.
    /// </summary>
    [Fact]
    public void Execute_AddsCommandToUndoStack()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(0, 0, 0, 5, CellState.Empty);

        // Act
        history.Execute(command, board);

        // Assert
        Assert.True(history.CanUndo);
        Assert.Equal(1, history.UndoCount);
    }

    /// <summary>
    /// Tests that Execute clears redo stack.
    /// </summary>
    [Fact]
    public void Execute_ClearsRedoStack()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var cmd1 = new SetValueCommand(0, 0, 0, 5, CellState.Empty);
        var cmd2 = new SetValueCommand(0, 1, 0, 3, CellState.Empty);
        var cmd3 = new SetValueCommand(0, 2, 0, 7, CellState.Empty);

        history.Execute(cmd1, board);
        history.Execute(cmd2, board);
        history.Undo(board);
        Assert.True(history.CanRedo);

        // Act
        history.Execute(cmd3, board);

        // Assert
        Assert.False(history.CanRedo);
    }

    /// <summary>
    /// Tests that Undo reverts the last command.
    /// </summary>
    [Fact]
    public void Undo_RevertsLastCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(0, 0, 0, 5, CellState.Empty);
        history.Execute(command, board);

        // Act
        var result = history.Undo(board);

        // Assert
        Assert.True(result);
        var cell = board.GetCell(0, 0);
        Assert.Equal(0, cell.Value);
    }

    /// <summary>
    /// Tests that Undo returns false when nothing to undo.
    /// </summary>
    [Fact]
    public void Undo_ReturnsFalse_WhenEmpty()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();

        // Act
        var result = history.Undo(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Redo reapplies an undone command.
    /// </summary>
    [Fact]
    public void Redo_ReappliesUndoneCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(0, 0, 0, 7, CellState.Empty);
        history.Execute(command, board);
        history.Undo(board);

        // Act
        var result = history.Redo(board);

        // Assert
        Assert.True(result);
        var cell = board.GetCell(0, 0);
        Assert.Equal(7, cell.Value);
    }

    /// <summary>
    /// Tests that Redo returns false when nothing to redo.
    /// </summary>
    [Fact]
    public void Redo_ReturnsFalse_WhenEmpty()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();

        // Act
        var result = history.Redo(board);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests full workflow: execute 3 commands, undo all, redo all.
    /// </summary>
    [Fact]
    public void FullWorkflow_Execute3_Undo3_Redo3_VerifyStates()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();

        // Phase 1: Execute 3 commands
        var cmd1 = new SetValueCommand(0, 0, 0, 1, CellState.Empty);
        var cmd2 = new SetValueCommand(0, 1, 0, 2, CellState.Empty);
        var cmd3 = new SetValueCommand(0, 2, 0, 3, CellState.Empty);

        history.Execute(cmd1, board);
        history.Execute(cmd2, board);
        history.Execute(cmd3, board);

        // Verify after execute
        Assert.Equal(1, board.GetCell(0, 0).Value);
        Assert.Equal(2, board.GetCell(0, 1).Value);
        Assert.Equal(3, board.GetCell(0, 2).Value);
        Assert.True(history.CanUndo);
        Assert.Equal(3, history.UndoCount);

        // Phase 2: Undo all 3
        history.Undo(board);
        Assert.Equal(0, board.GetCell(0, 2).Value);

        history.Undo(board);
        Assert.Equal(0, board.GetCell(0, 1).Value);

        history.Undo(board);
        Assert.Equal(0, board.GetCell(0, 0).Value);

        // Verify after undo
        Assert.False(history.CanUndo);
        Assert.True(history.CanRedo);
        Assert.Equal(3, history.RedoCount);

        // Phase 3: Redo all 3
        history.Redo(board);
        Assert.Equal(1, board.GetCell(0, 0).Value);

        history.Redo(board);
        Assert.Equal(2, board.GetCell(0, 1).Value);

        history.Redo(board);
        Assert.Equal(3, board.GetCell(0, 2).Value);

        // Verify final state
        Assert.True(history.CanUndo);
        Assert.False(history.CanRedo);
        Assert.Equal(3, history.UndoCount);
    }

    /// <summary>
    /// Tests that GetHistory returns commands in correct order.
    /// </summary>
    [Fact]
    public void GetHistory_ReturnsMostRecentLast()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var cmd1 = new SetValueCommand(0, 0, 0, 1, CellState.Empty);
        var cmd2 = new SetValueCommand(0, 1, 0, 2, CellState.Empty);
        var cmd3 = new SetValueCommand(0, 2, 0, 3, CellState.Empty);

        history.Execute(cmd1, board);
        history.Execute(cmd2, board);
        history.Execute(cmd3, board);

        // Act
        var historyList = history.GetHistory();

        // Assert
        Assert.Equal(3, historyList.Count);
        Assert.Equal("Set cell (0, 0) to 1", historyList[0].Description);
        Assert.Equal("Set cell (0, 1) to 2", historyList[1].Description);
        Assert.Equal("Set cell (0, 2) to 3", historyList[2].Description);
    }

    /// <summary>
    /// Tests that GetUndoDescription returns description of last undo command.
    /// </summary>
    [Fact]
    public void GetUndoDescription_ReturnsLastCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(1, 2, 0, 8, CellState.Empty);
        history.Execute(command, board);

        // Act
        var description = history.GetUndoDescription();

        // Assert
        Assert.NotNull(description);
        Assert.Contains("8", description);
    }

    /// <summary>
    /// Tests that GetUndoDescription returns null when nothing to undo.
    /// </summary>
    [Fact]
    public void GetUndoDescription_ReturnsNull_WhenNoUndo()
    {
        // Arrange
        var history = new CommandHistory();

        // Act
        var description = history.GetUndoDescription();

        // Assert
        Assert.Null(description);
    }

    /// <summary>
    /// Tests that GetRedoDescription returns description of last redo command.
    /// </summary>
    [Fact]
    public void GetRedoDescription_ReturnsLastCommand()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var command = new SetValueCommand(3, 4, 0, 6, CellState.Empty);
        history.Execute(command, board);
        history.Undo(board);

        // Act
        var description = history.GetRedoDescription();

        // Assert
        Assert.NotNull(description);
        Assert.Contains("6", description);
    }

    /// <summary>
    /// Tests that GetRedoDescription returns null when nothing to redo.
    /// </summary>
    [Fact]
    public void GetRedoDescription_ReturnsNull_WhenNoRedo()
    {
        // Arrange
        var history = new CommandHistory();

        // Act
        var description = history.GetRedoDescription();

        // Assert
        Assert.Null(description);
    }

    /// <summary>
    /// Tests that Clear removes all commands from both stacks.
    /// </summary>
    [Fact]
    public void Clear_EmptiesBothStacks()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();
        var cmd1 = new SetValueCommand(0, 0, 0, 5, CellState.Empty);
        var cmd2 = new SetValueCommand(0, 1, 0, 3, CellState.Empty);

        history.Execute(cmd1, board);
        history.Execute(cmd2, board);
        history.Undo(board);

        // Act
        history.Clear();

        // Assert
        Assert.False(history.CanUndo);
        Assert.False(history.CanRedo);
        Assert.Equal(0, history.UndoCount);
        Assert.Equal(0, history.RedoCount);
    }

    /// <summary>
    /// Tests that multiple command types work together.
    /// </summary>
    [Fact]
    public void MixedCommands_ExecuteUndoRedo()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();

        var valueCmd = new SetValueCommand(0, 0, 0, 5, CellState.Empty);
        var noteCmd = new SetNoteCommand(0, 1, digit: 3, wasPresent: false);
        var clearCmd = new ClearCellCommand(0, 2, new Cell(0, 2, value: 0));

        // Act - Execute all
        history.Execute(valueCmd, board);
        history.Execute(noteCmd, board);
        history.Execute(clearCmd, board);

        // Assert after execute
        Assert.Equal(3, history.UndoCount);
        Assert.True(history.CanUndo);

        // Act - Undo all
        history.Undo(board);
        history.Undo(board);
        history.Undo(board);

        // Assert after undo
        Assert.False(history.CanUndo);
        Assert.True(history.CanRedo);
        Assert.Equal(3, history.RedoCount);

        // Act - Redo all
        history.Redo(board);
        history.Redo(board);
        history.Redo(board);

        // Assert after redo
        Assert.True(history.CanUndo);
        Assert.False(history.CanRedo);
    }

    /// <summary>
    /// Tests that Execute throws on null command.
    /// </summary>
    [Fact]
    public void Execute_WithNullCommand_Throws()
    {
        // Arrange
        var history = new CommandHistory();
        var board = SudokuBoard.CreateEmpty();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => history.Execute(command: null!, board));
    }

    /// <summary>
    /// Tests that Execute throws on null board.
    /// </summary>
    [Fact]
    public void Execute_WithNullBoard_Throws()
    {
        // Arrange
        var history = new CommandHistory();
        var command = new SetValueCommand(0, 0, 0, 5, CellState.Empty);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => history.Execute(command, board: null!));
    }
}

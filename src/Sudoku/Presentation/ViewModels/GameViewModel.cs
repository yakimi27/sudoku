using Sudoku.Core.Commands;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for the active game, managing game state, board, commands, and service interactions.
/// Handles cell selection, value entry, note management, undo/redo, hints, and win detection.
/// Integrates autosave to persist game progress automatically.
/// </summary>
public class GameViewModel : ObservableObject
{
    private readonly IHintService _hintService;
    private readonly ITimerService _timerService;
    private readonly CommandHistory _commandHistory;
    private readonly IStatisticsService _statisticsService;
    private readonly IAutoSaveService _autoSaveService;

    private GameSession? _gameSession;
    private BoardViewModel? _boardViewModel;
    private CellViewModel? _selectedCell;
    private TimeSpan _elapsedTime;
    private int _hintsRemaining;
    private bool _isGameActive;
    private bool _isGamePaused;

    /// <summary>
    /// Event raised when the game is completed successfully.
    /// </summary>
    public event EventHandler? GameCompleted;

    /// <summary>
    /// Event raised when the player gives up on the game.
    /// </summary>
    public event EventHandler? GameGivenUp;

    /// <summary>
    /// Initializes a new instance of the GameViewModel class.
    /// </summary>
    /// <param name="hintService">The hint service for generating hints.</param>
    /// <param name="timerService">The timer service for tracking game time.</param>
    /// <param name="commandHistory">The command history for undo/redo.</param>
    /// <param name="statisticsService">The statistics service for recording completions.</param>
    /// <param name="autoSaveService">The autosave service for persisting game progress.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public GameViewModel(
        IHintService hintService,
        ITimerService timerService,
        CommandHistory commandHistory,
        IStatisticsService statisticsService,
        IAutoSaveService autoSaveService)
    {
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        _commandHistory = commandHistory ?? throw new ArgumentNullException(nameof(commandHistory));
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _autoSaveService = autoSaveService ?? throw new ArgumentNullException(nameof(autoSaveService));

        _elapsedTime = TimeSpan.Zero;
        _hintsRemaining = _hintService.MaxHints;
        _isGameActive = false;
        _isGamePaused = false;

        // Subscribe to timer changes
        _timerService.TimeChanged += OnTimerTimeChanged;
    }

    /// <summary>
    /// Gets the current game session.
    /// </summary>
    public GameSession? GameSession
    {
        get => _gameSession;
        private set => SetProperty(ref _gameSession, value);
    }

    /// <summary>
    /// Gets the board view model for UI binding.
    /// </summary>
    public BoardViewModel? BoardViewModel
    {
        get => _boardViewModel;
        private set => SetProperty(ref _boardViewModel, value);
    }

    /// <summary>
    /// Gets or sets the currently selected cell.
    /// </summary>
    public CellViewModel? SelectedCell
    {
        get => _selectedCell;
        set => SetProperty(ref _selectedCell, value);
    }

    /// <summary>
    /// Gets the elapsed time for the current game.
    /// </summary>
    public TimeSpan ElapsedTime
    {
        get => _elapsedTime;
        private set => SetProperty(ref _elapsedTime, value);
    }

    /// <summary>
    /// Gets the number of remaining hints.
    /// </summary>
    public int HintsRemaining
    {
        get => _hintsRemaining;
        private set => SetProperty(ref _hintsRemaining, value);
    }

    /// <summary>
    /// Gets a value indicating whether the game is currently active.
    /// </summary>
    public bool IsGameActive
    {
        get => _isGameActive;
        private set => SetProperty(ref _isGameActive, value);
    }

    /// <summary>
    /// Gets a value indicating whether the game is paused.
    /// </summary>
    public bool IsGamePaused
    {
        get => _isGamePaused;
        private set => SetProperty(ref _isGamePaused, value);
    }

    /// <summary>
    /// Gets a value indicating whether undo is available.
    /// </summary>
    public bool CanUndo => _commandHistory.CanUndo;

    /// <summary>
    /// Gets a value indicating whether redo is available.
    /// </summary>
    public bool CanRedo => _commandHistory.CanRedo;

    /// <summary>
    /// Gets a value indicating whether a hint can be used.
    /// </summary>
    public bool CanUseHint => IsGameActive && HintsRemaining > 0;

    /// <summary>
    /// Initializes a new game session.
    /// </summary>
    /// <param name="session">The game session to start.</param>
    /// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
    public void StartNewGame(GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        GameSession = session;
        BoardViewModel = new BoardViewModel(session.Board);
        ElapsedTime = TimeSpan.Zero;
        HintsRemaining = _hintService.MaxHints;
        IsGameActive = true;
        IsGamePaused = false;

        _timerService.Start();
    }

    /// <summary>
    /// Restores a previously saved game session and resumes playing.
    /// Restores board state, elapsed time, hints used, and resumes the timer.
    /// </summary>
    /// <param name="session">The saved game session to restore.</param>
    /// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
    public void RestorePreviousGame(GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        GameSession = session;
        BoardViewModel = new BoardViewModel(session.Board);
        ElapsedTime = session.ElapsedTime;
        HintsRemaining = _hintService.MaxHints - session.HintsUsed;
        IsGameActive = true;
        IsGamePaused = false;

        // Resume the timer from where it left off
        _timerService.Start();
    }

    /// <summary>
    /// Sets a value in a cell, executing a SetValueCommand and triggering autosave.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    /// <param name="value">The value to set (1-9, or 0 to clear).</param>
    public void SetCellValue(int row, int column, int value)
    {
        if (GameSession?.Board == null)
            return;

        var currentCell = GameSession.Board.GetCell(row, column);
        var command = new SetValueCommand(row, column, currentCell.Value, value, currentCell.State);
        _commandHistory.Execute(command, GameSession.Board);

        RefreshBoardView();
        CheckWinCondition();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));

        // Trigger autosave after command execution
        TriggerAutoSave();
    }

    /// <summary>
    /// Adds or removes a note (candidate) in a cell, executing a SetNoteCommand and triggering autosave.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    /// <param name="note">The note value (1-9).</param>
    /// <param name="isAdding">True to add the note, false to remove.</param>
    public void ToggleCellNote(int row, int column, int note, bool isAdding)
    {
        if (GameSession?.Board == null)
            return;

        var currentCell = GameSession.Board.GetCell(row, column);
        bool wasPresent = currentCell.Notes.Contains(note);

        var command = new SetNoteCommand(row, column, note, wasPresent);
        _commandHistory.Execute(command, GameSession.Board);

        RefreshBoardView();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));

        // Trigger autosave after command execution
        TriggerAutoSave();
    }

    /// <summary>
    /// Undoes the last command.
    /// </summary>
    public void UndoCommand()
    {
        if (GameSession?.Board == null)
            return;

        if (_commandHistory.Undo(GameSession.Board))
        {
            RefreshBoardView();
            CheckWinCondition();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
    }

    /// <summary>
    /// Redoes the last undone command.
    /// </summary>
    public void RedoCommand()
    {
        if (GameSession?.Board == null)
            return;

        if (_commandHistory.Redo(GameSession.Board))
        {
            RefreshBoardView();
            CheckWinCondition();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
    }

    /// <summary>
    /// Requests a hint from the hint service.
    /// </summary>
    /// <param name="hintType">The type of hint to request.</param>
    public async Task UseHintAsync(HintType hintType)
    {
        if (!_hintService.UseHint(hintType))
            return;

        HintsRemaining--;
        GameSession?.UseHint();

        if (GameSession?.Board == null)
            return;

        // Serialize board state for hint generation (placeholder)
        string gameState = SerializeGameState();

        try
        {
            await _hintService.GenerateHintAsync(hintType, gameState);
        }
        catch (Exception)
        {
            // Log or handle hint generation error
        }
    }

    /// <summary>
    /// Pauses the current game.
    /// </summary>
    public void PauseGame()
    {
        if (!IsGameActive || IsGamePaused)
            return;

        _timerService.Pause();
        IsGamePaused = true;
    }

    /// <summary>
    /// Resumes the paused game.
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGameActive || !IsGamePaused)
            return;

        _timerService.Resume();
        IsGamePaused = false;
    }

    /// <summary>
    /// Gives up on the current game.
    /// </summary>
    public void GiveUp()
    {
        if (!IsGameActive)
            return;

        _timerService.Stop();
        IsGameActive = false;

        GameGivenUp?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Checks whether the puzzle is completed and triggers win condition if so.
    /// </summary>
    private void CheckWinCondition()
    {
        if (GameSession?.Board == null || !IsGameActive)
            return;

        if (IsBoardFilled() && IsBoardValid())
        {
            CompleteGame();
        }
    }

    /// <summary>
    /// Completes the game successfully.
    /// </summary>
    private void CompleteGame()
    {
        _timerService.Stop();
        IsGameActive = false;

        // Record statistics
        if (GameSession != null)
        {
            _ = _statisticsService.RecordGameCompletionAsync(GameSession.Difficulty, ElapsedTime);
        }

        GameCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Checks whether all cells in the board are filled.
    /// </summary>
    private bool IsBoardFilled()
    {
        if (GameSession?.Board == null)
            return false;

        for (int row = 0; row < SudokuBoard.BoardSize; row++)
        {
            for (int col = 0; col < SudokuBoard.BoardSize; col++)
            {
                var cell = GameSession.Board.GetCell(row, col);
                if (cell.Value == 0)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates that the current board state is valid (no conflicts).
    /// </summary>
    private bool IsBoardValid()
    {
        if (GameSession?.Board == null)
            return false;

        // Check rows
        for (int row = 0; row < SudokuBoard.BoardSize; row++)
        {
            var values = new HashSet<int>();
            for (int col = 0; col < SudokuBoard.BoardSize; col++)
            {
                var cell = GameSession.Board.GetCell(row, col);
                if (cell.Value > 0)
                {
                    if (!values.Add(cell.Value))
                        return false;
                }
            }
        }

        // Check columns
        for (int col = 0; col < SudokuBoard.BoardSize; col++)
        {
            var values = new HashSet<int>();
            for (int row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var cell = GameSession.Board.GetCell(row, col);
                if (cell.Value > 0)
                {
                    if (!values.Add(cell.Value))
                        return false;
                }
            }
        }

        // Check 3x3 blocks
        for (int blockRow = 0; blockRow < 3; blockRow++)
        {
            for (int blockCol = 0; blockCol < 3; blockCol++)
            {
                var values = new HashSet<int>();
                for (int row = blockRow * 3; row < blockRow * 3 + 3; row++)
                {
                    for (int col = blockCol * 3; col < blockCol * 3 + 3; col++)
                    {
                        var cell = GameSession.Board.GetCell(row, col);
                        if (cell.Value > 0)
                        {
                            if (!values.Add(cell.Value))
                                return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Refreshes the board view model from the current game session board.
    /// </summary>
    private void RefreshBoardView()
    {
        if (GameSession?.Board != null && BoardViewModel != null)
        {
            BoardViewModel.RefreshFromBoard(GameSession.Board);
        }
    }

    /// <summary>
    /// Serializes the current game state for hint generation (placeholder).
    /// </summary>
    private string SerializeGameState()
    {
        // TODO: Implement proper serialization
        return string.Empty;
    }

    /// <summary>
    /// Handles timer time changed event.
    /// </summary>
    private void OnTimerTimeChanged(object? sender, TimeSpan elapsed)
    {
        ElapsedTime = elapsed;
    }

    /// <summary>
    /// Triggers an autosave of the current game session.
    /// Uses debouncing to avoid excessive disk writes.
    /// </summary>
    private void TriggerAutoSave()
    {
        if (GameSession != null)
        {
            // Fire and forget - don't await, as this is triggered during gameplay
            _ = _autoSaveService.SaveSessionAsync(GameSession);
        }
    }
}

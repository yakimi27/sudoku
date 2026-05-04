using Sudoku.Core.Enums;

namespace Sudoku.Core.Models;

/// <summary>
/// Represents an active game session with all game state.
/// Includes the board, mode, difficulty, timing, and completion tracking.
/// This is a serialization-friendly model with no circular references.
/// </summary>
public class GameSession
{
    /// <summary>
    /// Gets the Sudoku board for this game session.
    /// </summary>
    public SudokuBoard Board { get; }

    /// <summary>
    /// Gets the game mode for this session.
    /// </summary>
    public GameMode GameMode { get; }

    /// <summary>
    /// Gets the difficulty level of this puzzle.
    /// </summary>
    public Difficulty Difficulty { get; }

    /// <summary>
    /// Gets the total elapsed time in this game session.
    /// </summary>
    public TimeSpan ElapsedTime { get; private set; }

    /// <summary>
    /// Gets the number of hints used in this session.
    /// </summary>
    public int HintsUsed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the puzzle is completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the date and time when this session started.
    /// </summary>
    public DateTime StartedAt { get; }

    /// <summary>
    /// Gets the date and time when this session was completed, or null if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Gets a unique identifier for this game session.
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// Initializes a new instance of the GameSession class.
    /// </summary>
    /// <param name="board">The Sudoku board for this session.</param>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="difficulty">The puzzle difficulty.</param>
    /// <exception cref="ArgumentNullException">Thrown when board is null.</exception>
    public GameSession(SudokuBoard board, GameMode gameMode, Difficulty difficulty)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
        GameMode = gameMode;
        Difficulty = difficulty;
        ElapsedTime = TimeSpan.Zero;
        HintsUsed = 0;
        IsCompleted = false;
        StartedAt = DateTime.UtcNow;
        CompletedAt = null;
        SessionId = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Updates the elapsed time for this session.
    /// </summary>
    /// <param name="elapsedTime">The new elapsed time.</param>
    public void UpdateElapsedTime(TimeSpan elapsedTime)
    {
        if (!IsCompleted)
        {
            ElapsedTime = elapsedTime;
        }
    }

    /// <summary>
    /// Increments the hint usage counter.
    /// </summary>
    public void UseHint()
    {
        if (!IsCompleted)
        {
            HintsUsed++;
        }
    }

    /// <summary>
    /// Marks the session as completed.
    /// </summary>
    public void Complete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Determines if the board is fully solved (all cells filled without given values).
    /// </summary>
    /// <returns>True if all non-given cells are filled, false otherwise.</returns>
    public bool IsBoardFilled()
    {
        return Board.GetEmptyCellCount() == 0;
    }

    /// <summary>
    /// Gets session statistics for display or serialization.
    /// </summary>
    /// <returns>A summary of session data.</returns>
    public GameSessionSummary GetSummary()
    {
        return new GameSessionSummary
        {
            SessionId = SessionId,
            GameMode = GameMode,
            Difficulty = Difficulty,
            ElapsedTime = ElapsedTime,
            HintsUsed = HintsUsed,
            IsCompleted = IsCompleted,
            StartedAt = StartedAt,
            CompletedAt = CompletedAt
        };
    }
}

/// <summary>
/// A serialization-friendly summary of a game session.
/// Used for storing and displaying game history.
/// </summary>
public class GameSessionSummary
{
    /// <summary>
    /// Gets or sets the unique session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level.
    /// </summary>
    public Difficulty Difficulty { get; set; }

    /// <summary>
    /// Gets or sets the total elapsed time.
    /// </summary>
    public TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// Gets or sets the number of hints used.
    /// </summary>
    public int HintsUsed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the session was completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the session start time.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the session completion time.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

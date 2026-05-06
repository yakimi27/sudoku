namespace Sudoku.Infrastructure.Storage.Dtos;

/// <summary>
/// Data Transfer Object for a game session.
/// Plain data class used for serialization, with all state needed to restore a session.
/// </summary>
public class SessionDto
{
    /// <summary>
    /// Gets or sets the unique session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game mode as a string.
    /// </summary>
    public string GameMode { get; set; } = "Standard";

    /// <summary>
    /// Gets or sets the difficulty level as a string.
    /// </summary>
    public string Difficulty { get; set; } = "Medium";

    /// <summary>
    /// Gets or sets the board state as a grid of cell DTOs.
    /// </summary>
    public List<CellDto> Cells { get; set; } = new();

    /// <summary>
    /// Gets or sets the elapsed time in milliseconds.
    /// </summary>
    public long ElapsedTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the number of hints used in this session.
    /// </summary>
    public int HintsUsed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the puzzle is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this session started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this session was completed, or null if not completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the command history as a list of command DTOs.
    /// </summary>
    public List<CommandDto> Commands { get; set; } = new();

    /// <summary>
    /// Gets or sets the date this session was last saved.
    /// </summary>
    public DateTime LastSavedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the SessionDto class.
    /// </summary>
    public SessionDto()
    {
        LastSavedAt = DateTime.UtcNow;
    }
}

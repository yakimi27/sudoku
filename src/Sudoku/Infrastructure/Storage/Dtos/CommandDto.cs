namespace Sudoku.Infrastructure.Storage.Dtos;

/// <summary>
/// Data Transfer Object for a game command.
/// Plain data class used for serialization, representing command execution history.
/// </summary>
public class CommandDto
{
    /// <summary>
    /// Gets or sets the type name of the original command class.
    /// Used to identify the command type during deserialization.
    /// </summary>
    public string CommandType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable description of this command.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when this command was executed.
    /// </summary>
    public DateTime ExecutedAt { get; set; }

    /// <summary>
    /// Gets or sets the row affected by this command (if applicable).
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Gets or sets the column affected by this command (if applicable).
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Gets or sets the value used in this command (if applicable).
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the previous value before this command was executed (for undo).
    /// </summary>
    public int PreviousValue { get; set; }

    /// <summary>
    /// Gets or sets additional data specific to the command type.
    /// Can store serialized data for complex commands.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the CommandDto class.
    /// </summary>
    public CommandDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the CommandDto class with the specified values.
    /// </summary>
    /// <param name="commandType">The type name of the command.</param>
    /// <param name="description">The command description.</param>
    /// <param name="executedAt">When the command was executed.</param>
    public CommandDto(string commandType, string description, DateTime executedAt)
    {
        CommandType = commandType;
        Description = description;
        ExecutedAt = executedAt;
    }
}

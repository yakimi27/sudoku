using Sudoku.Core.Enums;

namespace Sudoku.Core.Models;

/// <summary>
/// Represents the result of a hint operation.
/// Contains the cells affected by the hint, localization key, and solving technique used.
/// </summary>
public record HintResult
{
    /// <summary>
    /// Gets the list of cell coordinates affected by this hint.
    /// Format: "(row, column)" where row and column are 0-based indices.
    /// </summary>
    public IReadOnlyList<(int Row, int Column)> AffectedCells { get; }

    /// <summary>
    /// Gets the localization key for the hint message.
    /// Used to display the hint explanation in the current language.
    /// </summary>
    public string MessageKey { get; }

    /// <summary>
    /// Gets the type of hint provided (e.g., RevealCell, HighlightError, etc.).
    /// </summary>
    public HintType HintType { get; }

    /// <summary>
    /// Gets the solving technique used or suggested by this hint.
    /// Examples: "Naked Single", "Hidden Single", "Direct Conflict", etc.
    /// </summary>
    public string TechniqueUsed { get; }

    /// <summary>
    /// Initializes a new instance of the HintResult record.
    /// </summary>
    /// <param name="affectedCells">List of affected cell coordinates.</param>
    /// <param name="messageKey">Localization key for the hint message.</param>
    /// <param name="hintType">The type of hint.</param>
    /// <param name="techniqueUsed">The solving technique name.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public HintResult(
        IReadOnlyList<(int Row, int Column)> affectedCells,
        string messageKey,
        HintType hintType,
        string techniqueUsed)
    {
        AffectedCells = affectedCells ?? throw new ArgumentNullException(nameof(affectedCells));
        MessageKey = messageKey ?? throw new ArgumentNullException(nameof(messageKey));
        HintType = hintType;
        TechniqueUsed = techniqueUsed ?? throw new ArgumentNullException(nameof(techniqueUsed));
    }
}

namespace Sudoku.Core.Enums;

/// <summary>
/// Defines the available game modes in Sudoku.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Standard 9x9 Sudoku with 3x3 boxes.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Thermometer Mode: values must strictly increase along thermometer paths.
    /// </summary>
    Thermometer = 1,

    /// <summary>
    /// Killer Mode: cells grouped into cages with sum constraints.
    /// </summary>
    Killer = 2,

    /// <summary>
    /// Mini Mode: 6x6 Sudoku with 2x3 blocks and digits 1-6.
    /// </summary>
    Mini = 3
}


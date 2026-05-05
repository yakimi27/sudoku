using Sudoku.Core.Enums;
using Sudoku.Core.GameModes;
using Xunit;

namespace Sudoku.Tests.GameModes;

/// <summary>
/// Unit tests for GameModeFactory.
/// </summary>
public class GameModeFactoryTests
{
    /// <summary>
    /// Tests that factory creates ClassicMode for Standard.
    /// </summary>
    [Fact]
    public void Create_WithStandard_ReturnsClassicMode()
    {
        // Act
        var mode = GameModeFactory.Create(GameMode.Standard);

        // Assert
        Assert.IsType<ClassicMode>(mode);
        Assert.Equal(GameMode.Standard, mode.ModeType);
    }

    /// <summary>
    /// Tests that factory creates ThermometerMode for Thermometer.
    /// </summary>
    [Fact]
    public void Create_WithThermometer_ReturnsThermometerMode()
    {
        // Act
        var mode = GameModeFactory.Create(GameMode.Thermometer);

        // Assert
        Assert.IsType<ThermometerMode>(mode);
        Assert.Equal(GameMode.Thermometer, mode.ModeType);
    }

    /// <summary>
    /// Tests that factory creates KillerMode for Killer.
    /// </summary>
    [Fact]
    public void Create_WithKiller_ReturnsKillerMode()
    {
        // Act
        var mode = GameModeFactory.Create(GameMode.Killer);

        // Assert
        Assert.IsType<KillerMode>(mode);
        Assert.Equal(GameMode.Killer, mode.ModeType);
    }

    /// <summary>
    /// Tests that factory creates MiniMode for Mini.
    /// </summary>
    [Fact]
    public void Create_WithMini_ReturnsMiniMode()
    {
        // Act
        var mode = GameModeFactory.Create(GameMode.Mini);

        // Assert
        Assert.IsType<MiniMode>(mode);
        Assert.Equal(GameMode.Mini, mode.ModeType);
    }

    /// <summary>
    /// Tests that factory throws for unknown mode.
    /// </summary>
    [Fact]
    public void Create_WithInvalidMode_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GameModeFactory.Create((GameMode)999));
    }

    /// <summary>
    /// Tests that GetDisplayNameKey returns correct keys for all modes.
    /// </summary>
    [Theory]
    [InlineData(GameMode.Standard, "mode.classic")]
    [InlineData(GameMode.Thermometer, "mode.thermometer")]
    [InlineData(GameMode.Killer, "mode.killer")]
    [InlineData(GameMode.Mini, "mode.mini")]
    public void GetDisplayNameKey_ReturnsCorrectKey(GameMode modeType, string expectedKey)
    {
        // Act
        var key = GameModeFactory.GetDisplayNameKey(modeType);

        // Assert
        Assert.Equal(expectedKey, key);
    }

    /// <summary>
    /// Tests that GetDisplayNameKey throws for unknown mode.
    /// </summary>
    [Fact]
    public void GetDisplayNameKey_WithInvalidMode_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GameModeFactory.GetDisplayNameKey((GameMode)999));
    }

    /// <summary>
    /// Tests that GetBoardDimensions returns correct dimensions for all modes.
    /// </summary>
    [Theory]
    [InlineData(GameMode.Standard, 9, 9, 3, 3)]
    [InlineData(GameMode.Thermometer, 9, 9, 3, 3)]
    [InlineData(GameMode.Killer, 9, 9, 3, 3)]
    [InlineData(GameMode.Mini, 6, 6, 2, 3)]
    public void GetBoardDimensions_ReturnsCorrectDimensions(
        GameMode modeType, int expectedRows, int expectedCols, int expectedBlockHeight, int expectedBlockWidth)
    {
        // Act
        var (rows, cols, blockHeight, blockWidth) = GameModeFactory.GetBoardDimensions(modeType);

        // Assert
        Assert.Equal(expectedRows, rows);
        Assert.Equal(expectedCols, cols);
        Assert.Equal(expectedBlockHeight, blockHeight);
        Assert.Equal(expectedBlockWidth, blockWidth);
    }

    /// <summary>
    /// Tests that GetBoardDimensions throws for unknown mode.
    /// </summary>
    [Fact]
    public void GetBoardDimensions_WithInvalidMode_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GameModeFactory.GetBoardDimensions((GameMode)999));
    }

    /// <summary>
    /// Tests that GetValidDigits returns correct digits for 9x9 modes.
    /// </summary>
    [Theory]
    [InlineData(GameMode.Standard)]
    [InlineData(GameMode.Thermometer)]
    [InlineData(GameMode.Killer)]
    public void GetValidDigits_For9x9Modes_Returns1Through9(GameMode modeType)
    {
        // Act
        var digits = GameModeFactory.GetValidDigits(modeType);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, digits);
    }

    /// <summary>
    /// Tests that GetValidDigits returns 1-6 for Mini mode.
    /// </summary>
    [Fact]
    public void GetValidDigits_ForMiniMode_Returns1Through6()
    {
        // Act
        var digits = GameModeFactory.GetValidDigits(GameMode.Mini);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, digits);
    }

    /// <summary>
    /// Tests that GetValidDigits throws for unknown mode.
    /// </summary>
    [Fact]
    public void GetValidDigits_WithInvalidMode_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GameModeFactory.GetValidDigits((GameMode)999));
    }
}

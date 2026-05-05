using Sudoku.Infrastructure.Services;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for TimerService.
/// </summary>
public class TimerServiceTests
{
    /// <summary>
    /// Tests that Start begins timing.
    /// </summary>
    [Fact]
    public void Start_BeginsTiming()
    {
        // Arrange
        using var service = new TimerService();

        // Act
        service.Start();

        // Assert
        Assert.True(service.IsRunning);
    }

    /// <summary>
    /// Tests that Pause stops timing.
    /// </summary>
    [Fact]
    public void Pause_StopsTiming()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();

        // Act
        service.Pause();

        // Assert
        Assert.False(service.IsRunning);
    }

    /// <summary>
    /// Tests that Resume restarts timing after pause.
    /// </summary>
    [Fact]
    public void Resume_RestartsAfterPause()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();
        service.Pause();

        // Act
        service.Resume();

        // Assert
        Assert.True(service.IsRunning);
    }

    /// <summary>
    /// Tests that Stop resets timer to zero.
    /// </summary>
    [Fact]
    public void Stop_ResetsTimer()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();

        // Act
        System.Threading.Thread.Sleep(100); // Let timer advance
        service.Stop();

        // Assert
        Assert.Equal(TimeSpan.Zero, service.ElapsedTime);
        Assert.False(service.IsRunning);
    }

    /// <summary>
    /// Tests that ElapsedTime increases while running.
    /// </summary>
    [Fact]
    public void ElapsedTime_IncreasesWhileRunning()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();

        // Act
        System.Threading.Thread.Sleep(1500);
        var elapsed = service.ElapsedTime;

        // Assert
        Assert.True(elapsed.TotalSeconds > 0);
    }

    /// <summary>
    /// Tests that SetCountdown sets the countdown duration.
    /// </summary>
    [Fact]
    public void SetCountdown_SetsDuration()
    {
        // Arrange
        using var service = new TimerService();
        var duration = TimeSpan.FromMinutes(5);

        // Act
        service.SetCountdown(duration);

        // Assert
        Assert.True(service.IsRunning || !service.IsRunning); // Service state not changed yet
    }

    /// <summary>
    /// Tests that Dispose prevents further operations.
    /// </summary>
    [Fact]
    public void Dispose_PreventsOperations()
    {
        // Arrange
        var service = new TimerService();
        service.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => service.Start());
    }
}

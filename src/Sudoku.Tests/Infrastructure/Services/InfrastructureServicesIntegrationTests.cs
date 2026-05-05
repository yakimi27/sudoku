using Sudoku.Core.Enums;
using Sudoku.Infrastructure.Services;
using Xunit;

namespace Sudoku.Tests.Infrastructure.Services;

/// <summary>
/// Integration tests for infrastructure services.
/// </summary>
public class InfrastructureServicesIntegrationTests
{
    /// <summary>
    /// Tests that TimerService can track elapsed time.
    /// </summary>
    [Fact]
    public void TimerService_TracksElapsedTime()
    {
        // Arrange
        using var service = new TimerService();

        // Act
        service.Start();
        System.Threading.Thread.Sleep(1500);
        var elapsed = service.ElapsedTime;
        service.Stop();

        // Assert
        Assert.True(elapsed.TotalSeconds > 0);
        Assert.False(service.IsRunning);
    }

    /// <summary>
    /// Tests that TimerService pause/resume works correctly.
    /// </summary>
    [Fact]
    public void TimerService_PauseResume_PreservesTime()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();
        System.Threading.Thread.Sleep(1500);
        var timeBeforePause = service.ElapsedTime;

        // Act
        service.Pause();
        System.Threading.Thread.Sleep(1000);
        var timeWhilePaused = service.ElapsedTime;
        service.Resume();
        System.Threading.Thread.Sleep(1500);
        var timeAfterResume = service.ElapsedTime;
        service.Stop();

        // Assert
        Assert.True(timeBeforePause.TotalSeconds > 0);
        Assert.Equal(timeBeforePause, timeWhilePaused); // Time should not change while paused
        Assert.True(timeAfterResume > timeWhilePaused); // Time should resume increasing
    }

    /// <summary>
    /// Tests that TimerService countdown mode works.
    /// </summary>
    [Fact]
    public void TimerService_Countdown_TriggersExpiration()
    {
        // Arrange
        using var service = new TimerService();
        var expired = false;
        service.TimerExpired += (sender, args) =>
        {
            expired = true;
        };

        // Act
        service.SetCountdown(TimeSpan.FromSeconds(2));
        service.Start();
        System.Threading.Thread.Sleep(2500); // Wait for countdown to expire

        // Assert
        Assert.True(expired);
    }

    /// <summary>
    /// Tests that TimerService TimeChanged event fires.
    /// </summary>
    [Fact]
    public void TimerService_TimeChanged_EventFires()
    {
        // Arrange
        using var service = new TimerService();
        var eventFired = false;
        service.TimeChanged += (sender, elapsed) =>
        {
            eventFired = true;
        };

        // Act
        service.Start();
        System.Threading.Thread.Sleep(1500);
        service.Stop();

        // Assert
        Assert.True(eventFired);
    }

    /// <summary>
    /// Tests that TimerService resets properly.
    /// </summary>
    [Fact]
    public void TimerService_Stop_ResetsToZero()
    {
        // Arrange
        using var service = new TimerService();
        service.Start();
        System.Threading.Thread.Sleep(1000);

        // Act
        service.Stop();

        // Assert
        Assert.Equal(TimeSpan.Zero, service.ElapsedTime);
        Assert.False(service.IsRunning);
    }

    /// <summary>
    /// Tests that multiple start/pause cycles work.
    /// </summary>
    [Fact]
    public void TimerService_MultipleCycles_WorkCorrectly()
    {
        // Arrange
        using var service = new TimerService();

        // Act & Assert - Cycle 1
        service.Start();
        System.Threading.Thread.Sleep(500);
        Assert.True(service.IsRunning);
        service.Pause();
        Assert.False(service.IsRunning);

        // Act & Assert - Cycle 2
        service.Resume();
        Assert.True(service.IsRunning);
        service.Stop();
        Assert.Equal(TimeSpan.Zero, service.ElapsedTime);
    }
}

# Session Autosave and Restore Integration Guide

This document describes how to integrate the session autosave and restore functionality into the WinUI 3 app when the runtime is available.

## Overview

The autosave system consists of:

1. **AutoSaveService** - Persists game state after every command with 500ms debounce
2. **SessionLifecycleService** - Coordinates load/clear operations during app lifecycle
3. **SessionRestoreCoordinator** - Handles the user confirmation flow for restoration
4. **GameViewModel updates** - Saves after every command execution

## Architecture

```
GameViewModel (triggers SaveSessionAsync after commands)
    ↓
AutoSaveService (debounces and delegates to SessionRepository)
    ↓
SessionRepository (persists GameSession to disk)

App.xaml.cs (on startup)
    ↓
SessionRestoreCoordinator (loads previous session)
    ↓
Show ContentDialog to user ("Continue previous game?")
    ↓
If Yes: GameViewModel.RestorePreviousGame(session)
If No: SessionLifecycleService.ClearSessionAsync()

App.xaml.cs (on suspend/shutdown)
    ↓
SessionLifecycleService.FlushPendingAutoSaveAsync()
```

## Dependency Injection Setup

Add these registrations to `App.xaml.cs` (or a DI configuration class):

```csharp
services.AddSingleton<IAutoSaveService>(sp =>
    new AutoSaveService(sp.GetRequiredService<SessionRepository>()));

services.AddSingleton<ISessionLifecycleService>(sp =>
    new SessionLifecycleService(
        sp.GetRequiredService<SessionRepository>(),
        sp.GetRequiredService<IAutoSaveService>()));

services.AddSingleton<SessionRestoreCoordinator>(sp =>
    new SessionRestoreCoordinator(sp.GetRequiredService<ISessionLifecycleService>()));

services.AddSingleton<GameViewModel>(sp =>
    new GameViewModel(
        sp.GetRequiredService<IHintService>(),
        sp.GetRequiredService<ITimerService>(),
        sp.GetRequiredService<CommandHistory>(),
        sp.GetRequiredService<IStatisticsService>(),
        sp.GetRequiredService<IAutoSaveService>()));
```

## App Lifecycle Integration

### On Application Startup

In the main app view model or page code-behind:

```csharp
private async Task LoadGameOnStartupAsync()
{
    var restoreCoordinator = ServiceLocator.GetService<SessionRestoreCoordinator>();

    var session = await restoreCoordinator.TryRestoreSessionWithUserConfirmAsync(
        async (previousSession) =>
        {
            // Show ContentDialog asking user if they want to continue
            var dialog = new ContentDialog
            {
                Title = "Continue Previous Game?",
                Content = $"You have a game in progress. Resume from {previousSession.ElapsedTime.TotalMinutes:F0} minutes?",
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Start New",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        });

    if (session != null)
    {
        var gameVM = ServiceLocator.GetService<GameViewModel>();
        gameVM.RestorePreviousGame(session);
        // Navigate to game page
    }
    else
    {
        // Show menu or new game selection
    }
}
```

### On Application Suspension

In `App.xaml.cs` class:

```csharp
public App()
{
    this.InitializeComponent();

    // Subscribe to app lifecycle events
    this.Suspending += App_Suspending;
    this.UnhandledException += App_UnhandledException;
}

private async void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
{
    var deferral = e.SuspendingOperation.GetDeferral();

    try
    {
        var lifecycleService = ServiceLocator.GetService<ISessionLifecycleService>();
        await lifecycleService.FlushPendingAutoSaveAsync();
    }
    finally
    {
        deferral.Complete();
    }
}

private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    try
    {
        var lifecycleService = ServiceLocator.GetService<ISessionLifecycleService>();
        await lifecycleService.FlushPendingAutoSaveAsync();
    }
    catch
    {
        // Suppress - emergency save shouldn't throw
    }

    // Log the exception and allow app to close gracefully
    Debug.WriteLine($"Unhandled exception: {e.Exception.Message}");
}
```

## Save Triggering (Already Implemented)

GameViewModel automatically triggers autosave after:
- `SetCellValue()` - When a user enters a digit
- `ToggleCellNote()` - When a user adds/removes a candidate

The autosave is debounced with a 500ms delay, so rapid commands (e.g., entering multiple values quickly) will be batched into a single save operation.

## Testing

To test the autosave and restore flow:

1. **Start a game** → Make a few moves → Close the app
2. **Restart the app** → You should see "Continue previous game?" dialog
3. **Click Continue** → Game should restore with the same board state and elapsed time
4. **Alternatively, click Start New** → Game should start fresh and clear the saved session

## Notes

- **Command History**: Currently, the command history (undo/redo) is NOT serialized to disk. The CommandDto class is defined but not yet integrated into the save path. If you need full undo/redo history restoration, you'll need to serialize each command type's data.

- **Debounce Duration**: The 500ms debounce is defined in AutoSaveService as `DebounceDelayMs`. Adjust if needed for your performance requirements.

- **Error Handling**: All autosave operations gracefully catch and log exceptions without throwing. The game continues even if saves fail.

- **Concurrency**: The autosave service uses CancellationTokenSource to manage concurrent save requests. The debounce timer is reset on each new save request, ensuring the most recent state is saved.

## Future Enhancements

1. **Serialized Command History**: Implement command history serialization to allow full game replay/undo restoration.
2. **Multiple Session Slots**: Support saving/loading multiple games instead of just one active session.
3. **Cloud Sync**: Extend SessionRepository to sync sessions to Azure or OneDrive.
4. **Compression**: Compress the serialized session data to reduce disk usage.

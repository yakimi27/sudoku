using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Sudoku.Core.Services;
using Sudoku.DI;

namespace Sudoku;

/// <summary>
/// Application entry point for the WinUI 3 Sudoku application.
/// Configures dependency injection and initializes the service container.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes a new instance of the App class.
    /// </summary>
    public App()
    {
        InitializeComponent();
        InitializeDependencyInjection();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    /// <summary>
    /// Configures all dependencies and initializes the service locator.
    /// This method must be called before accessing any services.
    /// </summary>
    private void InitializeDependencyInjection()
    {
        var services = new ServiceCollection();

        // Register Core Services
        RegisterCoreServices(services);

        // Register Infrastructure Services
        RegisterInfrastructureServices(services);

        // Register Presentation Services
        RegisterPresentationServices(services);

        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.Initialize(serviceProvider);
    }

    /// <summary>
    /// Registers all Core layer services (business logic, interfaces).
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection services)
    {
        // Placeholder: Core services will be registered here
        // services.AddSingleton<IStorageService, StorageService>();
        // services.AddSingleton<IStatisticsService, StatisticsService>();
        // services.AddSingleton<IProfileService, ProfileService>();
        // services.AddSingleton<ITimerService, TimerService>();
        // services.AddSingleton<IHintService, HintService>();
    }

    /// <summary>
    /// Registers all Infrastructure layer services (data access, external dependencies).
    /// </summary>
    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        // Placeholder: Infrastructure services will be registered here
    }

    /// <summary>
    /// Registers all Presentation layer services (ViewModels, converters).
    /// </summary>
    private static void RegisterPresentationServices(IServiceCollection services)
    {
        // Placeholder: Presentation services will be registered here
    }
}

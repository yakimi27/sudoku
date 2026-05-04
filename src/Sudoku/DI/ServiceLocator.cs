using Microsoft.Extensions.DependencyInjection;

namespace Sudoku.DI;

/// <summary>
/// Service Locator providing access to the application's dependency injection container.
/// Implements Singleton pattern - single instance throughout application lifecycle.
/// Acts as a thin wrapper around IServiceProvider for convenient global access.
/// </summary>
public sealed class ServiceLocator
{
    private static ServiceLocator? _instance;
    private readonly IServiceProvider _serviceProvider;

    private ServiceLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets the singleton instance of the ServiceLocator.
    /// </summary>
    public static ServiceLocator Instance => _instance ?? throw new InvalidOperationException(
        "ServiceLocator has not been initialized. Call Initialize() first.");

    /// <summary>
    /// Initializes the ServiceLocator with the given service provider.
    /// Must be called once during application startup.
    /// </summary>
    /// <param name="serviceProvider">The DI service provider to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if already initialized.</exception>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("ServiceLocator has already been initialized.");
        }

        _instance = new ServiceLocator(serviceProvider);
    }

    /// <summary>
    /// Resolves a service instance from the DI container.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>An instance of the requested service.</returns>
    /// <exception cref="InvalidOperationException">Thrown if service is not registered.</exception>
    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Attempts to resolve a service instance from the DI container.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <param name="service">The resolved service, or null if not found.</param>
    /// <returns>True if service was resolved, false otherwise.</returns>
    public bool TryGetService<T>(out T? service) where T : class
    {
        return _serviceProvider.GetService(typeof(T)) is T instance
            ? (service = instance, true).Item2
            : (service = null, false).Item2;
    }
}

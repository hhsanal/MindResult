using Microsoft.Extensions.DependencyInjection;

namespace MindResult.Core.DependencyInjection;

/// <summary>
/// Service registration extensions for MindResult integration with ASP.NET Core DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MindResult services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMindResult(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Register result factory service
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<IMindResultFactory, MindResultFactory>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<IMindResultFactory, MindResultFactory>();
                break;
            default:
                services.AddScoped<IMindResultFactory, MindResultFactory>();
                break;
        }

        // Register result context service for tracking correlation IDs and user context
        services.AddScoped<IMindResultContext, MindResultContext>();

        return services;
    }

    /// <summary>
    /// Adds MindResult services with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Configuration action for MindResult options.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMindResult(this IServiceCollection services, Action<MindResultOptions> configureOptions, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var options = new MindResultOptions();
        configureOptions(options);
        
        services.AddSingleton(options);
        
        return services.AddMindResult(lifetime);
    }
}

/// <summary>
/// Configuration options for MindResult.
/// </summary>
public class MindResultOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to automatically include correlation IDs in results.
    /// </summary>
    public bool IncludeCorrelationId { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically include execution time in results.
    /// </summary>
    public bool IncludeExecutionTime { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically include user context in results.
    /// </summary>
    public bool IncludeUserContext { get; set; } = false;

    /// <summary>
    /// Gets or sets the default correlation ID header name.
    /// </summary>
    public string CorrelationIdHeader { get; set; } = "X-Correlation-ID";
}
namespace MindResult.Core.DependencyInjection;

/// <summary>
/// Context interface for providing contextual information to MindResult instances.
/// </summary>
public interface IMindResultContext
{
    /// <summary>
    /// Gets or sets the current correlation ID.
    /// </summary>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the current user context.
    /// </summary>
    string? UserContext { get; set; }

    /// <summary>
    /// Gets or sets custom context data.
    /// </summary>
    Dictionary<string, object>? CustomData { get; set; }
}

/// <summary>
/// Context implementation for providing contextual information to MindResult instances.
/// </summary>
public class MindResultContext : IMindResultContext
{
    /// <inheritdoc />
    public string? CorrelationId { get; set; }

    /// <inheritdoc />
    public string? UserContext { get; set; }

    /// <inheritdoc />
    public Dictionary<string, object>? CustomData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MindResultContext"/> class.
    /// </summary>
    public MindResultContext()
    {
        // Generate a default correlation ID if none is provided
        CorrelationId = Guid.NewGuid().ToString("N")[..8];
    }
}
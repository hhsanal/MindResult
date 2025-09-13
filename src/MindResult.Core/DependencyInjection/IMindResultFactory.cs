namespace MindResult.Core.DependencyInjection;

/// <summary>
/// Factory interface for creating MindResult instances with automatic context information.
/// </summary>
public interface IMindResultFactory
{
    /// <summary>
    /// Creates a successful result without data.
    /// </summary>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult"/>.</returns>
    MindResult Success(Dictionary<string, object>? additionalFields = null);

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="data">The data to include in the result.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult{T}"/>.</returns>
    MindResult<T> Success<T>(T data, Dictionary<string, object>? additionalFields = null);

    /// <summary>
    /// Creates a successful result without data for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult{T}"/>.</returns>
    MindResult<T> Success<T>(Dictionary<string, object>? additionalFields = null);

    /// <summary>
    /// Creates a failed result without data.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A failed <see cref="MindResult"/>.</returns>
    MindResult Failure(string errorMessage, Dictionary<string, object>? additionalFields = null);

    /// <summary>
    /// Creates a failed result with data type.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A failed <see cref="MindResult{T}"/>.</returns>
    MindResult<T> Failure<T>(string errorMessage, Dictionary<string, object>? additionalFields = null);
}

/// <summary>
/// Factory implementation for creating MindResult instances with automatic context information.
/// </summary>
public class MindResultFactory : IMindResultFactory
{
    private readonly IMindResultContext _context;
    private readonly MindResultOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MindResultFactory"/> class.
    /// </summary>
    /// <param name="context">The result context.</param>
    /// <param name="options">The MindResult options.</param>
    public MindResultFactory(IMindResultContext context, MindResultOptions? options = null)
    {
        _context = context;
        _options = options ?? new MindResultOptions();
    }

    /// <inheritdoc />
    public MindResult Success(Dictionary<string, object>? additionalFields = null)
    {
        var fields = EnrichWithContext(additionalFields);
        return MindResult.Success(fields);
    }

    /// <inheritdoc />
    public MindResult<T> Success<T>(T data, Dictionary<string, object>? additionalFields = null)
    {
        var fields = EnrichWithContext(additionalFields);
        return MindResult<T>.Success(data, fields);
    }

    /// <inheritdoc />
    public MindResult<T> Success<T>(Dictionary<string, object>? additionalFields = null)
    {
        var fields = EnrichWithContext(additionalFields);
        return MindResult<T>.Success(fields);
    }

    /// <inheritdoc />
    public MindResult Failure(string errorMessage, Dictionary<string, object>? additionalFields = null)
    {
        var fields = EnrichWithContext(additionalFields);
        return MindResult.Failure(errorMessage, fields);
    }

    /// <inheritdoc />
    public MindResult<T> Failure<T>(string errorMessage, Dictionary<string, object>? additionalFields = null)
    {
        var fields = EnrichWithContext(additionalFields);
        return MindResult<T>.Failure(errorMessage, fields);
    }

    private Dictionary<string, object>? EnrichWithContext(Dictionary<string, object>? additionalFields)
    {
        var fields = additionalFields?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        
        if (_options.IncludeCorrelationId && !string.IsNullOrEmpty(_context.CorrelationId))
        {
            fields[MindResultFieldType.CorrelationId.ToString()] = _context.CorrelationId;
        }

        if (_options.IncludeUserContext && !string.IsNullOrEmpty(_context.UserContext))
        {
            fields[MindResultFieldType.UserContext.ToString()] = _context.UserContext;
        }

        return fields.Count > 0 ? fields : null;
    }
}
namespace MindResult.Core;

/// <summary>
/// Predefined additional field types for common scenarios.
/// Provides type safety for commonly used additional fields in results.
/// </summary>
public enum MindResultFieldType
{
    /// <summary>
    /// Indicates a warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Indicates information for debugging purposes.
    /// </summary>
    Debug,

    /// <summary>
    /// Indicates metadata about the operation.
    /// </summary>
    Metadata,

    /// <summary>
    /// Indicates validation errors.
    /// </summary>
    ValidationErrors,

    /// <summary>
    /// Indicates the execution time of the operation.
    /// </summary>
    ExecutionTime,

    /// <summary>
    /// Indicates the source of the operation.
    /// </summary>
    Source,

    /// <summary>
    /// Indicates correlation ID for tracking.
    /// </summary>
    CorrelationId,

    /// <summary>
    /// Indicates user context information.
    /// </summary>
    UserContext,

    /// <summary>
    /// Indicates pagination information.
    /// </summary>
    Pagination,

    /// <summary>
    /// Indicates caching information.
    /// </summary>
    Cache
}

/// <summary>
/// Extension methods for working with <see cref="MindResultFieldType"/> in a type-safe manner.
/// </summary>
public static class MindResultExtensions
{
    /// <summary>
    /// Adds a type-safe additional field to the result.
    /// </summary>
    /// <param name="result">The result to add the field to.</param>
    /// <param name="fieldType">The type of field to add.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>A new result with the additional field.</returns>
    public static MindResult WithField(this MindResult result, MindResultFieldType fieldType, object value)
    {
        return result.WithAdditionalField(fieldType.ToString(), value);
    }

    /// <summary>
    /// Adds a type-safe additional field to the result.
    /// </summary>
    /// <typeparam name="T">The type of data in the result.</typeparam>
    /// <param name="result">The result to add the field to.</param>
    /// <param name="fieldType">The type of field to add.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>A new result with the additional field.</returns>
    public static MindResult<T> WithField<T>(this MindResult<T> result, MindResultFieldType fieldType, object value)
    {
        return result.WithAdditionalField(fieldType.ToString(), value);
    }

    /// <summary>
    /// Gets a type-safe additional field from the result.
    /// </summary>
    /// <typeparam name="TField">The type of the field value.</typeparam>
    /// <param name="result">The result to get the field from.</param>
    /// <param name="fieldType">The type of field to get.</param>
    /// <returns>The field value if it exists and is of the correct type, otherwise the default value.</returns>
    public static TField? GetField<TField>(this MindResult result, MindResultFieldType fieldType)
    {
        return result.GetAdditionalField<TField>(fieldType.ToString());
    }

    /// <summary>
    /// Gets a type-safe additional field from the result.
    /// </summary>
    /// <typeparam name="T">The type of data in the result.</typeparam>
    /// <typeparam name="TField">The type of the field value.</typeparam>
    /// <param name="result">The result to get the field from.</param>
    /// <param name="fieldType">The type of field to get.</param>
    /// <returns>The field value if it exists and is of the correct type, otherwise the default value.</returns>
    public static TField? GetField<T, TField>(this MindResult<T> result, MindResultFieldType fieldType)
    {
        return result.GetAdditionalField<TField>(fieldType.ToString());
    }

    /// <summary>
    /// Adds warning information to the result.
    /// </summary>
    /// <param name="result">The result to add the warning to.</param>
    /// <param name="warning">The warning message.</param>
    /// <returns>A new result with the warning field.</returns>
    public static MindResult WithWarning(this MindResult result, string warning)
    {
        return result.WithField(MindResultFieldType.Warning, warning);
    }

    /// <summary>
    /// Adds warning information to the result.
    /// </summary>
    /// <typeparam name="T">The type of data in the result.</typeparam>
    /// <param name="result">The result to add the warning to.</param>
    /// <param name="warning">The warning message.</param>
    /// <returns>A new result with the warning field.</returns>
    public static MindResult<T> WithWarning<T>(this MindResult<T> result, string warning)
    {
        return result.WithField(MindResultFieldType.Warning, warning);
    }

    /// <summary>
    /// Adds correlation ID to the result for tracking purposes.
    /// </summary>
    /// <param name="result">The result to add the correlation ID to.</param>
    /// <param name="correlationId">The correlation ID.</param>
    /// <returns>A new result with the correlation ID field.</returns>
    public static MindResult WithCorrelationId(this MindResult result, string correlationId)
    {
        return result.WithField(MindResultFieldType.CorrelationId, correlationId);
    }

    /// <summary>
    /// Adds correlation ID to the result for tracking purposes.
    /// </summary>
    /// <typeparam name="T">The type of data in the result.</typeparam>
    /// <param name="result">The result to add the correlation ID to.</param>
    /// <param name="correlationId">The correlation ID.</param>
    /// <returns>A new result with the correlation ID field.</returns>
    public static MindResult<T> WithCorrelationId<T>(this MindResult<T> result, string correlationId)
    {
        return result.WithField(MindResultFieldType.CorrelationId, correlationId);
    }

    /// <summary>
    /// Adds execution time information to the result.
    /// </summary>
    /// <param name="result">The result to add the execution time to.</param>
    /// <param name="executionTime">The execution time.</param>
    /// <returns>A new result with the execution time field.</returns>
    public static MindResult WithExecutionTime(this MindResult result, TimeSpan executionTime)
    {
        return result.WithField(MindResultFieldType.ExecutionTime, executionTime);
    }

    /// <summary>
    /// Adds execution time information to the result.
    /// </summary>
    /// <typeparam name="T">The type of data in the result.</typeparam>
    /// <param name="result">The result to add the execution time to.</param>
    /// <param name="executionTime">The execution time.</param>
    /// <returns>A new result with the execution time field.</returns>
    public static MindResult<T> WithExecutionTime<T>(this MindResult<T> result, TimeSpan executionTime)
    {
        return result.WithField(MindResultFieldType.ExecutionTime, executionTime);
    }
}
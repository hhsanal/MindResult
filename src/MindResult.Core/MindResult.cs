namespace MindResult.Core;

/// <summary>
/// Represents the result of an operation without data.
/// Provides a way to handle success and failure states with optional additional information.
/// </summary>
public class MindResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets additional information about the result.
    /// </summary>
    public Dictionary<string, object>? AdditionalFields { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MindResult"/> class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    /// <param name="additionalFields">Additional fields to include with the result.</param>
    protected MindResult(bool isSuccess, string? errorMessage = null, Dictionary<string, object>? additionalFields = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        AdditionalFields = additionalFields;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult"/>.</returns>
    public static MindResult Success(Dictionary<string, object>? additionalFields = null)
    {
        return new MindResult(true, null, additionalFields);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A failed <see cref="MindResult"/>.</returns>
    public static MindResult Failure(string errorMessage, Dictionary<string, object>? additionalFields = null)
    {
        return new MindResult(false, errorMessage, additionalFields);
    }

    /// <summary>
    /// Adds an additional field to the result.
    /// </summary>
    /// <param name="key">The field key.</param>
    /// <param name="value">The field value.</param>
    /// <returns>A new <see cref="MindResult"/> with the additional field.</returns>
    public MindResult WithAdditionalField(string key, object value)
    {
        var fields = AdditionalFields?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        fields[key] = value;
        
        return new MindResult(IsSuccess, ErrorMessage, fields);
    }

    /// <summary>
    /// Gets the value of an additional field.
    /// </summary>
    /// <typeparam name="T">The type of the field value.</typeparam>
    /// <param name="key">The field key.</param>
    /// <returns>The field value if it exists and is of the correct type, otherwise the default value.</returns>
    public T? GetAdditionalField<T>(string key)
    {
        if (AdditionalFields?.TryGetValue(key, out var value) == true && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance for method chaining.</returns>
    public MindResult OnSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result is a failure.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance for method chaining.</returns>
    public MindResult OnFailure(Action<string?> action)
    {
        if (IsFailure)
        {
            action(ErrorMessage);
        }
        return this;
    }
}
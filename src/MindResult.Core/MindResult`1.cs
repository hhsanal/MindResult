namespace MindResult.Core;

/// <summary>
/// Represents the result of an operation with data of type T.
/// Provides a way to handle success and failure states with optional data and additional information.
/// </summary>
/// <typeparam name="T">The type of data contained in the result.</typeparam>
public class MindResult<T> : MindResult
{
    /// <summary>
    /// Gets the data value if the operation was successful.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets a value indicating whether the result contains data.
    /// </summary>
    public bool HasData => Data is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MindResult{T}"/> class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful.</param>
    /// <param name="data">The data value.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    /// <param name="additionalFields">Additional fields to include with the result.</param>
    protected MindResult(bool isSuccess, T? data = default, string? errorMessage = null, Dictionary<string, object>? additionalFields = null)
        : base(isSuccess, errorMessage, additionalFields)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <param name="data">The data to include in the result.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult{T}"/>.</returns>
    public static MindResult<T> Success(T data, Dictionary<string, object>? additionalFields = null)
    {
        return new MindResult<T>(true, data, null, additionalFields);
    }

    /// <summary>
    /// Creates a successful result without data.
    /// </summary>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A successful <see cref="MindResult{T}"/>.</returns>
    public static new MindResult<T> Success(Dictionary<string, object>? additionalFields = null)
    {
        return new MindResult<T>(true, default, null, additionalFields);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="additionalFields">Optional additional fields to include.</param>
    /// <returns>A failed <see cref="MindResult{T}"/>.</returns>
    public static new MindResult<T> Failure(string errorMessage, Dictionary<string, object>? additionalFields = null)
    {
        return new MindResult<T>(false, default, errorMessage, additionalFields);
    }

    /// <summary>
    /// Adds an additional field to the result.
    /// </summary>
    /// <param name="key">The field key.</param>
    /// <param name="value">The field value.</param>
    /// <returns>A new <see cref="MindResult{T}"/> with the additional field.</returns>
    public new MindResult<T> WithAdditionalField(string key, object value)
    {
        var fields = AdditionalFields?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        fields[key] = value;
        
        return new MindResult<T>(IsSuccess, Data, ErrorMessage, fields);
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute with the data.</param>
    /// <returns>The current result instance for method chaining.</returns>
    public MindResult<T> OnSuccess(Action<T?> action)
    {
        if (IsSuccess)
        {
            action(Data);
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result is successful and has data.
    /// </summary>
    /// <param name="action">The action to execute with the non-null data.</param>
    /// <returns>The current result instance for method chaining.</returns>
    public MindResult<T> OnSuccessWithData(Action<T> action)
    {
        if (IsSuccess && HasData)
        {
            action(Data!);
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result is a failure.
    /// </summary>
    /// <param name="action">The action to execute with the error message.</param>
    /// <returns>The current result instance for method chaining.</returns>
    public new MindResult<T> OnFailure(Action<string?> action)
    {
        if (IsFailure)
        {
            action(ErrorMessage);
        }
        return this;
    }

    /// <summary>
    /// Maps the data to a new type if the result is successful.
    /// </summary>
    /// <typeparam name="TResult">The type to map to.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>A new <see cref="MindResult{TResult}"/> with the mapped data.</returns>
    public MindResult<TResult> Map<TResult>(Func<T?, TResult> mapper)
    {
        if (IsFailure)
        {
            return MindResult<TResult>.Failure(ErrorMessage!, AdditionalFields);
        }

        try
        {
            var mappedData = mapper(Data);
            return MindResult<TResult>.Success(mappedData, AdditionalFields);
        }
        catch (Exception ex)
        {
            return MindResult<TResult>.Failure($"Mapping failed: {ex.Message}", AdditionalFields);
        }
    }

    /// <summary>
    /// Binds the result to another operation if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the result data.</typeparam>
    /// <param name="binder">The binding function that returns a new MindResult.</param>
    /// <returns>The result of the binding operation.</returns>
    public MindResult<TResult> Bind<TResult>(Func<T?, MindResult<TResult>> binder)
    {
        if (IsFailure)
        {
            return MindResult<TResult>.Failure(ErrorMessage!, AdditionalFields);
        }

        try
        {
            return binder(Data);
        }
        catch (Exception ex)
        {
            return MindResult<TResult>.Failure($"Binding operation failed: {ex.Message}", AdditionalFields);
        }
    }

    /// <summary>
    /// Implicitly converts the result data to the result type.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    public static implicit operator T?(MindResult<T> result)
    {
        return result.Data;
    }

    /// <summary>
    /// Implicitly converts data to a successful result.
    /// </summary>
    /// <param name="data">The data to convert.</param>
    public static implicit operator MindResult<T>(T data)
    {
        return Success(data);
    }
}
using MindResult.Core;

namespace MindResult.Tests;

public class MindResultGenericTests
{
    [Fact]
    public void Success_With_Data_Should_Create_Successful_Result()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = MindResult<string>.Success(data);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(data, result.Data);
        Assert.True(result.HasData);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Success_Without_Data_Should_Create_Successful_Result()
    {
        // Act
        var result = MindResult<string>.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Data);
        Assert.False(result.HasData);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Failure_Should_Create_Failed_Result()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = MindResult<string>.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Data);
        Assert.False(result.HasData);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void WithAdditionalField_Should_Add_Field_To_Result()
    {
        // Arrange
        var result = MindResult<int>.Success(42);

        // Act
        var newResult = result.WithAdditionalField("test", "value");

        // Assert
        Assert.NotNull(newResult.AdditionalFields);
        Assert.Equal("value", newResult.AdditionalFields["test"]);
        Assert.Equal(42, newResult.Data);
        Assert.NotSame(result, newResult); // Should create new instance
    }

    [Fact]
    public void OnSuccess_Should_Execute_Action_With_Data()
    {
        // Arrange
        var data = "test data";
        var result = MindResult<string>.Success(data);
        string? receivedData = null;

        // Act
        result.OnSuccess(d => receivedData = d);

        // Assert
        Assert.Equal(data, receivedData);
    }

    [Fact]
    public void OnSuccessWithData_Should_Execute_Action_Only_When_Has_Data()
    {
        // Arrange
        var data = "test data";
        var resultWithData = MindResult<string>.Success(data);
        var resultWithoutData = MindResult<string>.Success();
        var executedCount = 0;

        // Act
        resultWithData.OnSuccessWithData(_ => executedCount++);
        resultWithoutData.OnSuccessWithData(_ => executedCount++);

        // Assert
        Assert.Equal(1, executedCount); // Only executed for result with data
    }

    [Fact]
    public void Map_Should_Transform_Data_For_Successful_Result()
    {
        // Arrange
        var result = MindResult<int>.Success(42);

        // Act
        var mappedResult = result.Map(x => $"Value: {x}");

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("Value: 42", mappedResult.Data);
    }

    [Fact]
    public void Map_Should_Preserve_Failure_State()
    {
        // Arrange
        var result = MindResult<int>.Failure("Original error");

        // Act
        var mappedResult = result.Map(x => $"Value: {x}");

        // Assert
        Assert.False(mappedResult.IsSuccess);
        Assert.Equal("Original error", mappedResult.ErrorMessage);
        Assert.Null(mappedResult.Data);
    }

    [Fact]
    public void Map_Should_Handle_Mapping_Exceptions()
    {
        // Arrange
        var result = MindResult<int>.Success(42);

        // Act
        var mappedResult = result.Map<string>(_ => throw new InvalidOperationException("Mapping failed"));

        // Assert
        Assert.False(mappedResult.IsSuccess);
        Assert.Contains("Mapping failed", mappedResult.ErrorMessage!);
    }

    [Fact]
    public void Bind_Should_Chain_Successful_Operations()
    {
        // Arrange
        var result = MindResult<int>.Success(42);

        // Act
        var boundResult = result.Bind(x => MindResult<string>.Success($"Value: {x}"));

        // Assert
        Assert.True(boundResult.IsSuccess);
        Assert.Equal("Value: 42", boundResult.Data);
    }

    [Fact]
    public void Bind_Should_Preserve_Original_Failure()
    {
        // Arrange
        var result = MindResult<int>.Failure("Original error");

        // Act
        var boundResult = result.Bind(x => MindResult<string>.Success($"Value: {x}"));

        // Assert
        Assert.False(boundResult.IsSuccess);
        Assert.Equal("Original error", boundResult.ErrorMessage);
    }

    [Fact]
    public void Bind_Should_Handle_Binding_Exceptions()
    {
        // Arrange
        var result = MindResult<int>.Success(42);

        // Act
        var boundResult = result.Bind<string>(_ => throw new InvalidOperationException("Binding failed"));

        // Assert
        Assert.False(boundResult.IsSuccess);
        Assert.Contains("Binding operation failed", boundResult.ErrorMessage!);
    }

    [Fact]
    public void Implicit_Conversion_From_Data_Should_Create_Successful_Result()
    {
        // Act
        MindResult<string> result = "test data";

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("test data", result.Data);
    }

    [Fact]
    public void Implicit_Conversion_To_Data_Should_Return_Data()
    {
        // Arrange
        var result = MindResult<string>.Success("test data");

        // Act
        string? data = result;

        // Assert
        Assert.Equal("test data", data);
    }

    [Fact]
    public void Implicit_Conversion_To_Data_Should_Return_Null_For_Failed_Result()
    {
        // Arrange
        var result = MindResult<string>.Failure("error");

        // Act
        string? data = result;

        // Assert
        Assert.Null(data);
    }
}
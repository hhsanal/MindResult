using MindResult.Core;

namespace MindResult.Tests;

public class MindResultTests
{
    [Fact]
    public void Success_Should_Create_Successful_Result()
    {
        // Act
        var result = Core.MindResult.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.AdditionalFields);
    }

    [Fact]
    public void Success_With_AdditionalFields_Should_Include_Fields()
    {
        // Arrange
        var fields = new Dictionary<string, object> { { "key1", "value1" } };

        // Act
        var result = Core.MindResult.Success(fields);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.AdditionalFields);
        Assert.Equal("value1", result.AdditionalFields["key1"]);
    }

    [Fact]
    public void Failure_Should_Create_Failed_Result()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Core.MindResult.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.AdditionalFields);
    }

    [Fact]
    public void Failure_With_AdditionalFields_Should_Include_Fields()
    {
        // Arrange
        var errorMessage = "Something went wrong";
        var fields = new Dictionary<string, object> { { "errorCode", 500 } };

        // Act
        var result = Core.MindResult.Failure(errorMessage, fields);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.NotNull(result.AdditionalFields);
        Assert.Equal(500, result.AdditionalFields["errorCode"]);
    }

    [Fact]
    public void WithAdditionalField_Should_Add_Field_To_Result()
    {
        // Arrange
        var result = Core.MindResult.Success();

        // Act
        var newResult = result.WithAdditionalField("test", "value");

        // Assert
        Assert.NotNull(newResult.AdditionalFields);
        Assert.Equal("value", newResult.AdditionalFields["test"]);
        Assert.NotSame(result, newResult); // Should create new instance
    }

    [Fact]
    public void GetAdditionalField_Should_Return_Typed_Value()
    {
        // Arrange
        var fields = new Dictionary<string, object> { { "number", 42 } };
        var result = Core.MindResult.Success(fields);

        // Act
        var value = result.GetAdditionalField<int>("number");

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void GetAdditionalField_Should_Return_Default_For_Missing_Key()
    {
        // Arrange
        var result = Core.MindResult.Success();

        // Act
        var value = result.GetAdditionalField<string>("missing");

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public void GetAdditionalField_Should_Return_Default_For_Wrong_Type()
    {
        // Arrange
        var fields = new Dictionary<string, object> { { "text", "hello" } };
        var result = Core.MindResult.Success(fields);

        // Act
        var value = result.GetAdditionalField<int>("text");

        // Assert
        Assert.Equal(0, value);
    }

    [Fact]
    public void OnSuccess_Should_Execute_Action_For_Successful_Result()
    {
        // Arrange
        var result = Core.MindResult.Success();
        var executed = false;

        // Act
        result.OnSuccess(() => executed = true);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void OnSuccess_Should_Not_Execute_Action_For_Failed_Result()
    {
        // Arrange
        var result = Core.MindResult.Failure("error");
        var executed = false;

        // Act
        result.OnSuccess(() => executed = true);

        // Assert
        Assert.False(executed);
    }

    [Fact]
    public void OnFailure_Should_Execute_Action_For_Failed_Result()
    {
        // Arrange
        var result = Core.MindResult.Failure("error message");
        string? receivedError = null;

        // Act
        result.OnFailure(error => receivedError = error);

        // Assert
        Assert.Equal("error message", receivedError);
    }

    [Fact]
    public void OnFailure_Should_Not_Execute_Action_For_Successful_Result()
    {
        // Arrange
        var result = Core.MindResult.Success();
        var executed = false;

        // Act
        result.OnFailure(_ => executed = true);

        // Assert
        Assert.False(executed);
    }
}
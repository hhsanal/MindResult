using MindResult.Core;

namespace MindResult.Tests;

public class MindResultExtensionsTests
{
    [Fact]
    public void WithField_Should_Add_Type_Safe_Field()
    {
        // Arrange
        var result = Core.MindResult.Success();

        // Act
        var newResult = result.WithField(MindResultFieldType.Warning, "This is a warning");

        // Assert
        Assert.NotNull(newResult.AdditionalFields);
        Assert.Equal("This is a warning", newResult.AdditionalFields[MindResultFieldType.Warning.ToString()]);
    }

    [Fact]
    public void WithField_Generic_Should_Add_Type_Safe_Field()
    {
        // Arrange
        var result = MindResult<string>.Success("data");

        // Act
        var newResult = result.WithField(MindResultFieldType.Debug, "Debug info");

        // Assert
        Assert.NotNull(newResult.AdditionalFields);
        Assert.Equal("Debug info", newResult.AdditionalFields[MindResultFieldType.Debug.ToString()]);
        Assert.Equal("data", newResult.Data);
    }

    [Fact]
    public void GetField_Should_Return_Type_Safe_Field()
    {
        // Arrange
        var result = Core.MindResult.Success()
            .WithField(MindResultFieldType.ExecutionTime, TimeSpan.FromMilliseconds(100));

        // Act
        var executionTime = result.GetField<TimeSpan>(MindResultFieldType.ExecutionTime);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(100), executionTime);
    }

    [Fact]
    public void GetField_Generic_Should_Return_Type_Safe_Field()
    {
        // Arrange
        var result = MindResult<string>.Success("data")
            .WithField(MindResultFieldType.CorrelationId, "abc123");

        // Act
        var correlationId = result.GetField<string, string>(MindResultFieldType.CorrelationId);

        // Assert
        Assert.Equal("abc123", correlationId);
    }

    [Fact]
    public void WithWarning_Should_Add_Warning_Field()
    {
        // Arrange
        var result = Core.MindResult.Success();
        var warning = "This is a warning message";

        // Act
        var newResult = result.WithWarning(warning);

        // Assert
        var actualWarning = newResult.GetField<string>(MindResultFieldType.Warning);
        Assert.Equal(warning, actualWarning);
    }

    [Fact]
    public void WithWarning_Generic_Should_Add_Warning_Field()
    {
        // Arrange
        var result = MindResult<int>.Success(42);
        var warning = "This is a warning message";

        // Act
        var newResult = result.WithWarning(warning);

        // Assert
        var actualWarning = newResult.GetField<int, string>(MindResultFieldType.Warning);
        Assert.Equal(warning, actualWarning);
        Assert.Equal(42, newResult.Data);
    }

    [Fact]
    public void WithCorrelationId_Should_Add_Correlation_Id_Field()
    {
        // Arrange
        var result = Core.MindResult.Success();
        var correlationId = "correlation-123";

        // Act
        var newResult = result.WithCorrelationId(correlationId);

        // Assert
        var actualCorrelationId = newResult.GetField<string>(MindResultFieldType.CorrelationId);
        Assert.Equal(correlationId, actualCorrelationId);
    }

    [Fact]
    public void WithCorrelationId_Generic_Should_Add_Correlation_Id_Field()
    {
        // Arrange
        var result = MindResult<string>.Success("data");
        var correlationId = "correlation-123";

        // Act
        var newResult = result.WithCorrelationId(correlationId);

        // Assert
        var actualCorrelationId = newResult.GetField<string, string>(MindResultFieldType.CorrelationId);
        Assert.Equal(correlationId, actualCorrelationId);
        Assert.Equal("data", newResult.Data);
    }

    [Fact]
    public void WithExecutionTime_Should_Add_Execution_Time_Field()
    {
        // Arrange
        var result = Core.MindResult.Success();
        var executionTime = TimeSpan.FromMilliseconds(150);

        // Act
        var newResult = result.WithExecutionTime(executionTime);

        // Assert
        var actualExecutionTime = newResult.GetField<TimeSpan>(MindResultFieldType.ExecutionTime);
        Assert.Equal(executionTime, actualExecutionTime);
    }

    [Fact]
    public void WithExecutionTime_Generic_Should_Add_Execution_Time_Field()
    {
        // Arrange
        var result = MindResult<int>.Success(42);
        var executionTime = TimeSpan.FromMilliseconds(150);

        // Act
        var newResult = result.WithExecutionTime(executionTime);

        // Assert
        var actualExecutionTime = newResult.GetField<int, TimeSpan>(MindResultFieldType.ExecutionTime);
        Assert.Equal(executionTime, actualExecutionTime);
        Assert.Equal(42, newResult.Data);
    }

    [Fact]
    public void Multiple_Extensions_Should_Be_Chainable()
    {
        // Arrange
        var result = MindResult<string>.Success("test data");

        // Act
        var enrichedResult = result
            .WithWarning("This is a warning")
            .WithCorrelationId("correlation-123")
            .WithExecutionTime(TimeSpan.FromMilliseconds(100));

        // Assert
        Assert.Equal("This is a warning", enrichedResult.GetField<string, string>(MindResultFieldType.Warning));
        Assert.Equal("correlation-123", enrichedResult.GetField<string, string>(MindResultFieldType.CorrelationId));
        Assert.Equal(TimeSpan.FromMilliseconds(100), enrichedResult.GetField<string, TimeSpan>(MindResultFieldType.ExecutionTime));
        Assert.Equal("test data", enrichedResult.Data);
    }

    [Theory]
    [InlineData(MindResultFieldType.Warning)]
    [InlineData(MindResultFieldType.Debug)]
    [InlineData(MindResultFieldType.Metadata)]
    [InlineData(MindResultFieldType.ValidationErrors)]
    [InlineData(MindResultFieldType.Source)]
    [InlineData(MindResultFieldType.UserContext)]
    [InlineData(MindResultFieldType.Pagination)]
    [InlineData(MindResultFieldType.Cache)]
    public void All_MindResultFieldType_Values_Should_Work_With_Extensions(MindResultFieldType fieldType)
    {
        // Arrange
        var result = Core.MindResult.Success();
        var testValue = $"Test value for {fieldType}";

        // Act
        var newResult = result.WithField(fieldType, testValue);

        // Assert
        var retrievedValue = newResult.GetField<string>(fieldType);
        Assert.Equal(testValue, retrievedValue);
    }
}
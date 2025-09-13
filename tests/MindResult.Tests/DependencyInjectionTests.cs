using Microsoft.Extensions.DependencyInjection;
using MindResult.Core;
using MindResult.Core.DependencyInjection;

namespace MindResult.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddMindResult_Should_Register_Services_With_Default_Scoped_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMindResult();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var factory = serviceProvider.GetRequiredService<IMindResultFactory>();
        var context = serviceProvider.GetRequiredService<IMindResultContext>();
        
        Assert.NotNull(factory);
        Assert.NotNull(context);
        Assert.IsType<MindResultFactory>(factory);
        Assert.IsType<MindResultContext>(context);
    }

    [Fact]
    public void AddMindResult_Should_Register_Services_With_Singleton_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMindResult(ServiceLifetime.Singleton);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var factory1 = serviceProvider.GetRequiredService<IMindResultFactory>();
        var factory2 = serviceProvider.GetRequiredService<IMindResultFactory>();
        
        Assert.Same(factory1, factory2); // Should be same instance for Singleton
    }

    [Fact]
    public void AddMindResult_Should_Register_Services_With_Transient_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMindResult(ServiceLifetime.Transient);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var factory1 = serviceProvider.GetRequiredService<IMindResultFactory>();
        var factory2 = serviceProvider.GetRequiredService<IMindResultFactory>();
        
        Assert.NotSame(factory1, factory2); // Should be different instances for Transient
    }

    [Fact]
    public void AddMindResult_With_Options_Should_Register_Custom_Options()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMindResult(options =>
        {
            options.IncludeCorrelationId = false;
            options.IncludeExecutionTime = true;
            options.CorrelationIdHeader = "Custom-Correlation-ID";
        });
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<MindResultOptions>();
        Assert.False(options.IncludeCorrelationId);
        Assert.True(options.IncludeExecutionTime);
        Assert.Equal("Custom-Correlation-ID", options.CorrelationIdHeader);
    }

    [Fact]
    public void MindResultContext_Should_Generate_Default_CorrelationId()
    {
        // Act
        var context = new MindResultContext();

        // Assert
        Assert.NotNull(context.CorrelationId);
        Assert.NotEmpty(context.CorrelationId);
        Assert.Equal(8, context.CorrelationId.Length);
    }

    [Fact]
    public void MindResultContext_Should_Allow_Setting_Properties()
    {
        // Arrange
        var context = new MindResultContext();
        var correlationId = "custom-correlation";
        var userContext = "user123";
        var customData = new Dictionary<string, object> { { "key", "value" } };

        // Act
        context.CorrelationId = correlationId;
        context.UserContext = userContext;
        context.CustomData = customData;

        // Assert
        Assert.Equal(correlationId, context.CorrelationId);
        Assert.Equal(userContext, context.UserContext);
        Assert.Equal(customData, context.CustomData);
    }
}

public class MindResultFactoryTests
{
    [Fact]
    public void Factory_Success_Should_Create_Successful_Result()
    {
        // Arrange
        var context = new MindResultContext();
        var options = new MindResultOptions { IncludeCorrelationId = false };
        var factory = new MindResultFactory(context, options);

        // Act
        var result = factory.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Factory_Success_Generic_Should_Create_Successful_Result_With_Data()
    {
        // Arrange
        var context = new MindResultContext();
        var options = new MindResultOptions { IncludeCorrelationId = false };
        var factory = new MindResultFactory(context, options);
        var data = "test data";

        // Act
        var result = factory.Success(data);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public void Factory_Failure_Should_Create_Failed_Result()
    {
        // Arrange
        var context = new MindResultContext();
        var options = new MindResultOptions { IncludeCorrelationId = false };
        var factory = new MindResultFactory(context, options);
        var errorMessage = "Test error";

        // Act
        var result = factory.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Factory_Should_Include_CorrelationId_When_Enabled()
    {
        // Arrange
        var context = new MindResultContext { CorrelationId = "test-correlation" };
        var options = new MindResultOptions { IncludeCorrelationId = true };
        var factory = new MindResultFactory(context, options);

        // Act
        var result = factory.Success();

        // Assert
        var correlationId = result.GetField<string>(MindResultFieldType.CorrelationId);
        Assert.Equal("test-correlation", correlationId);
    }

    [Fact]
    public void Factory_Should_Not_Include_CorrelationId_When_Disabled()
    {
        // Arrange
        var context = new MindResultContext { CorrelationId = "test-correlation" };
        var options = new MindResultOptions { IncludeCorrelationId = false };
        var factory = new MindResultFactory(context, options);

        // Act
        var result = factory.Success();

        // Assert
        var correlationId = result.GetField<string>(MindResultFieldType.CorrelationId);
        Assert.Null(correlationId);
    }

    [Fact]
    public void Factory_Should_Include_UserContext_When_Enabled()
    {
        // Arrange
        var context = new MindResultContext { UserContext = "user123" };
        var options = new MindResultOptions { IncludeUserContext = true };
        var factory = new MindResultFactory(context, options);

        // Act
        var result = factory.Success();

        // Assert
        var userContext = result.GetField<string>(MindResultFieldType.UserContext);
        Assert.Equal("user123", userContext);
    }

    [Fact]
    public void Factory_Should_Merge_Additional_Fields()
    {
        // Arrange
        var context = new MindResultContext { CorrelationId = "test-correlation" };
        var options = new MindResultOptions { IncludeCorrelationId = true };
        var factory = new MindResultFactory(context, options);
        var additionalFields = new Dictionary<string, object> { { "custom", "value" } };

        // Act
        var result = factory.Success(additionalFields);

        // Assert
        var correlationId = result.GetField<string>(MindResultFieldType.CorrelationId);
        var customValue = result.GetAdditionalField<string>("custom");
        
        Assert.Equal("test-correlation", correlationId);
        Assert.Equal("value", customValue);
    }
}
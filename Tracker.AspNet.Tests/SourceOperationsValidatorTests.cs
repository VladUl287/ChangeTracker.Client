using Microsoft.AspNetCore.Builder;
using NSubstitute;
using Tracker.AspNet.Services;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Tests;

public class SourceOperationsValidatorTests
{
    public interface ITestSourceOperations : ISourceOperations
    {
        // Marker interface for testing
    }

    [Fact]
    public void Configure_WithValidOperations_ReturnsNextDelegate()
    {
        // Arrange
        var mockSource1 = Substitute.For<ISourceOperations>();
        mockSource1.SourceId.Returns("source1");

        var mockSource2 = Substitute.For<ISourceOperations>();
        mockSource2.SourceId.Returns("source2");

        var operations = new List<ISourceOperations> { mockSource1, mockSource2 };

        var validator = new SourceOperationsValidator(operations);
        var mockAppBuilder = Substitute.For<IApplicationBuilder>();

        Action<IApplicationBuilder> next = builder =>
        {
            // Simulate next action
        };

        // Act
        var result = validator.Configure(next);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Action<IApplicationBuilder>>(result);

        // Verify no exception was thrown
        result.Invoke(mockAppBuilder);
        Assert.True(true); // Just to show the test passed without exception
    }

    [Fact]
    public void Configure_WithEmptyOperations_ThrowsInvalidOperationException()
    {
        // Arrange
        var emptyOperations = new List<ISourceOperations>();
        var validator = new SourceOperationsValidator(emptyOperations);

        Action<IApplicationBuilder> next = builder => { };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => validator.Configure(next));

        Assert.Contains($"At least one {nameof(ISourceOperations)} implementation is required", exception.Message);
    }

    [Fact]
    public void Configure_WithSingleOperation_DoesNotThrow()
    {
        // Arrange
        var mockSource = Substitute.For<ISourceOperations>();
        mockSource.SourceId.Returns("single-source");

        var operations = new List<ISourceOperations> { mockSource };
        var validator = new SourceOperationsValidator(operations);

        Action<IApplicationBuilder> next = builder => { };
        var mockAppBuilder = Substitute.For<IApplicationBuilder>();

        // Act
        var result = validator.Configure(next);

        // Assert - Should not throw
        result.Invoke(mockAppBuilder);
        Assert.True(true); // Just to show the test passed without exception
    }

    [Fact]
    public void Configure_WithDuplicateSourceIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockSource1 = Substitute.For<ISourceOperations>();
        mockSource1.SourceId.Returns("duplicate-id");

        var mockSource2 = Substitute.For<ISourceOperations>();
        mockSource2.SourceId.Returns("duplicate-id");

        var mockSource3 = Substitute.For<ISourceOperations>();
        mockSource3.SourceId.Returns("unique-id");

        var operations = new List<ISourceOperations> { mockSource1, mockSource2, mockSource3 };
        var validator = new SourceOperationsValidator(operations);

        Action<IApplicationBuilder> next = builder => { };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => validator.Configure(next));

        Assert.Contains($"Duplicate {nameof(ISourceOperations.SourceId)} values found", exception.Message);
        Assert.Contains("duplicate-id", exception.Message);
    }

    [Fact]
    public void Configure_WithMultipleDuplicateSourceIds_IncludesAllInExceptionMessage()
    {
        // Arrange
        var mockSource1 = Substitute.For<ISourceOperations>();
        mockSource1.SourceId.Returns("duplicate1");

        var mockSource2 = Substitute.For<ISourceOperations>();
        mockSource2.SourceId.Returns("duplicate1");

        var mockSource3 = Substitute.For<ISourceOperations>();
        mockSource3.SourceId.Returns("duplicate2");

        var mockSource4 = Substitute.For<ISourceOperations>();
        mockSource4.SourceId.Returns("duplicate2");

        var mockSource5 = Substitute.For<ISourceOperations>();
        mockSource5.SourceId.Returns("unique");

        var operations = new List<ISourceOperations> { mockSource1, mockSource2, mockSource3, mockSource4, mockSource5 };
        var validator = new SourceOperationsValidator(operations);

        Action<IApplicationBuilder> next = builder => { };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => validator.Configure(next));

        Assert.Contains($"Duplicate {nameof(ISourceOperations.SourceId)} values found", exception.Message);
        Assert.Contains("duplicate1", exception.Message);
        Assert.Contains("duplicate2", exception.Message);
    }

    [Fact]
    public void Configure_WithValidOperations_CallsNextDelegateWithBuilder()
    {
        // Arrange
        var mockSource = Substitute.For<ISourceOperations>();
        mockSource.SourceId.Returns("source1");

        var operations = new List<ISourceOperations> { mockSource };
        var validator = new SourceOperationsValidator(operations);

        var mockAppBuilder = Substitute.For<IApplicationBuilder>();
        var nextWasCalled = false;
        IApplicationBuilder passedBuilder = null;

        Action<IApplicationBuilder> next = builder =>
        {
            nextWasCalled = true;
            passedBuilder = builder;
        };

        // Act
        var result = validator.Configure(next);
        result.Invoke(mockAppBuilder);

        // Assert
        Assert.True(nextWasCalled);
        Assert.Same(mockAppBuilder, passedBuilder);
    }

    [Fact]
    public void Configure_ReturnsSameDelegate_WhenNoExceptions()
    {
        // Arrange
        var mockSource = Substitute.For<ISourceOperations>();
        mockSource.SourceId.Returns("source1");

        var operations = new List<ISourceOperations> { mockSource };
        var validator = new SourceOperationsValidator(operations);

        var mockAppBuilder = Substitute.For<IApplicationBuilder>();
        Action<IApplicationBuilder> originalNext = builder => { };

        // Act
        var returnedDelegate = validator.Configure(originalNext);

        // Assert
        Assert.Same(originalNext, returnedDelegate);
    }

    [Fact]
    public void Configure_WithEmptySourceId_DoesNotThrow()
    {
        // Arrange
        var mockSource1 = Substitute.For<ISourceOperations>();
        mockSource1.SourceId.Returns("");

        var mockSource2 = Substitute.For<ISourceOperations>();
        mockSource2.SourceId.Returns("source2");

        var operations = new List<ISourceOperations> { mockSource1, mockSource2 };
        var validator = new SourceOperationsValidator(operations);

        Action<IApplicationBuilder> next = builder => { };
        var mockAppBuilder = Substitute.For<IApplicationBuilder>();

        // Act
        var result = validator.Configure(next);

        // Assert - Should not throw
        result.Invoke(mockAppBuilder);
        Assert.True(true); // Just to show the test passed without exception
    }

    [Fact]
    public void Configure_WithDuplicateEmptySourceIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockSource1 = Substitute.For<ISourceOperations>();
        mockSource1.SourceId.Returns("");

        var mockSource2 = Substitute.For<ISourceOperations>();
        mockSource2.SourceId.Returns("");

        var operations = new List<ISourceOperations> { mockSource1, mockSource2 };
        var validator = new SourceOperationsValidator(operations);

        Action<IApplicationBuilder> next = builder => { };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => validator.Configure(next));

        Assert.Contains($"Duplicate {nameof(ISourceOperations.SourceId)} values found", exception.Message);
    }
}

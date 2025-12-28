using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tracker.AspNet.Extensions;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Tests.ExtensionsTests;

public class EndpointBuilderExtensionsTests
{
    [Fact]
    public void WithTracking_TBuilder_TContext_WithOptions_ShouldAddFilterFactory()
    {
        // Arrange
        var builder = new TestEndpointBuilder();
        var options = new GlobalOptions();
        var endpointBuilder = new Mock<EndpointBuilder>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockOptionsBuilder = new Mock<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
        var mockEtagService = new Mock<IRequestHandler>();
        var mockRequestFilter = new Mock<IRequestFilter>();

        mockServiceProvider.Setup(x => x.GetService(typeof(IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>)))
            .Returns(mockOptionsBuilder.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler)))
            .Returns(mockEtagService.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestFilter)))
            .Returns(mockRequestFilter.Object);

        var immutableOptions = new ImmutableGlobalOptions();
        mockOptionsBuilder.Setup(x => x.Build<DbContext>(options))
            .Returns(immutableOptions);

        // Act
        var result = builder.WithTracking<TestEndpointBuilder, DbContext>(options);

        // Assert
        Assert.NotEmpty(builder.Conventions);
        Assert.Same(builder, result);

        var factoryContext = new EndpointFilterFactoryContext
        {
            ApplicationServices = mockServiceProvider.Object,
            MethodInfo = null
        };

        builder.Conventions.FirstOrDefault()?.Invoke(endpointBuilder.Object);

        static ValueTask<object?> next(EndpointFilterInvocationContext context) => ValueTask.FromResult<object?>(null);
        var filterDelegate = endpointBuilder.Object.FilterFactories.FirstOrDefault()?.Invoke(factoryContext, next);

        // Verify services were requested
        Assert.NotEmpty(endpointBuilder.Object.FilterFactories);
        mockServiceProvider.Verify(x => x.GetService(typeof(IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>)), Times.Once);
        mockServiceProvider.Verify(x => x.GetService(typeof(IRequestHandler)), Times.Once);
        mockServiceProvider.Verify(x => x.GetService(typeof(IRequestFilter)), Times.Once);
        mockOptionsBuilder.Verify(x => x.Build<DbContext>(options), Times.Once);
    }

    [Fact]
    public void WithTracking_TBuilder_TContext_WithConfigure_ShouldCreateOptionsAndCallOverload()
    {
        // Arrange
        var builder = new TestEndpointBuilder();
        var wasConfigured = false;

        // Act
        var result = builder.WithTracking<TestEndpointBuilder, DbContext>(options =>
        {
            wasConfigured = true;
        });

        // Assert
        Assert.True(wasConfigured);
        Assert.NotEmpty(builder.Conventions);
        Assert.Same(builder, result);
    }

    [Fact]
    public void WithTracking_TBuilder_Generic_ShouldAddFilter()
    {
        // Arrange
        var builder = new TestEndpointBuilder();

        // Act
        var result = builder.WithTracking<TestEndpointBuilder>();

        // Assert
        Assert.NotEmpty(builder.Conventions);
        Assert.Same(builder, result);
    }

    [Fact]
    public void WithTracking_TBuilder_WithOptions_ShouldAddFilterFactory()
    {
        // Arrange
        var builder = new TestEndpointBuilder();
        var options = new GlobalOptions();
        var endpointBuilder = new Mock<EndpointBuilder>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockOptionsBuilder = new Mock<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
        var mockEtagService = new Mock<IRequestHandler>();
        var mockRequestFilter = new Mock<IRequestFilter>();

        mockServiceProvider.Setup(x => x.GetService(typeof(IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>)))
            .Returns(mockOptionsBuilder.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler)))
            .Returns(mockEtagService.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestFilter)))
            .Returns(mockRequestFilter.Object);

        var immutableOptions = new ImmutableGlobalOptions();
        mockOptionsBuilder.Setup(x => x.Build(options))
            .Returns(immutableOptions);

        // Act
        var result = builder.WithTracking(options);

        // Assert
        Assert.NotEmpty(builder.Conventions);
        Assert.Same(builder, result);

        // Verify the filter factory creates the correct filter
        var factoryContext = new EndpointFilterFactoryContext
        {
            ApplicationServices = mockServiceProvider.Object,
            MethodInfo = null
        };

        builder.Conventions.FirstOrDefault()?.Invoke(endpointBuilder.Object);

        static ValueTask<object?> next(EndpointFilterInvocationContext context) => ValueTask.FromResult<object?>(null);
        var filterDelegate = endpointBuilder.Object.FilterFactories.FirstOrDefault()?.Invoke(factoryContext, next);

        mockServiceProvider.Verify(x => x.GetService(typeof(IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>)), Times.Once);
        mockServiceProvider.Verify(x => x.GetService(typeof(IRequestHandler)), Times.Once);
        mockServiceProvider.Verify(x => x.GetService(typeof(IRequestFilter)), Times.Once);
        mockOptionsBuilder.Verify(x => x.Build(options), Times.Once);
    }

    [Fact]
    public void WithTracking_TBuilder_WithConfigure_ShouldCreateOptionsAndCallOverload()
    {
        // Arrange
        var builder = new TestEndpointBuilder();
        var wasConfigured = false;

        // Act
        var result = builder.WithTracking(options =>
        {
            wasConfigured = true;
        });

        // Assert
        Assert.True(wasConfigured);
        Assert.NotEmpty(builder.Conventions);
        Assert.Same(builder, result);
    }

    [Fact]
    public void FilterFactory_ShouldCreateTrackerEndpointFilter_WithCorrectDependencies()
    {
        // Arrange
        var builder = new TestEndpointBuilder();
        var options = new GlobalOptions();
        var endpointBuilder = new Mock<EndpointBuilder>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockOptionsBuilder = new Mock<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
        var mockEtagService = new Mock<IRequestHandler>();
        var mockRequestFilter = new Mock<IRequestFilter>();

        mockServiceProvider.Setup(x => x.GetService(typeof(IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>)))
            .Returns(mockOptionsBuilder.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler)))
            .Returns(mockEtagService.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestFilter)))
            .Returns(mockRequestFilter.Object);

        var immutableOptions = new ImmutableGlobalOptions();
        mockOptionsBuilder.Setup(x => x.Build<DbContext>(options))
            .Returns(immutableOptions);

        // Act
        builder.WithTracking<TestEndpointBuilder, DbContext>(options);

        var factoryContext = new EndpointFilterFactoryContext
        {
            ApplicationServices = mockServiceProvider.Object,
            MethodInfo = null
        };

        builder.Conventions.FirstOrDefault()?.Invoke(endpointBuilder.Object);

        static ValueTask<object?> next(EndpointFilterInvocationContext context) => ValueTask.FromResult<object?>(null);
        var filterDelegate = endpointBuilder.Object.FilterFactories.FirstOrDefault()?.Invoke(factoryContext, next);

        Assert.NotNull(filterDelegate);
    }

    [Fact]
    public void WithTracking_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new TestEndpointBuilder();

        // Act & Assert for each overload
        var result1 = builder.WithTracking<TestEndpointBuilder, DbContext>(new GlobalOptions());
        Assert.Same(builder, result1);

        var result2 = builder.WithTracking<TestEndpointBuilder, DbContext>(_ => { });
        Assert.Same(builder, result2);

        var result3 = builder.WithTracking<TestEndpointBuilder>();
        Assert.Same(builder, result3);

        var result4 = builder.WithTracking<TestEndpointBuilder>(new GlobalOptions());
        Assert.Same(builder, result4);

        var result5 = builder.WithTracking<TestEndpointBuilder>(_ => { });
        Assert.Same(builder, result5);
    }

    [Fact]
    public void WithTracking_ShouldHandleNullOptions()
    {
        // Arrange
        var builder = new TestEndpointBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.WithTracking<TestEndpointBuilder, DbContext>(options: null!));

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithTracking<TestEndpointBuilder>(options: null!));
    }

    [Fact]
    public void WithTracking_ShouldHandleNullConfigureAction()
    {
        // Arrange
        var builder = new TestEndpointBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.WithTracking<TestEndpointBuilder, DbContext>(configure: null!));

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithTracking<TestEndpointBuilder>(configure: null!));
    }

    private sealed class TestEndpointBuilder : IEndpointConventionBuilder
    {
        public List<Action<EndpointBuilder>> Conventions { get; } = [];

        public void Add(Action<EndpointBuilder> convention)
        {
            Conventions.Add(convention);
        }
    }
}

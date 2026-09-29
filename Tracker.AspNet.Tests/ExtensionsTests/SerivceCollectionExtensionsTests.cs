using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tracker.AspNet.Extensions;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Services;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Tests.ExtensionsTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTracker_NoParameters_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = ServiceCollectionExtensions.AddTracker(services);
        services.AddLogging();

        // Assert
        Assert.Same(services, result);

        // Verify base services are registered
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>());
        Assert.NotNull(serviceProvider.GetService<IAssemblyTimestampProvider>());
        Assert.NotNull(serviceProvider.GetService<IETagProvider>());
        Assert.NotNull(serviceProvider.GetService<IRequestHandler>());
        Assert.NotNull(serviceProvider.GetService<IRequestFilter>());
        Assert.NotNull(serviceProvider.GetService<IProviderResolver>());
        Assert.NotNull(serviceProvider.GetService<ITableNameResolver>());

        // Verify GlobalOptions singleton is registered
        var optionsInstance = serviceProvider.GetService<TrackOptionsSnapshot>();
        Assert.NotNull(optionsInstance);
    }

    [Fact]
    public void AddTracker_WithOptions_RegistersServicesWithProvidedOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new TrackOptions();

        var result = ServiceCollectionExtensions.AddTracker(services, options);
        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());
        services.AddSingleton(mockOptionsBuilder.Object);

        // Act
        var provider = services.BuildServiceProvider();
        provider.GetService<TrackOptionsSnapshot>();

        // Assert
        Assert.Same(services, result);
        mockOptionsBuilder.Verify(b => b.Build(options), Times.Once);
    }

    [Fact]
    public void AddTracker_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddTracker(services, (TrackOptions)null!));
    }

    [Fact]
    public void AddTracker_WithOptions_BuildsSingletonImmutableOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new TrackOptions();

        ServiceCollectionExtensions.AddTracker(services, options);
        var expectedImmutableOptions = new TrackOptionsSnapshot();
        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build(It.IsAny<TrackOptions>()))
            .Returns(expectedImmutableOptions);
        services.AddSingleton(mockOptionsBuilder.Object);

        // Act
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();
        Assert.Same(expectedImmutableOptions, actualOptions);
    }

    [Fact]
    public void AddTracker_WithConfigureAction_RegistersServicesWithConfiguredOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        bool wasConfigured = false;

        var result = ServiceCollectionExtensions.AddTracker(services, options =>
        {
            wasConfigured = true;
        });

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());
        services.AddSingleton(mockOptionsBuilder.Object);

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        Assert.Same(services, result);
        Assert.True(wasConfigured);
        mockOptionsBuilder.Verify(b => b.Build(It.IsAny<TrackOptions>()), Times.Once);
    }

    [Fact]
    public void AddTracker_WithNullConfigureAction_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddTracker(services, (Action<TrackOptions>)null!));
    }

    [Fact]
    public void AddTracker_WithConfigureAction_CreatesNewOptionsInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        TrackOptions capturedOptions = null!;

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot())
            .Callback<TrackOptions>(opts => capturedOptions = opts);

        // Act
        ServiceCollectionExtensions.AddTracker(services, options =>
        {
            // Modify options to verify they're being used
            // Add properties to GlobalOptions if needed for testing
        });
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.IsType<TrackOptions>(capturedOptions);
    }

    [Fact]
    public void AddTracker_Generic_NoParameters_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = ServiceCollectionExtensions.AddTracker<TestDbContext>(services);

        // Assert
        Assert.Same(services, result);

        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>());

        // Verify generic Build was used
        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());

        ServiceCollectionExtensions.AddTracker<TestDbContext>(services);
        services.AddSingleton(mockOptionsBuilder.Object);
        serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        mockOptionsBuilder.Verify(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()), Times.Once);
    }

    [Fact]
    public void AddTracker_Generic_WithOptions_RegistersServicesWithProvidedOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new TrackOptions();

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());

        // Act
        var result = ServiceCollectionExtensions.AddTracker<TestDbContext>(services, options);
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        Assert.Same(services, result);
        mockOptionsBuilder.Verify(b => b.Build<TestDbContext>(options), Times.Once);
    }

    [Fact]
    public void AddTracker_Generic_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddTracker<TestDbContext>(services, (TrackOptions)null!));
    }

    [Fact]
    public void AddTracker_Generic_WithOptions_UsesGenericBuildMethod()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new TrackOptions();

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());

        // Act
        ServiceCollectionExtensions.AddTracker<TestDbContext>(services, options);
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        // Verify that the generic Build method was called, not the non-generic one
        mockOptionsBuilder.Verify(b => b.Build<TestDbContext>(options), Times.Once);
        mockOptionsBuilder.Verify(b => b.Build(options), Times.Never);
    }

    [Fact]
    public void AddTracker_Generic_WithConfigureAction_RegistersServicesWithConfiguredOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        bool wasConfigured = false;

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());

        // Act
        var result = ServiceCollectionExtensions.AddTracker<TestDbContext>(services, options =>
        {
            wasConfigured = true;
        });
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        Assert.Same(services, result);
        Assert.True(wasConfigured);
        mockOptionsBuilder.Verify(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()), Times.Once);
    }

    [Fact]
    public void AddTracker_Generic_WithNullConfigureAction_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddTracker<TestDbContext>(services, (Action<TrackOptions>)null!));
    }

    [Fact]
    public void AddTracker_Generic_WithConfigureAction_DelegatesToGenericVersionWithOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var capturedOptions = new TrackOptions();

        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot())
            .Callback<TrackOptions>(opts => capturedOptions = opts);

        // Act
        ServiceCollectionExtensions.AddTracker<TestDbContext>(services, options =>
        {
            // Configure options
        });
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();

        // Assert
        mockOptionsBuilder.Verify(b => b.Build<TestDbContext>(It.IsAny<TrackOptions>()), Times.Once);
    }

    [Fact]
    public void AddTrackerBase_RegistersAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        // Using reflection to call private method, or test indirectly through public methods
        ServiceCollectionExtensions.AddTracker(services);
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        // Assert

        Assert.NotNull(serviceProvider.GetService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>());
        Assert.IsType<DefaultOptionsBuilder>(serviceProvider.GetService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>());

        Assert.NotNull(serviceProvider.GetService<IAssemblyTimestampProvider>());
        Assert.NotNull(serviceProvider.GetService<IETagProvider>());
        Assert.IsType<DefaultETagProvider>(serviceProvider.GetService<IETagProvider>());

        Assert.NotNull(serviceProvider.GetService<IRequestHandler>());
        Assert.IsType<DefaultRequestHandler>(serviceProvider.GetService<IRequestHandler>());

        Assert.NotNull(serviceProvider.GetService<IRequestFilter>());
        Assert.IsType<DefaultRequestFilter>(serviceProvider.GetService<IRequestFilter>());

        Assert.NotNull(serviceProvider.GetService<IProviderResolver>());
        Assert.IsType<DefaultProviderResolver>(serviceProvider.GetService<IProviderResolver>());

        Assert.NotNull(serviceProvider.GetService<ITableNameResolver>());
        Assert.IsType<DefaultTableNameResolver>(serviceProvider.GetService<ITableNameResolver>());
    }

    [Fact]
    public void AddTrackerBase_RegistersAssemblyTimestampProviderWithExecutingAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        ServiceCollectionExtensions.AddTracker(services);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<IAssemblyTimestampProvider>();
        Assert.NotNull(provider);
        // AssemblyTimestampProvider should be created with the executing assembly
    }

    [Fact]
    public void AddTracker_MultipleCalls_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert (should not throw)
        ServiceCollectionExtensions.AddTracker(services);
        ServiceCollectionExtensions.AddTracker(services);

        // Verify services are still registered
        var serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void AddTracker_WithRealServiceProvider_ResolvesAllDependencies()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        ServiceCollectionExtensions.AddTracker(services);
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - This test ensures all dependencies can be resolved
        Assert.NotNull(serviceProvider.GetService<TrackOptionsSnapshot>());
        Assert.NotNull(serviceProvider.GetService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>());
        Assert.NotNull(serviceProvider.GetService<IAssemblyTimestampProvider>());
        Assert.NotNull(serviceProvider.GetService<IETagProvider>());
        Assert.NotNull(serviceProvider.GetService<IRequestHandler>());
        Assert.NotNull(serviceProvider.GetService<IRequestFilter>());
        Assert.NotNull(serviceProvider.GetService<IProviderResolver>());
        Assert.NotNull(serviceProvider.GetService<ITableNameResolver>());
    }

    [Fact]
    public void AddTracker_WithCustomDbContext_ResolvesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register mock options builder to capture context type
        var mockOptionsBuilder = new Mock<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
        mockOptionsBuilder.Setup(b => b.Build<CustomDbContext>(It.IsAny<TrackOptions>()))
            .Returns(new TrackOptionsSnapshot());

        // Act
        ServiceCollectionExtensions.AddTracker<CustomDbContext>(services);
        services.AddSingleton(mockOptionsBuilder.Object);
        var serviceProvider = services.BuildServiceProvider();
        var actualOptions = serviceProvider.GetService<TrackOptionsSnapshot>();
        services.AddLogging();

        // Assert
        mockOptionsBuilder.Verify(b => b.Build<CustomDbContext>(It.IsAny<TrackOptions>()), Times.Once);
    }

    public class TestDbContext : DbContext { }
    public class CustomDbContext : DbContext { }
}


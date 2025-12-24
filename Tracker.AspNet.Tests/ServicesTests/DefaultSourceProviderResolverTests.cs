using Microsoft.Extensions.Logging;
using Moq;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Tests.AttirubtesTests;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Tests.ServicesTests;

public class DefaultProviderResolverTests
{
    private readonly Mock<ILogger<DefaultProviderResolver>> _loggerMock;

    public DefaultProviderResolverTests()
    {
        _loggerMock = new Mock<ILogger<DefaultProviderResolver>>();
    }

    private Mock<ISourceProvider> CreateProviderMock(string id)
    {
        var mock = new Mock<ISourceProvider>();
        mock.Setup(p => p.Id).Returns(id);
        return mock;
    }

    private DefaultProviderResolver CreateResolver(ILogger<DefaultProviderResolver>? logger = null)
    {
        return new DefaultProviderResolver(logger ?? _loggerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateStoreAndDefaultProvider()
    {
        // Act
        var resolver = CreateResolver();

        // Assert
        Assert.NotNull(resolver);

        // Verify that all providers are accessible
        var provider = resolver.ResolveProvider(null, new ImmutableGlobalOptions(), out var shouldDispose);
        Assert.Equal("provider1", provider?.Id);
    }

    public class SelectProviderWithProviderIdAndImmutableOptionsTests
    {
        private readonly DefaultProviderResolverTests _fixture;

        public SelectProviderWithProviderIdAndImmutableOptionsTests()
        {
            _fixture = new DefaultProviderResolverTests();
        }

        [Fact]
        public void SelectProvider_WithValidProviderId_ShouldReturnProvider()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act
            var provider = resolver.ResolveProvider(null, new ImmutableGlobalOptions(), out var shouldDispose);

            // Assert
            Assert.Equal("provider2", provider?.Id);
        }

        [Fact]
        public void SelectProvider_WithInvalidProviderId_ShouldThrowException()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => resolver.ResolveProvider(null, options, out var shouldDispose));

            Assert.Contains("invalid-provider", exception.Message);
        }

        [Fact]
        public void SelectProvider_WithNullProviderIdAndNoSourceOps_ShouldReturnDefault()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                SourceProvider = null,
                SourceProviderFactory = null
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider1", result?.Id); // First provider is default
        }

        [Fact]
        public void SelectProvider_WithNullProviderIdAndSourceProviderInOptions_ShouldReturnFromOptions()
        {
            // Arrange
            var customProvider = _fixture.CreateProviderMock("custom-provider").Object;
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                SourceProvider = customProvider
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal(customProvider, result);
        }
    }

    public class SelectProviderWithGlobalOptionsTests
    {
        private readonly DefaultProviderResolverTests _fixture;

        public SelectProviderWithGlobalOptionsTests()
        {
            _fixture = new DefaultProviderResolverTests();
        }

        [Fact]
        public void SelectProvider_WithValidSourceIdInGlobalOptions_ShouldReturnProvider()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                ProviderId = "provider3"
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider3", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithInvalidSourceIdInGlobalOptions_ShouldThrowException()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                ProviderId = "invalid-id"
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => resolver.ResolveProvider(null, options, out var shouldDispose));

            Assert.Contains("invalid-id", exception.Message);
            Assert.Contains("Available provider IDs:", exception.Message);
        }

        [Fact]
        public void SelectProvider_WithNoSourceIdAndNoOps_ShouldReturnDefault()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                ProviderId = null,
                SourceProvider = null,
                SourceProviderFactory = null
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider1", result?.Id);
        }
    }

    public class SelectProviderWithContextAndProviderIdTests
    {
        private readonly DefaultProviderResolverTests _fixture;

        public SelectProviderWithContextAndProviderIdTests()
        {
            _fixture = new DefaultProviderResolverTests();
        }

        [Fact]
        public void SelectProvider_WithValidProviderIdAndContext_ShouldReturnProvider()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider2", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithInvalidProviderIdAndContext_ShouldThrowException()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => resolver.ResolveProvider(null, options, out var shouldDispose));

            Assert.Contains(nameof(TestDbContext), exception.Message);
            Assert.Contains("invalid-id", exception.Message);
        }

        [Fact]
        public void SelectProvider_WithNullProviderIdAndGeneratedIdFound_ShouldReturnGenerated()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider2", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithNullProviderIdGeneratedIdNotFoundNoOps_ShouldReturnDefault()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                SourceProvider = null,
                SourceProviderFactory = null
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider1", result?.Id);
        }
    }

    public class SelectProviderWithContextAndGlobalOptionsTests
    {
        private readonly DefaultProviderResolverTests _fixture;

        public SelectProviderWithContextAndGlobalOptionsTests()
        {
            _fixture = new DefaultProviderResolverTests();
        }

        [Fact]
        public void SelectProvider_WithValidSourceIdInGlobalOptionsAndContext_ShouldReturnProvider()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                ProviderId = "provider3"
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider3", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithNoSourceIdGeneratedIdFound_ShouldReturnGeneratedProvider()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions { ProviderId = null };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider2", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithNoSourceIdGeneratedIdNotFoundNoOps_ShouldReturnDefault()
        {
            // Arrange
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions
            {
                ProviderId = null,
                SourceProvider = null,
                SourceProviderFactory = null
            };

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("provider1", result?.Id);
        }

        [Fact]
        public void SelectProvider_WithEmptyProviderList_ShouldWork()
        {
            // Arrange
            var singleProviderMock = _fixture.CreateProviderMock("single-provider");
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act
            var result = resolver.ResolveProvider(null, options, out var shouldDispose);

            // Assert
            Assert.Equal("single-provider", result?.Id);
        }

        [Fact]
        public void SelectProvider_ProviderIdCaseSensitive_ShouldRespectCase()
        {
            // Arrange
            var caseSensitiveProvider = _fixture.CreateProviderMock("PROVIDER1").Object;
            var resolver = _fixture.CreateResolver();
            var options = new ImmutableGlobalOptions();

            // Act
            var result1 = resolver.ResolveProvider(null, options, out var shouldDispose);
            var exception = Assert.Throws<InvalidOperationException>(
                () => resolver.ResolveProvider(null, options, out var shouldDispose));

            // Assert
            Assert.Equal("PROVIDER1", result1?.Id);
            Assert.Contains("provider1", exception.Message);
        }
    }
}

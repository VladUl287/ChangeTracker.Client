using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Tests;

public class RequestHandlerTests
{
    private readonly Mock<IETagProvider> _mockETagService;
    private readonly Mock<ISourceOperationsResolver> _mockOperationsResolver;
    private readonly Mock<ITrackerHasher> _mockTimestampsHasher;
    private readonly Mock<ILogger<DefaultRequestHandler>> _mockLogger;
    private readonly DefaultRequestHandler _handler;

    public RequestHandlerTests()
    {
        _mockETagService = new Mock<IETagProvider>();
        _mockOperationsResolver = new Mock<ISourceOperationsResolver>();
        _mockTimestampsHasher = new Mock<ITrackerHasher>();
        _mockLogger = new Mock<ILogger<DefaultRequestHandler>>();

        _handler = new DefaultRequestHandler(
            _mockETagService.Object,
            _mockOperationsResolver.Object,
            _mockTimestampsHasher.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task IsNotModified_ShouldThrowArgumentNullException_WhenHttpContextIsNull()
    {
        // Arrange
        var options = new ImmutableGlobalOptions();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _handler.IsNotModified(null, options, CancellationToken.None));
    }

    [Fact]
    public async Task IsNotModified_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _handler.IsNotModified(context, null, CancellationToken.None));
    }

    private delegate bool TryResolveDelegate(string sourceId, out ISourceOperations? sourceOperations);

    [Fact]
    public async Task IsNotModified_ShouldReturnNotModified_WhenETagMatches()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var etag = "test-etag";

        context.Request.Headers.IfNoneMatch = etag;

        var options = new ImmutableGlobalOptions
        {
            Tables = [],
            CacheControl = "no-cache"
        };

        var mockSourceOperations = new Mock<ISourceOperations>();
        mockSourceOperations.Setup(x => x.GetLastVersion(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks);

        //_mockOperationsResolver.Setup(x => x.TryResolve(
        //        It.IsAny<string>(),
        //        out It.Ref<ISourceOperations?>.IsAny))
        //    .Returns(new TryResolveDelegate((string id, out ISourceOperations? ops) =>
        //    {
        //        ops = mockSourceOperations.Object;
        //        return true;
        //    }));

        _mockOperationsResolver.Setup(x => x.First)
            .Returns(mockSourceOperations.Object);

        _mockETagService.Setup(x => x.Compare(etag, It.IsAny<ulong>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        var result = await _handler.IsNotModified(context, options, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status304NotModified, context.Response.StatusCode);
    }

    [Fact]
    public async Task IsNotModified_ShouldReturnModified_WhenETagDoesNotMatch()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var etag = "test-etag";
        var newEtag = "new-etag";

        context.Request.Headers.IfNoneMatch = etag;

        var options = new ImmutableGlobalOptions
        {
            Tables = [],
            CacheControl = "no-cache"
        };

        var mockSourceOperations = new Mock<ISourceOperations>();
        mockSourceOperations.Setup(x => x.GetLastVersion(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks);

        _mockOperationsResolver.Setup(x => x.First)
            .Returns(mockSourceOperations.Object);

        _mockETagService.Setup(x => x.Compare(etag, It.IsAny<ulong>(), It.IsAny<string>()))
            .Returns(false);

        _mockETagService.Setup(x => x.Generate(It.IsAny<ulong>(), It.IsAny<string>()))
            .Returns(newEtag);

        // Act
        var result = await _handler.IsNotModified(context, options, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Equal(newEtag, context.Response.Headers.ETag);
        Assert.Equal("no-cache", context.Response.Headers.CacheControl);
    }
}
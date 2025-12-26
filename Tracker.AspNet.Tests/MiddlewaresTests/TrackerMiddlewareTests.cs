using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Tracker.AspNet.Middlewares;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Tests.MiddlewaresTests;

public class TrackerMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IRequestFilter> _filterMock;
    private readonly Mock<IRequestHandler> _serviceMock;
    private readonly ImmutableGlobalOptions _options;
    private readonly TrackerMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public TrackerMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _filterMock = new Mock<IRequestFilter>();
        _serviceMock = new Mock<IRequestHandler>();
        _options = new ImmutableGlobalOptions(/* initialize with test values */);
        _middleware = new TrackerMiddleware(_nextMock.Object, _filterMock.Object, _serviceMock.Object, _options);
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_WhenFilterValidAndHandlerReturnsTrue_ShouldNotCallNext()
    {
        // Arrange
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(true);
        _serviceMock.Setup(s => s.HandleRequest(_httpContext, _options, default)).ReturnsAsync(true);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenFilterValidAndHandlerReturnsFalse_ShouldCallNext()
    {
        // Arrange
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(true);
        _serviceMock.Setup(s => s.HandleRequest(_httpContext, _options, default)).ReturnsAsync(false);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenFilterInvalid_ShouldCallNext()
    {
        // Arrange
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(false);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
        _serviceMock.Verify(s => s.HandleRequest(It.IsAny<HttpContext>(), It.IsAny<ImmutableGlobalOptions>(), default), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenFilterThrowsException_ShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Filter error");
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Throws(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(_httpContext));
        _nextMock.Verify(next => next(_httpContext), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenHandlerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Handler error");
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(true);
        _serviceMock.Setup(s => s.HandleRequest(_httpContext, _options, default)).ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(_httpContext));
        _nextMock.Verify(next => next(_httpContext), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithSpecificHttpContext_ShouldPassCorrectParameters()
    {
        // Arrange
        var customHttpContext = new DefaultHttpContext
        {
            Request =
            {
                Method = "POST",
                Path = "/api/track",
                ContentType = "application/json"
            },
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }))
        };

        _filterMock.Setup(f => f.ValidRequest(customHttpContext, _options)).Returns(true);
        _serviceMock.Setup(s => s.HandleRequest(customHttpContext, _options, default)).ReturnsAsync(true);

        // Act
        await _middleware.InvokeAsync(customHttpContext);

        // Assert
        _filterMock.Verify(f => f.ValidRequest(customHttpContext, _options), Times.Once);
        _serviceMock.Verify(s => s.HandleRequest(customHttpContext, _options, default), Times.Once);
        _nextMock.Verify(next => next(customHttpContext), Times.Never);
    }

    [Theory]
    [InlineData(true, true, false)]  // Valid request & handler succeeds = no next
    [InlineData(true, false, true)]  // Valid request & handler fails = call next
    [InlineData(false, false, true)] // Invalid request = call next (handler not called)
    public async Task InvokeAsync_WithVariousCombinations_ShouldBehaveCorrectly(
        bool filterValid, bool handlerResult, bool shouldCallNext)
    {
        // Arrange
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(filterValid);

        if (filterValid)
        {
            _serviceMock.Setup(s => s.HandleRequest(_httpContext, _options, default))
                       .ReturnsAsync(handlerResult);
        }

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var expectedTimes = shouldCallNext ? Times.Once() : Times.Never();
        _nextMock.Verify(next => next(_httpContext), expectedTimes);

        if (!filterValid)
        {
            _serviceMock.Verify(s => s.HandleRequest(It.IsAny<HttpContext>(), It.IsAny<ImmutableGlobalOptions>(), default), Times.Never);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenNextDelegateThrows_ShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Next delegate error");
        _filterMock.Setup(f => f.ValidRequest(_httpContext, _options)).Returns(false);
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(_httpContext));
    }
}

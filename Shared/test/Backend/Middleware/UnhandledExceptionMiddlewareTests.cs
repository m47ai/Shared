namespace M47.Shared.Tests.Middleware;

using FluentValidation;
using FluentValidation.Results;
using M47.Shared.Domain.Api;
using M47.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;

public class UnhandledExceptionMiddlewareTests
{
    private readonly ILogger<UnhandledExceptionMiddleware> _logger;
    private readonly DefaultHttpContext _context;

    public UnhandledExceptionMiddlewareTests()
    {
        _logger = Substitute.For<ILogger<UnhandledExceptionMiddleware>>();
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task Should_GiveCustomErrorResponseAndInternalServerErrorHttpStatus500_When_UnExpectedExceptionIsRaised()
    {
        // Arrange
        var expected = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "some unexpected exception",
            Status = (int)HttpStatusCode.InternalServerError
        };
        var middleware = new UnhandledExceptionMiddleware(next: _ => throw new Exception(expected.Detail), _logger);

        //Act
        await middleware.InvokeAsync(_context);

        //Assert
        var actual = await ReadReponseAsync<ProblemDetails>();
        actual.Should().BeEquivalentTo(expected);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Should_GiveCustomErrorResponseAndHttpStatus400_When_ArgumentNullExceptionIsRaised()
    {
        // Arrange
        var expected = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "Value cannot be null.",
            Status = (int)HttpStatusCode.InternalServerError
        };

        var middleware = new UnhandledExceptionMiddleware(next: _ => throw new ArgumentNullException(), _logger);

        //Act
        await middleware.InvokeAsync(_context);

        //Assert
        var actual = await ReadReponseAsync<ProblemDetails>();
        actual.Should().BeEquivalentTo(expected);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Should_GiveCustomErrorResponseAndHttpStatus400_When_ArgumentExceptionIsRaised()
    {
        // Arrange
        var expected = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "Value does not fall within the expected range.",
            Status = (int)HttpStatusCode.InternalServerError
        };
        var middleware = new UnhandledExceptionMiddleware(next: _ => throw new ArgumentException(), _logger);

        //Act
        await middleware.InvokeAsync(_context);

        //Assert
        var actual = await ReadReponseAsync<ProblemDetails>();
        actual.Should().BeEquivalentTo(expected);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Should_GiveCustomErrorResponseAndHttpStatus400_When_ValidationdExceptionIsRaised()
    {
        // Arrange
        var errors = new List<ValidationFailure>()
        {
            new()
            {
                PropertyName = "Url",
                ErrorMessage = "Uri exepected"
            }
        };

        var expected = new ValidationFailureResponse
        {
            Errors = errors.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage,
            })
        };
        var middleware = new UnhandledExceptionMiddleware(next: _ => throw new ValidationException("Error", errors), _logger);

        //Act
        await middleware.InvokeAsync(_context);

        //Assert
        var actual = await ReadReponseAsync<ValidationFailureResponse>();
        actual.Should().BeEquivalentTo(expected);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnHttpStatusOK200AndNullResponse_When_NoExceptionOccured()
    {
        // Arrange
        var middleware = new UnhandledExceptionMiddleware(next: (innerHttpContext) =>
        {
            return Task.CompletedTask;
        }, _logger);

        //Act
        await middleware.InvokeAsync(_context);

        //Assert
        var expectionDetails = await ReadReponseAsync<ProblemDetails>();
        expectionDetails.Should().BeNull();
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    private async Task<T> ReadReponseAsync<T>()
    {
        if (_context.Response.Body.CanSeek)
        {
            _context.Response.Body.Seek(0, SeekOrigin.Begin);
        }

        using var stream = new StreamReader(_context.Response.Body);
        var response = await stream.ReadToEndAsync();

        return JsonConvert.DeserializeObject<T>(response)!;
    }
}
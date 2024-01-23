namespace M47.Shared.Middleware;

using FluentValidation;
using M47.Shared.Domain.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

public class UnhandledExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UnhandledExceptionMiddleware> _logger;

    public UnhandledExceptionMiddleware(RequestDelegate next, ILogger<UnhandledExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            _logger.LogInformation("Request served successful");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error: {error}", ex.Message);

            await ResponseAsBadRequest(context, ex);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError(ex, "Http bad request {error}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = new[]
                {
                    new ValidationResponse
                    {
                        Message = ex.Message,
                    }
                }
            };

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
        catch (AggregateException ex)
        {
            foreach (var innerException in ex.InnerExceptions)
            {
                _logger.LogError(ex, "Unknown error: {error}", ex.Message);
            }

            await ResponseWithError(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access error: {error}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = new[]
                {
                    new ValidationResponse
                    {
                        Message = ex.Message,
                    }
                }
            };

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error: {error}", ex.Message);

            await ResponseWithError(context, ex);
        }
    }

    private static async Task ResponseAsBadRequest(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var validationFailureResponse = new ValidationFailureResponse
        {
            Errors = ex.Errors.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage,
            })
        };

        await context.Response.WriteAsJsonAsync(validationFailureResponse);
    }

    private static async Task ResponseWithError(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var exceptionDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "Internal Server Error",
            Detail = ex.Message,
        };

        await context.Response.WriteAsJsonAsync(exceptionDetails);
    }
}
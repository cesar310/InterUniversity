using System.Net;
using System.Text.Json;
using StudentEnrollment.Domain.Exceptions;
using FluentValidation;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                notFound.ErrorCode,
                notFound.Message,
                null
            ),
            DomainValidationException validation => (
                HttpStatusCode.BadRequest,
                validation.ErrorCode,
                validation.Message,
                null
            ),
            BusinessRuleException businessRule => (
                HttpStatusCode.UnprocessableEntity,
                businessRule.ErrorCode,
                businessRule.Message,
                null
            ),
            DuplicateException duplicate => (
                HttpStatusCode.Conflict,
                duplicate.ErrorCode,
                duplicate.Message,
                null
            ),
            AuthenticationException auth => (
                HttpStatusCode.Unauthorized,
                auth.ErrorCode,
                auth.Message,
                null
            ),
            AuthorizationException authz => (
                HttpStatusCode.Forbidden,
                authz.ErrorCode,
                authz.Message,
                null
            ),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "VALIDATION_ERROR",
                "One or more validation errors occurred",
                validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList()
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "INTERNAL_ERROR",
                "An internal server error occurred",
                null
            )
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Error(
            message,
            errorCode,
            errors
        );

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

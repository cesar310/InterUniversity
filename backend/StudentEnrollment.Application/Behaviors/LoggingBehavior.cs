using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace StudentEnrollment.Application.Behaviors;

/// <summary>
/// Pipeline behavior para logging de requests y performance
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "Handling {RequestName} at {DateTime}",
            requestName,
            DateTime.UtcNow
        );

        try
        {
            var response = await next();

            stopwatch.Stop();

            logger.LogInformation(
                "Completed {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds
            );

            // Log performance warning if request takes too long
            if (stopwatch.ElapsedMilliseconds > 3000)
            {
                logger.LogWarning(
                    "Long running request: {RequestName} took {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms: {ErrorMessage}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                ex.Message
            );

            throw;
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Rakushu.Api.Middleware;

public class GlobalExceptionHandler (
	ILogger<GlobalExceptionHandler> logger,
	IProblemDetailsService problemDetailsService
	) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		MiddlewareLogDefinitions.LogUnhandledCriticalException(
			logger,
			exception.GetType().Name,
			httpContext.Request.Path,
			httpContext.TraceIdentifier);

		var problemDetails = new ProblemDetails()
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Internal Server Error",
			Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
			Detail = exception.Message,
			Instance = httpContext.Request.Path
		};

		problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
		problemDetails.Extensions["exceptionName"] = exception.GetType().FullName;
		problemDetails.Extensions["stackTrace"] = exception.StackTrace;

		if (exception.InnerException != null)
		{
			problemDetails.Extensions["innerException"] = exception.InnerException.Message;
		}

		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			ProblemDetails = problemDetails,
			Exception = exception
		});

	}
}

internal static partial class MiddlewareLogDefinitions
{
	[LoggerMessage(EventId = 500, Level = LogLevel.Error, Message = "Unhandled exception occurred: {ExceptionName} at {Path}. TraceId: {TraceId}")]
	public static partial void LogUnhandledCriticalException(ILogger logger, string exceptionName, string path, string traceId);
}
using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Domain.Common.Results;
using System.Diagnostics;

namespace Rakushu.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;

		LogDefinitions.LogExecutingRequest(_logger, requestName);

		var stopwatch = Stopwatch.StartNew();

		try
		{
			var response = await next();

			stopwatch.Stop();

			if (response.IsSuccess)
			{
				LogDefinitions.LogRequestSuccess(
					_logger, 
					requestName, 
					stopwatch.ElapsedMilliseconds);
			}
			else
			{
				LogDefinitions.LogRequestBusinessFailure(
					_logger, 
					requestName, 
					stopwatch.ElapsedMilliseconds, 
					response.Error.Code, 
					response.Error.Message);
			}

			return response;
		}
		catch (Exception ex)
		{
			stopwatch.Stop();

			var exceptionName = ex.GetType().Name;
			var exceptionMessage = ex.Message;
			LogDefinitions.LogRequestUnhandledException(
				_logger, 
				requestName, 
				stopwatch.ElapsedMilliseconds,
				exceptionName, 
				exceptionMessage);

			throw;
		}
	}
}

internal static partial class LogDefinitions
{
    [LoggerMessage (
		EventId = 1, 
		Level = LogLevel.Information, 
		Message = "START: Request [{RequestName}] start executing" )]
    public static partial void LogExecutingRequest(ILogger logger, string requestName);


    [LoggerMessage (
		EventId = 2, 
		Level = LogLevel.Information, 
		Message = "SUCCESS: Request [{RequestName}] executed successfully in {ElapsedMilliseconds}ms" )]
    public static partial void LogRequestSuccess(ILogger logger, string requestName, long elapsedMilliseconds);


    [LoggerMessage (
		EventId = 3, 
		Level = LogLevel.Warning, 
		Message = "BUSSINESS FAILURE WITH CODE [{ErrorCode}]: Request {RequestName} executed with Business Failure in {ElapsedMilliseconds}ms. Reason: {ErrorMessage}")]
    public static partial void LogRequestBusinessFailure(ILogger logger, string requestName, long elapsedMilliseconds, string? errorCode, string? errorMessage);


    [LoggerMessage (
		EventId = 4, 
		Level = LogLevel.Error, 
		Message = "UNHANDLED EXCEPTION [{ExceptionName}]: Request {RequestName} failed with unhandled exception after {ElapsedMilliseconds}ms. Error: {ExceptionMessage}")]
    public static partial void LogRequestUnhandledException(ILogger logger, string requestName, long elapsedMilliseconds, string exceptionName, string exceptionMessage);
}
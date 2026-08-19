using Microsoft.AspNetCore.Mvc;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Api.Extensions;

public static class ProblemExtension
{
	public static IResult Problem(this Result result)
	{
		if (result.IsSuccess) throw new InvalidOperationException("Cannot create problem from successful result");
		return DispatchProblem(result.Error);
	}

	public static IResult Problem<T>(this Result<T> result)
	{
		if (result.IsSuccess) throw new InvalidOperationException("Cannot create problem from successful result");
		return DispatchProblem(result.Error);
	}

	private static IResult DispatchProblem(Error error)
	{
		var statusCode = GetStatusCode(error.Type);
		var errorTitle = GetTitle(error.Type);
		var typeUrl = GetTypeUrl(statusCode);

		if (error is ValidationError validationError)
		{
			var errorsDictionary = validationError.Errors
				.GroupBy(e => e.Code) /// Property Name
				.ToDictionary(
					g => g.Key,
					g => g.Select(e => e.Message).ToArray()
				);

			/// PROBLEM DETAILS: Tạo response chuẩn RFC 7807 cho lỗi validation
			return Microsoft.AspNetCore.Http.Results.ValidationProblem(
				statusCode: statusCode,
				title: errorTitle,
				detail: validationError.Message,
				type: typeUrl,
				errors: errorsDictionary
			);
		}

		/// PROBLEM DETAILS: Tạo response chuẩn RFC 7807 cho các lỗi khác (NotFound, Conflict, Unauthorized, Forbidden, Failure)
		var problemDetails = new ProblemDetails
		{
			Status = statusCode,
			Title = errorTitle,
			Detail = error.Message,
			Type = typeUrl,
		};
		problemDetails.Extensions["errorCode"] = error.Code;

		return Microsoft.AspNetCore.Http.Results.Problem(problemDetails);
	}

	private static int GetStatusCode(ErrorType errorType) => errorType switch
	{
		ErrorType.Validation => StatusCodes.Status400BadRequest,
		ErrorType.NotFound => StatusCodes.Status404NotFound,
		ErrorType.Conflict => StatusCodes.Status409Conflict,
		ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
		ErrorType.Forbidden => StatusCodes.Status403Forbidden,
		ErrorType.Failure => StatusCodes.Status400BadRequest,
		ErrorType.None => StatusCodes.Status200OK,
		_ => StatusCodes.Status500InternalServerError
	};

	private static string GetTitle(ErrorType errorType) => errorType switch
	{
		ErrorType.Validation => "Validation",
		ErrorType.NotFound => "Not Found",
		ErrorType.Conflict => "Conflict",
		ErrorType.Unauthorized => "Unauthorized",
		ErrorType.Forbidden => "Forbidden",
		ErrorType.Failure => "Bad Request",
		_ => "Internal Server Error"
	};

	private static string GetTypeUrl(int statusCode) => $"https://httpstatuses.com/{statusCode}";
}
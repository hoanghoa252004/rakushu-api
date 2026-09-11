using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Api.Extensions;

public static class ResultExtension
{
	/// SUCCESS: 200
	public static IResult MatchOk(this Result result)
	{
		return result.IsSuccess ? Results.Ok() : result.Problem();
	}

	public static IResult MatchOk<T>(this Result<T> result)
	{
		return result.IsSuccess ? Results.Ok(result.Value) : result.Problem();
	}

	public static IResult MatchOk(this Result result, Func<IResult> onSuccess)
	{
		return result.IsSuccess ? onSuccess() : result.Problem();
	}

	public static IResult MatchOk<T>(this Result<T> result, Func<T, IResult> onSuccess)
	{
		return result.IsSuccess ? onSuccess(result.Value) : result.Problem();
	}

	/// CREATED: 201
	public static IResult MatchCreated<T>(
	this Result<T> result,
	string routeName,
	Func<T, object> routeValues)
	{
		return result.IsSuccess
			? Microsoft.AspNetCore.Http.Results.CreatedAtRoute(routeName, routeValues(result.Value), result.Value)
			: result.Problem();
	}
	/// ACCEPTED: 202
	public static IResult MatchAccepted<T>(this Result<T> result, string? uri = null)
	{
		return result.IsSuccess
			? Microsoft.AspNetCore.Http.Results.Accepted(uri, result.Value)
			: result.Problem();
	}

	/// NO CONTENT: 204
	public static IResult MatchNoContent(this Result result)
	{
		return result.IsSuccess 
			? Microsoft.AspNetCore.Http.Results.NoContent() 
			: result.Problem();
	}

	
}

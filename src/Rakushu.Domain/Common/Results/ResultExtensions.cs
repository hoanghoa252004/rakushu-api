using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Common.Results;

public static class ResultExtensions
{
	// ToResult: Chuyển một object (có thể null) thành Result
	public static Result<T> ToResult<T>(this T? value, Error error)
		=> value is not null ? Result.Success(value) : Result.Failure<T>(error);

	// Bind: Chạy hàm tiếp theo nếu thành công, nếu thất bại thì trả về Error ngay lập tức
	public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
		=> result.IsSuccess ? func(result.Value) : Result.Failure<TOut>(result.Error);

	// BindAsync: Hỗ trợ async/await trong chuỗi ROP
	public static async Task<Result<TOut>> Bind<TIn, TOut>(
		this Task<Result<TIn>> resultTask,
		Func<TIn, Task<Result<TOut>>> func)
	{
		var result = await resultTask;
		return result.IsSuccess ? await func(result.Value) : Result.Failure<TOut>(result.Error);
	}

	// Tap: Thực hiện hành động (side-effect) như SaveChanges nếu thành công
	public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Func<Task> action)
	{
		var result = await resultTask;
		if (result.IsSuccess) await action();
		return result;
	}

	// Match: Bước cuối cùng để chuyển đổi sang IResult (Web API) hoặc View Model
	public static TOut Match<TIn, TOut>(
		this Result<TIn> result,
		Func<TIn, TOut> onSuccess,
		Func<Result<TIn>, TOut> onFailure)
		=> result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
}

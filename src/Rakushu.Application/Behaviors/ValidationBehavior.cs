using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using System.Collections.Concurrent;
using System.Reflection;

namespace Rakushu.Application.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	// Sử dụng ConcurrentDictionary để Cache lại các hàm Reflection (Tránh quét CPU nhiều lần)
	private static readonly ConcurrentDictionary<Type, MethodInfo> _failureMethodCache = new();

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next();
		}

		// 1. SỬA BUG: Gọi đích danh ValidationContext của FluentValidation
		var context = new FluentValidation.ValidationContext<TRequest>(request);

		var validationResults = await Task.WhenAll(
			_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

		// 2. TỐI ƯU: Chỉ lọc và tạo mảng lỗi KHI VÀ CHỈ KHI thực sự có lỗi xảy ra
		var failures = validationResults
			.SelectMany(r => r.Errors)
			.Where(f => f is not null)
			.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
			.Distinct() // Tránh trùng lặp một lỗi nhiều lần
			.ToList();

		if (failures.Count == 0)
		{
			return await next();
		}

		// Chuyển sang Array ở bước cuối cùng khi chắc chắn có lỗi
		return CreateValidationResult<TResponse>(failures.ToArray());
	}

	private static TResult CreateValidationResult<TResult>(Error[] errors)
		where TResult : Result
	{
		var validationError = new ValidationError(errors);

		// Case 1: Result thường (Non-generic)
		if (typeof(TResult) == typeof(Result))
		{
			return (Result.Failure(validationError) as TResult)!;
		}

		// Case 2: Result<T> (Generic) - Áp dụng Kỹ thuật Cache Method Info siêu tốc
		var resultType = typeof(TResult).GenericTypeArguments[0];

		var genericMethod = _failureMethodCache.GetOrAdd(resultType, type =>
		{
			var methodInfo = typeof(Result)
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.Where(m => m.Name == nameof(Result.Failure))
				.Where(m => m.IsGenericMethodDefinition)
				.Single(m => m.GetGenericArguments().Length == 1);

			return methodInfo.MakeGenericMethod(type);
		});

		var result = genericMethod.Invoke(null, [validationError]); // Cú pháp mảng mới C# 12+ []

		return (TResult)result!;
	}
}
using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Common.Results;

public class Result
{
	// Constructors
	private protected Result(bool isSuccess, Error error)
	{
		if (isSuccess && error != Error.None() || !isSuccess && error == Error.None())
			throw new InvalidOperationException("Invalid error state.");

		IsSuccess = isSuccess;
		Error = error;
	}

	// Properties 
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public Error Error { get; }

	// Factory Methods
	public static Result Success() => new(true, Error.None());
	public static Result Failure(Error error) => new(false, error);
	public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None());
	public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}

public class Result<TValue> : Result
{
	// Fields
	private readonly TValue? _value;

	// Constructors
	protected internal Result(TValue? value, bool isSuccess, Error error)
		: base(isSuccess, error) => _value = value;

	// Properties
	public TValue Value => IsSuccess
		? _value!
		: throw new InvalidOperationException("Failure result has no value.");

	// Implicit conversion giúp return trực tiếp TValue hoặc Error
	//public static implicit operator Result<TValue>(TValue? value) => Success(value);
	//public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}
using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Common.Results;

public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
	// Constructors
	private ValidationResult(Error[] errors)
		: base(default, false, new ValidationError(errors))
	{
		Errors = errors;
	}

	// Properties
	public Error[] Errors { get; }

	// Factory Methods
	public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
}
namespace Rakushu.Domain.Common.Errors;

public sealed record ValidationError(Error[] Errors)
	: Error("GENERAL.VALIDATION", "One or more validation errors occurred", ErrorType.Validation);
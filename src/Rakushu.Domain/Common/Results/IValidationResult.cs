using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Common.Results;

public interface IValidationResult
{
	public Error[] Errors { get; }
}

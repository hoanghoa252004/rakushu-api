using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs.QuestionType;

public sealed class QuestionType : AggregateRoot<QuestionTypeId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; }

	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	private QuestionType() { }
	private QuestionType(QuestionTypeId id) : base(id) { }

	public static Result<QuestionType> Create(
		string code,
		string name,
		string? description,
		DateTimeOffset now,
		bool isActive = true)
	{
		var validation = Validate(code, name);
		if (validation.IsFailure)
			return Result.Failure<QuestionType>(validation.Error);

		return Result.Success(new QuestionType(QuestionTypeId.Create())
		{
			Code = code.Trim().ToUpperInvariant(),
			Name = name.Trim(),
			Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
			IsActive = isActive,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void Deactivate(DateTimeOffset now)
	{
		IsActive = false;
		UpdatedAt = now;
	}

	public void Activate(DateTimeOffset now)
	{
		IsActive = true;
		UpdatedAt = now;
	}

	private static Result Validate(string code, string name)
	{
		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure(QuestionTypeErrors.EmptyCode);

		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(QuestionTypeErrors.EmptyName);

		return Result.Success();
	}
}

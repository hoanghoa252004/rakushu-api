using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public sealed class BankQuestion : AggregateRoot<BankQuestionId>
{
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public QuestionTypeId QuestionTypeId { get; private set; } = null!;
	public string Prompt { get; private set; } = null!;
	public string ChoicesJson { get; private set; } = "[]";
	public string CorrectChoiceId { get; private set; } = null!;
	public string? Explanation { get; private set; }
	public string Difficulty { get; private set; } = "Medium";
	public bool IsActive { get; private set; }

	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public QuestionType.QuestionType QuestionType { get; private set; } = null!;

	private BankQuestion() { }
	private BankQuestion(BankQuestionId id) : base(id) { }

	public static Result<BankQuestion> Create(
		KnowledgeMeaningId knowledgeMeaningId,
		QuestionTypeId questionTypeId,
		string prompt,
		string choicesJson,
		string correctChoiceId,
		DateTimeOffset now,
		string? explanation = null,
		string difficulty = "Medium",
		bool isActive = true)
	{
		var validation = Validate(knowledgeMeaningId, questionTypeId, prompt, correctChoiceId);
		if (validation.IsFailure)
			return Result.Failure<BankQuestion>(validation.Error);

		return Result.Success(new BankQuestion(BankQuestionId.Create())
		{
			KnowledgeMeaningId = knowledgeMeaningId,
			QuestionTypeId = questionTypeId,
			Prompt = prompt.Trim(),
			ChoicesJson = string.IsNullOrWhiteSpace(choicesJson) ? "[]" : choicesJson.Trim(),
			CorrectChoiceId = correctChoiceId.Trim(),
			Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim(),
			Difficulty = string.IsNullOrWhiteSpace(difficulty) ? "Medium" : difficulty.Trim(),
			IsActive = isActive,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void Update(
		string prompt,
		string choicesJson,
		string correctChoiceId,
		string? explanation,
		string difficulty,
		DateTimeOffset now)
	{
		Prompt = prompt.Trim();
		ChoicesJson = choicesJson.Trim();
		CorrectChoiceId = correctChoiceId.Trim();
		Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
		Difficulty = string.IsNullOrWhiteSpace(difficulty) ? "Medium" : difficulty.Trim();
		UpdatedAt = now;
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

	private static Result Validate(
		KnowledgeMeaningId knowledgeMeaningId,
		QuestionTypeId questionTypeId,
		string prompt,
		string correctChoiceId)
	{
		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
			return Result.Failure(BankQuestionErrors.InvalidMeaning);

		if (questionTypeId is null || questionTypeId.Value == Guid.Empty)
			return Result.Failure(BankQuestionErrors.InvalidType);

		if (string.IsNullOrWhiteSpace(prompt))
			return Result.Failure(BankQuestionErrors.EmptyPrompt);

		if (string.IsNullOrWhiteSpace(correctChoiceId))
			return Result.Failure(BankQuestionErrors.EmptyCorrectChoice);

		return Result.Success();
	}
}

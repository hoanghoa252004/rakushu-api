using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public sealed class BankQuestion : AggregateRoot<BankQuestionId>
{
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public QuestionTypeId QuestionTypeId { get; private set; } = null!;
	public string Question { get; private set; } = null!;
	public string Answers { get; private set; } = "[]";
	public string CorrectAnswer { get; private set; } = null!;
	public string? Explanation { get; private set; }
	public bool IsActive { get; private set; }
	public QuestionStatus Status { get; private set; } = QuestionStatus.Published;
	public DateTimeOffset? ReviewedAt { get; private set; }
	public string MediaType { get; private set; } = "NONE";
	public string? MediaUrl { get; private set; }
	public string? MediaMetadata { get; private set; }

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
		string question,
		string answers,
		string correctAnswer,
		DateTimeOffset now,
		string? explanation = null,
		bool isActive = true,
		string mediaType = "NONE",
		string? mediaUrl = null,
		string? mediaMetadata = null)
	{
		var validation = Validate(knowledgeMeaningId, questionTypeId, question, correctAnswer);
		if (validation.IsFailure)
			return Result.Failure<BankQuestion>(validation.Error);

		return Result.Success(new BankQuestion(BankQuestionId.Create())
		{
			KnowledgeMeaningId = knowledgeMeaningId,
			QuestionTypeId = questionTypeId,
			Question = question.Trim(),
			Answers = string.IsNullOrWhiteSpace(answers) ? "[]" : answers.Trim(),
			CorrectAnswer = correctAnswer.Trim(),
			Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim(),
			IsActive = isActive,
			Status = QuestionStatus.Published,
			ReviewedAt = now,
			MediaType = string.IsNullOrWhiteSpace(mediaType) ? "NONE" : mediaType.Trim().ToUpperInvariant(),
			MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim(),
			MediaMetadata = string.IsNullOrWhiteSpace(mediaMetadata) ? null : mediaMetadata.Trim(),
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public static Result<BankQuestion> CreateDraftFromAi(
		KnowledgeMeaningId knowledgeMeaningId,
		QuestionTypeId questionTypeId,
		string question,
		string answers,
		string correctAnswer,
		DateTimeOffset now,
		string? explanation = null,
		string mediaType = "NONE",
		string? mediaUrl = null,
		string? mediaMetadata = null)
	{
		var validation = Validate(knowledgeMeaningId, questionTypeId, question, correctAnswer);
		if (validation.IsFailure)
			return Result.Failure<BankQuestion>(validation.Error);

		return Result.Success(new BankQuestion(BankQuestionId.Create())
		{
			KnowledgeMeaningId = knowledgeMeaningId,
			QuestionTypeId = questionTypeId,
			Question = question.Trim(),
			Answers = string.IsNullOrWhiteSpace(answers) ? "[]" : answers.Trim(),
			CorrectAnswer = correctAnswer.Trim(),
			Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim(),
			IsActive = false,
			Status = QuestionStatus.Draft,
			ReviewedAt = null,
			MediaType = string.IsNullOrWhiteSpace(mediaType) ? "NONE" : mediaType.Trim().ToUpperInvariant(),
			MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim(),
			MediaMetadata = string.IsNullOrWhiteSpace(mediaMetadata) ? null : mediaMetadata.Trim(),
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void Publish(DateTimeOffset now)
	{
		Status = QuestionStatus.Published;
		IsActive = true;
		ReviewedAt = now;
		UpdatedAt = now;
	}

	public void UpdateAndPublish(
		string question,
		string answers,
		string correctAnswer,
		string? explanation,
		DateTimeOffset now,
		string? mediaType = null,
		string? mediaUrl = null,
		string? mediaMetadata = null)
	{
		Update(question, answers, correctAnswer, explanation, now, mediaType, mediaUrl, mediaMetadata);
		Publish(now);
	}

	public void Update(
		string question,
		string answers,
		string correctAnswer,
		string? explanation,
		DateTimeOffset now,
		string? mediaType = null,
		string? mediaUrl = null,
		string? mediaMetadata = null)
	{
		Question = question.Trim();
		Answers = string.IsNullOrWhiteSpace(answers) ? "[]" : answers.Trim();
		CorrectAnswer = correctAnswer.Trim();
		Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
		if (mediaType is not null)
			MediaType = string.IsNullOrWhiteSpace(mediaType) ? "NONE" : mediaType.Trim().ToUpperInvariant();
		if (mediaUrl is not null)
			MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim();
		if (mediaMetadata is not null)
			MediaMetadata = string.IsNullOrWhiteSpace(mediaMetadata) ? null : mediaMetadata.Trim();
		UpdatedAt = now;
	}

	public void Deactivate(DateTimeOffset now)
	{
		IsActive = false;
		UpdatedAt = now;
	}

	public void Reactivate(
		string answers,
		string correctAnswer,
		string? explanation,
		DateTimeOffset now,
		string? mediaType = null,
		string? mediaUrl = null,
		string? mediaMetadata = null)
	{
		Answers = string.IsNullOrWhiteSpace(answers) ? "[]" : answers.Trim();
		CorrectAnswer = correctAnswer.Trim();
		Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
		if (mediaType is not null)
			MediaType = string.IsNullOrWhiteSpace(mediaType) ? "NONE" : mediaType.Trim().ToUpperInvariant();
		if (mediaUrl is not null)
			MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim();
		if (mediaMetadata is not null)
			MediaMetadata = string.IsNullOrWhiteSpace(mediaMetadata) ? null : mediaMetadata.Trim();
		IsActive = true;
		UpdatedAt = now;
	}

	private static Result Validate(
		KnowledgeMeaningId knowledgeMeaningId,
		QuestionTypeId questionTypeId,
		string question,
		string correctAnswer)
	{
		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
			return Result.Failure(BankQuestionErrors.InvalidMeaning);

		if (questionTypeId is null || questionTypeId.Value == Guid.Empty)
			return Result.Failure(BankQuestionErrors.InvalidType);

		if (string.IsNullOrWhiteSpace(question))
			return Result.Failure(BankQuestionErrors.EmptyPrompt);

		if (string.IsNullOrWhiteSpace(correctAnswer))
			return Result.Failure(BankQuestionErrors.EmptyCorrectChoice);

		return Result.Success();
	}
}

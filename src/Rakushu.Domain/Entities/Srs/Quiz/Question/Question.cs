using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.Quiz.Question;

public sealed class Question : Entity<QuestionId>
{
	public QuizId QuizId { get; private set; } = null!;
	public SavedItemId SavedItemId { get; private set; } = null!;
	public BankQuestionId? BankQuestionId { get; private set; }
	public QuestionTypeId QuestionTypeId { get; private set; } = null!;
	public int Position { get; private set; }
	public string Prompt { get; private set; } = null!;
	public string? Reading { get; private set; }
	public string ChoicesJson { get; private set; } = "[]";
	public string CorrectOptionId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public Quiz Quiz { get; private set; } = null!;
	public SavedItem.SavedItem SavedItem { get; private set; } = null!;
	public BankQuestion.BankQuestion? BankQuestion { get; private set; }
	public QuestionType.QuestionType QuestionType { get; private set; } = null!;

	private Question() { }
	private Question(QuestionId id) : base(id) { }

	public static Result<Question> Create(
		SavedItemId savedItemId,
		QuestionTypeId questionTypeId,
		int position,
		string prompt,
		string? reading,
		string choicesJson,
		string correctOptionId,
		DateTimeOffset now,
		BankQuestionId? bankQuestionId = null)
	{
		var validation = Validate(savedItemId, questionTypeId, position, prompt, correctOptionId);
		if (validation.IsFailure)
			return Result.Failure<Question>(validation.Error);

		return Result.Success(new Question(QuestionId.Create())
		{
			SavedItemId = savedItemId,
			QuestionTypeId = questionTypeId,
			BankQuestionId = bankQuestionId,
			Position = position,
			Prompt = prompt.Trim(),
			Reading = string.IsNullOrWhiteSpace(reading) ? null : reading.Trim(),
			ChoicesJson = string.IsNullOrWhiteSpace(choicesJson) ? "[]" : choicesJson.Trim(),
			CorrectOptionId = correctOptionId.Trim(),
			CreatedAt = now
		});
	}

	internal void AssignToQuiz(QuizId quizId, int position)
	{
		QuizId = quizId;
		Position = position;
	}

	private static Result Validate(
		SavedItemId savedItemId,
		QuestionTypeId questionTypeId,
		int position,
		string prompt,
		string correctOptionId)
	{
		if (savedItemId is null || savedItemId.Value == Guid.Empty)
			return Result.Failure(QuestionErrors.InvalidSavedItemId);

		if (questionTypeId is null || questionTypeId.Value == Guid.Empty)
			return Result.Failure(QuestionErrors.InvalidTypeId);

		if (position < 1)
			return Result.Failure(QuestionErrors.InvalidPosition);

		if (string.IsNullOrWhiteSpace(prompt))
			return Result.Failure(QuestionErrors.EmptyPrompt);

		if (string.IsNullOrWhiteSpace(correctOptionId))
			return Result.Failure(QuestionErrors.EmptyCorrectOption);

		return Result.Success();
	}
}

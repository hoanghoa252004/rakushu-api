using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs;

public sealed class Question : Entity<QuestionId>
{
	// MAIN PROPERTIES----------
	public QuizId QuizId { get; private set; } = null!;
	public SrsItemId SrsItemId { get; private set; } = null!;
	public int Position { get; private set; }
	public string QuestionType { get; private set; } = "WORD_TO_MEANING_MCQ";
	public string Prompt { get; private set; } = null!;
	public string? Reading { get; private set; }
	public string ChoicesJson { get; private set; } = "[]";
	public string CorrectOptionId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	public Quiz Quiz { get; private set; } = null!;
	public SrsItem SrsItem { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS----------
	private Question() { }
	private Question(QuestionId id) : base(id) { }

	public static Result<Question> Create(
		SrsItemId itemId,
		int position,
		string questionType,
		string prompt,
		string? reading,
		string choicesJson,
		string correctOptionId,
		DateTimeOffset now)
	{
		if (itemId is null || itemId.Value == Guid.Empty)
			return Result.Failure<Question>(QuizAttemptErrors.InvalidItemId);
		if (position < 1)
			return Result.Failure<Question>(QuizErrors.InvalidQuestionPosition);
		if (string.IsNullOrWhiteSpace(prompt))
			return Result.Failure<Question>(QuizErrors.InvalidPrompt);
		if (string.IsNullOrWhiteSpace(correctOptionId))
			return Result.Failure<Question>(QuizErrors.InvalidCorrectOptionId);

		return Result.Success(new Question(QuestionId.Create())
		{
			SrsItemId = itemId,
			Position = position,
			QuestionType = questionType,
			Prompt = prompt.Trim(),
			Reading = string.IsNullOrWhiteSpace(reading) ? null : reading.Trim(),
			ChoicesJson = string.IsNullOrWhiteSpace(choicesJson) ? "[]" : choicesJson,
			CorrectOptionId = correctOptionId.Trim(),
			CreatedAt = now
		});
	}

	internal void AssignToQuiz(QuizId quizId, int position)
	{
		QuizId = quizId;
		Position = position;
	}
}

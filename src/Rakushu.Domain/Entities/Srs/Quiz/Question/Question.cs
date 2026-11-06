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
	public BankQuestionId BankQuestionId { get; private set; } = null!;
	public int Sequence { get; private set; }
	public string QuestionText { get; private set; } = null!;
	public string Answers { get; private set; } = "[]";
	public string CorrectAnswer { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	public Quiz Quiz { get; private set; } = null!;
	public SavedItem.SavedItem SavedItem { get; private set; } = null!;
	public BankQuestion.BankQuestion BankQuestion { get; private set; } = null!;
	public QuestionType.QuestionType? QuestionType => BankQuestion?.QuestionType;

	private Question() { }
	private Question(QuestionId id) : base(id) { }

	public static Result<Question> Create(
		SavedItemId savedItemId,
		BankQuestionId bankQuestionId,
		int sequence,
		string questionText,
		string answers,
		string correctAnswer)
	{
		var validation = Validate(savedItemId, bankQuestionId, sequence, questionText, correctAnswer);
		if (validation.IsFailure)
			return Result.Failure<Question>(validation.Error);

		return Result.Success(new Question(Rakushu.Domain.Entities.Srs.Quiz.Question.QuestionId.Create())
		{
			SavedItemId = savedItemId,
			BankQuestionId = bankQuestionId,
			Sequence = sequence,
			QuestionText = questionText.Trim(),
			Answers = string.IsNullOrWhiteSpace(answers) ? "[]" : answers.Trim(),
			CorrectAnswer = correctAnswer.Trim()
		});
	}

	internal void AssignToQuiz(QuizId quizId, int sequence)
	{
		QuizId = quizId;
		Sequence = sequence;
	}

	private static Result Validate(
		SavedItemId savedItemId,
		BankQuestionId bankQuestionId,
		int sequence,
		string questionText,
		string correctAnswer)
	{
		if (savedItemId is null || savedItemId.Value == Guid.Empty)
			return Result.Failure(QuestionErrors.InvalidSavedItemId);

		if (bankQuestionId is null || bankQuestionId.Value == Guid.Empty)
			return Result.Failure(QuestionErrors.NotFound);

		if (sequence < 1)
			return Result.Failure(QuestionErrors.InvalidPosition);

		if (string.IsNullOrWhiteSpace(questionText))
			return Result.Failure(QuestionErrors.EmptyPrompt);

		if (string.IsNullOrWhiteSpace(correctAnswer))
			return Result.Failure(QuestionErrors.EmptyCorrectOption);

		return Result.Success();
	}
}

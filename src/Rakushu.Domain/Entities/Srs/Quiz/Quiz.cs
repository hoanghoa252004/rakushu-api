using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz.Question;

namespace Rakushu.Domain.Entities.Srs.Quiz;

public sealed class Quiz : AggregateRoot<QuizId>
{
	public UserId UserId { get; private set; } = null!;
	public int TotalQuestions { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;

	private readonly List<Question.Question> _questions = [];
	public IReadOnlyCollection<Question.Question> Questions => _questions.AsReadOnly();

	private Quiz() { }
	private Quiz(QuizId id) : base(id) { }

	public static Result<Quiz> Create(UserId userId, DateTimeOffset now)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure<Quiz>(QuizErrors.InvalidUserId);

		return Result.Success(new Quiz(QuizId.Create())
		{
			UserId = userId,
			CreatedAt = now
		});
	}

	public Result AddQuestion(Question.Question question)
	{
		if (question is null)
			return Result.Failure(QuizErrors.NullQuestion);

		question.AssignToQuiz(Id, _questions.Count + 1);
		_questions.Add(question);
		TotalQuestions = _questions.Count;
		return Result.Success();
	}
}

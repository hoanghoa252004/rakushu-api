using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public sealed class Quiz : AggregateRoot<QuizId>
{
	// MAIN PROPERTIES----------
	public UserId UserId { get; private set; } = null!;
	public int TotalQuestions { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	public User User { get; private set; } = null!;

	private readonly List<Question> _questions = [];
	public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

	private readonly List<QuizAttempt> _attempts = [];
	public IReadOnlyCollection<QuizAttempt> Attempts => _attempts.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Quiz() { }
	private Quiz(QuizId id) : base(id) { }

	public static Result<Quiz> Create(UserId userId, DateTimeOffset now)
	{
		if (userId is null)
			return Result.Failure<Quiz>(QuizErrors.InvalidUserId);

		return Result.Success(new Quiz(QuizId.Create())
		{
			UserId = userId,
			CreatedAt = now
		});
	}

	public Result AddQuestion(Question question)
	{
		if (question is null)
			return Result.Failure(QuizErrors.InvalidQuestion);

		question.AssignToQuiz(Id, _questions.Count + 1);
		_questions.Add(question);
		TotalQuestions = _questions.Count;
		return Result.Success();
	}
}

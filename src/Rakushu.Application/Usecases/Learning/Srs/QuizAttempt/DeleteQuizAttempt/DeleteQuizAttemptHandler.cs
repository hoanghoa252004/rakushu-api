using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteQuizAttempt;

internal sealed class DeleteQuizAttemptHandler : IRequestHandler<DeleteQuizAttemptCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(
				QuizAttemptId.From(request.AttemptId),
				cancellationToken);

			if (attempt is null)
				return Result.Failure(QuizAttemptErrors.NotFound);

			if (attempt.UserId != userId)
				return Result.Failure(QuizAttemptErrors.NotFound);

			if (attempt.AnsweredItems > 0)
				return Result.Failure(QuizAttemptErrors.CannotDeleteAttemptWithAnswers);

			_quizAttemptRepository.Delete(attempt);
			return Result.Success();
		}, cancellationToken);
	}
}

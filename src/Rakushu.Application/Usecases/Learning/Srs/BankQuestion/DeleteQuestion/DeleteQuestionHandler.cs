using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.DeleteQuestion;

internal sealed class DeleteQuestionHandler : IRequestHandler<DeleteQuestionCommand, Result>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteQuestionHandler(
		IBankQuestionRepository bankQuestionRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteQuestionCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var questionId = BankQuestionId.From(request.Id);
			var question = await _bankQuestionRepository.GetByIdAsync(questionId, cancellationToken);
			if (question is null || !question.IsActive)
			{
				return Result.Failure(BankQuestionErrors.NotFound);
			}

			bool hasBeenUsed = await _bankQuestionRepository.HasBeenUsedInQuizAsync(questionId, cancellationToken);

			if (hasBeenUsed)
			{
				// Case 2: Đã có người dùng làm bài thi => Soft delete (chuyển isActive = false để bảo vệ tính toàn vẹn lịch sử)
				question.Deactivate(_systemClock.UtcNow);
				_bankQuestionRepository.Update(question);
			}
			else
			{
				// Case 1: Câu hỏi mới tạo / chưa từng ai làm => Xóa cứng hoàn toàn khỏi cơ sở dữ liệu
				_bankQuestionRepository.Delete(question);
			}

			return Result.Success();
		}, cancellationToken);
	}
}

using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.DeleteSrsItem;

internal sealed class DeleteSrsItemHandler : IRequestHandler<DeleteSrsItemCommand, Result>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteSrsItemHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var itemId = SavedItemId.From(request.Id);
			var item = await _savedItemRepository.GetByIdWithScheduleAsync(itemId, cancellationToken);
			if (item is null || item.UserId != userId)
				return Result.Failure(SavedItemErrors.NotFound);

			bool hasReviewHistory = item.Schedule != null && item.Schedule.Details.Count > 0;
			if (hasReviewHistory)
			{
				// Case 2: Đã có lịch sử ôn tập/bài thi => Xóa mềm (Deactivate) để bảo toàn tính toàn vẹn
				item.Deactivate();
				_savedItemRepository.Update(item);
			}
			else
			{
				// Case 1: Chưa từng ôn tập lần nào => Xóa cứng hoàn toàn khỏi cơ sở dữ liệu
				_savedItemRepository.Delete(item);
			}

			return Result.Success();
		}, cancellationToken);
	}
}

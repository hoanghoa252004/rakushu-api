using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.DeleteKnowledgeMeaning;

public sealed record DeleteKnowledgeMeaningCommand(Guid KnowledgeMeaningId) : IRequest<Result>;

internal sealed class DeleteKnowledgeMeaningHandler : IRequestHandler<DeleteKnowledgeMeaningCommand, Result>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteKnowledgeMeaningHandler(
		IKnowledgeMeaningRepository repository,
		ISavedItemRepository savedItemRepository,
		IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_savedItemRepository = savedItemRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteKnowledgeMeaningCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaningId = KnowledgeMeaningId.From(request.KnowledgeMeaningId);
			var meaning = await _repository.GetByIdAsync(meaningId, cancellationToken);

			if (meaning is null || !meaning.IsActive)
				return Result.Failure(KnowledgeMeaningErrors.NotFound);

			bool isInUse = meaning.LearningUnits.Any() || await _savedItemRepository.HasAnyWithMeaningIdAsync(meaningId, cancellationToken);

			if (isInUse)
			{
				// Case 2: Đã có video gán hoặc có học viên lưu => Xóa mềm để bảo toàn tính toàn vẹn
				meaning.Deactivate();
				_repository.Update(meaning);
			}
			else
			{
				// Case 1: Chưa từng ai dùng => Xóa cứng hoàn toàn khỏi cơ sở dữ liệu
				_repository.Delete(meaning);
			}

			return Result.Success();
		}, cancellationToken);
	}
}

using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.ChangeVideoStatus;

internal sealed class ChangeVideoStatusHandler : IRequestHandler<ChangeVideoStatusCommand, Result>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public ChangeVideoStatusHandler(
		IVideoRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ChangeVideoStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var updateResult = video.ChangeStatus(request.NewStatus);
			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}

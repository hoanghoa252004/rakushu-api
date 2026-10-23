using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.UpdateVideo;

internal sealed class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public UpdateVideoHandler(
		IVideoRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var now = _systemClock.UtcNow;
			var updateResult = video.UpdateBasicInfo(
				request.Title,
				request.Description,
				//ContentCategoryId.From(request.ContentCategoryId),
				request.SeriesId.HasValue ? SeriesId.From(request.SeriesId.Value) : null,
				request.SortOrder,
				now);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}

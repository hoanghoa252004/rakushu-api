using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.UpdateVideo;

internal sealed class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateVideoHandler(IVideoRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var updateResult = video.Update(
				request.slug,
				request.title,
				request.duration,
				ContentCategoryId.From(request.contentCategoryId),
				SeriesId.From(request.seriesId),
				request.sortOrder,
				request.sourceType,
				request.status,
				now,
				request.description,
				request.sourceUrl);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}
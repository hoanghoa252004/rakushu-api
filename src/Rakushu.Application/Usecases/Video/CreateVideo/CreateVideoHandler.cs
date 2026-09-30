using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Video.CreateVideo;

internal sealed class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<Guid>>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateVideoHandler(IVideoRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = DateTimeOffset.UtcNow;
			var videoResult = Domain.Entities.Video.Video.Create(
				request.slug,
				request.title,
				request.duration,
				ContentCategoryId.From(request.contentCategoryId),
				SeriesId.From(request.seriesId),
				request.sortOrder,
				request.sourceType,
				request.status,
				UserId.From(request.createdBy),
				now,
				now,
				request.description,
				request.sourceUrl);

			if (videoResult.IsFailure)
				return Result.Failure<Guid>(videoResult.Error);

			_repository.Add(videoResult.Value);
			return Result.Success(videoResult.Value.Id.Value);
		}, cancellationToken);
	}
}
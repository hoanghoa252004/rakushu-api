using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.DeleteVideo;

internal sealed class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result>
{
	private readonly IVideoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteVideoHandler(IVideoRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			_repository.Delete(video);
			return Result.Success();
		}, cancellationToken);
	}
}
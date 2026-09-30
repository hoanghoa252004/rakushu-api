using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Bunsetsu.DeleteBunsetsu;

internal sealed class DeleteBunsetsuHandler : IRequestHandler<DeleteBunsetsuCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteBunsetsuHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteBunsetsuCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var bunsetsuId = BunsetsuId.From(request.BunsetsuId);
			var video = await _videoRepository.GetByBunsetsuIdAsync(bunsetsuId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(BunsetsuErrors.NotFound);

			var segment = video.Transcript.TranscriptSegments
				.FirstOrDefault(s => s.Bunsetsu.Any(b => b.Id == bunsetsuId));

			if (segment is null)
				return Result.Failure(BunsetsuErrors.NotFound);

			return segment.RemoveBunsetsu(bunsetsuId);
		}, cancellationToken);
	}
}
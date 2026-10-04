using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Bunsetsu.UpdateBunsetsu;

internal sealed class UpdateBunsetsuHandler : IRequestHandler<UpdateBunsetsuCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateBunsetsuHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateBunsetsuCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var bunsetsuId = BunsetsuId.From(request.BunsetsuId);
			var video = await _videoRepository.GetByBunsetsuIdAsync(bunsetsuId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(BunsetsuErrors.NotFound);

			var bunsetsu = video.Transcript.TranscriptSegments
				.SelectMany(s => s.Bunsetsu)
				.FirstOrDefault(b => b.Id == bunsetsuId);

			if (bunsetsu is null)
				return Result.Failure(BunsetsuErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return bunsetsu.Update(request.text, request.startIndex, request.endIndex, request.sequence, now);
		}, cancellationToken);
	}
}
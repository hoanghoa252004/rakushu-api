using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.LearningUnit.RemoveLearningUnit;

internal sealed class RemoveLearningUnitHandler : IRequestHandler<RemoveLearningUnitCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public RemoveLearningUnitHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(RemoveLearningUnitCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			if (video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			var segmentId = TranscriptSegmentId.From(request.SegmentId);
			var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			return segment.RemoveLearningUnit(LearningUnitId.From(request.LearningUnitId));
		}, cancellationToken);
	}
}

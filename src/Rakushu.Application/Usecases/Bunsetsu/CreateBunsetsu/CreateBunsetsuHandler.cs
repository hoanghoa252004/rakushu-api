using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Bunsetsu.CreateBunsetsu;

internal sealed class CreateBunsetsuHandler : IRequestHandler<CreateBunsetsuCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateBunsetsuHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateBunsetsuCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var segmentId = TranscriptSegmentId.From(request.transcriptSegmentId);
			var video = await _videoRepository.GetByTranscriptSegmentIdAsync(segmentId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure<Guid>(TranscriptSegmentErrors.NotFound);

			var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure<Guid>(TranscriptSegmentErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var result = segment.AddBunsetsu(
				request.text,
				request.startIndex,
				request.endIndex,
				request.sequence,
				now,
				now);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
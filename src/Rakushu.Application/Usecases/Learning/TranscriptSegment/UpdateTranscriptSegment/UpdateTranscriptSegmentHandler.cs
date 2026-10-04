using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateTranscriptSegment;

internal sealed class UpdateTranscriptSegmentHandler : IRequestHandler<UpdateTranscriptSegmentCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTranscriptSegmentHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateTranscriptSegmentCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var segmentId = TranscriptSegmentId.From(request.TranscriptSegmentId);
			var video = await _videoRepository.GetByTranscriptSegmentIdAsync(segmentId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return segment.Update(request.text, request.startTime, request.endTime, request.sequence, now);
		}, cancellationToken);
	}
}
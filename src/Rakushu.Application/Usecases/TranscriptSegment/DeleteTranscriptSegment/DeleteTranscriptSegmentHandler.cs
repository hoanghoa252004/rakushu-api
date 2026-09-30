using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.TranscriptSegment.DeleteTranscriptSegment;

internal sealed class DeleteTranscriptSegmentHandler : IRequestHandler<DeleteTranscriptSegmentCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTranscriptSegmentHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteTranscriptSegmentCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var segmentId = TranscriptSegmentId.From(request.TranscriptSegmentId);
			var video = await _videoRepository.GetByTranscriptSegmentIdAsync(segmentId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			return video.Transcript.RemoveSegment(segmentId);
		}, cancellationToken);
	}
}
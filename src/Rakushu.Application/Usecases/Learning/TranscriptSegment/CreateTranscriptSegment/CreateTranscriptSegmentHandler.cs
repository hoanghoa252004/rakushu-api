using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateTranscriptSegment;

internal sealed class CreateTranscriptSegmentHandler : IRequestHandler<CreateTranscriptSegmentCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTranscriptSegmentHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateTranscriptSegmentCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var transcriptId = TranscriptId.From(request.transcriptId);
			var video = await _videoRepository.GetByTranscriptIdAsync(transcriptId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure<Guid>(TranscriptErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var result = video.Transcript.AddSegment(
				request.text,
				request.startTime,
				request.endTime,
				request.sequence,
				now,
				now);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
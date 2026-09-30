using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.TranscriptSegment.GetTranscriptSegments;

internal sealed class GetTranscriptSegmentsHandler : IRequestHandler<GetTranscriptSegmentsQuery, Result<IReadOnlyCollection<TranscriptSegmentDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptSegmentsHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<IReadOnlyCollection<TranscriptSegmentDto>>> Handle(GetTranscriptSegmentsQuery request, CancellationToken cancellationToken)
	{
		var videos = await _videoRepository.GetAllAsync(cancellationToken);
		var segments = videos
			.Where(v => v.Transcript != null)
			.SelectMany(v => v.Transcript!.TranscriptSegments)
			.Select(TranscriptSegmentDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<TranscriptSegmentDto>>(segments);
	}
}
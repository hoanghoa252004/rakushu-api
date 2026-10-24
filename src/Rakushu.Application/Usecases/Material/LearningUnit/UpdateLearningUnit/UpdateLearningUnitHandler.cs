using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.LearningUnit.UpdateLearningUnit;

internal sealed class UpdateLearningUnitHandler : IRequestHandler<UpdateLearningUnitCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IKnowledgeMeaningRepository _meaningRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateLearningUnitHandler(
		IVideoRepository videoRepository,
		IKnowledgeMeaningRepository meaningRepository,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_meaningRepository = meaningRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateLearningUnitCommand request, CancellationToken cancellationToken)
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

			var learningUnitId = LearningUnitId.From(request.LearningUnitId);
			var learningUnit = segment.LearningUnits.FirstOrDefault(x => x.Id == learningUnitId);
			if (learningUnit is null)
				return Result.Failure(LearningUnitErrors.NotFound);

			var meaning = await _meaningRepository.GetByIdAsync(
				KnowledgeMeaningId.From(request.KnowledgeMeaningId), cancellationToken);
			if (meaning is null || meaning.KnowledgeUnit is null || !meaning.KnowledgeUnit.IsActive)
				return Result.Failure(LearningUnitErrors.KnowledgeMeaningNotFound);

			var tokenCount = segment.Bunsetsus.Sum(b => b.Tokens.Count);
			if (request.EndTokenIndex >= tokenCount)
				return Result.Failure(LearningUnitErrors.InvalidTokenRange);

			if (segment.LearningUnits.Any(x =>
				x.Id != learningUnitId &&
				x.KnowledgeMeaningId == meaning.Id &&
				x.StartTokenIndex == request.StartTokenIndex &&
				x.EndTokenIndex == request.EndTokenIndex))
				return Result.Failure(LearningUnitErrors.AlreadyExists);

			learningUnit.Update(meaning, request.StartTokenIndex, request.EndTokenIndex);
			return Result.Success();
		}, cancellationToken);
	}
}

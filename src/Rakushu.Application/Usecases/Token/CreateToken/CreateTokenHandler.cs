using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Token.CreateToken;

internal sealed class CreateTokenHandler : IRequestHandler<CreateTokenCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTokenHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var bunsetsuId = BunsetsuId.From(request.bunsetsuId);
			var video = await _videoRepository.GetByBunsetsuIdAsync(bunsetsuId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure<Guid>(BunsetsuErrors.NotFound);

			var bunsetsu = video.Transcript.TranscriptSegments
				.SelectMany(s => s.Bunsetsu)
				.FirstOrDefault(b => b.Id == bunsetsuId);

			if (bunsetsu is null)
				return Result.Failure<Guid>(BunsetsuErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var result = bunsetsu.AddToken(
				request.surface,
				request.lemma,
				request.reading,
				JapanesePartOfSpeechId.From(request.japanesePartOfSpeechId),
				UniversalPartOfSpeechId.From(request.universalPartOfSpeechId),
				DependencyRelationshipId.From(request.dependencyRelationshipId),
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
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Token.UpdateToken;

internal sealed class UpdateTokenHandler : IRequestHandler<UpdateTokenCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTokenHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var tokenId = TokenId.From(request.TokenId);
			var video = await _videoRepository.GetByTokenIdAsync(tokenId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TokenErrors.NotFound);

			var token = video.Transcript.TranscriptSegments
				.SelectMany(s => s.Bunsetsu)
				.SelectMany(b => b.Tokens)
				.FirstOrDefault(t => t.Id == tokenId);

			if (token is null)
				return Result.Failure(TokenErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return token.Update(
				request.surface,
				request.lemma,
				request.reading,
				JapanesePartOfSpeechId.From(request.japanesePartOfSpeechId),
				UniversalPartOfSpeechId.From(request.universalPartOfSpeechId),
				DependencyRelationshipId.From(request.dependencyRelationshipId),
				request.startIndex,
				request.endIndex,
				request.sequence,
				now);
		}, cancellationToken);
	}
}
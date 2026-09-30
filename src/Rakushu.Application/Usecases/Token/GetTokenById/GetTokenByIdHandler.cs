using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Token.GetTokenById;

internal sealed class GetTokenByIdHandler : IRequestHandler<GetTokenByIdQuery, Result<TokenDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTokenByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<TokenDto>> Handle(GetTokenByIdQuery request, CancellationToken cancellationToken)
	{
		var tokenId = TokenId.From(request.TokenId);
		var video = await _videoRepository.GetByTokenIdAsync(tokenId, cancellationToken);
		if (video is null || video.Transcript is null)
			return Result.Failure<TokenDto>(TokenErrors.NotFound);

		var token = video.Transcript.TranscriptSegments
			.SelectMany(s => s.Bunsetsu)
			.SelectMany(b => b.Tokens)
			.FirstOrDefault(t => t.Id == tokenId);

		if (token is null)
			return Result.Failure<TokenDto>(TokenErrors.NotFound);

		return Result.Success(TokenDto.FromEntity(token));
	}
}
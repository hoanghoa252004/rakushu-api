using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechById;

internal sealed class GetJapanesePartOfSpeechByIdHandler : IRequestHandler<GetJapanesePartOfSpeechByIdQuery, Result<JapanesePartOfSpeechDto>>
{
	private readonly IJapanesePartOfSpeechRepository _repository;

	public GetJapanesePartOfSpeechByIdHandler(IJapanesePartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<JapanesePartOfSpeechDto>> Handle(GetJapanesePartOfSpeechByIdQuery request, CancellationToken cancellationToken)
	{
		var pos = await _repository.GetByIdAsync(JapanesePartOfSpeechId.From(request.JapanesePartOfSpeechId), cancellationToken);
		if (pos is null)
			return Result.Failure<JapanesePartOfSpeechDto>(Error.NotFound("JAPANESE_PART_OF_SPEECH.NOT_FOUND", "Japanese part of speech not found."));

		return Result.Success(JapanesePartOfSpeechDto.FromEntity(pos));
	}
}
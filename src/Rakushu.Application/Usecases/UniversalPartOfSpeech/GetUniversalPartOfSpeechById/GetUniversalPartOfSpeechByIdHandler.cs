using MediatR;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

internal sealed class GetUniversalPartOfSpeechByIdHandler : IRequestHandler<GetUniversalPartOfSpeechByIdQuery, Result<UniversalPartOfSpeechDto>>
{
	private readonly IUniversalPartOfSpeechRepository _repository;

	public GetUniversalPartOfSpeechByIdHandler(IUniversalPartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<UniversalPartOfSpeechDto>> Handle(GetUniversalPartOfSpeechByIdQuery request, CancellationToken cancellationToken)
	{
		var pos = await _repository.GetByIdAsync(UniversalPartOfSpeechId.From(request.UniversalPartOfSpeechId), cancellationToken);
		if (pos is null)
			return Result.Failure<UniversalPartOfSpeechDto>(Error.NotFound("UNIVERSAL_PART_OF_SPEECH.NOT_FOUND", "Universal part of speech not found."));

		return Result.Success(UniversalPartOfSpeechDto.FromEntity(pos));
	}
}
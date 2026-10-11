using MediatR;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

internal sealed class GetUniversalPartOfSpeechByIdHandler : IRequestHandler<GetUniversalPartOfSpeechByIdQuery, Result<UniversalPartOfSpeechDto>>
{
	private readonly IUniversalPartOfSpeechRepository _repository;

	public GetUniversalPartOfSpeechByIdHandler(IUniversalPartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<UniversalPartOfSpeechDto>> Handle(GetUniversalPartOfSpeechByIdQuery request, CancellationToken cancellationToken)
	{
		var item = await _repository.GetByIdAsync(UniversalPartOfSpeechId.From(request.Id), cancellationToken);

		if (item == null) return Result.Failure<UniversalPartOfSpeechDto>(Error.NotFound("UniversalPartOfSpeech.NOT_FOUND", "KO co thay ma oi"));

		return Result.Success(new UniversalPartOfSpeechDto(
			item.Id.Value,
			item.Code,
			item.Name,
			item.JapaneseName,
			item.Description
			));
	}
}
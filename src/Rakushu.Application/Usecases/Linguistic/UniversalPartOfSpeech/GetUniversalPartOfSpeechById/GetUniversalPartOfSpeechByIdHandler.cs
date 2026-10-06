using MediatR;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;
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
		return Result.Success(new UniversalPartOfSpeechDto());
	}
}
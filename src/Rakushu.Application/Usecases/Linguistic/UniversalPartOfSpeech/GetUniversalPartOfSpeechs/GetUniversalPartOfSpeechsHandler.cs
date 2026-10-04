using MediatR;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

internal sealed class GetUniversalPartOfSpeechsHandler : IRequestHandler<GetUniversalPartOfSpeechsQuery, Result<IReadOnlyCollection<UniversalPartOfSpeechDto>>>
{
	private readonly IUniversalPartOfSpeechRepository _repository;

	public GetUniversalPartOfSpeechsHandler(IUniversalPartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<UniversalPartOfSpeechDto>>> Handle(GetUniversalPartOfSpeechsQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<UniversalPartOfSpeechDto>>(list.Select(UniversalPartOfSpeechDto.FromEntity).ToArray());
	}
}
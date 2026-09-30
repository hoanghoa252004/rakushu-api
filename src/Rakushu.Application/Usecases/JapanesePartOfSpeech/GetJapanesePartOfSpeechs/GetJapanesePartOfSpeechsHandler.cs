using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

internal sealed class GetJapanesePartOfSpeechsHandler : IRequestHandler<GetJapanesePartOfSpeechsQuery, Result<IReadOnlyCollection<JapanesePartOfSpeechDto>>>
{
	private readonly IJapanesePartOfSpeechRepository _repository;

	public GetJapanesePartOfSpeechsHandler(IJapanesePartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<JapanesePartOfSpeechDto>>> Handle(GetJapanesePartOfSpeechsQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<JapanesePartOfSpeechDto>>(list.Select(JapanesePartOfSpeechDto.FromEntity).ToArray());
	}
}
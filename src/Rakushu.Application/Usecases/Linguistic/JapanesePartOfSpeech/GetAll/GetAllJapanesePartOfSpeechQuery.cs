using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetAll;

public sealed record GetAllJapanesePartOfSpeechQuery() : IRequest<IEnumerable<JapanesePartOfSpeechDto>>;

public sealed class GetAllJapanesePartOfSpeechHandler : IRequestHandler<GetAllJapanesePartOfSpeechQuery, IEnumerable<JapanesePartOfSpeechDto>>
{
	private readonly IJapanesePartOfSpeechRepository _repository;

	public GetAllJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<JapanesePartOfSpeechDto>> Handle(GetAllJapanesePartOfSpeechQuery query, CancellationToken cancellationToken)
	{
		var entities = await _repository.GetAllAsync(cancellationToken);

		return entities.Select(e => new JapanesePartOfSpeechDto
		{
			Id = e.Id.Value,
			Code = e.Code,
			Name = e.Name,
			JapaneseName = e.JapaneseName,
			Description = e.Description
		}).ToList();
	}
}

using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetById;

public sealed record GetJapanesePartOfSpeechByIdQuery(Guid Id) : IRequest<JapanesePartOfSpeechDto?>;

public sealed class GetJapanesePartOfSpeechByIdHandler : IRequestHandler<GetJapanesePartOfSpeechByIdQuery, JapanesePartOfSpeechDto?>
{
	private readonly IJapanesePartOfSpeechRepository _repository;

	public GetJapanesePartOfSpeechByIdHandler(IJapanesePartOfSpeechRepository repository)
	{
		_repository = repository;
	}

	public async Task<JapanesePartOfSpeechDto?> Handle(GetJapanesePartOfSpeechByIdQuery query, CancellationToken cancellationToken)
	{
		var entity = await _repository.GetByIdAsync(JapanesePartOfSpeechId.From(query.Id), cancellationToken);

		if (entity is null)
			return null;

		return new JapanesePartOfSpeechDto
		{
			Id = entity.Id.Value,
			Code = entity.Code,
			Name = entity.Name,
			JapaneseName = entity.JapaneseName,
			Description = entity.Description
		};
	}
}

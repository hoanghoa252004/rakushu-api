using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.CreateJapanesePartOfSpeech;

internal sealed class CreateJapanesePartOfSpeechHandler : IRequestHandler<CreateJapanesePartOfSpeechCommand, Result<Guid>>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateJapanesePartOfSpeechCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var pos = Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech.Create(
				JapanesePartOfSpeechId.Create(),
				request.code,
				request.name,
				request.vietnameseName,
				request.description);

			_repository.Add(pos);
			return Result.Success(pos.Id.Value);
		}, cancellationToken);
	}
}
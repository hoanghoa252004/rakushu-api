using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech.CreateUniversalPartOfSpeech;

internal sealed class CreateUniversalPartOfSpeechHandler : IRequestHandler<CreateUniversalPartOfSpeechCommand, Result<Guid>>
{
	private readonly IUniversalPartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateUniversalPartOfSpeechHandler(IUniversalPartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateUniversalPartOfSpeechCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var pos = Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech.Create(
				UniversalPartOfSpeechId.Create(),
				request.code,
				request.name,
				request.vietnameseName,
				request.description);

			_repository.Add(pos);
			return Result.Success(pos.Id.Value);
		}, cancellationToken);
	}
}
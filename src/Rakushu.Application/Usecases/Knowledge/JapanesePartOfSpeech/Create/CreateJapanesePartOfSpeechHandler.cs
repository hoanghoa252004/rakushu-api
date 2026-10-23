using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using JapanesePartOfSpeechEntity = Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Create;

internal sealed class CreateJapanesePartOfSpeechHandler : IRequestHandler<CreateJapanesePartOfSpeechCommand, Result<Guid>>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateJapanesePartOfSpeechCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = JapanesePartOfSpeechId.Create();
			var entity = JapanesePartOfSpeechEntity.Create(
				id,
				command.Code,
				command.Name,
				command.JapaneseName,
				command.Description);

			_repository.Add(entity);
			return Result.Success(id.Value);
		});
	}
}

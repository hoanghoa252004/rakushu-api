using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

internal sealed class UpdateUniversalPartOfSpeechHandler : IRequestHandler<UpdateUniversalPartOfSpeechCommand, Result>
{
	private readonly IUniversalPartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateUniversalPartOfSpeechHandler(IUniversalPartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateUniversalPartOfSpeechCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var pos = await _repository.GetByIdAsync(UniversalPartOfSpeechId.From(request.UniversalPartOfSpeechId), cancellationToken);
			if (pos is null)
				return Result.Failure(Error.NotFound("UNIVERSAL_PART_OF_SPEECH.NOT_FOUND", "Universal part of speech not found."));

			pos.Update(request.name, request.vietnameseName, request.description);
			return Result.Success();
		}, cancellationToken);
	}
}
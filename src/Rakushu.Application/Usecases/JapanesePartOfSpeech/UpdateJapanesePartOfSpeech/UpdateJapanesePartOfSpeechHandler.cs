using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.UpdateJapanesePartOfSpeech;

internal sealed class UpdateJapanesePartOfSpeechHandler : IRequestHandler<UpdateJapanesePartOfSpeechCommand, Result>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateJapanesePartOfSpeechCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var pos = await _repository.GetByIdAsync(JapanesePartOfSpeechId.From(request.JapanesePartOfSpeechId), cancellationToken);
			if (pos is null)
				return Result.Failure(Error.NotFound("JAPANESE_PART_OF_SPEECH.NOT_FOUND", "Japanese part of speech not found."));

			pos.Update(request.name, request.vietnameseName, request.description);
			return Result.Success();
		}, cancellationToken);
	}
}
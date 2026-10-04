using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.DeleteJapanesePartOfSpeech;

internal sealed class DeleteJapanesePartOfSpeechHandler : IRequestHandler<DeleteJapanesePartOfSpeechCommand, Result>
{
	private readonly IJapanesePartOfSpeechRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteJapanesePartOfSpeechHandler(IJapanesePartOfSpeechRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteJapanesePartOfSpeechCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var pos = await _repository.GetByIdAsync(JapanesePartOfSpeechId.From(request.JapanesePartOfSpeechId), cancellationToken);
			if (pos is null)
				return Result.Failure(Error.NotFound("JAPANESE_PART_OF_SPEECH.NOT_FOUND", "Japanese part of speech not found."));

			_repository.Delete(pos);
			return Result.Success();
		}, cancellationToken);
	}
}
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.OovCandidate;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Curator.Oov.IngestOovCandidates;

internal sealed class IngestOovCandidatesHandler : IRequestHandler<IngestOovCandidatesCommand, Result<int>>
{
	// DAOs
	private readonly IOovCandidateRepository _oovRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public IngestOovCandidatesHandler(
		IOovCandidateRepository oovRepository,
		IUnitOfWork unitOfWork)
	{
		_oovRepository = oovRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<int>> Handle(
		IngestOovCandidatesCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Ingest candidates
			int count = 0;
			foreach (var item in request.Items)
			{
				if (string.IsNullOrWhiteSpace(item.Term)) continue;

				var existing = await _oovRepository.GetByTermAsync(item.Term.Trim(), cancellationToken);
				if (existing is not null) continue;

				var candResult = OovCandidate.Create(
					term: item.Term,
					confidenceScore: item.ConfidenceScore,
					tokenId: item.TokenId,
					tentativeReading: item.TentativeReading,
					tentativePos: item.TentativePos,
					suggestedMeaning: item.SuggestedMeaning,
					contextSnippet: item.ContextSnippet
				);

				if (candResult.IsSuccess)
				{
					_oovRepository.Add(candResult.Value);
					count++;
				}
			}

			return Result.Success(count);
		}, cancellationToken);
	}
}

using MediatR;
using Rakushu.Application.Abstractions.Infrastructure;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.CuratorReview;
using Rakushu.Domain.Entities.DictionaryEntry;
using Rakushu.Domain.Entities.OovCandidate;
using Rakushu.Domain.Entities.User;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Curator.Oov.ManualAddOovCandidate;

internal sealed class ManualAddOovCandidateHandler : IRequestHandler<ManualAddOovCandidateCommand, Result<Guid>>
{
	// DAOs
	private readonly IOovCandidateRepository _oovRepository;
	private readonly IDictionaryEntryRepository _dictionaryRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;
	private readonly IAiServiceClient _aiServiceClient;

	public ManualAddOovCandidateHandler(
		IOovCandidateRepository oovRepository,
		IDictionaryEntryRepository dictionaryRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		IAiServiceClient aiServiceClient)
	{
		_oovRepository = oovRepository;
		_dictionaryRepository = dictionaryRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_aiServiceClient = aiServiceClient;
	}

	public async Task<Result<Guid>> Handle(ManualAddOovCandidateCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Get Candidate
			var candId = OovCandidateId.From(request.OovCandidateId);
			var candidate = await _oovRepository.GetWithReviewAsync(candId, cancellationToken);
			if (candidate is null)
			{
				return Result.Failure<Guid>(OovCandidateErrors.NotFound);
			}

			// 2. Validate Status
			if (candidate.Status != OovStatus.PendingCuratorReview)
			{
				return Result.Failure<Guid>(OovCandidateErrors.AlreadyReviewed);
			}

			var now = _systemClock.UtcNow;
			var curatorId = UserId.From(request.CuratorId);

			// 3. Resolve Candidate & Create Curator Review
			candidate.ResolveManual(now);

			var reviewResult = CuratorReview.Create(
				candId,
				curatorId,
				CuratorDecision.ManualAdd,
				now,
				request.Term,
				request.Reading,
				request.Pos,
				request.Meaning,
				request.Comment
			);

			if (reviewResult.IsFailure)
			{
				return Result.Failure<Guid>(reviewResult.Error);
			}

			candidate.SetReview(reviewResult.Value);

			// 4. Merge into System Knowledge (PostgreSQL Dictionary)
			var existingDict = await _dictionaryRepository.GetByTermAsync(request.Term.Trim(), cancellationToken);
			if (existingDict is not null)
			{
				existingDict.Update(request.Reading, request.Pos, request.Meaning, request.Meaning, updatedAt: now);
			}
			else
			{
				var newEntryResult = DictionaryEntry.Create(
					term: request.Term,
					reading: request.Reading,
					pos: request.Pos,
					meaning: request.Meaning,
					meaningVietnamese: request.Meaning,
					definitionTags: "curator-verified",
					createdAt: now
				);
				if (newEntryResult.IsSuccess)
				{
					_dictionaryRepository.Add(newEntryResult.Value);
				}
			}

			// 5. Synchronize into SQLite dictionary on AI Service
			await _aiServiceClient.SyncDictionaryEntryAsync(
				request.Term,
				request.Reading,
				request.Pos,
				request.Meaning,
				"curator-verified",
				originalTerm: candidate.Term,
				status: "RESOLVED_MANUAL",
				cancellationToken: CancellationToken.None
			);

			return Result.Success(candidate.Id.Value);
		}, cancellationToken);
	}
}

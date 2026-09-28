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

namespace Rakushu.Application.Usecases.Curator.Oov.ReviewOovCandidate;

internal sealed class ReviewOovCandidateHandler : IRequestHandler<ReviewOovCandidateCommand, Result<Guid>>
{
	// DAOs
	private readonly IOovCandidateRepository _oovRepository;
	private readonly IDictionaryEntryRepository _dictionaryRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;
	private readonly IAiServiceClient _aiServiceClient;

	public ReviewOovCandidateHandler(
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

	public async Task<Result<Guid>> Handle(ReviewOovCandidateCommand request, CancellationToken cancellationToken)
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

			// 3. Process Decision
			if (request.Decision == CuratorDecision.Adapt)
			{
				var finalTerm = IsValidValue(request.EditedTerm) ? request.EditedTerm!.Trim() : candidate.Term;
				var finalReading = IsValidValue(request.EditedReading) ? request.EditedReading!.Trim() : candidate.TentativeReading ?? candidate.Term;
				var finalPos = IsValidValue(request.EditedPos) ? request.EditedPos!.Trim() : candidate.TentativePos ?? "NOUN";
				var finalMeaning = IsValidValue(request.EditedMeaning) ? request.EditedMeaning!.Trim() : candidate.SuggestedMeaning ?? finalTerm;

				candidate.Adapt(now);

				var reviewResult = CuratorReview.Create(
					candId,
					curatorId,
					CuratorDecision.Adapt,
					now,
					finalTerm,
					finalReading,
					finalPos,
					finalMeaning,
					IsValidValue(request.Comment) ? request.Comment : null
				);

				if (reviewResult.IsFailure)
				{
					return Result.Failure<Guid>(reviewResult.Error);
				}

				candidate.SetReview(reviewResult.Value);

				// Merge into System Knowledge (PostgreSQL Dictionary)
				var existingDict = await _dictionaryRepository.GetByTermAsync(finalTerm, cancellationToken);
				if (existingDict is not null)
				{
					existingDict.Update(finalReading, finalPos, finalMeaning, finalMeaning, updatedAt: now);
				}
				else
				{
					var newEntryResult = DictionaryEntry.Create(
						term: finalTerm,
						reading: finalReading,
						pos: finalPos,
						meaning: finalMeaning,
						meaningVietnamese: finalMeaning,
						definitionTags: "curator-verified",
						createdAt: now
					);
					if (newEntryResult.IsSuccess)
					{
						_dictionaryRepository.Add(newEntryResult.Value);
					}
				}

				// Synchronize into SQLite dictionary on AI Service
				await _aiServiceClient.SyncDictionaryEntryAsync(
					finalTerm,
					finalReading,
					finalPos,
					finalMeaning,
					"curator-verified",
					originalTerm: candidate.Term,
					status: "ADAPTED",
					cancellationToken: CancellationToken.None
				);

				return Result.Success(candidate.Id.Value);
			}
			else if (request.Decision == CuratorDecision.Reject)
			{
				candidate.Reject(now);

				var reviewResult = CuratorReview.Create(
					candId,
					curatorId,
					CuratorDecision.Reject,
					now,
					comment: IsValidValue(request.Comment) ? request.Comment : null
				);

				if (reviewResult.IsFailure)
				{
					return Result.Failure<Guid>(reviewResult.Error);
				}

				candidate.SetReview(reviewResult.Value);

				// Synchronize rejection to SQLite on AI Service
				await _aiServiceClient.SyncOovStatusAsync(
					candidate.Term,
					"REJECTED",
					candidate.Id.Value.ToString(),
					cancellationToken: CancellationToken.None
				);

				return Result.Success(candidate.Id.Value);
			}

			return Result.Failure<Guid>(CuratorReviewErrors.InvalidDecision);
		}, cancellationToken);
	}

	private static bool IsValidValue(string? val) =>
		!string.IsNullOrWhiteSpace(val) && !string.Equals(val.Trim(), "string", StringComparison.OrdinalIgnoreCase);
}

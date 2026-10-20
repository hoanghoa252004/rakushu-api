using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Nlp;
using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Nlp.KnowledgeNlpProcess;

public sealed class KnowledgeNlpProcessHandler : IRequestHandler<KnowledgeNlpProcessCommand, Result>
{
	private readonly IKnowledgeUnitRepository _knowledgeUnitRepository;
	private readonly IJapanesePartOfSpeechRepository _partOfSpeechRepository;
	private readonly IDependencyRelationshipRepository _dependencyRepository;
	private readonly INlpService _nlpService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public KnowledgeNlpProcessHandler(
		IKnowledgeUnitRepository knowledgeUnitRepository,
		IJapanesePartOfSpeechRepository partOfSpeechRepository,
		IDependencyRelationshipRepository dependencyRepository,
		INlpService nlpService,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_knowledgeUnitRepository = knowledgeUnitRepository;
		_partOfSpeechRepository = partOfSpeechRepository;
		_dependencyRepository = dependencyRepository;
		_nlpService = nlpService;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
	KnowledgeNlpProcessCommand request,
	CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			const int batchSize = 500;

			var partOfSpeeches = await _partOfSpeechRepository
				.GetAllAsync(cancellationToken);

			var dependencies = await _dependencyRepository
				.GetAllAsync(cancellationToken);

			var partOfSpeechByCode = partOfSpeeches
				.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

			var dependencyByCode = dependencies
				.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

			var page = 0;

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var knowledgeUnits = (await _knowledgeUnitRepository
					.GetPagedAsync(
						page,
						batchSize,
						cancellationToken))
					.ToList();

				if (knowledgeUnits.Count == 0)
					break;

				// Chỉ lấy những KnowledgeUnit cần NLP xử lý.
				var unitsToProcess = knowledgeUnits
					.Where(x => x.RequiresNlpProcessing())
					.ToList();

				// Không có unit nào cần xử lý thì chuyển sang trang tiếp theo.
				if (unitsToProcess.Count == 0)
				{
					page++;
					continue;
				}

				var requests = unitsToProcess
					.Select(x => new KnowledgeUnitNlpRequest
					{
						Id = x.Id.Value,
						Expression = x.Expression
					})
					.ToList();

				// Chỉ gửi những unit có hash thay đổi hoặc chưa có hash.
				var nlpResponse = await _nlpService.KnowledgeAnalyzeAsync(
					requests,
					cancellationToken);

				var resultsByUnitId = nlpResponse.Results
					.ToDictionary(x => x.Id);

				foreach (var unit in unitsToProcess)
				{
					if (!resultsByUnitId.TryGetValue(
							unit.Id.Value,
							out var unitResult))
					{
						throw new InvalidOperationException(
							$"NLP response missing unit {unit.Id}.");
					}

					unit.ClearElements();

					foreach (var elementResult in unitResult.Tokens
								 .OrderBy(x => x.Index))
					{
						if (!partOfSpeechByCode.TryGetValue(
								elementResult.Tag,
								out var partOfSpeech))
						{
							throw new InvalidOperationException(
								$"JapanesePartOfSpeech '{elementResult.Tag}' " +
								$"not found for unit {unit.Id}.");
						}

						if (!dependencyByCode.TryGetValue(
								elementResult.Dependency,
								out var dependency))
						{
							throw new InvalidOperationException(
								$"DependencyRelationship '{elementResult.Dependency}' " +
								$"not found for unit {unit.Id}.");
						}

						var startIndex = elementResult.StartIndex;
						var endIndex = startIndex + elementResult.Text.Length;

						var result = unit.AddElement(
							surface: elementResult.Text,
							lemma: elementResult.Lemma,
							reading: elementResult.Reading,
							headSequence: elementResult.Head,
							japanesePartOfSpeechId: partOfSpeech.Id,
							dependencyRelationshipId: dependency.Id,
							startIndex: startIndex,
							endIndex: endIndex,
							sequence: elementResult.Index);

						if (result.IsFailure)
						{
							throw new InvalidOperationException(
								$"Failed to create element '{elementResult.Text}' " +
								$"for KnowledgeUnit {unit.Id}: {result.Error}");
						}
					}

					// Chỉ cập nhật hash sau khi xử lý thành công.
					unit.MarkNlpProcessed();
				}

				await _unitOfWork.SaveChangesAsync(cancellationToken);

				page++;
			}

			return Result.Success();
		}, cancellationToken);
	}
}
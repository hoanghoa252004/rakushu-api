using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using BankQuestionEntity = Rakushu.Domain.Entities.Srs.BankQuestion.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GenerateDraftQuestions;

internal sealed class GenerateDraftQuestionsHandler : IRequestHandler<GenerateDraftQuestionsCommand, Result<int>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<GenerateDraftQuestionsHandler> _logger;

	public GenerateDraftQuestionsHandler(
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		IQuizContentGenerator quizContentGenerator,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork,
		ILogger<GenerateDraftQuestionsHandler> logger)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_quizContentGenerator = quizContentGenerator;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task<Result<int>> Handle(
		GenerateDraftQuestionsCommand request,
		CancellationToken cancellationToken)
	{
		int batchSize = Math.Clamp(request.BatchSize, 1, 50);

		var meanings = await _bankQuestionRepository.GetMeaningsNeedingDraftQuestionsAsync(
			batchSize,
			cancellationToken);

		if (meanings.Count == 0)
		{
			return Result.Success(0);
		}

		var questionTypeId = await _bankQuestionRepository.GetQuestionTypeIdByCodeAsync("WORD_TO_MEANING", cancellationToken)
			?? await _bankQuestionRepository.GetDefaultQuestionTypeIdAsync(cancellationToken);

		var meaningGuids = meanings.Select(m => m.Id.Value).ToList();
		var mediaContexts = await _learningUnitRepository.GetMediaContextByMeaningIdsAsync(meaningGuids, cancellationToken);

		var prompts = meanings.Select(m =>
		{
			mediaContexts.TryGetValue(m.Id.Value, out var mc);
			var sentence = mc?.OriginalSentence ?? m.KnowledgeUnit?.Expression ?? m.Meaning;
			return new GenerateQuizContentPrompt(
				m.Id.Value,
				m.KnowledgeUnit?.Expression ?? string.Empty,
				m.KnowledgeUnit?.Reading,
				m.Meaning,
				sentence);
		}).ToList();

		IReadOnlyDictionary<Guid, GeneratedQuizPromptAndExplanation> llmResults;
		try
		{
			llmResults = await _quizContentGenerator.GenerateQuizContentBatchAsync(prompts, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to call AI quiz content generator, falling back to templates.");
			llmResults = new Dictionary<Guid, GeneratedQuizPromptAndExplanation>();
		}

		var excludeIds = meanings.Select(m => m.Id);
		var distractorPool = await _bankQuestionRepository.GetRandomDistractorMeaningsAsync(
			excludeIds,
			meanings.Count * 3,
			cancellationToken);

		var distractorQueue = new Queue<string>(distractorPool);
		var now = _systemClock.UtcNow;
		int generatedCount = 0;

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			foreach (var meaning in meanings)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				// Double check if question was added concurrently
				bool alreadyExists = await _bankQuestionRepository.HasDraftOrPublishedQuestionAsync(meaning.Id, cancellationToken);
				if (alreadyExists)
					continue;

				var itemDistractors = new List<string>();
				while (itemDistractors.Count < 3 && distractorQueue.Count > 0)
				{
					var d = distractorQueue.Dequeue();
					if (d != meaning.Meaning && !itemDistractors.Contains(d))
					{
						itemDistractors.Add(d);
					}
				}

				var fallbackChoices = new[]
				{
					"Điểm thắc mắc, nghi vấn",
					"Vấn đề cần giải quyết",
					"Lịch trình dự kiến",
					"Nội dung được ghi chép",
					"Phương pháp thực hiện",
					"Khi ..., vào dịp ..."
				};

				foreach (var fb in fallbackChoices)
				{
					if (itemDistractors.Count >= 3) break;
					if (fb != meaning.Meaning && !itemDistractors.Contains(fb))
					{
						itemDistractors.Add(fb);
					}
				}

				var options = new List<(string Text, bool IsCorrect)>
				{
					(meaning.Meaning, true),
					(itemDistractors.ElementAtOrDefault(0) ?? "Ý nghĩa khác A", false),
					(itemDistractors.ElementAtOrDefault(1) ?? "Ý nghĩa khác B", false),
					(itemDistractors.ElementAtOrDefault(2) ?? "Ý nghĩa khác C", false)
				};

				var shuffled = options.OrderBy(_ => Random.Shared.Next()).ToList();
				var labels = new[] { "A", "B", "C", "D" };
				string correctChoiceId = "A";
				var choiceList = new List<object>();

				for (int i = 0; i < shuffled.Count; i++)
				{
					var label = labels[i];
					if (shuffled[i].IsCorrect)
					{
						correctChoiceId = label;
					}
					choiceList.Add(new { id = label, text = shuffled[i].Text });
				}

				var choicesJson = JsonSerializer.Serialize(choiceList);

				var expression = meaning.KnowledgeUnit?.Expression ?? string.Empty;
				var reading = meaning.KnowledgeUnit?.Reading;

				string promptText;
				string? explanationText;

				if (llmResults.TryGetValue(meaning.Id.Value, out var gen) &&
					!string.IsNullOrWhiteSpace(gen.Prompt))
				{
					promptText = gen.Prompt;
					explanationText = gen.Explanation;
				}
				else
				{
					promptText = $"Từ/ngữ pháp 「{expression}」 có ý nghĩa là gì?";
					explanationText = $"「{expression}」{(string.IsNullOrWhiteSpace(reading) ? "" : $"({reading})")} có nghĩa là: {meaning.Meaning}.";
				}

				mediaContexts.TryGetValue(meaning.Id.Value, out var mediaContext);

				string mediaType = "NONE";
				string? mediaUrl = null;
				string? mediaMetadata = null;

				if (mediaContext is not null)
				{
					mediaType = "VIDEO";
					mediaUrl = mediaContext.StorageKey ?? mediaContext.SourceUrl;

					var metadataObj = new
					{
						startTime = mediaContext.StartTime.TotalSeconds,
						endTime = mediaContext.EndTime.TotalSeconds,
						segmentId = mediaContext.TranscriptSegmentId?.ToString(),
						targetWord = expression,
						reading = reading,
						fullSentence = mediaContext.OriginalSentence,
						vietnameseSub = mediaContext.TranslatedSentence
					};
					mediaMetadata = JsonSerializer.Serialize(metadataObj);
				}

				var draftResult = BankQuestionEntity.CreateDraftFromAi(
					meaning.Id,
					questionTypeId,
					promptText,
					choicesJson,
					correctChoiceId,
					now,
					explanationText,
					mediaType: mediaType,
					mediaUrl: mediaUrl,
					mediaMetadata: mediaMetadata);

				if (draftResult.IsSuccess)
				{
					_bankQuestionRepository.Add(draftResult.Value);
					generatedCount++;
				}
				else
				{
					_logger.LogWarning("Failed to create draft question for meaning {MeaningId}: {Error}",
						meaning.Id.Value, draftResult.Error);
				}
			}

			return Result.Success(generatedCount);
		}, cancellationToken);
	}
}

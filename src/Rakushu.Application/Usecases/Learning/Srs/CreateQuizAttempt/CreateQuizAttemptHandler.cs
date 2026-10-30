using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.SavedItem;

using BankQuestionEntity = Rakushu.Domain.Entities.Srs.BankQuestion.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttemptHandler : IRequestHandler<CreateQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly IQuizRepository _quizRepository;
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateQuizAttemptHandler(
		ISavedItemRepository savedItemRepository,
		IBankQuestionRepository bankQuestionRepository,
		IQuizRepository quizRepository,
		IQuizAttemptRepository quizAttemptRepository,
		IQuizContentGenerator quizContentGenerator,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_quizRepository = quizRepository;
		_quizAttemptRepository = quizAttemptRepository;
		_quizContentGenerator = quizContentGenerator;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		CreateQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptStartedResponse>(UserErrors.NotFound);

		if (request.ItemIds is null || request.ItemIds.Count == 0)
			return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.EmptySelectedItems);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var activeAttempt = await _quizAttemptRepository.GetActiveAttemptForUserAsync(userId, cancellationToken);
			if (activeAttempt is not null)
			{
				if (activeAttempt.IsExpired(now, TimeSpan.FromHours(1)))
				{
					activeAttempt.Abandon(now);
				}
				else
				{
					return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.ActiveAttemptInProgress);
				}
			}

			var ids = request.ItemIds.Select(SavedItemId.From).Distinct().ToList();
			var items = await _savedItemRepository.GetByIdsWithScheduleAsync(ids, userId, cancellationToken);

			if (items.Count == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var meaningIds = items.Select(i => i.KnowledgeMeaningId).Distinct().ToList();
			var bankQuestions = (await _bankQuestionRepository.GetByMeaningIdsAsync(meaningIds, cancellationToken)).ToList();

			// Fallback: Generate BankQuestions for items that do not have matching bank questions
			var itemsNeedingQuestions = items
				.Where(item => !bankQuestions.Any(bq => bq.KnowledgeMeaningId == item.KnowledgeMeaningId))
				.GroupBy(item => item.KnowledgeMeaningId)
				.Select(g => g.First())
				.ToList();

			if (itemsNeedingQuestions.Count > 0)
			{
				var llmPrompts = itemsNeedingQuestions.Select(item => new GenerateQuizContentPrompt(
					item.KnowledgeMeaningId.Value,
					item.Expression,
					item.Reading,
					item.Meaning,
					item.TranscriptSegment?.Text)).ToList();

				var llmResults = await _quizContentGenerator.GenerateQuizContentBatchAsync(llmPrompts, cancellationToken);

				var excludeIds = items.Select(i => i.KnowledgeMeaningId);
				var distractorPool = await _bankQuestionRepository.GetRandomDistractorMeaningsAsync(
					excludeIds,
					itemsNeedingQuestions.Count * 3,
					cancellationToken);

				var distractorQueue = new Queue<string>(distractorPool);
				var defaultQuestionTypeId = QuestionTypeId.From(Guid.Parse("10000000-0000-0000-0000-000000000001")); // MULTIPLE_CHOICE

				foreach (var item in itemsNeedingQuestions)
				{
					var itemDistractors = new List<string>();
					while (itemDistractors.Count < 3 && distractorQueue.Count > 0)
					{
						var d = distractorQueue.Dequeue();
						if (d != item.Meaning && !itemDistractors.Contains(d))
						{
							itemDistractors.Add(d);
						}
					}

					var fallbackChoices = new[] { "Điểm thắc mắc, nghi vấn", "Vấn đề cần giải quyết", "Lịch trình dự kiến", "Nội dung được ghi chép" };
					foreach (var fb in fallbackChoices)
					{
						if (itemDistractors.Count >= 3) break;
						if (fb != item.Meaning && !itemDistractors.Contains(fb))
						{
							itemDistractors.Add(fb);
						}
					}

					var options = new List<(string Text, bool IsCorrect)>
					{
						(item.Meaning, true),
						(itemDistractors[0], false),
						(itemDistractors[1], false),
						(itemDistractors[2], false)
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

					string promptText;
					string? explanationText;

					if (llmResults.TryGetValue(item.KnowledgeMeaningId.Value, out var gen) &&
						!string.IsNullOrWhiteSpace(gen.Prompt))
					{
						promptText = gen.Prompt;
						explanationText = gen.Explanation;
					}
					else
					{
						promptText = $"Từ/ngữ pháp 「{item.Expression}」 có ý nghĩa là gì?";
						explanationText = $"「{item.Expression}」{(string.IsNullOrWhiteSpace(item.Reading) ? "" : $"({item.Reading})")} có nghĩa là: {item.Meaning}.";
					}

					var newBankQResult = BankQuestionEntity.Create(
						item.KnowledgeMeaningId,
						defaultQuestionTypeId,
						promptText,
						choicesJson,
						correctChoiceId,
						now,
						explanationText,
						difficulty: "Medium",
						isActive: true);

					if (newBankQResult.IsSuccess)
					{
						var newBankQ = newBankQResult.Value;
						_bankQuestionRepository.Add(newBankQ);
						bankQuestions.Add(newBankQ);
					}
				}
			}

			if (bankQuestions.Count == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var quizResult = Quiz.Create(userId, now);
			if (quizResult.IsFailure)
				return Result.Failure<QuizAttemptStartedResponse>(quizResult.Error);

			var quiz = quizResult.Value;
			var clientQuestions = new List<QuizQuestionDto>();
			int position = 1;

			foreach (var item in items)
			{
				var matchingQuestions = bankQuestions
					.Where(bq => bq.KnowledgeMeaningId == item.KnowledgeMeaningId)
					.ToList();

				if (matchingQuestions.Count == 0)
					continue;

				// Pick first or random matching question
				var bankQ = matchingQuestions[Random.Shared.Next(matchingQuestions.Count)];

				var questionResult = Question.Create(
					item.Id,
					bankQ.QuestionTypeId,
					position,
					bankQ.Prompt,
					item.Reading,
					bankQ.ChoicesJson,
					bankQ.CorrectChoiceId,
					now,
					bankQ.Id);

				if (questionResult.IsFailure)
					continue;

				var question = questionResult.Value;
				quiz.AddQuestion(question);

				var choices = QuizOptionParser.Parse(question.ChoicesJson);
				clientQuestions.Add(new QuizQuestionDto(
					question.Id.Value,
					position,
					question.Prompt,
					question.Reading,
					choices));

				position++;
			}

			if (quiz.TotalQuestions == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var attemptResult = QuizAttempt.Create(userId, quiz.Id, now, QuizAttemptMode.SrsReview);
			if (attemptResult.IsFailure)
				return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);

			var attempt = attemptResult.Value;

			_quizRepository.Add(quiz);
			_quizAttemptRepository.Add(attempt);

			return Result.Success(new QuizAttemptStartedResponse(
				attempt.Id.Value,
				quiz.Id.Value,
				attempt.Mode.ToString(),
				quiz.TotalQuestions,
				attempt.StartedAt,
				attempt.StartedAt.AddHours(1),
				clientQuestions));
		}, cancellationToken);
	}
}

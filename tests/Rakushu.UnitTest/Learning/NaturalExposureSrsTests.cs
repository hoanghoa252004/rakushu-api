using System.Linq.Expressions;
using System.Reflection;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemSchedule;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.DeleteVideoWatchHistory;
using Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.QuestionType;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

namespace Rakushu.UnitTest.Learning;

public sealed class NaturalExposureSrsTests
{
	private readonly UserId _userId = UserId.Create();
	private readonly LearningFakeSystemClock _clock = new() { UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero) };
	private readonly LearningFakeCurrentUserContext _userContext;
	private readonly FlushingUnitOfWork _uow;
	private readonly InMemoryVideoRepository _videoRepo = new();
	private readonly InMemoryTrackingTranscriptRepository _transcriptRepo = new();
	private readonly InMemoryLearnerVideoProgressRepository _progressRepo = new();
	private readonly InMemoryLearnerEncounteredLearningUnitRepository _encounterRepo = new();
	private readonly InMemoryLearnerEncounterEventRepository _encounterEventRepo = new();
	private readonly InMemorySavedItemRepository _savedItemRepo = new();

	public NaturalExposureSrsTests()
	{
		_userContext = new LearningFakeCurrentUserContext { UserId = _userId, Role = "Learner" };
		_uow = new FlushingUnitOfWork(_encounterEventRepo.Flush);
	}

	private RecordWatchHeartbeatHandler CreateHeartbeatHandler() => new(
		_videoRepo,
		_transcriptRepo,
		_progressRepo,
		_encounterRepo,
		_encounterEventRepo,
		_savedItemRepo,
		_userContext,
		_clock,
		_uow);

	private GetSrsItemsHandler CreateGetSrsItemsHandler() => new(
		_savedItemRepo,
		_encounterEventRepo,
		_userContext,
		_clock);

	private DeleteVideoWatchHistoryHandler CreateDeleteHistoryHandler() => new(
		_progressRepo,
		_encounterRepo,
		_encounterEventRepo,
		_userContext,
		_uow);

	private KnowledgeMeaning BuildMeaning(string expression, string reading, string meaningText)
	{
		var unit = KnowledgeUnit.Create(
			KnowledgeCollectionId.Create(),
			ProficiencyLevelId.Create(),
			KnowledgeUnitType.Word,
			expression,
			reading,
			KnowledgeUnitSourceType.Defined,
			isActive: true,
			_clock.UtcNow).Value;

		var meaning = KnowledgeMeaning.Create(unit.Id, 1, meaningText, expression).Value;
		typeof(KnowledgeMeaning).GetProperty(nameof(KnowledgeMeaning.KnowledgeUnit))?.SetValue(meaning, unit);
		return meaning;
	}

	private (Video Video, TranscriptSegment Segment, LearningUnit Unit) BuildVideoContext(
		KnowledgeMeaning meaning,
		string slug,
		string sentence,
		double startSec = 10,
		double endSec = 15)
	{
		var video = Video.Create(
			slug,
			$"Video {slug}",
			TimeSpan.FromSeconds(120),
			ContentCategoryId.Create(),
			null,
			1,
			VideoSource.Curated,
			VideoStatus.Ready,
			UserId.Create(),
			_clock.UtcNow.AddDays(-5),
			_clock.UtcNow.AddDays(-4)).Value;

		var transcript = Transcript.Create(video.Id, sentence, _clock.UtcNow.AddDays(-5), _clock.UtcNow.AddDays(-4)).Value;
		var segment = transcript.AddSegment(
			null,
			sentence,
			TimeSpan.FromSeconds(startSec),
			TimeSpan.FromSeconds(endSec),
			1,
			_clock.UtcNow.AddDays(-4)).Value;
		typeof(TranscriptSegment).GetProperty(nameof(TranscriptSegment.Transcript))?.SetValue(segment, transcript);

		var unit = LearningUnit.Create(
			LearningUnitId.Create(),
			meaning,
			segment.Id,
			0,
			Math.Max(1, meaning.KnowledgeUnit.Expression.Length),
			1.0,
			isReviewed: true,
			MatchingMethod.KnowledgeMatching);
		typeof(LearningUnit).GetProperty(nameof(LearningUnit.KnowledgeMeaning))?.SetValue(unit, meaning);
		typeof(LearningUnit).GetProperty(nameof(LearningUnit.TranscriptSegment))?.SetValue(unit, segment);

		var field = typeof(TranscriptSegment).GetField("_learningUnits", BindingFlags.NonPublic | BindingFlags.Instance)!;
		((List<LearningUnit>)field.GetValue(segment)!).Add(unit);

		_videoRepo.Items.Add(video);
		_transcriptRepo.AddVideoSegment(video.Id, segment);
		return (video, segment, unit);
	}

	private SavedItem SeedSavedItem(
		KnowledgeMeaning meaning,
		DateTimeOffset dueAt,
		int? initialIntervalDays = 3,
		DateTimeOffset? lastReviewedAt = null)
	{
		var saved = SavedItem.Create(_userId, meaning.Id, _clock.UtcNow.AddDays(-10)).Value;
		typeof(SavedItem).GetProperty(nameof(SavedItem.KnowledgeMeaning))?.SetValue(saved, meaning);

		var schedule = SrsSchedule.Create(saved.Id, _clock.UtcNow.AddDays(-10)).Value;
		schedule.UpdateSchedule(dueAt, lastReviewedAt ?? _clock.UtcNow.AddDays(-5));

		if (initialIntervalDays.HasValue)
		{
			var detail = SrsScheduleDetail.Create(
				schedule.Id,
				reviewNumber: 1,
				quality: 4,
				intervalDays: initialIntervalDays.Value,
				easinessFactor: 2.5,
				repetitions: 2,
				lapses: 0,
				reviewedAt: lastReviewedAt ?? _clock.UtcNow.AddDays(-5)).Value;
			schedule.AddDetail(detail);
		}

		saved.AttachSchedule(schedule);
		_savedItemRepo.Items.Add(saved);
		return saved;
	}

	private Task<Result<RecordWatchHeartbeatResponseDto>> WatchSegmentAsync(
		Video video,
		double startSec = 10,
		double endSec = 15)
	{
		return CreateHeartbeatHandler().Handle(
			new RecordWatchHeartbeatCommand(
				video.Id.Value,
				CurrentPositionSeconds: endSec,
				WatchedRange: new WatchedRangeInput(startSec, endSec),
				PlaybackRate: 1.0,
				VideoDurationSeconds: 120),
			CancellationToken.None);
	}

	[Fact]
	public async Task Heartbeat_WatermarkPreventsDoubleDipping_AndDueTodayBonusDoesNotInflateIntervalDays()
	{
		var meaning = BuildMeaning("担当", "たんとう", "phụ trách");
		var (v1, _, _) = BuildVideoContext(meaning, "v1", "担当しております。");
		var (v2, _, _) = BuildVideoContext(meaning, "v2", "私が担当です。");
		var (v3, _, _) = BuildVideoContext(meaning, "v3", "担当者に連絡します。");
		var (v4, _, _) = BuildVideoContext(meaning, "v4", "担当が変わりました。");
		var (v5, _, _) = BuildVideoContext(meaning, "v5", "担当部署はこちらです。");
		var (v6, _, _) = BuildVideoContext(meaning, "v6", "新しい担当です。");

		// Due on Nov 1 -> Overdue on Nov 2..4; IntervalDays = 3
		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.Zero), initialIntervalDays: 3);

		// Nov 2, Nov 3 -> 1st & 2nd encounter (< 3, no shift)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		var r1 = await WatchSegmentAsync(v1);
		Assert.True(r1.IsSuccess);
		Assert.Equal(0, r1.Value.SrsAdjustedItemsCount);

		_clock.UtcNow = new DateTimeOffset(2026, 11, 3, 10, 0, 0, TimeSpan.Zero);
		var r2 = await WatchSegmentAsync(v2);
		Assert.Equal(0, r2.Value.SrsAdjustedItemsCount);

		// Nov 4 -> 3rd encounter -> OverdueRescue (+2 days => Nov 6)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 4, 10, 0, 0, TimeSpan.Zero);
		var r3 = await WatchSegmentAsync(v3);
		Assert.Equal(1, r3.Value.SrsAdjustedItemsCount);
		Assert.Equal(SrsImmersionAdjustmentTypes.OverdueRescue, r3.Value.SrsAdjustedItems[0].AdjustmentType);
		Assert.Equal(new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero), saved.Schedule!.DueAt);
		Assert.Equal(1, saved.Schedule.ConsecutivePassiveBoosts);
		Assert.Equal(new DateTimeOffset(2026, 11, 4, 10, 0, 0, TimeSpan.Zero), saved.Schedule.LastImmersionBoostAt);

		// Nov 6 (new due date, STILL within 7 days of Nov 2..4):
		// Watch ONLY 1 new video -> old encounters from Nov 2..4 are <= LastImmersionBoostAt, so MUST NOT trigger!
		_clock.UtcNow = new DateTimeOffset(2026, 11, 6, 9, 0, 0, TimeSpan.Zero);
		var r4 = await WatchSegmentAsync(v4);
		Assert.Equal(0, r4.Value.SrsAdjustedItemsCount);
		Assert.Equal(new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero), saved.Schedule.DueAt);

		// Watch 2nd new video after watermark -> still only 2 new events (< 3)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero);
		var r5 = await WatchSegmentAsync(v5);
		Assert.Equal(0, r5.Value.SrsAdjustedItemsCount);

		// Watch 3rd new video after watermark -> Triggers DueTodayBonus (+4 days => Nov 10)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 6, 11, 0, 0, TimeSpan.Zero);
		var r6 = await WatchSegmentAsync(v6);
		Assert.Equal(1, r6.Value.SrsAdjustedItemsCount);
		Assert.Equal(SrsImmersionAdjustmentTypes.DueTodayBonus, r6.Value.SrsAdjustedItems[0].AdjustmentType);
		Assert.Equal(new DateTimeOffset(2026, 11, 10, 11, 0, 0, TimeSpan.Zero), saved.Schedule.DueAt);
		Assert.Equal(2, saved.Schedule.ConsecutivePassiveBoosts);

		// Critical: Active-recall IntervalDays stays 3 (never inflated toward mastered=21 by passive listening)
		Assert.Equal(3, saved.Schedule.Details.OrderByDescending(d => d.ReviewNumber).First().IntervalDays);
	}

	[Fact]
	public async Task Heartbeat_MaxConsecutivePassiveBoostsBlocksThirdSnooze_UntilActiveReviewResetsCounter()
	{
		var meaning = BuildMeaning("確認", "かくにん", "xác nhận");
		var videos = Enumerable.Range(1, 12)
			.Select(i => BuildVideoContext(meaning, $"cap-v{i}", $"確認します {i}").Video)
			.ToList();

		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero), initialIntervalDays: 3);

		// Boost #1 on Nov 2 (DueTodayBonus -> Nov 6)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(videos[0]);
		await WatchSegmentAsync(videos[1]);
		var boost1 = await WatchSegmentAsync(videos[2]);
		Assert.Equal(1, boost1.Value.SrsAdjustedItemsCount);
		Assert.Equal(1, saved.Schedule!.ConsecutivePassiveBoosts);

		// Boost #2 on Nov 6 (DueTodayBonus -> Nov 10)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(videos[3]);
		await WatchSegmentAsync(videos[4]);
		var boost2 = await WatchSegmentAsync(videos[5]);
		Assert.Equal(1, boost2.Value.SrsAdjustedItemsCount);
		Assert.Equal(2, saved.Schedule.ConsecutivePassiveBoosts);

		// Attempt #3 on Nov 10 -> Blocked by MaxConsecutivePassiveBoosts = 2!
		_clock.UtcNow = new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(videos[6]);
		await WatchSegmentAsync(videos[7]);
		var blockedBoost3 = await WatchSegmentAsync(videos[8]);
		Assert.Equal(0, blockedBoost3.Value.SrsAdjustedItemsCount);
		Assert.Equal(new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero), saved.Schedule.DueAt);
		Assert.Equal(2, saved.Schedule.ConsecutivePassiveBoosts);

		// Active recall via FlashcardSrsService resets ConsecutivePassiveBoosts to 0
		var flashcardService = new FlashcardSrsService(_savedItemRepo);
		var session = FlashcardSession.Create(_userId, _clock.UtcNow).Value;
		var sessionItem = FlashcardSessionItem.Create(session.Id, saved.Id, FlashcardRating.Easy, _clock.UtcNow).Value;
		session.AddItem(sessionItem);
		await flashcardService.ApplySrsUpdatesAsync(session, _clock.UtcNow);

		Assert.Equal(0, saved.Schedule.ConsecutivePassiveBoosts);

		// When the new active-review due date arrives, passive boost works again with 3 fresh encounters
		_clock.UtcNow = saved.Schedule.DueAt;
		await WatchSegmentAsync(videos[9]);
		await WatchSegmentAsync(videos[10]);
		var boostAfterReview = await WatchSegmentAsync(videos[11]);
		Assert.Equal(1, boostAfterReview.Value.SrsAdjustedItemsCount);
		Assert.Equal(1, saved.Schedule.ConsecutivePassiveBoosts);
	}

	[Fact]
	public async Task Heartbeat_ReplayingOldSentenceAfter30Days_DoesNotReviveHistoricalEncounters()
	{
		var meaning = BuildMeaning("営業", "えいぎょう", "kinh doanh");
		var (video, _, _) = BuildVideoContext(meaning, "old-replay", "営業部で働いています。");

		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 12, 1, 10, 0, 0, TimeSpan.Zero));

		// Nov 2: replay same sentence 3 times across >30 min cooldowns -> lifetime EncounterCount = 3
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(video);
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 11, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(video);
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 12, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(video);

		Assert.Equal(3, _encounterRepo.Items.Single().EncounterCount);
		Assert.Equal(3, _encounterEventRepo.Items.Count);

		// Dec 2 (30 days later, overdue by 1 day): replay the EXACT same sentence ONCE
		_clock.UtcNow = new DateTimeOffset(2026, 12, 2, 10, 0, 0, TimeSpan.Zero);
		var replayResult = await WatchSegmentAsync(video);

		// Lifetime counter is 4, but 7-day rolling event count is strictly 1 (< 3) -> NO OverdueRescue!
		Assert.Equal(4, _encounterRepo.Items.Single().EncounterCount);
		Assert.Equal(0, replayResult.Value.SrsAdjustedItemsCount);
		Assert.Equal(new DateTimeOffset(2026, 12, 1, 10, 0, 0, TimeSpan.Zero), saved.Schedule!.DueAt);

		// And GET /api/srs/items badge shows exact 7-day count = 1 (NOT 4)
		var itemsResult = await CreateGetSrsItemsHandler().Handle(new GetSrsItemsQuery(), CancellationToken.None);
		Assert.True(itemsResult.IsSuccess);
		Assert.Equal(1, itemsResult.Value.Single().RecentEncounterCount);
	}

	[Fact]
	public async Task Heartbeat_PreReviewEncountersDoNotCountAfterFailedActiveReview()
	{
		var meaning = BuildMeaning("準備", "じゅんび", "chuẩn bị");
		var (v1, _, _) = BuildVideoContext(meaning, "pre1", "準備ができました。");
		var (v2, _, _) = BuildVideoContext(meaning, "pre2", "会議の準備をします。");
		var (v3, _, _) = BuildVideoContext(meaning, "post1", "旅行の準備中です。");

		// Saved & initially due Nov 5
		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 11, 5, 10, 0, 0, TimeSpan.Zero));

		// Hear twice BEFORE active review (Nov 2, Nov 3)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(v1);
		_clock.UtcNow = new DateTimeOffset(2026, 11, 3, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(v2);

		// On Nov 4, learner FAILS active review -> next due Nov 5, ReviewedAt = Nov 4
		_clock.UtcNow = new DateTimeOffset(2026, 11, 4, 10, 0, 0, TimeSpan.Zero);
		var failDetail = SrsScheduleDetail.Create(
			saved.Schedule!.Id,
			reviewNumber: 2,
			quality: 1,
			intervalDays: 1,
			easinessFactor: 2.3,
			repetitions: 0,
			lapses: 1,
			reviewedAt: _clock.UtcNow).Value;
		saved.Schedule.AddDetail(failDetail);
		saved.Schedule.RecordActiveReview(new DateTimeOffset(2026, 11, 5, 10, 0, 0, TimeSpan.Zero), _clock.UtcNow);

		// On Nov 5, learner hears word ONCE -> pre-review events (Nov 2, 3) are excluded -> total = 1 < 3 -> NO boost!
		_clock.UtcNow = new DateTimeOffset(2026, 11, 5, 10, 0, 0, TimeSpan.Zero);
		var res = await WatchSegmentAsync(v3);
		Assert.Equal(0, res.Value.SrsAdjustedItemsCount);
		Assert.Equal(new DateTimeOffset(2026, 11, 5, 10, 0, 0, TimeSpan.Zero), saved.Schedule.DueAt);
	}

	[Fact]
	public async Task Heartbeat_CooldownAndNotYetDueAndSameDayGuard_AreEnforced()
	{
		var meaning = BuildMeaning("連絡", "れんらく", "liên lạc");
		var (v1, _, _) = BuildVideoContext(meaning, "cd1", "連絡してください。");
		var (v2, _, _) = BuildVideoContext(meaning, "cd2", "後で連絡します。");
		var (v3, _, _) = BuildVideoContext(meaning, "cd3", "連絡先を教えてください。");

		// Not yet due (Nov 10)
		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero));

		// Replay same sentence within 10 minutes -> only 1 event recorded (cooldown = 30 min)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(v1);
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 10, 0, TimeSpan.Zero);
		await WatchSegmentAsync(v1);
		Assert.Single(_encounterEventRepo.Items);
		Assert.Equal(2, _encounterRepo.Items.Single().RawPlaybackCount);
		Assert.Equal(1, _encounterRepo.Items.Single().EncounterCount);

		// Add 2 more distinct encounters while still Not Yet Due -> DueAt MUST stay Nov 10
		await WatchSegmentAsync(v2);
		var notDueRes = await WatchSegmentAsync(v3);
		Assert.Equal(3, _encounterEventRepo.Items.Count);
		Assert.Equal(0, notDueRes.Value.SrsAdjustedItemsCount);
		Assert.Equal(new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero), saved.Schedule!.DueAt);
	}

	[Fact]
	public async Task CreateQuizAttempt_OrdersByPriorityScore_WithCappedImmersionBonus()
	{
		_clock.UtcNow = new DateTimeOffset(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);

		var meaningA = BuildMeaning("単語A", "たんごA", "Word A - overdue 5d, 10 encounters");
		var meaningB = BuildMeaning("単語B", "たんごB", "Word B - overdue 1d, 0 encounters");
		var meaningC = BuildMeaning("単語C", "たんごC", "Word C - overdue 4d, 1 encounter");
		var meaningD = BuildMeaning("単語D", "たんごD", "Word D - not due, 3 encounters");

		var itemA = SeedSavedItem(meaningA, _clock.UtcNow.AddDays(-5)); // Urgency 10.0 - Capped(5)*1.5(7.5) = +2.5
		var itemB = SeedSavedItem(meaningB, _clock.UtcNow.AddDays(-1)); // Urgency 2.0  - 0 = +2.0
		var itemC = SeedSavedItem(meaningC, _clock.UtcNow.AddDays(-4)); // Urgency 8.0  - 1*1.5 = +6.5
		var itemD = SeedSavedItem(meaningD, _clock.UtcNow.AddDays(+3)); // Urgency 0.0  - 3*1.5 = -4.5

		// Seed recent 7-day events: A=10, B=0, C=1, D=3 (plus 1 expired event 10 days ago for B which must be ignored)
		for (int i = 0; i < 10; i++)
		{
			_encounterEventRepo.Add(LearnerEncounterEvent.Create(
				_userId, LearningUnitId.Create(), meaningA.Id, VideoId.Create(), _clock.UtcNow.AddHours(-(i + 1))).Value);
		}
		_encounterEventRepo.Add(LearnerEncounterEvent.Create(
			_userId, LearningUnitId.Create(), meaningB.Id, VideoId.Create(), _clock.UtcNow.AddDays(-10)).Value);
		_encounterEventRepo.Add(LearnerEncounterEvent.Create(
			_userId, LearningUnitId.Create(), meaningC.Id, VideoId.Create(), _clock.UtcNow.AddDays(-1)).Value);
		for (int i = 0; i < 3; i++)
		{
			_encounterEventRepo.Add(LearnerEncounterEvent.Create(
				_userId, LearningUnitId.Create(), meaningD.Id, VideoId.Create(), _clock.UtcNow.AddDays(-2)).Value);
		}

		var qt = QuestionType.Create("WORD_TO_MEANING", "Word to Meaning", null, _clock.UtcNow).Value;
		var bankQuestionRepo = new InMemoryQuizBankQuestionRepository();
		foreach (var m in new[] { meaningA, meaningB, meaningC, meaningD })
		{
			var bq = BankQuestion.Create(
				m.Id,
				qt.Id,
				$"Question for {m.KnowledgeUnit.Expression}",
				"[{\"id\":\"A\",\"text\":\"OK\"},{\"id\":\"B\",\"text\":\"NO\"}]",
				"A",
				_clock.UtcNow).Value;
			typeof(BankQuestion).GetProperty(nameof(BankQuestion.QuestionType))?.SetValue(bq, qt);
			bankQuestionRepo.Items.Add(bq);
		}

		var quizRepo = new InMemoryQuizRepository();
		var attemptRepo = new InMemoryQuizAttemptRepository();
		var handler = new CreateQuizAttemptHandler(
			_savedItemRepo,
			bankQuestionRepo,
			new LearningNullLearningUnitRepository(),
			_encounterEventRepo,
			quizRepo,
			attemptRepo,
			new NullQuizContentGenerator(),
			new QuizAttemptSrsService(_savedItemRepo),
			_userContext,
			_clock,
			_uow);

		var result = await handler.Handle(
			new CreateQuizAttemptCommand([itemD.Id.Value, itemB.Id.Value, itemA.Id.Value, itemC.Id.Value]),
			CancellationToken.None);

		Assert.True(result.IsSuccess);
		var orderedSavedItemIds = quizRepo.Items.Single().Questions
			.OrderBy(q => q.Sequence)
			.Select(q => q.SavedItemId)
			.ToList();

		// Expected order: C (+6.5) -> A (+2.5, capped!) -> B (+2.0) -> D (-4.5)
		Assert.Equal([itemC.Id, itemA.Id, itemB.Id, itemD.Id], orderedSavedItemIds);
	}

	[Fact]
	public async Task DeleteVideoWatchHistory_DeletesSummaryAndEventsOnlyForTargetVideo()
	{
		var meaning = BuildMeaning("会議", "かいぎ", "cuộc họp");
		var (v1, _, _) = BuildVideoContext(meaning, "del-v1", "会議を始めます。");
		var (v2, _, _) = BuildVideoContext(meaning, "del-v2", "明日は会議があります。");

		await WatchSegmentAsync(v1);
		await WatchSegmentAsync(v2);

		Assert.Equal(2, _encounterRepo.Items.Count);
		Assert.Equal(2, _encounterEventRepo.Items.Count);

		var deleteResult = await CreateDeleteHistoryHandler().Handle(
			new DeleteVideoWatchHistoryCommand(v1.Id.Value),
			CancellationToken.None);

		Assert.True(deleteResult.IsSuccess);
		Assert.Single(_encounterRepo.Items);
		Assert.Equal(v2.Id, _encounterRepo.Items.Single().VideoId);
		Assert.Single(_encounterEventRepo.Items);
		Assert.Equal(v2.Id, _encounterEventRepo.Items.Single().VideoId);
	}

	[Fact]
	public async Task GetSavedItemSchedule_ReturnsPassiveBoostMetadata_AndResetsAfterActiveReview()
	{
		var meaning = BuildMeaning("日程", "にってい", "lịch trình");
		var videos = Enumerable.Range(1, 3)
			.Select(i => BuildVideoContext(meaning, $"sched-v{i}", $"日程を確認します {i}").Video)
			.ToList();

		var saved = SeedSavedItem(meaning, new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero), initialIntervalDays: 3);
		var scheduleHandler = new GetSavedItemScheduleHandler(_savedItemRepo, _userContext, _clock);

		// 1. Before any passive boost
		var initial = await scheduleHandler.Handle(new GetSavedItemScheduleQuery(saved.Id.Value), CancellationToken.None);
		Assert.True(initial.IsSuccess);
		Assert.Null(initial.Value.LastImmersionBoostAt);
		Assert.Equal(0, initial.Value.ConsecutivePassiveBoosts);
		Assert.Equal(SrsSchedule.MaxConsecutivePassiveBoosts, initial.Value.MaxConsecutivePassiveBoosts);

		// 2. After Boost #1 (DueTodayBonus -> +4 days, IntervalDays untouched)
		_clock.UtcNow = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		await WatchSegmentAsync(videos[0]);
		await WatchSegmentAsync(videos[1]);
		await WatchSegmentAsync(videos[2]);

		var afterBoost = await scheduleHandler.Handle(new GetSavedItemScheduleQuery(saved.Id.Value), CancellationToken.None);
		Assert.True(afterBoost.IsSuccess);
		Assert.Equal(_clock.UtcNow, afterBoost.Value.LastImmersionBoostAt);
		Assert.Equal(1, afterBoost.Value.ConsecutivePassiveBoosts);
		Assert.Equal(2, afterBoost.Value.MaxConsecutivePassiveBoosts);
		Assert.Equal(3, afterBoost.Value.CurrentIntervalDays);
		Assert.NotNull(afterBoost.Value.BoostHistory);
		var boostLog = Assert.Single(afterBoost.Value.BoostHistory!);
		Assert.Equal(SrsImmersionAdjustmentTypes.DueTodayBonus, boostLog.AdjustmentType);
		Assert.Equal(4, boostLog.BonusDays);
		Assert.Equal(1, boostLog.BoostNumber);
		Assert.Equal(3, boostLog.EncounterCountInWindow);
		Assert.Equal(new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero), boostLog.PreviousDueAt);
		Assert.Equal(new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero), boostLog.NewDueAt);

		// 3. After active Flashcard review -> ConsecutivePassiveBoosts resets to 0, while historical BoostLogs stay intact!
		var flashcardService = new FlashcardSrsService(_savedItemRepo);
		var session = FlashcardSession.Create(_userId, _clock.UtcNow.AddHours(1)).Value;
		session.AddItem(FlashcardSessionItem.Create(session.Id, saved.Id, FlashcardRating.Easy, _clock.UtcNow.AddHours(1)).Value);
		await flashcardService.ApplySrsUpdatesAsync(session, _clock.UtcNow.AddHours(1));

		var afterReview = await scheduleHandler.Handle(new GetSavedItemScheduleQuery(saved.Id.Value), CancellationToken.None);
		Assert.True(afterReview.IsSuccess);
		Assert.Equal(0, afterReview.Value.ConsecutivePassiveBoosts);
		Assert.Equal(8, afterReview.Value.CurrentIntervalDays);
		Assert.Single(afterReview.Value.BoostHistory!);
	}

	[Fact]
	public void ApplyPassiveImmersionBoost_EnforcesAdjustmentTypeAndCapInvariants()
	{
		var now = new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
		var schedule = SrsSchedule.Create(SavedItemId.Create(), now).Value;

		var invalidTypeResult = schedule.ApplyPassiveImmersionBoost(
			now.AddDays(4),
			"UnknownType",
			4,
			3,
			now);
		Assert.True(invalidTypeResult.IsFailure);
		Assert.Equal(SrsImmersionBoostLogErrors.InvalidAdjustmentType, invalidTypeResult.Error);

		Assert.True(schedule.ApplyPassiveImmersionBoost(now.AddDays(2), SrsImmersionAdjustmentTypes.OverdueRescue, 2, 3, now).IsSuccess);
		Assert.True(schedule.ApplyPassiveImmersionBoost(now.AddDays(6), SrsImmersionAdjustmentTypes.DueTodayBonus, 4, 3, now.AddDays(2)).IsSuccess);

		var cappedResult = schedule.ApplyPassiveImmersionBoost(
			now.AddDays(10),
			SrsImmersionAdjustmentTypes.DueTodayBonus,
			4,
			3,
			now.AddDays(6));
		Assert.True(cappedResult.IsFailure);
		Assert.Equal(SrsScheduleErrors.PassiveBoostCapReached, cappedResult.Error);
		Assert.Equal(2, schedule.ConsecutivePassiveBoosts);
		Assert.Equal(2, schedule.BoostLogs.Count);
	}
}

internal sealed class LearningFakeSystemClock : ISystemClock
{
	public DateTimeOffset UtcNow { get; set; } = new(2026, 11, 2, 10, 0, 0, TimeSpan.Zero);
}

internal sealed class LearningFakeCurrentUserContext : ICurrentUserContext
{
	public UserId? UserId { get; set; }
	public string? Role { get; set; }
}

internal sealed class LearningNullLearningUnitRepository : ILearningUnitRepository
{
	public Task<LearningUnit?> GetByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default) => Task.FromResult<LearningUnit?>(null);
	public Task<IEnumerable<LearningUnit>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IEnumerable<LearningUnit>> FindAsync(Expression<Func<LearningUnit, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public void Add(LearningUnit entity) => throw new NotImplementedException();
	public void Update(LearningUnit entity) => throw new NotImplementedException();
	public void Delete(LearningUnit entity) => throw new NotImplementedException();
	public void DeleteMultiple(List<LearningUnit> entities) => throw new NotImplementedException();
	public Task<LearningUnit?> GetDetailByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<(IReadOnlyList<LearningUnit> Items, int TotalCount)> GetPagedByVideoIdAsync(VideoId videoId, KnowledgeUnitType? knowledgeType, int pageNumber, int pageSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextByMeaningIdsAsync(IReadOnlyCollection<Guid> meaningIds, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyDictionary<Guid, IReadOnlyList<LearningUnitMediaContext>>> GetMediaContextsByMeaningIdsAsync(IReadOnlyCollection<Guid> meaningIds, int maxVideosPerMeaning = 5, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<LearningUnitMediaContext>> GetMediaContextsByMeaningIdAsync(KnowledgeMeaningId meaningId, int maxCount = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextBySegmentIdsAsync(IReadOnlyCollection<Guid> segmentIds, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<(LearningUnit? Unit, IReadOnlyList<LearningUnit> SegmentUnits)> GetCurationContextByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default) => Task.FromResult<(LearningUnit?, IReadOnlyList<LearningUnit>)>((null, []));
}

internal sealed class InMemoryVideoRepository : IVideoRepository
{
	public List<Video> Items { get; } = [];
	public Task<Video?> GetByIdAsync(VideoId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<Video>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Video>>(Items);
	public Task<IEnumerable<Video>> FindAsync(Expression<Func<Video, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Video>>(Items.AsQueryable().Where(predicate).ToList());
	public void Add(Video entity) => Items.Add(entity);
	public void Update(Video entity) { }
	public void Delete(Video entity) => Items.Remove(entity);
	public void DeleteMultiple(List<Video> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<(IReadOnlyList<Video> Items, int TotalCount)> GetPagedAsync(IReadOnlyCollection<VideoSource> sourceTypes, VideoStatus? status, Guid? learnerId, string? searchKey, Guid? categoryId, Guid? seriesId, int pageNumber, int pageSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetByMediaAssetIdAsync(MediaAssetId mediaAssetId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetByTranscriptIdAsync(TranscriptId transcriptId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetByTranscriptSegmentIdAsync(TranscriptSegmentId segmentId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetByBunsetsuIdAsync(BunsetsuId bunsetsuId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<Video?> GetByTokenIdAsync(TokenId tokenId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptAsync(int batchSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptForTranslateAsync(int batchSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<Video>> GetModeratingVideosAsync(int batchSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<Video>> GetImportingVideosAsync(int batchSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<Video>> GetProcessingVideosWithProcessedResultAsync(int batchSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

internal sealed class InMemoryTrackingTranscriptRepository : ITranscriptRepository
{
	private readonly Dictionary<VideoId, List<TranscriptSegment>> _byVideo = [];

	public void AddVideoSegment(VideoId videoId, TranscriptSegment segment)
	{
		if (!_byVideo.TryGetValue(videoId, out var list))
		{
			list = [];
			_byVideo[videoId] = list;
		}
		list.Add(segment);
	}

	public Task<IReadOnlyList<TranscriptSegment>> GetOverlappingSegmentsWithLearningUnitsAsync(
		VideoId videoId,
		TimeSpan rangeStart,
		TimeSpan rangeEnd,
		CancellationToken cancellationToken = default)
	{
		if (!_byVideo.TryGetValue(videoId, out var list))
		{
			return Task.FromResult<IReadOnlyList<TranscriptSegment>>([]);
		}

		IReadOnlyList<TranscriptSegment> overlapping = list
			.Where(s => s.EndTime >= rangeStart && s.StartTime <= rangeEnd)
			.ToList();
		return Task.FromResult(overlapping);
	}

	public Task<Transcript?> GetByIdAsync(TranscriptId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IEnumerable<Transcript>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IEnumerable<Transcript>> FindAsync(Expression<Func<Transcript, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public void Add(Transcript entity) => throw new NotImplementedException();
	public void Update(Transcript entity) => throw new NotImplementedException();
	public void Delete(Transcript entity) => throw new NotImplementedException();
	public void DeleteMultiple(List<Transcript> entities) => throw new NotImplementedException();
	public Task<Transcript?> GetByVideoIdAsync(VideoId videoId, int pageNumber, int pageSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<TranscriptSegment?> GetSegmentWithCurationContextByIdAsync(TranscriptSegmentId segmentId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

internal sealed class InMemoryLearnerVideoProgressRepository : ILearnerVideoProgressRepository
{
	public List<LearnerVideoProgress> Items { get; } = [];
	public Task<LearnerVideoProgress?> GetByIdAsync(LearnerVideoProgressId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<LearnerVideoProgress>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerVideoProgress>>(Items);
	public Task<IEnumerable<LearnerVideoProgress>> FindAsync(Expression<Func<LearnerVideoProgress, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerVideoProgress>>(Items.AsQueryable().Where(predicate).ToList());
	public void Add(LearnerVideoProgress entity) => Items.Add(entity);
	public void Update(LearnerVideoProgress entity) { }
	public void Delete(LearnerVideoProgress entity) => Items.Remove(entity);
	public void DeleteMultiple(List<LearnerVideoProgress> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<LearnerVideoProgress?> GetByUserAndVideoAsync(UserId userId, VideoId videoId, CancellationToken cancellationToken = default)
		=> Task.FromResult(Items.FirstOrDefault(x => x.UserId == userId && x.VideoId == videoId));
	public Task<IReadOnlyList<LearnerVideoProgress>> GetByUserWithVideoDetailsAsync(UserId userId, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<LearnerVideoProgress>>(Items.Where(x => x.UserId == userId).ToList());
	public Task<bool> DeleteByUserAndVideoAsync(UserId userId, VideoId videoId, CancellationToken cancellationToken = default)
		=> Task.FromResult(Items.RemoveAll(x => x.UserId == userId && x.VideoId == videoId) > 0);
}

internal sealed class InMemoryLearnerEncounteredLearningUnitRepository : ILearnerEncounteredLearningUnitRepository
{
	public List<LearnerEncounteredLearningUnit> Items { get; } = [];
	public Task<LearnerEncounteredLearningUnit?> GetByIdAsync(LearnerEncounteredLearningUnitId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<LearnerEncounteredLearningUnit>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerEncounteredLearningUnit>>(Items);
	public Task<IEnumerable<LearnerEncounteredLearningUnit>> FindAsync(Expression<Func<LearnerEncounteredLearningUnit, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerEncounteredLearningUnit>>(Items.AsQueryable().Where(predicate).ToList());
	public void Add(LearnerEncounteredLearningUnit entity) => Items.Add(entity);
	public void Update(LearnerEncounteredLearningUnit entity) { }
	public void Delete(LearnerEncounteredLearningUnit entity) => Items.Remove(entity);
	public void DeleteMultiple(List<LearnerEncounteredLearningUnit> entities) => entities.ForEach(e => Items.Remove(e));

	public Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndLearningUnitIdsAsync(UserId userId, IReadOnlyCollection<LearningUnitId> learningUnitIds, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<LearnerEncounteredLearningUnit>>(Items.Where(x => x.UserId == userId && learningUnitIds.Contains(x.LearningUnitId)).ToList());

	public Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndVideoAsync(UserId userId, VideoId videoId, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<LearnerEncounteredLearningUnit>>(Items.Where(x => x.UserId == userId && x.VideoId == videoId).ToList());

	public Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserWithDetailsAsync(UserId userId, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<LearnerEncounteredLearningUnit>>(Items.Where(x => x.UserId == userId).ToList());

	public Task<int> DeleteByUserAndVideoAsync(UserId userId, VideoId videoId, CancellationToken cancellationToken = default)
		=> Task.FromResult(Items.RemoveAll(x => x.UserId == userId && x.VideoId == videoId));

	public Task<IReadOnlyList<Video>> GetReadyVideosWithLearningUnitsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<(string LevelCode, string LevelName, int SortOrder, int TotalUnitsCount)>> GetSystemVocabularyCountsByLevelAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<(Guid CategoryId, string CategoryName, IReadOnlyCollection<Guid> KnowledgeUnitIds)>> GetSystemVocabularyByCategoryAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

internal sealed class FlushingUnitOfWork : IUnitOfWork
{
	private readonly Action _onFlush;

	public FlushingUnitOfWork(Action onFlush)
	{
		_onFlush = onFlush;
	}

	public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
	{
		var result = await action();
		_onFlush();
		return result;
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		_onFlush();
		return Task.FromResult(1);
	}
}

internal sealed class InMemoryLearnerEncounterEventRepository : ILearnerEncounterEventRepository
{
	public List<LearnerEncounterEvent> Items { get; } = [];
	private readonly List<LearnerEncounterEvent> _pendingAdded = [];

	public void Flush() => _pendingAdded.Clear();

	public Task<LearnerEncounterEvent?> GetByIdAsync(LearnerEncounterEventId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<LearnerEncounterEvent>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerEncounterEvent>>(Items);
	public Task<IEnumerable<LearnerEncounterEvent>> FindAsync(Expression<Func<LearnerEncounterEvent, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<LearnerEncounterEvent>>(Items.AsQueryable().Where(predicate).ToList());

	public void Add(LearnerEncounterEvent entity)
	{
		Items.Add(entity);
		_pendingAdded.Add(entity);
	}

	public void Update(LearnerEncounterEvent entity) { }
	public void Delete(LearnerEncounterEvent entity) => Items.Remove(entity);
	public void DeleteMultiple(List<LearnerEncounterEvent> entities) => entities.ForEach(e => Items.Remove(e));

	public Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsByMeaningsAsync(
		UserId userId,
		IReadOnlyCollection<KnowledgeMeaningId> meaningIds,
		DateTimeOffset sinceUtc,
		CancellationToken cancellationToken = default)
	{
		IReadOnlyDictionary<KnowledgeMeaningId, int> dict = Items
			.Where(x => x.UserId == userId && meaningIds.Contains(x.KnowledgeMeaningId) && x.EncounteredAt >= sinceUtc)
			.GroupBy(x => x.KnowledgeMeaningId)
			.ToDictionary(g => g.Key, g => g.Count());
		return Task.FromResult(dict);
	}

	public Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsInWindowAsync(
		UserId userId,
		IReadOnlyDictionary<KnowledgeMeaningId, DateTimeOffset?> exclusiveAfterByMeaning,
		DateTimeOffset minSinceUtc,
		CancellationToken cancellationToken = default)
	{
		// Mirror real EF Core behavior: AsNoTracking() reads ONLY previously flushed rows (excludes _pendingAdded from current UoW).
		var flushed = Items.Except(_pendingAdded).ToList();

		var counts = new Dictionary<KnowledgeMeaningId, int>();
		foreach (var row in flushed)
		{
			if (row.UserId == userId
				&& row.EncounteredAt >= minSinceUtc
				&& exclusiveAfterByMeaning.TryGetValue(row.KnowledgeMeaningId, out var exclusiveAfter)
				&& (!exclusiveAfter.HasValue || row.EncounteredAt > exclusiveAfter.Value))
			{
				counts[row.KnowledgeMeaningId] = counts.GetValueOrDefault(row.KnowledgeMeaningId, 0) + 1;
			}
		}

		return Task.FromResult<IReadOnlyDictionary<KnowledgeMeaningId, int>>(counts);
	}

	public Task<int> DeleteByUserAndVideoAsync(UserId userId, VideoId videoId, CancellationToken cancellationToken = default)
		=> Task.FromResult(Items.RemoveAll(x => x.UserId == userId && x.VideoId == videoId));
}

internal sealed class InMemorySavedItemRepository : ISavedItemRepository
{
	public List<SavedItem> Items { get; } = [];
	public Task<SavedItem?> GetByIdAsync(SavedItemId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<SavedItem>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<SavedItem>>(Items);
	public Task<IEnumerable<SavedItem>> FindAsync(Expression<Func<SavedItem, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<SavedItem>>(Items.AsQueryable().Where(predicate).ToList());
	public void Add(SavedItem entity) => Items.Add(entity);
	public void Update(SavedItem entity) { }
	public void Delete(SavedItem entity) => Items.Remove(entity);
	public void DeleteMultiple(List<SavedItem> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<SavedItem?> GetByIdWithScheduleAsync(SavedItemId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<SavedItem?> GetByUserAndMeaningAsync(UserId userId, KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.UserId == userId && x.KnowledgeMeaningId == meaningId));
	public Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(UserId userId, int page, int pageSize, string? filter = null, string? sortBy = null, string? sortOrder = null, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<SavedItem>>(Items.Where(x => x.UserId == userId).ToList());
	public Task<IReadOnlyList<SavedItem>> GetUserItemsAsync(UserId userId, SavedItemQueryOptions options, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<SavedItem>>(Items.Where(x => x.UserId == userId).ToList());
	public Task<int> CountUserItemsAsync(UserId userId, string? filter = null, CancellationToken cancellationToken = default) => Task.FromResult(Items.Count(x => x.UserId == userId));
	public Task<int> CountUserItemsAsync(UserId userId, SavedItemQueryOptions options, CancellationToken cancellationToken = default) => Task.FromResult(Items.Count(x => x.UserId == userId));
	public Task<IReadOnlyList<SavedItem>> GetDueItemsAsync(UserId userId, DateTimeOffset now, int limit, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<SavedItem>> GetByIdsWithScheduleAsync(IEnumerable<SavedItemId> ids, UserId userId, CancellationToken cancellationToken = default)
	{
		var idSet = ids.ToHashSet();
		return Task.FromResult<IReadOnlyList<SavedItem>>(Items.Where(x => x.UserId == userId && idSet.Contains(x.Id)).ToList());
	}
	public Task<SrsDashboardMetrics> GetDashboardMetricsAsync(UserId userId, DateTimeOffset now, DateTimeOffset? startOfTodayUtc = null, DateTimeOffset? endOfTodayUtc = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<bool> HasAnyWithMeaningIdAsync(KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default) => Task.FromResult(Items.Any(x => x.KnowledgeMeaningId == meaningId));
	public Task<IReadOnlyList<SavedItem>> GetByUserAndMeaningsWithScheduleAsync(UserId userId, IReadOnlyCollection<KnowledgeMeaningId> meaningIds, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<SavedItem>>(Items.Where(x => x.UserId == userId && x.IsActive && meaningIds.Contains(x.KnowledgeMeaningId)).ToList());
}

internal sealed class InMemoryQuizBankQuestionRepository : IBankQuestionRepository
{
	public List<BankQuestion> Items { get; } = [];
	public Task<BankQuestion?> GetByIdAsync(BankQuestionId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<BankQuestion>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<BankQuestion>>(Items);
	public Task<IEnumerable<BankQuestion>> FindAsync(Expression<Func<BankQuestion, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public void Add(BankQuestion entity) => Items.Add(entity);
	public void Update(BankQuestion entity) { }
	public void Delete(BankQuestion entity) => Items.Remove(entity);
	public void DeleteMultiple(List<BankQuestion> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<BankQuestion?> GetByIdWithDetailsAsync(BankQuestionId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<bool> QuestionTypeExistsAsync(Guid questionTypeId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<BankQuestion>> GetByMeaningIdsAsync(IEnumerable<KnowledgeMeaningId> meaningIds, CancellationToken cancellationToken = default)
	{
		var idSet = meaningIds.ToHashSet();
		return Task.FromResult<IReadOnlyList<BankQuestion>>(Items.Where(x => idSet.Contains(x.KnowledgeMeaningId)).ToList());
	}
	public Task<IReadOnlyList<BankQuestion>> GetActiveByMeaningIdAsync(KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<(IReadOnlyList<BankQuestion> Items, int TotalCount)> GetQuestionsAsync(KnowledgeMeaningId? meaningId = null, Guid? questionTypeId = null, bool? isActive = null, QuestionStatus? status = null, QuestionSourceType? sourceType = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<(IReadOnlyList<BankQuestion> Items, int TotalCount)> GetReviewQueueAsync(QuestionStatus? status = QuestionStatus.Draft, QuestionSourceType? sourceType = null, string? level = null, string? questionTypeCode = null, string? mediaType = null, string? search = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<string>> GetRandomDistractorMeaningsAsync(IEnumerable<KnowledgeMeaningId> excludeMeaningIds, int count, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<bool> HasBeenUsedInQuizAsync(BankQuestionId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<bool> HasCurationReferencesAsync(BankQuestionId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<bool> ExistsDuplicateAsync(KnowledgeMeaningId meaningId, Guid questionTypeId, string questionText, BankQuestionId? excludeId = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<BankQuestion?> FindDuplicateAsync(KnowledgeMeaningId meaningId, Guid questionTypeId, string questionText, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<bool> HasDraftOrPublishedQuestionAsync(KnowledgeMeaningId meaningId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<QuestionTypeId?> GetQuestionTypeIdByCodeAsync(string code, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<QuestionTypeId> GetDefaultQuestionTypeIdAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<KnowledgeMeaning>> GetMeaningsNeedingDraftQuestionsAsync(int limit, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

internal sealed class InMemoryQuizRepository : IQuizRepository
{
	public List<Quiz> Items { get; } = [];
	public Task<Quiz?> GetByIdAsync(QuizId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<Quiz>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Quiz>>(Items);
	public Task<IEnumerable<Quiz>> FindAsync(Expression<Func<Quiz, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public void Add(Quiz entity) => Items.Add(entity);
	public void Update(Quiz entity) { }
	public void Delete(Quiz entity) => Items.Remove(entity);
	public void DeleteMultiple(List<Quiz> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<Quiz?> GetByIdWithQuestionsAsync(QuizId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
}

internal sealed class InMemoryQuizAttemptRepository : IQuizAttemptRepository
{
	public List<QuizAttempt> Items { get; } = [];
	public Task<QuizAttempt?> GetByIdAsync(QuizAttemptId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<IEnumerable<QuizAttempt>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<QuizAttempt>>(Items);
	public Task<IEnumerable<QuizAttempt>> FindAsync(Expression<Func<QuizAttempt, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public void Add(QuizAttempt entity) => Items.Add(entity);
	public void Update(QuizAttempt entity) { }
	public void Delete(QuizAttempt entity) => Items.Remove(entity);
	public void DeleteMultiple(List<QuizAttempt> entities) => entities.ForEach(e => Items.Remove(e));
	public Task<QuizAttempt?> GetByIdWithItemsAndQuizAsync(QuizAttemptId id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
	public Task<QuizAttempt?> GetActiveAttemptForUserAsync(UserId userId, CancellationToken cancellationToken = default) => Task.FromResult<QuizAttempt?>(null);
	public void RemoveItems(IEnumerable<Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem.QuizAttemptItem> items) { }
	public Task<(IReadOnlyList<QuizAttempt> Items, int TotalCount)> GetAttemptsByUserAsync(UserId userId, QuizAttemptStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyDictionary<Guid, QuizItemAnswerHistory>> GetAnswerHistoryBySavedItemAsync(UserId userId, IReadOnlyCollection<SavedItemId> savedItemIds, QuizAttemptId excludeAttemptId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>> GetFeedbackHistoryBySavedItemAsync(UserId userId, IReadOnlyCollection<SavedItemId> savedItemIds, QuizAttemptId excludeAttemptId, int maxEntriesPerItem = 3, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<QuizAttemptId>> GetPendingEvaluationAttemptIdsAsync(int maxCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<IReadOnlyList<QuizAttempt>> GetStaleAttemptsAsync(DateTimeOffset now, int maxCount, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	public Task<QuizAnswerSummary> GetAnswerSummaryByUserAsync(UserId userId, DateTimeOffset now, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

internal sealed class NullQuizContentGenerator : IQuizContentGenerator
{
	public Task<IReadOnlyDictionary<Guid, string>> GenerateAdaptiveFeedbackAsync(IReadOnlyList<QuizFeedbackPrompt> questions, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyDictionary<Guid, string>>(new Dictionary<Guid, string>());

	public Task<IReadOnlyDictionary<Guid, GeneratedQuizPromptAndExplanation>> GenerateQuizContentBatchAsync(IReadOnlyList<GenerateQuizContentPrompt> prompts, CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyDictionary<Guid, GeneratedQuizPromptAndExplanation>>(new Dictionary<Guid, GeneratedQuizPromptAndExplanation>());
}

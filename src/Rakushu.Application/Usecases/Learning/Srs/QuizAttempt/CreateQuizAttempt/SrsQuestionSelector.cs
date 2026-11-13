using SavedItemEntity = Rakushu.Domain.Entities.Srs.SavedItem.SavedItem;
using BankQuestionEntity = Rakushu.Domain.Entities.Srs.BankQuestion.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;

internal static class SrsQuestionSelector
{
	private static readonly string[] RecognitionTypes = ["WORD_TO_MEANING", "KANJI_TO_READING", "READING_TO_KANJI"];
	private static readonly string[] ContextualTypes = ["VIDEO_CLOZE", "TEXT_CLOZE", "READING_TO_KANJI"];
	private static readonly string[] ProductionTypes = ["AUDIO_DICTATION", "TYPE_READING"];

	public static BankQuestionEntity PickQuestionForSrsStage(
		SavedItemEntity item,
		IReadOnlyList<BankQuestionEntity> matchingQuestions)
	{
		if (matchingQuestions.Count == 0)
			throw new InvalidOperationException("matchingQuestions cannot be empty.");

		if (matchingQuestions.Count == 1)
			return matchingQuestions[0];

		// Current SRS repetition level from latest schedule review audit log
		int repetitions = item.Schedule?.Details
			.OrderByDescending(d => d.ReviewNumber)
			.Select(d => d.Repetitions)
			.FirstOrDefault() ?? 0;

		string[] primaryPreferences;
		string[] secondaryPreferences;

		if (repetitions <= 1)
		{
			// Giai đoạn 1: Mới học (Rep 0 - 1) -> Nhận diện cơ bản (Recognition)
			primaryPreferences = RecognitionTypes;
			secondaryPreferences = ContextualTypes;
		}
		else if (repetitions <= 4)
		{
			// Giai đoạn 2: Đang củng cố (Rep 2 - 4) -> Ngữ cảnh & Video/Text Cloze
			primaryPreferences = ContextualTypes;
			secondaryPreferences = ProductionTypes;
		}
		else
		{
			// Giai đoạn 3: Thành thạo / Master (Rep > 4) -> Active Recall: Dictation & Typing
			primaryPreferences = ProductionTypes;
			secondaryPreferences = ContextualTypes;
		}

		// 1. Try finding questions matching primary preferences
		var primaryMatches = matchingQuestions
			.Where(q => q.QuestionType?.Code is not null && primaryPreferences.Contains(q.QuestionType.Code, StringComparer.OrdinalIgnoreCase))
			.ToList();

		if (primaryMatches.Count > 0)
			return primaryMatches[Random.Shared.Next(primaryMatches.Count)];

		// 2. Fallback to secondary preferences
		var secondaryMatches = matchingQuestions
			.Where(q => q.QuestionType?.Code is not null && secondaryPreferences.Contains(q.QuestionType.Code, StringComparer.OrdinalIgnoreCase))
			.ToList();

		if (secondaryMatches.Count > 0)
			return secondaryMatches[Random.Shared.Next(secondaryMatches.Count)];

		// 3. Fallback to any available question
		return matchingQuestions[Random.Shared.Next(matchingQuestions.Count)];
	}
}

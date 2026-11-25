using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Ai;
using Rakushu.Application.Abstractions.Infrastructure.AiService;

namespace Rakushu.Infrastructure.AiService;

public sealed class BedrockQuizContentGenerator : IQuizContentGenerator
{
	private const int MaxQuizContentChunkSize = 5;
	private const int MaxFeedbackChunkSize = 8;

	private static readonly JsonDocumentOptions LenientJsonOptions = new()
	{
		AllowTrailingCommas = true,
		CommentHandling = JsonCommentHandling.Skip
	};

	private readonly IAiService _aiService;
	private readonly ILogger<BedrockQuizContentGenerator> _logger;

	public BedrockQuizContentGenerator(
		IAiService aiService,
		ILogger<BedrockQuizContentGenerator> logger)
	{
		_aiService = aiService;
		_logger = logger;
	}

	public async Task<IReadOnlyDictionary<Guid, string>> GenerateAdaptiveFeedbackAsync(
		IReadOnlyList<QuizFeedbackPrompt> questions,
		CancellationToken cancellationToken = default)
	{
		if (questions.Count == 0)
		{
			return new Dictionary<Guid, string>();
		}

		try
		{
			var correctQuestions = questions.Where(q => q.IsCorrect).ToList();
			var incorrectQuestions = questions.Where(q => !q.IsCorrect).ToList();

			var feedback = new Dictionary<Guid, string>();

			if (correctQuestions.Count > 0)
			{
				await GenerateFeedbackGroupAsync(correctQuestions, isCorrect: true, feedback, cancellationToken);
			}

			if (incorrectQuestions.Count > 0)
			{
				await GenerateFeedbackGroupAsync(incorrectQuestions, isCorrect: false, feedback, cancellationToken);
			}

			_logger.LogInformation(
				"[QuizLLM][Bedrock][ParsedFeedback] Model={Model}; TotalFeedbackCount={FeedbackCount}; QuestionIds={QuestionIds}",
				_aiService.ModelId, feedback.Count, string.Join(",", feedback.Keys));

			return feedback;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "[QuizLLM][Bedrock] Adaptive quiz feedback failed: {Message}", exception.Message);
			return new Dictionary<Guid, string>();
		}
	}

	public async Task<IReadOnlyDictionary<Guid, GeneratedQuizPromptAndExplanation>> GenerateQuizContentBatchAsync(
		IReadOnlyList<GenerateQuizContentPrompt> prompts,
		CancellationToken cancellationToken = default)
	{
		if (prompts.Count == 0)
		{
			return new Dictionary<Guid, GeneratedQuizPromptAndExplanation>();
		}

		var result = new Dictionary<Guid, GeneratedQuizPromptAndExplanation>();

		try
		{
			foreach (var chunk in prompts.Chunk(MaxQuizContentChunkSize))
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					await GenerateQuizContentChunkAsync(chunk, result, cancellationToken);
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					throw;
				}
				catch (Exception chunkException)
				{
					_logger.LogWarning(
						chunkException,
						"[QuizLLM][Bedrock] Failed generating quiz content chunk of {ChunkCount} item(s): {Message}",
						chunk.Length,
						chunkException.Message);
				}
			}

			_logger.LogInformation(
				"[QuizLLM][Bedrock][GeneratedContent] Model={Model}; Generated {Count}/{Total} items",
				_aiService.ModelId, result.Count, prompts.Count);

			return result;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "[QuizLLM][Bedrock] Quiz content generation failed: {Message}", exception.Message);
			return result;
		}
	}

	private async Task GenerateQuizContentChunkAsync(
		IReadOnlyList<GenerateQuizContentPrompt> chunk,
		Dictionary<Guid, GeneratedQuizPromptAndExplanation> result,
		CancellationToken cancellationToken)
	{
		var validMeaningIds = chunk.Select(p => p.MeaningId).ToHashSet();
		var itemsBuilder = new StringBuilder();
		for (int i = 0; i < chunk.Count; i++)
		{
			var p = chunk[i];
			var idx = i + 1;
			itemsBuilder.AppendLine($"Mục {idx} (meaningId: \"{p.MeaningId}\"):");
			itemsBuilder.AppendLine($"- Từ/ngữ pháp mục tiêu: {p.Expression}");
			if (!string.IsNullOrWhiteSpace(p.Reading))
			{
				itemsBuilder.AppendLine($"- Cách đọc: {p.Reading}");
			}
			itemsBuilder.AppendLine($"- Nghĩa tiếng Việt: {p.Meaning}");
			itemsBuilder.AppendLine($"- Câu ngữ cảnh: {p.ContextSentence}");
			itemsBuilder.AppendLine();
		}

		var jsonTemplateBuilder = new StringBuilder();
		for (int i = 0; i < chunk.Count; i++)
		{
			var p = chunk[i];
			var escapedExpression = p.Expression.Replace("\\", "\\\\").Replace("\"", "\\\"");
			jsonTemplateBuilder.Append($$"""
				    {
				      "meaningId": "{{p.MeaningId}}",
				      "prompt": "<Bối cảnh giao tiếp tự nhiên>: \"<Trích dẫn câu thoại thực tế>\" Từ/ngữ pháp 「{{escapedExpression}}」 trong câu trên mang ý nghĩa gì?",
				      "explanation": "<Lời giải thích ngắn gọn bằng tiếng Việt về ý nghĩa, cách đọc và sắc thái của từ trong câu thoại>"
				    }
				""");
			if (i < chunk.Count - 1)
			{
				jsonTemplateBuilder.Append(',');
			}
			jsonTemplateBuilder.AppendLine();
		}

		var userPrompt = $$"""
			Tôi cần biên soạn câu hỏi trắc nghiệm tình huống tiếng Nhật và lời giải thích cho toàn bộ {{chunk.Count}} từ vựng/ngữ pháp tiếng Nhật đã được cung cấp đầy đủ ở phần Learning unit ở trên (không cần thêm ngữ cảnh video hay transcript).

			VAI TRÒ & YÊU CẦU BIÊN SOẠN SƯ PHẠM:
			- Đóng vai chuyên gia sư phạm tiếng Nhật của Rakushu, chuyên biên soạn câu hỏi trắc nghiệm tình huống thực tế (context-based situational quiz).
			- TUYỆT ĐỐI KHÔNG đặt câu hỏi cụt lủn chỉ có 'Từ này có ý nghĩa là gì?'.
			- Với mỗi mục trong phần Learning unit:
			  1. Dựa vào "Câu ngữ cảnh" thực tế để xác định bối cảnh giao tiếp tự nhiên (ví dụ: tư vấn kỹ thuật cho khách hàng, họp nhóm dự án, trao đổi email với đồng nghiệp, hội thoại đời thường...).
			  2. Viết câu hỏi tình huống sinh động bằng tiếng Việt (trường "prompt"):
			     + Mở đầu bằng bối cảnh giao tiếp tự nhiên và trích dẫn câu thoại gốc (đặt trong dấu nháy kép "...").
			     + Kết thúc câu hỏi BẮT BUỘC bằng định dạng chuẩn: Từ/ngữ pháp 「từ_vựng」 trong câu trên mang ý nghĩa gì?
			     + Định dạng mẫu: [Bối cảnh giao tiếp tự nhiên]: "[Câu thoại trích dẫn]" Từ/ngữ pháp 「[Từ/ngữ pháp mục tiêu]」 trong câu trên mang ý nghĩa gì?
			  3. Viết lời giải thích ngắn gọn, rõ ràng bằng tiếng Việt (trường "explanation") về ý nghĩa tiếng Việt, cách đọc Hiragana và sắc thái thực tế của từ vựng trong câu thoại.
			- BẮT BUỘC trả về đủ tất cả {{chunk.Count}} mục ứng với đúng meaningId tương ứng.

			QUAN TRỌNG VỀ ĐỊNH DẠNG ĐẦU RA:
			CHỈ trả về DUY NHẤT một đối tượng JSON hợp lệ theo cấu trúc chính xác bên dưới. KHÔNG thêm lời chào, KHÔNG giải thích ngoài JSON, KHÔNG bọc markdown code block:
			{
			  "items": [
			{{jsonTemplateBuilder}}
			  ]
			}
			""";

		using var responseDoc = await CallConverseForJsonAsync(
			userPrompt,
			itemsBuilder.ToString(),
			"generate_quiz_content",
			cancellationToken);

		if (responseDoc is null)
		{
			return;
		}

		JsonElement itemsElement;
		if (responseDoc.RootElement.ValueKind == JsonValueKind.Array)
		{
			itemsElement = responseDoc.RootElement;
		}
		else if (responseDoc.RootElement.TryGetProperty("items", out var array) &&
			array.ValueKind == JsonValueKind.Array)
		{
			itemsElement = array;
		}
		else
		{
			_logger.LogWarning("[QuizLLM][Bedrock] Could not find items array in quiz content response.");
			return;
		}

		foreach (var el in itemsElement.EnumerateArray())
		{
			if (el.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			if (el.TryGetProperty("meaningId", out var idProp) &&
				idProp.ValueKind == JsonValueKind.String &&
				Guid.TryParse(idProp.GetString()?.Trim(), out var meaningId) &&
				validMeaningIds.Contains(meaningId) &&
				el.TryGetProperty("prompt", out var promptProp) &&
				promptProp.ValueKind == JsonValueKind.String &&
				el.TryGetProperty("explanation", out var expProp) &&
				expProp.ValueKind == JsonValueKind.String)
			{
				var promptText = promptProp.GetString()?.Trim();
				var expText = expProp.GetString()?.Trim();

				if (!string.IsNullOrWhiteSpace(promptText) && !string.IsNullOrWhiteSpace(expText))
				{
					result[meaningId] = new GeneratedQuizPromptAndExplanation(meaningId, promptText, expText);
				}
			}
		}
	}

	private async Task GenerateFeedbackGroupAsync(
		IReadOnlyList<QuizFeedbackPrompt> items,
		bool isCorrect,
		Dictionary<Guid, string> feedback,
		CancellationToken cancellationToken)
	{
		foreach (var chunk in items.Chunk(MaxFeedbackChunkSize))
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				await GenerateFeedbackChunkAsync(chunk, isCorrect, feedback, cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogWarning(
					ex,
					"[QuizLLM][Bedrock] Failed generating feedback chunk (size={ChunkSize}) for group isCorrect={IsCorrect}.",
					chunk.Length,
					isCorrect);
			}
		}
	}

	private async Task GenerateFeedbackChunkAsync(
		IReadOnlyList<QuizFeedbackPrompt> items,
		bool isCorrect,
		Dictionary<Guid, string> result,
		CancellationToken cancellationToken)
	{
		if (items.Count == 0)
		{
			return;
		}

		var itemByIndex = new Dictionary<int, Guid>();
		var itemsPromptBuilder = new StringBuilder();

		for (int i = 0; i < items.Count; i++)
		{
			var index = i + 1;
			var item = items[i];
			itemByIndex[index] = item.QuestionId;

			itemsPromptBuilder.Append($"- Câu {index}: ");
			if (!string.IsNullOrWhiteSpace(item.QuestionPrompt))
			{
				itemsPromptBuilder.Append($"Câu hỏi: \"{item.QuestionPrompt}\". ");
			}
			if (!string.IsNullOrWhiteSpace(item.Reading))
			{
				itemsPromptBuilder.Append($"Cách đọc: \"{item.Reading}\". ");
			}

			if (isCorrect)
			{
				if (!string.IsNullOrWhiteSpace(item.CorrectAnswer))
				{
					itemsPromptBuilder.Append($"Đáp án đúng đã chọn: \"{item.CorrectAnswer}\". ");
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(item.SelectedAnswer))
				{
					itemsPromptBuilder.Append($"Học viên chọn nhầm: \"{item.SelectedAnswer}\". ");
				}
				if (!string.IsNullOrWhiteSpace(item.CorrectAnswer))
				{
					itemsPromptBuilder.Append($"Đáp án đúng là: \"{item.CorrectAnswer}\". ");
				}
			}

			var historyText = BuildHistoryContextText(item, isCorrect);
			itemsPromptBuilder.Append($"Lịch sử học tập: {historyText}");
			itemsPromptBuilder.AppendLine();
		}

		var jsonTemplateBuilder = new StringBuilder();
		for (int i = 0; i < items.Count; i++)
		{
			var idx = i + 1;
			jsonTemplateBuilder.Append($"    {{ \"index\": {idx}, \"message\": \"<lời nhận xét cho câu {idx}>\" }}");
			if (i < items.Count - 1)
			{
				jsonTemplateBuilder.Append(',');
			}
			jsonTemplateBuilder.AppendLine();
		}

		string userPrompt = isCorrect
			? $$"""
				Tôi cần viết nhận xét khích lệ cá nhân hóa bằng tiếng Việt cho {{items.Count}} câu hỏi kiểm tra từ vựng tiếng Nhật học viên vừa trả lời ĐÚNG, dựa trên toàn bộ dữ liệu câu hỏi và lịch sử học tập SRS đã cung cấp đầy đủ ở phần Learning unit ở trên (không cần thêm ngữ cảnh video hay transcript).

				YÊU CẦU ADAPTIVE FEEDBACK:
				Viết 1 lời nhận xét ngắn (1-2 câu) bằng tiếng Việt tự nhiên và cá nhân hóa cho TỪNG câu:
				- Nếu từng làm sai nay làm đúng: chúc mừng sự tiến bộ khi đã khắc phục được lỗi sai.
				- Nếu đúng liên tiếp nhiều lần: khen ngợi khả năng ghi nhớ vững vàng, phong độ ổn định.
				- Nếu là lần đầu: động viên khởi đầu tốt.
				- Xưng hô thân thiện là 'Bạn'.
				- BẮT BUỘC trả về đủ tất cả {{items.Count}} câu trong mảng feedback (từ index 1 đến index {{items.Count}}).

				QUAN TRỌNG VỀ ĐỊNH DẠNG ĐẦU RA:
				CHỈ trả về DUY NHẤT một đối tượng JSON hợp lệ theo cấu trúc bên dưới. KHÔNG thêm lời chào, KHÔNG giải thích ngoài JSON, KHÔNG bọc markdown code block:
				{
				  "feedback": [
				{{jsonTemplateBuilder}}
				  ]
				}
				"""
			: $$"""
				Tôi cần viết nhận xét động viên cá nhân hóa bằng tiếng Việt cho {{items.Count}} câu hỏi kiểm tra từ vựng tiếng Nhật học viên vừa trả lời SAI, dựa trên toàn bộ dữ liệu câu hỏi và lịch sử học tập SRS đã cung cấp đầy đủ ở phần Learning unit ở trên (không cần thêm ngữ cảnh video hay transcript).

				YÊU CẦU ADAPTIVE FEEDBACK:
				Viết 1 lời nhận xét mang tính động viên, khích lệ ngắn (1-2 câu) bằng tiếng Việt tự nhiên cho TỪNG câu:
				- Dựa vào lịch sử học tập để đưa ra lời động viên phù hợp:
				  + Nếu từng nhớ đúng nay lại chọn nhầm: an ủi nhẹ và khuyên củng cố lại để không bị mai một trí nhớ.
				  + Nếu sai nhiều lần liên tiếp: động viên học viên kiên trì, đừng nản lòng, gợi ý nên ghi chú lại hoặc đặt câu ví dụ để khắc sâu.
				  + Nếu là lần đầu làm sai: động viên tinh thần, nhắc học viên chú ý ôn tập lại.
				- Xưng hô thân thiện là 'Bạn'.
				- BẮT BUỘC trả về đủ tất cả {{items.Count}} câu trong mảng feedback (từ index 1 đến index {{items.Count}}).

				QUAN TRỌNG VỀ ĐỊNH DẠNG ĐẦU RA:
				CHỈ trả về DUY NHẤT một đối tượng JSON hợp lệ theo cấu trúc bên dưới. KHÔNG thêm lời chào, KHÔNG giải thích ngoài JSON, KHÔNG bọc markdown code block:
				{
				  "feedback": [
				{{jsonTemplateBuilder}}
				  ]
				}
				""";

		using var response = await CallConverseForJsonAsync(
			userPrompt,
			itemsPromptBuilder.ToString(),
			isCorrect ? "quiz_feedback_correct" : "quiz_feedback_incorrect",
			cancellationToken);

		if (response is null)
		{
			return;
		}

		JsonElement feedbackElement;
		if (response.RootElement.ValueKind == JsonValueKind.Array)
		{
			feedbackElement = response.RootElement;
		}
		else if (response.RootElement.TryGetProperty("feedback", out var array) && array.ValueKind == JsonValueKind.Array)
		{
			feedbackElement = array;
		}
		else
		{
			_logger.LogWarning(
				"[QuizLLM][Bedrock] Could not find feedback array in response for group isCorrect={IsCorrect}.",
				isCorrect);
			return;
		}

		int arrayIndex = 0;
		foreach (var element in feedbackElement.EnumerateArray())
		{
			if (element.ValueKind != JsonValueKind.Object)
			{
				arrayIndex++;
				continue;
			}

			int? targetIndex = null;
			if (element.TryGetProperty("index", out var indexElement))
			{
				targetIndex = TryReadIntIndex(indexElement);
			}
			else if (element.TryGetProperty("id", out var idElement))
			{
				targetIndex = TryReadIntIndex(idElement);
			}

			if (!targetIndex.HasValue && arrayIndex < items.Count)
			{
				targetIndex = arrayIndex + 1;
			}
			arrayIndex++;

			if (targetIndex.HasValue && itemByIndex.TryGetValue(targetIndex.Value, out var questionId))
			{
				if (element.TryGetProperty("message", out var messageElement) &&
					messageElement.ValueKind == JsonValueKind.String &&
					!string.IsNullOrWhiteSpace(messageElement.GetString()))
				{
					result[questionId] = messageElement.GetString()!.Trim();
				}
			}
		}
	}

	private async Task<JsonDocument?> CallConverseForJsonAsync(
		string userPrompt,
		string learningUnitsContext,
		string purpose,
		CancellationToken cancellationToken)
	{
		var aiRequest = new AiChatRequest(
			History: [],
			Question: new AiChatTurn(IsUser: true, Text: userPrompt),
			VideoContext: string.Empty,
			TranscriptContext: string.Empty,
			LearningUnitsContext: string.IsNullOrWhiteSpace(learningUnitsContext) ? "None" : learningUnitsContext);

		var result = await _aiService.ConverseAsync(aiRequest, cancellationToken);

		if (result.Blocked)
		{
			_logger.LogWarning(
				"[QuizLLM][BedrockBlocked] Model={Model}; Purpose={Purpose}; InputTokens={InputTokens}; OutputTokens={OutputTokens}",
				_aiService.ModelId, purpose, result.InputTokens, result.OutputTokens);
			return null;
		}

		if (string.IsNullOrWhiteSpace(result.Text))
		{
			_logger.LogWarning(
				"[QuizLLM][BedrockEmptyResponse] Model={Model}; Purpose={Purpose}; InputTokens={InputTokens}; OutputTokens={OutputTokens}",
				_aiService.ModelId, purpose, result.InputTokens, result.OutputTokens);
			return null;
		}

		_logger.LogInformation(
			"[QuizLLM][Bedrock][RawResponse] Model={Model}; Purpose={Purpose}; InputTokens={InputTokens}; OutputTokens={OutputTokens}; Length={Length}",
			_aiService.ModelId, purpose, result.InputTokens, result.OutputTokens, result.Text.Length);

		return ParseJsonPayload(result.Text, purpose);
	}

	private JsonDocument? ParseJsonPayload(string rawContent, string purpose)
	{
		var candidate = StripThinkingBlocks(rawContent).Trim();
		if (candidate.Length == 0)
		{
			return null;
		}

		if (TryParseJson(candidate, out var directDoc))
		{
			return directDoc;
		}

		foreach (var fenced in ExtractMarkdownCodeBlocks(candidate))
		{
			if (TryParseJson(fenced, out var fencedDoc))
			{
				return fencedDoc;
			}
		}

		var firstObj = candidate.IndexOf('{');
		var firstArr = candidate.IndexOf('[');

		if (firstArr >= 0 && (firstObj < 0 || firstArr < firstObj))
		{
			if (TryParseBalancedJson(candidate, '[', ']', out var arrDoc) ||
				TryParseBalancedJson(candidate, '{', '}', out arrDoc))
			{
				return arrDoc;
			}
		}
		else
		{
			if (TryParseBalancedJson(candidate, '{', '}', out var objDoc) ||
				TryParseBalancedJson(candidate, '[', ']', out objDoc))
			{
				return objDoc;
			}
		}

		var preview = candidate.Length > 1000 ? candidate[..1000] : candidate;
		_logger.LogWarning(
			"[QuizLLM][BedrockInvalidJson] Model={Model}; Purpose={Purpose}; ResponsePreview={Preview}",
			_aiService.ModelId, purpose, preview);
		return null;
	}

	private static string StripThinkingBlocks(string text)
	{
		var current = text;
		while (true)
		{
			var start = current.IndexOf("<think>", StringComparison.OrdinalIgnoreCase);
			if (start < 0)
			{
				break;
			}

			var end = current.IndexOf("</think>", start, StringComparison.OrdinalIgnoreCase);
			if (end < 0)
			{
				current = current[..start];
				break;
			}

			current = string.Concat(current.AsSpan(0, start), current.AsSpan(end + 8));
		}

		return current;
	}

	private static IEnumerable<string> ExtractMarkdownCodeBlocks(string text)
	{
		int searchFrom = 0;
		while (searchFrom < text.Length)
		{
			var fenceStart = text.IndexOf("```", searchFrom, StringComparison.Ordinal);
			if (fenceStart < 0)
			{
				yield break;
			}

			var contentStart = text.IndexOf('\n', fenceStart + 3);
			if (contentStart < 0)
			{
				yield break;
			}
			contentStart++;

			var fenceEnd = text.IndexOf("```", contentStart, StringComparison.Ordinal);
			if (fenceEnd < 0)
			{
				yield break;
			}

			var block = text[contentStart..fenceEnd].Trim();
			if (block.Length > 0)
			{
				yield return block;
			}

			searchFrom = fenceEnd + 3;
		}
	}

	private static bool TryParseBalancedJson(
		string text,
		char openChar,
		char closeChar,
		out JsonDocument? document)
	{
		int searchFrom = 0;
		while (searchFrom < text.Length)
		{
			var start = text.IndexOf(openChar, searchFrom);
			if (start < 0)
			{
				break;
			}

			var end = FindMatchingClosingDelimiter(text, start, openChar, closeChar);
			if (end > start)
			{
				var slice = text[start..(end + 1)];
				if (TryParseJson(slice, out document))
				{
					return true;
				}
			}

			searchFrom = start + 1;
		}

		document = null;
		return false;
	}

	private static int FindMatchingClosingDelimiter(string text, int startIndex, char openChar, char closeChar)
	{
		int depth = 0;
		bool inString = false;
		bool escaped = false;

		for (int i = startIndex; i < text.Length; i++)
		{
			char ch = text[i];

			if (inString)
			{
				if (escaped)
				{
					escaped = false;
				}
				else if (ch == '\\')
				{
					escaped = true;
				}
				else if (ch == '"')
				{
					inString = false;
				}
				continue;
			}

			if (ch == '"')
			{
				inString = true;
			}
			else if (ch == openChar)
			{
				depth++;
			}
			else if (ch == closeChar)
			{
				depth--;
				if (depth == 0)
				{
					return i;
				}
			}
		}

		return -1;
	}

	private static bool TryParseJson(string json, out JsonDocument? document)
	{
		try
		{
			document = JsonDocument.Parse(json, LenientJsonOptions);
			return true;
		}
		catch (JsonException)
		{
			document = null;
			return false;
		}
	}

	private static int? TryReadIntIndex(JsonElement element)
	{
		if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var numericIdx))
		{
			return numericIdx;
		}

		if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString()?.Trim(), out var parsedIdx))
		{
			return parsedIdx;
		}

		return null;
	}

	private static string BuildHistoryContextText(QuizFeedbackPrompt item, bool isCorrect)
	{
		var lastEntry = item.PreviousFeedbackHistory?.FirstOrDefault();

		if (isCorrect)
		{
			if (lastEntry != null && !lastEntry.IsCorrect)
			{
				var prevWrongChoice = !string.IsNullOrWhiteSpace(lastEntry.SelectedAnswer)
					? $" (lần trước từng chọn nhầm là '{lastEntry.SelectedAnswer}')"
					: string.Empty;
				return $"Lần trước bạn từng trả lời SAI từ này{prevWrongChoice}, nhưng lần này bạn đã khắc phục và chọn ĐÚNG!";
			}

			if (item.PreviousAttemptCount >= 2 && item.PreviousIncorrectCount == 0)
			{
				return $"Bạn đã trả lời ĐÚNG liên tiếp {item.PreviousAttemptCount + 1} lần qua các bài ôn tập!";
			}

			if (item.PreviousAttemptCount > 0)
			{
				return "Bạn tiếp tục duy trì việc ghi nhớ đúng từ này.";
			}

			return "Lần đầu tiên bạn làm bài kiểm tra với từ vựng này và đã trả lời đúng.";
		}
		else
		{
			if (lastEntry != null && lastEntry.IsCorrect)
			{
				return "Lần trước bạn đã nhớ và trả lời ĐÚNG từ này rồi, nhưng lần này lại bị nhầm lẫn.";
			}

			if (item.PreviousIncorrectCount >= 1)
			{
				return $"Bạn đã bị trả lời SAI từ này {item.PreviousIncorrectCount + 1} lần rồi.";
			}

			return "Lần đầu tiên bạn gặp câu hỏi về từ này và đã chọn nhầm.";
		}
	}
}

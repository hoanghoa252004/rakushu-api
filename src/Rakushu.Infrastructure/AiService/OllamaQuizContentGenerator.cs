using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;

namespace Rakushu.Infrastructure.AiService;

public sealed class OllamaQuizContentGenerator : IQuizContentGenerator
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
	private readonly HttpClient _httpClient;
	private readonly ILogger<OllamaQuizContentGenerator> _logger;
	private readonly string _model;

	public OllamaQuizContentGenerator(
		HttpClient httpClient,
		IConfiguration configuration,
		ILogger<OllamaQuizContentGenerator> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
		var baseUrl = configuration["QuizLlm:BaseUrl"] ?? "http://localhost:11434";
		_model = configuration["QuizLlm:Model"] ?? "qwen3:4b";
		if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
			(baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
		{
			throw new InvalidOperationException("QuizLlm:BaseUrl must be an absolute HTTP(S) URL.");
		}

		_httpClient.BaseAddress = new Uri(baseUri.ToString().TrimEnd('/') + "/");
		var timeoutSeconds = int.TryParse(configuration["QuizLlm:TimeoutSeconds"], out var configuredTimeout)
			? Math.Clamp(configuredTimeout, 5, 86400)
			: 7200;
		_httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
	}

	private string NormalizeHexEncodedJapanese(string? value, Guid cardId)
	{
		if (string.IsNullOrWhiteSpace(value)) return value ?? string.Empty;
		var candidate = value.Trim();
		if (candidate.Length < 4 || candidate.Length > 128 || candidate.Length % 4 != 0 || candidate.Any(character => !Uri.IsHexDigit(character))) return value;
		try
		{
			var decoded = System.Text.Encoding.BigEndianUnicode.GetString(Convert.FromHexString(candidate));
			var hasJapaneseOrCjk = decoded.Any(character => character is >= '\u3040' and <= '\u30FF' or >= '\u3400' and <= '\u9FFF' or >= '\uF900' and <= '\uFAFF');
			if (!hasJapaneseOrCjk || decoded.Any(char.IsControl) || decoded.Any(char.IsSurrogate)) return value;
			_logger.LogWarning(
				"[QuizLLM][InputNormalization] Decoded hex-encoded Japanese text for card {CardId}. EncodedLength={EncodedLength}; DecodedValue={DecodedValue}",
				cardId, candidate.Length, decoded);
			return decoded;
		}
		catch (FormatException) { return value; }
	}

	public async Task<IReadOnlyList<GeneratedQuizQuestion>> GenerateQuestionsAsync(
		string questionFormat,
		IReadOnlyList<QuizGenerationCard> cards,
		CancellationToken cancellationToken = default)
	{
		if (!string.Equals(questionFormat, "MC", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentOutOfRangeException(nameof(questionFormat), "Only MC question format is currently supported.");
		}

		if (cards.Count < 1 || cards.Count > 50)
		{
			throw new ArgumentOutOfRangeException(nameof(cards), "Quiz generation requires 1 to 50 cards.");
		}

		if (cards.Count > 3)
		{
			const int maxBatchSize = 3;
			var allQuestions = new List<GeneratedQuizQuestion>(cards.Count);
			var offset = 0;
			var batchNumber = 1;
			while (offset < cards.Count)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var remaining = cards.Count - offset;
				var batchSize = remaining == 4 ? 2 : Math.Min(maxBatchSize, remaining);
				var batch = cards.Skip(offset).Take(batchSize).ToArray();
				_logger.LogInformation("Generating quiz batch {BatchNumber} with {CardCount} cards using model {Model}.", batchNumber, batch.Length, _model);
				var batchQuestions = await GenerateQuestionsAsync(questionFormat, batch, cancellationToken);
				allQuestions.AddRange(batchQuestions);
				_logger.LogInformation("Quiz batch {BatchNumber} generated {QuestionCount} questions.", batchNumber, batchQuestions.Count);
				offset += batchSize;
				batchNumber++;
			}

			return allQuestions;
		}

		var normalizedCards = cards.Select(card => new QuizGenerationCard(
			card.CardId,
			NormalizeHexEncodedJapanese(card.Expression, card.CardId),
			NormalizeHexEncodedJapanese(card.Reading, card.CardId),
			card.Meaning,
			card.QuestionDirection)).ToArray();
		// Do not expose GUIDs as generation identifiers. Short aliases are easier for small local models to copy exactly;
		// the backend maps aliases back to the authoritative GUIDs after validating the complete batch.
		var cardIdsByAlias = normalizedCards
			.Select((card, index) => new { Alias = $"CARD_{index + 1:D3}", card.CardId })
			.ToDictionary(item => item.Alias, item => item.CardId, StringComparer.OrdinalIgnoreCase);
		var payload = JsonSerializer.Serialize(normalizedCards.Select((card, index) => new
		{
			cardId = $"CARD_{index + 1:D3}",
			expression = card.Expression,
			authoritativeMeaning = card.Meaning,
			questionDirection = card.QuestionDirection
		}), JsonOptions);

		var prompt = $$"""
            You create Japanese vocabulary learning quizzes for Vietnamese learners.
            Generate exactly one multiple-choice (MC) question for every input card.
            Return JSON only, using this schema:
            {
              "questions": [
                {
                  "cardId": "copy the exact input cardId",
                  "prompt": "a natural, varied learner-facing question prompt in Vietnamese or Japanese, as appropriate"
                }
              ]
            }
            Rules:
            - Do not omit, duplicate, or invent cardId values.
            - Generate only the question prompt. The backend selects all four answer choices from trusted database data.
            - Do not generate distractors, answer choices, an answer key, or an explanation.

            - Respect each input card's questionDirection.
            - For JapaneseToVietnamese, show the Japanese expression and ask for its Vietnamese meaning. Choices are Vietnamese meanings. Example: "会社 có nghĩa là gì trong tiếng Việt?"
            - For VietnameseToJapanese, show authoritativeMeaning and ask which Japanese expression has that meaning. Choices are Japanese expressions. Example: "Từ tiếng Nhật nào có nghĩa là 'công ty'?" Do not include the target Japanese expression in this prompt.
            - Never contradict the requested direction or reveal the answer in the prompt. Vary wording naturally; never replace supplied text with an ID, hexadecimal string, byte sequence, or placeholder. Do not include a reading/pronunciation in the prompt; the backend supplies the authoritative reading separately.


            - Do not include any text outside JSON.
            INPUT CARDS:
            {{payload}}
            """;

		for (var generationAttempt = 1; generationAttempt <= 2; generationAttempt++)
		{
			try
			{
				var attemptPrompt = generationAttempt == 1
					? prompt
					: prompt + "\nRETRY REQUIREMENT: Return exactly one question for every supplied CARD_NNN alias. Copy each alias character-for-character. Do not omit or duplicate any alias.";
				using var response = await CallForJsonAsync(attemptPrompt, "quiz_question_generation", cancellationToken);
				var root = response.RootElement;
				if (!root.TryGetProperty("questions", out var questionsElement) ||
					questionsElement.ValueKind != JsonValueKind.Array)
				{
					throw new JsonException("LLM response does not contain a questions array.");
				}

				var questions = new List<GeneratedQuizQuestion>();
				var seenAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (var element in questionsElement.EnumerateArray())
				{
					if (!element.TryGetProperty("cardId", out var cardIdElement) ||
						cardIdElement.ValueKind != JsonValueKind.String ||
						!element.TryGetProperty("prompt", out var promptElement) ||
						promptElement.ValueKind != JsonValueKind.String)
					{
						throw new JsonException("LLM returned an invalid quiz question.");
					}

					var alias = cardIdElement.GetString()?.Trim();
					if (string.IsNullOrWhiteSpace(alias) ||
						!cardIdsByAlias.TryGetValue(alias, out var cardId) ||
						!seenAliases.Add(alias))
					{
						throw new JsonException("LLM returned an unknown or duplicate card alias.");
					}

					var questionPrompt = promptElement.GetString()?.Trim();
					if (string.IsNullOrWhiteSpace(questionPrompt) || questionPrompt.Length > 500)
					{
						throw new JsonException("LLM returned an empty or oversized quiz prompt.");
					}

					questions.Add(new GeneratedQuizQuestion(cardId, questionPrompt));
				}

				if (questions.Count != normalizedCards.Length || seenAliases.Count != cardIdsByAlias.Count)
				{
					throw new JsonException($"LLM returned {questions.Count} questions for {normalizedCards.Length} cards.");
				}

				_logger.LogInformation(
					"[QuizLLM][ParsedQuestions] Model={Model}; ParsedCount={QuestionCount}; Questions={Questions}",
					_model, questions.Count, JsonSerializer.Serialize(questions, JsonOptions));
				return questions;
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception exception) when (generationAttempt == 1)
			{
				_logger.LogWarning(exception,
					"[QuizLLM][Retry] Invalid quiz generation response for {CardCount} cards. Retrying once.", normalizedCards.Length);
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception,
					"[QuizLLM][Fallback] Quiz generation remained invalid after retry for {CardCount} cards. Using deterministic prompts.", normalizedCards.Length);
				return normalizedCards.Select(card => new GeneratedQuizQuestion(
					card.CardId,
					string.Equals(card.QuestionDirection, "VietnameseToJapanese", StringComparison.OrdinalIgnoreCase)
						? $"Từ tiếng Nhật nào có nghĩa là '{card.Meaning.Trim()}'?"
						: $"Từ tiếng Nhật {card.Expression} có nghĩa là gì trong tiếng Việt?"))
					.ToArray();
			}
		}

		throw new JsonException("Quiz question generation failed unexpectedly.");
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
				var correctFeedback = await GenerateFeedbackGroupAsync(correctQuestions, isCorrect: true, cancellationToken);
				foreach (var (itemId, message) in correctFeedback)
				{
					feedback[itemId] = message;
				}
			}

			if (incorrectQuestions.Count > 0)
			{
				var incorrectFeedback = await GenerateFeedbackGroupAsync(incorrectQuestions, isCorrect: false, cancellationToken);
				foreach (var (itemId, message) in incorrectFeedback)
				{
					feedback[itemId] = message;
				}
			}

			_logger.LogInformation(
				"[QuizLLM][ParsedFeedback] Model={Model}; TotalFeedbackCount={FeedbackCount}; ItemIds={ItemIds}",
				_model, feedback.Count, string.Join(",", feedback.Keys));

			return feedback;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "LLM adaptive quiz feedback failed.");
			return new Dictionary<Guid, string>();
		}
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
				return $"Lần trước bạn từng trả lời SAI từ này{prevWrongChoice}, nhưng lần này bạn đã xuất sắc khắc phục và chọn ĐÚNG!";
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
				return $"Bạn đã bị trả lời SAI từ này {item.PreviousIncorrectCount + 1} lần rồi!";
			}

			return "Lần đầu tiên bạn gặp câu hỏi về từ này và đã chọn nhầm.";
		}
	}

	private async Task<Dictionary<Guid, string>> GenerateFeedbackGroupAsync(
		IReadOnlyList<QuizFeedbackPrompt> items,
		bool isCorrect,
		CancellationToken cancellationToken)
	{
		var result = new Dictionary<Guid, string>();
		if (items.Count == 0)
			return result;

		var itemByIndex = new Dictionary<int, Guid>();
		var itemsPromptBuilder = new System.Text.StringBuilder();

		for (int i = 0; i < items.Count; i++)
		{
			var index = i + 1;
			var item = items[i];
			itemByIndex[index] = item.ItemId;

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

		var jsonTemplateBuilder = new System.Text.StringBuilder();
		for (int i = 0; i < items.Count; i++)
		{
			var idx = i + 1;
			jsonTemplateBuilder.Append($"    {{ \"index\": {idx}, \"message\": \"...\" }}");
			if (i < items.Count - 1)
				jsonTemplateBuilder.Append(",");
			jsonTemplateBuilder.AppendLine();
		}

		string systemPrompt = "Bạn là gia sư AI tiếng Nhật Rakushu. Bạn luôn xưng hô là 'Bạn', đưa ra nhận xét cá nhân hóa (Adaptive Feedback) dựa trên lịch sử học tập và trả về JSON hợp lệ.";
		string prompt;

		if (isCorrect)
		{
			prompt = $$"""
				Dưới đây là các câu hỏi kiểm tra từ vựng tiếng Nhật học viên vừa trả lời ĐÚNG, kèm theo lịch sử học tập SRS:
				{{itemsPromptBuilder}}

				YÊU CẦU ADAPTIVE FEEDBACK:
				Viết 1 câu nhận xét ngắn bằng tiếng Việt tự nhiên và cá nhân hóa cho TỪNG câu:
				- Hãy tự dùng vốn từ phong phú, sinh động của bạn, tuyệt đối KHÔNG sao chép nguyên văn các câu chữ hướng dẫn này.
				- Nếu từng làm sai nay làm đúng: bày tỏ vui mừng hoặc khen ngợi sự tiến bộ vượt bậc khi đã khắc phục được lỗi sai.
				- Nếu đúng liên tiếp: khen ngợi khả năng ghi nhớ vững vàng, phong độ ổn định.
				- Nếu là lần đầu: động viên khởi đầu tốt.
				- Xưng hô thân thiện là 'Bạn'.
				- BẮT BUỘC trả về đủ tất cả {{items.Count}} câu trong mảng feedback (từ index 1 đến index {{items.Count}}).

				Chỉ trả về JSON hợp lệ có cấu trúc:
				{
				  "feedback": [
				{{jsonTemplateBuilder}}
				  ]
				}
				""";
		}
		else
		{
			prompt = $$"""
				Dưới đây là các câu hỏi kiểm tra từ vựng tiếng Nhật học viên vừa trả lời SAI, kèm theo lịch sử học tập SRS:
				{{itemsPromptBuilder}}

				YÊU CẦU ADAPTIVE FEEDBACK:
				Viết 1 câu nhận xét ngắn bằng tiếng Việt tự nhiên nhắc học viên ôn lại từ và chú ý đáp án đúng (TUYỆT ĐỐI không khen ngợi vì học viên đã trả lời sai):
				- Hãy tự dùng vốn từ phong phú, sinh động của bạn, tuyệt đối KHÔNG sao chép nguyên văn các câu chữ hướng dẫn này.
				- Nếu từng nhớ đúng nay lại chọn nhầm: tiếc nuối nhẹ và khuyên củng cố lại để không bị rơi rụng trí nhớ.
				- Nếu sai nhiều lần liên tiếp: khuyên học viên chú ý đặc biệt, nên ghi chú lại hoặc đặt câu ví dụ để khắc sâu.
				- Nếu là lần đầu làm sai: chỉ ra đáp án đúng và nhắc ôn tập nhẹ nhàng.
				- Xưng hô thân thiện là 'Bạn'.
				- BẮT BUỘC trả về đủ tất cả {{items.Count}} câu trong mảng feedback (từ index 1 đến index {{items.Count}}).

				Chỉ trả về JSON hợp lệ có cấu trúc:
				{
				  "feedback": [
				{{jsonTemplateBuilder}}
				  ]
				}
				""";
		}

		try
		{
			using var response = await CallForJsonAsync(
				prompt,
				isCorrect ? "quiz_feedback_correct" : "quiz_feedback_incorrect",
				cancellationToken,
				systemPrompt);

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
				_logger.LogWarning("[QuizLLM] Could not find feedback array in LLM response for group isCorrect={IsCorrect}.", isCorrect);
				return result;
			}

			int arrayIndex = 0;
			foreach (var element in feedbackElement.EnumerateArray())
			{
				int? targetIndex = null;
				if (element.TryGetProperty("index", out var indexElement))
				{
					if (indexElement.ValueKind == JsonValueKind.Number && indexElement.TryGetInt32(out var idx))
					{
						targetIndex = idx;
					}
					else if (indexElement.ValueKind == JsonValueKind.String && int.TryParse(indexElement.GetString(), out var parsedIdx))
					{
						targetIndex = parsedIdx;
					}
				}
				else if (element.TryGetProperty("id", out var idElement))
				{
					var idStr = idElement.GetString()?.Trim();
					if (idStr != null && int.TryParse(idStr, out var parsedIdx))
					{
						targetIndex = parsedIdx;
					}
				}

				if (!targetIndex.HasValue && arrayIndex < items.Count)
				{
					targetIndex = arrayIndex + 1;
				}
				arrayIndex++;

				if (targetIndex.HasValue && itemByIndex.TryGetValue(targetIndex.Value, out var itemId))
				{
					string? message = null;
					if (element.TryGetProperty("message", out var messageElement) &&
						messageElement.ValueKind == JsonValueKind.String)
					{
						message = messageElement.GetString()?.Trim();
					}
					else
					{
						// Fallback: look for ANY string property in case of slight key malformation
						foreach (var prop in element.EnumerateObject())
						{
							if (prop.Value.ValueKind == JsonValueKind.String &&
								!string.IsNullOrWhiteSpace(prop.Value.GetString()) &&
								!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(prop.Name, "index", StringComparison.OrdinalIgnoreCase))
							{
								message = prop.Value.GetString()?.Trim();
								break;
							}
						}
					}

					if (!string.IsNullOrWhiteSpace(message))
					{
						// Sanity check: if item is incorrect, do not allow praise keywords
						if (!isCorrect && (message.Contains("chúc mừng", StringComparison.OrdinalIgnoreCase) ||
										   message.Contains("chính xác", StringComparison.OrdinalIgnoreCase) ||
										   message.Contains("xuất sắc", StringComparison.OrdinalIgnoreCase) ||
										   message.Contains("làm tốt", StringComparison.OrdinalIgnoreCase)))
						{
							_logger.LogWarning("[QuizLLM] Rejected praise feedback for incorrect item {ItemId}: {Message}", itemId, message);
						}
						else
						{
							result[itemId] = message;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "[QuizLLM] Failed generating feedback for group isCorrect={IsCorrect}.", isCorrect);
		}

		return result;
	}

	private async Task<JsonDocument> CallForJsonAsync(
		string prompt,
		string purpose,
		CancellationToken cancellationToken,
		string? systemPrompt = null)
	{
		var request = new
		{
			model = _model,
			messages = new[]
			{
				new { role = "system", content = systemPrompt ?? "You are Rakushu's quiz content engine. Return valid JSON only." },
				new { role = "user", content = prompt }
			},
			stream = false,
			format = "json",
			think = false,
			keep_alive = "30m",
			options = new
			{
				temperature = purpose == "quiz_question_generation" ? 0.7 : 0.5,
				repeat_penalty = 1.15,
				top_p = 0.9,
				num_predict = 4096
			}
		};

		using var response = await _httpClient.PostAsJsonAsync("api/chat", request, cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			if (errorBody.Length > 2000)
				errorBody = errorBody[..2000];

			_logger.LogError(
				"Ollama request {Purpose} failed with HTTP {StatusCode}. Response body (truncated to 2000 chars): {ResponseBody}",
				purpose, (int)response.StatusCode, errorBody);
			response.EnsureSuccessStatusCode();
		}

		var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
		using var body = JsonDocument.Parse(responseBody);
		if (!body.RootElement.TryGetProperty("message", out var message) ||
			message.ValueKind != JsonValueKind.Object ||
			!message.TryGetProperty("content", out var content) ||
			content.ValueKind != JsonValueKind.String ||
			string.IsNullOrWhiteSpace(content.GetString()))
		{
			var rootProperties = body.RootElement.ValueKind == JsonValueKind.Object
				? string.Join(", ", body.RootElement.EnumerateObject().Select(property => property.Name))
				: body.RootElement.ValueKind.ToString();
			var messageProperties = message.ValueKind == JsonValueKind.Object
				? string.Join(", ", message.EnumerateObject().Select(property => property.Name))
				: "missing-or-not-an-object";
			var error = body.RootElement.ValueKind == JsonValueKind.Object && body.RootElement.TryGetProperty("error", out var errorElement)
				? errorElement.ToString()
				: null;
			var done = body.RootElement.ValueKind == JsonValueKind.Object && body.RootElement.TryGetProperty("done", out var doneElement)
				? doneElement.ToString()
				: "missing";
			var doneReason = body.RootElement.ValueKind == JsonValueKind.Object && body.RootElement.TryGetProperty("done_reason", out var doneReasonElement)
				? doneReasonElement.ToString()
				: "missing";
			var truncatedBody = responseBody.Length > 2000 ? responseBody[..2000] : responseBody;
			_logger.LogError(
				"Ollama request {Purpose} returned no usable message.content. Root properties: [{RootProperties}]. Message properties: [{MessageProperties}]. Done: {Done}. Done reason: {DoneReason}. Error: {OllamaError}. Response body (truncated to 2000 chars): {ResponseBody}",
				purpose, rootProperties, messageProperties, done, doneReason, error, truncatedBody);
			throw new JsonException($"Ollama response for {purpose} did not contain usable message.content (done={done}, done_reason={doneReason}, error={error ?? "none"}).");
		}

		var contentText = content.GetString()!;
		_logger.LogInformation(
			"[QuizLLM][RawResponse] Model={Model}; Purpose={Purpose}; FullContent={Content}",
			_model, purpose, contentText);

		var sanitizedJson = contentText.Trim();
		if (sanitizedJson.Contains("<think>"))
		{
			var thinkEnd = sanitizedJson.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
			if (thinkEnd >= 0)
			{
				sanitizedJson = sanitizedJson[(thinkEnd + 8)..].Trim();
			}
		}

		if (sanitizedJson.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
		{
			sanitizedJson = sanitizedJson[7..];
		}
		else if (sanitizedJson.StartsWith("```", StringComparison.OrdinalIgnoreCase))
		{
			sanitizedJson = sanitizedJson[3..];
		}

		if (sanitizedJson.EndsWith("```", StringComparison.OrdinalIgnoreCase))
		{
			sanitizedJson = sanitizedJson[..^3];
		}

		return JsonDocument.Parse(sanitizedJson.Trim());
	}
}

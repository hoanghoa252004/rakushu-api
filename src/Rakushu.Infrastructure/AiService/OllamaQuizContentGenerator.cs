using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AiService;

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
			: 60;
		_httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
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
				foreach (var (questionId, message) in correctFeedback)
				{
					feedback[questionId] = message;
				}
			}

			if (incorrectQuestions.Count > 0)
			{
				var incorrectFeedback = await GenerateFeedbackGroupAsync(incorrectQuestions, isCorrect: false, cancellationToken);
				foreach (var (questionId, message) in incorrectFeedback)
				{
					feedback[questionId] = message;
				}
			}

			_logger.LogInformation(
				"[QuizLLM][ParsedFeedback] Model={Model}; TotalFeedbackCount={FeedbackCount}; QuestionIds={QuestionIds}",
				_model, feedback.Count, string.Join(",", feedback.Keys));

			return feedback;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "[QuizLLM] LLM adaptive quiz feedback failed: {Message}", exception.Message);
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

		try
		{
			var itemsBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < prompts.Count; i++)
			{
				var p = prompts[i];
				var idx = i + 1;
				itemsBuilder.AppendLine($"Mục {idx} (meaningId: \"{p.MeaningId}\"):");
				itemsBuilder.AppendLine($"- Từ/ngữ pháp: {p.Expression}");
				if (!string.IsNullOrWhiteSpace(p.Reading))
					itemsBuilder.AppendLine($"- Cách đọc: {p.Reading}");
				itemsBuilder.AppendLine($"- Nghĩa tiếng Việt: {p.Meaning}");
				if (!string.IsNullOrWhiteSpace(p.ContextSentence))
					itemsBuilder.AppendLine($"- Câu ngữ cảnh: {p.ContextSentence}");
				itemsBuilder.AppendLine();
			}

			var jsonTemplateBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < prompts.Count; i++)
			{
				var p = prompts[i];
				jsonTemplateBuilder.Append($$"""
					    {
					      "meaningId": "{{p.MeaningId}}",
					      "prompt": "Từ/ngữ pháp 「{{p.Expression}}」 có ý nghĩa là gì?",
					      "explanation": "「{{p.Expression}}」{{(string.IsNullOrWhiteSpace(p.Reading) ? "" : $"({p.Reading})")}} có nghĩa là: {{p.Meaning}}."
					    }
					""");
				if (i < prompts.Count - 1)
					jsonTemplateBuilder.Append(",");
				jsonTemplateBuilder.AppendLine();
			}

			const string systemPrompt = "Bạn là chuyên gia sư phạm tiếng Nhật của Rakushu. Bạn luôn trả về JSON hợp lệ theo đúng cấu trúc yêu cầu, không kèm bất kỳ giải thích ngoài JSON.";

			var prompt = $$"""
				Dưới đây là danh sách từ vựng/ngữ pháp tiếng Nhật cần tạo câu hỏi trắc nghiệm và lời giải thích:
				{{itemsBuilder}}

				YÊU CẦU:
				1. Với mỗi từ vựng, hãy viết một câu hỏi trắc nghiệm ngắn gọn, tự nhiên bằng tiếng Việt (trường "prompt") để kiểm tra ý nghĩa của từ vựng đó.
				2. Viết một lời giải thích ngắn gọn, rõ ràng (trường "explanation") về ý nghĩa, cách đọc và cách dùng của từ vựng đó trong tiếng Nhật.
				3. BẮT BUỘC trả về đủ tất cả {{prompts.Count}} mục ứng với meaningId tương ứng.

				Chỉ trả về JSON hợp lệ có cấu trúc chính xác như sau:
				{
				  "items": [
				{{jsonTemplateBuilder}}
				  ]
				}
				""";

			using var responseDoc = await CallForJsonAsync(
				prompt,
				"generate_quiz_content",
				cancellationToken,
				systemPrompt);

			var result = new Dictionary<Guid, GeneratedQuizPromptAndExplanation>();

			if (responseDoc.RootElement.TryGetProperty("items", out var itemsElement) &&
				itemsElement.ValueKind == JsonValueKind.Array)
			{
				foreach (var el in itemsElement.EnumerateArray())
				{
					if (el.TryGetProperty("meaningId", out var idProp) &&
						Guid.TryParse(idProp.GetString(), out var meaningId) &&
						el.TryGetProperty("prompt", out var promptProp) &&
						el.TryGetProperty("explanation", out var expProp))
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

			_logger.LogInformation(
				"[QuizLLM][GeneratedContent] Model={Model}; Generated {Count}/{Total} items",
				_model, result.Count, prompts.Count);

			return result;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "[QuizLLM] LLM quiz content generation failed: {Message}", exception.Message);
			return new Dictionary<Guid, GeneratedQuizPromptAndExplanation>();
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

		var jsonTemplateBuilder = new System.Text.StringBuilder();
		for (int i = 0; i < items.Count; i++)
		{
			var idx = i + 1;
			jsonTemplateBuilder.Append($"    {{ \"index\": {idx}, \"message\": \"<lời nhận xét cho câu {idx}>\" }}");
			if (i < items.Count - 1)
				jsonTemplateBuilder.Append(",");
			jsonTemplateBuilder.AppendLine();
		}

		const string systemPrompt = "Bạn là gia sư AI tiếng Nhật Rakushu. Bạn luôn xưng hô là 'Bạn', luôn trả về mảng feedback JSON hợp lệ theo đúng cấu trúc yêu cầu. Tuyệt đối không từ chối hoặc xuất thông báo lỗi.";
		string prompt;

		if (isCorrect)
		{
			prompt = $$"""
				Dưới đây là các câu hỏi kiểm tra từ vựng tiếng Nhật học viên vừa trả lời ĐÚNG, kèm theo lịch sử học tập SRS:
				{{itemsPromptBuilder}}

				YÊU CẦU ADAPTIVE FEEDBACK:
				Viết 1 câu nhận xét ngắn (1-2 câu) bằng tiếng Việt tự nhiên và cá nhân hóa cho TỪNG câu:
				- Nếu từng làm sai nay làm đúng: chúc mừng sự tiến bộ khi đã khắc phục được lỗi sai.
				- Nếu đúng liên tiếp nhiều lần: khen ngợi khả năng ghi nhớ vững vàng, phong độ ổn định.
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
				Viết 1 câu nhận xét mang tính động viên, khích lệ ngắn (1-2 câu) bằng tiếng Việt tự nhiên cho TỪNG câu:
				- Dựa vào lịch sử học tập để đưa ra lời động viên phù hợp:
				  + Nếu từng nhớ đúng nay lại chọn nhầm: an ủi nhẹ và khuyên củng cố lại để không bị mai một trí nhớ.
				  + Nếu sai nhiều lần liên tiếp: động viên học viên kiên trì, đừng nản lòng, gợi ý nên ghi chú lại hoặc đặt câu ví dụ để khắc sâu.
				  + Nếu là lần đầu làm sai: động viên tinh thần, nhắc học viên chú ý ôn tập lại.
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

				if (targetIndex.HasValue && itemByIndex.TryGetValue(targetIndex.Value, out var questionId))
				{
					if (element.TryGetProperty("message", out var messageElement) &&
						messageElement.ValueKind == JsonValueKind.String &&
						!string.IsNullOrWhiteSpace(messageElement.GetString()))
					{
						var message = messageElement.GetString()!.Trim();
						result[questionId] = message;
					}
				}
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
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
				temperature = 0.3,
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
				"Ollama request {Purpose} failed with HTTP {StatusCode}. Response body (truncated): {ResponseBody}",
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
			throw new JsonException($"Ollama response for {purpose} did not contain usable message.content.");
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

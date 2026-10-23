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
			? Math.Clamp(configuredTimeout, 5, 3600)
			: 3600;
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
				var seenAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);				foreach (var element in questionsElement.EnumerateArray())
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

	private static bool IsCompleteAdaptiveFeedback(string? message) =>
		!string.IsNullOrWhiteSpace(message) &&
		message.Trim().Length <= 180;

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
			var payload = JsonSerializer.Serialize(questions.Select(question => new
			{
				itemId = question.ItemId,
				isCorrect = question.IsCorrect,
				previousAttemptCount = question.PreviousAttemptCount,
				previousIncorrectCount = question.PreviousIncorrectCount
			}), JsonOptions);

			var prompt = $$"""
                You give brief, personalized memory-retention feedback in Vietnamese after a quiz.
                For each item, use ONLY isCorrect and the supplied per-card history counts. These counts describe this learner's completed prior attempts for this card; the current attempt is not included. Do not explain vocabulary, meanings, grammar, or examples, and do not repeat the answer.
                Personalization rules:
                - If previousAttemptCount is 0: treat this as the learner's first recorded encounter; encourage or reassure based on isCorrect.
                - If isCorrect is true and previousIncorrectCount > 0: acknowledge that the learner got it right after earlier mistakes; encourage maintaining the progress without claiming a long-term trend.
                - If isCorrect is true and previousIncorrectCount is 0: acknowledge successful recall; when previousAttemptCount > 0, encourage consistency.
                - If isCorrect is false and previousIncorrectCount >= 2: gently identify that this card has been difficult repeatedly and suggest active recall or another short review.
                - If isCorrect is false and previousIncorrectCount == 1: note that this was missed before and suggest another focused review.
                - Otherwise, if incorrect after a prior correct attempt, encourage reviewing the card again without blaming the learner.
                Keep each message to one short Vietnamese sentence, at most 160 characters. Never change the correctness result, infer response speed, or invent history.
                Return JSON only with this schema:
                {"feedback":[{"itemId":"exact input GUID","message":"one short Vietnamese sentence about memory/recall"}]}
                Return exactly one feedback item per input item, preserve itemId, and do not include markdown formatting.
                INPUT:
                {{payload}}
                """;

			using var response = await CallForJsonAsync(prompt, "quiz_adaptive_feedback", cancellationToken);
			if (!response.RootElement.TryGetProperty("feedback", out var feedbackElement) ||
				feedbackElement.ValueKind != JsonValueKind.Array)
			{
				return new Dictionary<Guid, string>();
			}

			var expectedItemIds = questions.Select(question => question.ItemId).ToHashSet();
			var feedback = new Dictionary<Guid, string>();
			foreach (var element in feedbackElement.EnumerateArray())
			{
				if (!element.TryGetProperty("itemId", out var idElement) ||
					!Guid.TryParse(idElement.GetString(), out var itemId) ||
					!expectedItemIds.Contains(itemId) ||
					feedback.ContainsKey(itemId) ||
					!element.TryGetProperty("message", out var messageElement) ||
					messageElement.ValueKind != JsonValueKind.String)
				{
					continue;
				}

				var message = messageElement.GetString()?.Trim();
				if (IsCompleteAdaptiveFeedback(message))
				{
					feedback[itemId] = message!;
				}
			}

			_logger.LogInformation(
				"[QuizLLM][ParsedFeedback] Model={Model}; ParsedCount={FeedbackCount}; ItemIds={ItemIds}",
				_model, feedback.Count, string.Join(",", feedback.Keys));
			return feedback;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "LLM adaptive quiz feedback failed; deterministic fallback will be used.");
			return new Dictionary<Guid, string>();
		}
	}

	private async Task<JsonDocument> CallForJsonAsync(
		string prompt,
		string purpose,
		CancellationToken cancellationToken)
	{
		var request = new
		{
			model = _model,
			messages = new[]
			{
				new { role = "system", content = "You are Rakushu's quiz content engine. Return valid JSON only." },
				new { role = "user", content = prompt }
			},
			stream = false,
			format = "json",
			think = false,
			keep_alive = "30m",
			options = new { temperature = purpose == "quiz_question_generation" ? 0.7 : 0.2, num_predict = 4096 }
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
		if (purpose == "quiz_question_generation")
		{
			// Question output is vocabulary content, useful at Information level for quiz-generation diagnostics.
			_logger.LogInformation(
				"[QuizLLM][RawResponse] Model={Model}; Purpose={Purpose}; FullContent={Content}",
				_model, purpose, contentText);
		}
		else
		{
			// Feedback can include learner performance history, so keep its raw content at Debug level.
			_logger.LogDebug(
				"[QuizLLM][RawResponse] Model={Model}; Purpose={Purpose}; FullContent={Content}",
				_model, purpose, contentText);
		}

		return JsonDocument.Parse(contentText);
	}

}

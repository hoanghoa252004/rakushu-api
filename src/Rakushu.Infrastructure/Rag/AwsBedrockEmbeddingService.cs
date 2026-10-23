using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Rag;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Rag;

public sealed class AwsBedrockEmbeddingService : IEmbeddingService
{
	private readonly IAmazonBedrockRuntime _embeddingClient;
	private readonly AwsSettings _awsSettings;

	public AwsBedrockEmbeddingService(IAmazonBedrockRuntime embeddingClient, IOptions<AwsSettings> awsSettings)
	{
		_embeddingClient = embeddingClient;
		_awsSettings = awsSettings.Value;
	}

	public async Task<float[]> GenerateEmbeddingAsync(
	string input,
	CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentException(
				"Input text cannot be empty.",
				nameof(input));
		}

		var requestBody = new
		{
			inputText = input,
			dimensions = 1024,
			normalize = true
		};

		var json = JsonSerializer.Serialize(requestBody);

		var request = new InvokeModelRequest
		{
			ModelId = _awsSettings.BedrockModelId,
			ContentType = "application/json",
			Accept = "application/json",
			Body = new MemoryStream(
				Encoding.UTF8.GetBytes(json))
		};

		var response = await _embeddingClient.InvokeModelAsync(
			request,
			cancellationToken);

		using var reader = new StreamReader(response.Body);

		var responseJson = await reader.ReadToEndAsync(
			cancellationToken);

		using var document = JsonDocument.Parse(responseJson);

		var root = document.RootElement;

		if (!root.TryGetProperty("embedding", out var embeddingElement))
		{
			throw new InvalidOperationException(
				$"Bedrock response does not contain 'embedding'. " +
				$"ModelId: '{_awsSettings.BedrockModelId}'. " +
				$"Response: {responseJson}");
		}

		if (embeddingElement.ValueKind != JsonValueKind.Array)
		{
			throw new InvalidOperationException(
				"Bedrock response property 'embedding' is not an array.");
		}

		var embedding = embeddingElement
			.EnumerateArray()
			.Select(x => x.GetSingle())
			.ToArray();

		if (embedding.Length != 1024)
		{
			throw new InvalidOperationException(
				$"Expected 1024 embedding dimensions, " +
				$"but received {embedding.Length}. " +
				$"ModelId: '{_awsSettings.BedrockModelId}'.");
		}

		return embedding;
	}
}

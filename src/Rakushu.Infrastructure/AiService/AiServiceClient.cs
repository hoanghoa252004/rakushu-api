using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.AiService;

public sealed class AiServiceClient : IAiServiceClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<AiServiceClient> _logger;

	public AiServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<AiServiceClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;

		var baseUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
		if (!baseUrl.EndsWith('/'))
		{
			baseUrl += "/";
		}
		_httpClient.BaseAddress = new Uri(baseUrl);
		_httpClient.Timeout = TimeSpan.FromSeconds(10);
	}

	public async Task<bool> SyncDictionaryEntryAsync(
		string term,
		string reading,
		string pos,
		string meaning,
		string? definitionTags = "curator-verified",
		string? originalTerm = null,
		string? status = "ADAPTED",
		CancellationToken cancellationToken = default)
	{
		try
		{
			var payload = new
			{
				term,
				reading,
				pos,
				meaning,
				definition_tags = definitionTags,
				original_term = originalTerm,
				status
			};

			var response = await _httpClient.PostAsJsonAsync("api/v1/internal/dictionary/sync", payload, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("Successfully synced '{Term}' to AI Service dictionary.", term);
				return true;
			}

			_logger.LogWarning("AI Service returned {StatusCode} when syncing '{Term}'.", response.StatusCode, term);
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to connect to AI Service to sync '{Term}'. Offline sync queued.", term);
			return false;
		}
	}

	public async Task<bool> SyncOovStatusAsync(
		string term,
		string status,
		string? candidateId = null,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var payload = new
			{
				term,
				status,
				candidate_id = candidateId
			};

			var response = await _httpClient.PostAsJsonAsync("api/v1/internal/oov/sync-status", payload, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("Successfully synced status '{Status}' for '{Term}' to AI Service.", status, term);
				return true;
			}

			_logger.LogWarning("AI Service returned {StatusCode} when syncing status for '{Term}'.", response.StatusCode, term);
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to connect to AI Service to sync status for '{Term}'.", term);
			return false;
		}
	}
}

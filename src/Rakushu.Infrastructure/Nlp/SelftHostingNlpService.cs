using Rakushu.Application.Abstractions.Infrastructure.Nlp;
using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Nlp;

public sealed class SelftHostingNlpService : INlpService
{
	private readonly HttpClient _httpClient;

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public SelftHostingNlpService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<NlpAnalysisResponse> AnalyzeAsync(
		IReadOnlyCollection<NlpSegmentRequest> segments,
		CancellationToken cancellationToken)
	{
		using var response = await _httpClient.PostAsJsonAsync(
			"/api/analyze",
			segments,
			JsonOptions,
			cancellationToken);

		response.EnsureSuccessStatusCode();

		var result = await response.Content.ReadFromJsonAsync<NlpAnalysisResponse>(
			JsonOptions,
			cancellationToken);

		return result
			?? throw new InvalidOperationException(
				"NLP service returned an empty response.");
	}
}
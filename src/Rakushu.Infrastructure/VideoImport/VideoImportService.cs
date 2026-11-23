using System.Net;
using System.Net.Http.Headers;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Infrastructure.VideoImport;
using Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;
using VideoLibrary;

namespace Rakushu.Infrastructure.VideoImport;

internal sealed class VideoImportService : IVideoImportService
{
	private const int PartSizeBytes = 8 * 1024 * 1024;
	private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

	private static readonly HttpClient DownloadClient = CreateClient();

	private readonly IStorageService _storageService;

	public VideoImportService(IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<VideoImportResultDto> ImportAsync(string sourceUrl, CancellationToken cancellationToken)
	{
		var videos = (await YouTube.Default.GetAllVideosAsync(sourceUrl)).ToList();

		var video = videos
			.Where(v => v.AudioBitrate > 0 && v.Format == VideoFormat.Mp4)
			.OrderByDescending(v => v.Resolution)
			.FirstOrDefault()
			?? videos.OrderByDescending(v => v.Resolution).FirstOrDefault()
			?? throw new InvalidOperationException("No downloadable video stream was found for the given URL.");

		var extension = string.IsNullOrWhiteSpace(video.FileExtension) ? ".mp4" : video.FileExtension;
		var contentType = extension.Equals(".webm", StringComparison.OrdinalIgnoreCase) ? "video/webm" : "video/mp4";
		var key = $"videos/{Guid.NewGuid():N}{extension}";

		var upload = await _storageService.CreateMultipartUploadAsync(key, contentType, cancellationToken);

		try
		{
			var parts = new List<CompletedPartDto>();
			var totalBytes = await UploadRangedAsync(video, key, upload.UploadId, parts, cancellationToken)
				?? await UploadFromBytesAsync(video, key, upload.UploadId, parts, cancellationToken);

			await _storageService.CompleteMultipartUploadAsync(key, upload.UploadId, parts, cancellationToken);

			return new VideoImportResultDto(key, contentType, Path.GetFileName(key), totalBytes);
		}
		catch
		{
			await _storageService.AbortMultipartUploadAsync(key, upload.UploadId, CancellationToken.None);
			throw;
		}
	}

	// Download by ranged requests (YouTube rejects full-length requests with 403). Returns null when ranged download is not permitted.
	private async Task<long?> UploadRangedAsync(
		YouTubeVideo video,
		string key,
		string uploadId,
		List<CompletedPartDto> parts,
		CancellationToken cancellationToken)
	{
		long offset = 0;
		long? total = null;
		var partNumber = 1;

		var baseUri = video.Uri;
		var clenMatch = System.Text.RegularExpressions.Regex.Match(baseUri, @"[?&]clen=(\d+)");
		if (!clenMatch.Success)
			return null;
		total = long.Parse(clenMatch.Groups[1].Value);

		while (offset < total)
		{
			// YouTube CDN expects the range as a query parameter rather than a Range header
			var end = Math.Min(offset + PartSizeBytes, total.Value) - 1;
			using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUri}&range={offset}-{end}");

			using var response = await DownloadClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

			if (!response.IsSuccessStatusCode)
				return null;

			var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
			if (bytes.Length == 0)
				break;

			await UploadPartAsync(key, uploadId, partNumber++, bytes, bytes.Length, parts, cancellationToken);
			offset += bytes.Length;
		}

		return offset;
	}

	private async Task<long> UploadFromBytesAsync(
		YouTubeVideo video,
		string key,
		string uploadId,
		List<CompletedPartDto> parts,
		CancellationToken cancellationToken)
	{
		parts.Clear();
		var data = await video.GetBytesAsync();

		var partNumber = 1;
		for (var offset = 0; offset < data.Length; offset += PartSizeBytes)
		{
			var length = Math.Min(PartSizeBytes, data.Length - offset);
			var chunk = new byte[length];
			Buffer.BlockCopy(data, offset, chunk, 0, length);
			await UploadPartAsync(key, uploadId, partNumber++, chunk, length, parts, cancellationToken);
		}

		return data.Length;
	}

	private async Task UploadPartAsync(
		string key,
		string uploadId,
		int partNumber,
		byte[] buffer,
		int length,
		List<CompletedPartDto> parts,
		CancellationToken cancellationToken)
	{
		await using var partStream = new MemoryStream(buffer, 0, length, writable: false);
		var eTag = await _storageService.UploadMultipartPartAsync(key, uploadId, partNumber, partStream, cancellationToken);
		parts.Add(new CompletedPartDto(partNumber, eTag));
	}

	private static HttpClient CreateClient()
	{
		var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
		client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
		return client;
	}
}

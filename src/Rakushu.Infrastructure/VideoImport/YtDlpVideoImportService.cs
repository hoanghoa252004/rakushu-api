using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Infrastructure.VideoImport;
using Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Rakushu.Infrastructure.VideoImport;

internal sealed class YtDlpVideoImportService : IVideoImportService
{
	private const int PartSizeBytes = 8 * 1024 * 1024;

	private readonly IStorageService _storageService;
	private readonly YtDlpBinaries _binaries;
	private readonly ILogger<YtDlpVideoImportService> _logger;

	public YtDlpVideoImportService(IStorageService storageService, YtDlpBinaries binaries, ILogger<YtDlpVideoImportService> logger)
	{
		_storageService = storageService;
		_binaries = binaries;
		_logger = logger;
	}

	public async Task<VideoImportResultDto> ImportAsync(string sourceUrl, CancellationToken cancellationToken)
	{
		var workFolder = Path.Combine(Path.GetTempPath(), "rakushu-import", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(workFolder);

		try
		{
			var ytdl = new YoutubeDL
			{
				YoutubeDLPath = _binaries.YtDlpPath,
				FFmpegPath = _binaries.FFmpegPath,
				OutputFolder = workFolder,
				OverwriteFiles = true
			};

			_logger.LogInformation(
				"yt-dlp import started. Url={Url}, YtDlp={YtDlp} (exists={YtDlpExists}), FFmpeg={FFmpeg} (exists={FFmpegExists}), WorkFolder={Folder}",
				sourceUrl, ytdl.YoutubeDLPath, File.Exists(ytdl.YoutubeDLPath), ytdl.FFmpegPath, File.Exists(ytdl.FFmpegPath), workFolder);

			var output = new Progress<string>(line => _logger.LogInformation("[yt-dlp] {Line}", line));

			var options = new OptionSet();
			if (!string.IsNullOrWhiteSpace(_binaries.DenoPath))
				options.AddCustomOption("--js-runtimes", $"deno:{_binaries.DenoPath}");

			var result = await ytdl.RunVideoDownload(
				sourceUrl,
				format: "best[ext=mp4]/bestvideo[ext=mp4]+bestaudio[ext=m4a]/best",
				mergeFormat: DownloadMergeFormat.Mp4,
				ct: cancellationToken,
				output: output,
				overrideOptions: options);

			var filesInFolder = Directory.GetFiles(workFolder);
			_logger.LogInformation(
				"yt-dlp finished. Success={Success}, Data={Data}, Files=[{Files}]",
				result.Success, result.Data, string.Join(", ", filesInFolder.Select(Path.GetFileName)));

			var filePath = !string.IsNullOrWhiteSpace(result.Data) && File.Exists(result.Data)
				? result.Data
				: filesInFolder.FirstOrDefault(f => !f.EndsWith(".part", StringComparison.OrdinalIgnoreCase));

			if (filePath is null)
			{
				var errors = string.Join(Environment.NewLine, result.ErrorOutput ?? Array.Empty<string>());
				_logger.LogError("yt-dlp produced no file. Success={Success}. ErrorOutput: {Errors}", result.Success, errors);
				throw new InvalidOperationException($"yt-dlp failed (Success={result.Success}): {errors}");
			}

			_logger.LogInformation("Uploading {File} ({Size} bytes) to storage...", filePath, new FileInfo(filePath).Length);
			return await UploadFileAsync(filePath, cancellationToken);
		}
		finally
		{
			try { Directory.Delete(workFolder, recursive: true); } catch { /* best effort cleanup */ }
		}
	}

	private async Task<VideoImportResultDto> UploadFileAsync(string filePath, CancellationToken cancellationToken)
	{
		var extension = Path.GetExtension(filePath);
		var contentType = extension.Equals(".webm", StringComparison.OrdinalIgnoreCase) ? "video/webm" : "video/mp4";
		var key = $"videos/{Guid.NewGuid():N}{extension}";

		var upload = await _storageService.CreateMultipartUploadAsync(key, contentType, cancellationToken);

		try
		{
			var parts = new List<CompletedPartDto>();
			long totalBytes = 0;
			var partNumber = 1;
			var buffer = new byte[PartSizeBytes];

			await using var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);

			while (true)
			{
				var read = 0;
				while (read < buffer.Length)
				{
					var n = await file.ReadAsync(buffer.AsMemory(read), cancellationToken);
					if (n == 0) break;
					read += n;
				}

				if (read == 0)
					break;

				await using (var partStream = new MemoryStream(buffer, 0, read, writable: false))
				{
					var eTag = await _storageService.UploadMultipartPartAsync(key, upload.UploadId, partNumber, partStream, cancellationToken);
					parts.Add(new CompletedPartDto(partNumber, eTag));
				}

				totalBytes += read;
				partNumber++;

				if (read < buffer.Length)
					break;
			}

			await _storageService.CompleteMultipartUploadAsync(key, upload.UploadId, parts, cancellationToken);

			return new VideoImportResultDto(key, contentType, Path.GetFileName(key), totalBytes);
		}
		catch
		{
			await _storageService.AbortMultipartUploadAsync(key, upload.UploadId, CancellationToken.None);
			throw;
		}
	}
}

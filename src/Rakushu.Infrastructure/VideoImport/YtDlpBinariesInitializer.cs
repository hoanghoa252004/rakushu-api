using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeDLSharp;

namespace Rakushu.Infrastructure.VideoImport;

// Holds resolved paths of yt-dlp / ffmpeg binaries
internal sealed class YtDlpBinaries
{
	public string YtDlpPath { get; set; } = "yt-dlp";
	public string FFmpegPath { get; set; } = "ffmpeg";
	public string? DenoPath { get; set; }
}

// Downloads yt-dlp, ffmpeg and deno once when the worker starts
internal sealed class YtDlpBinariesInitializer(
	YtDlpBinaries binaries,
	IConfiguration configuration,
	ILogger<YtDlpBinariesInitializer> logger) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var folder = configuration["YtDlp:BinariesFolder"]
			?? Path.Combine(AppContext.BaseDirectory, "tools");
		Directory.CreateDirectory(folder);

		try
		{
			logger.LogInformation("Preparing yt-dlp tools in {Folder} (first run may take several minutes)...", folder);

			if (File.Exists(Path.Combine(folder, Utils.YtDlpBinaryName)))
			{
				Interlocked.Increment(ref _step);
				logger.LogInformation("[{Step}/3] yt-dlp already exists, reused.", _step);
			}
			else
				await RunStepAsync("yt-dlp", folder, () => Utils.DownloadYtDlp(folder));

			if (File.Exists(Path.Combine(folder, Utils.FfmpegBinaryName)))
			{
				Interlocked.Increment(ref _step);
				logger.LogInformation("[{Step}/3] ffmpeg already exists, reused.", _step);
			}
			else
				await RunStepAsync("ffmpeg", folder, () => Utils.DownloadFFmpeg(folder));

			// for YouTube downloads
			var denoPath = Path.Combine(folder, OperatingSystem.IsWindows() ? "deno.exe" : "deno");
			if (File.Exists(denoPath))
			{
				Interlocked.Increment(ref _step);
				logger.LogInformation("[{Step}/3] deno already exists, reused.", _step);
			}
			else
				await RunStepAsync("deno", folder, () => Utils.DownloadDeno(folder));

			binaries.YtDlpPath = Path.Combine(folder, Utils.YtDlpBinaryName);
			binaries.FFmpegPath = Path.Combine(folder, Utils.FfmpegBinaryName);
			binaries.DenoPath = File.Exists(denoPath) ? denoPath : null;
			if (binaries.DenoPath is null)
				logger.LogWarning("deno not found at {Path}; YouTube extraction may fail.", denoPath);

			logger.LogInformation("yt-dlp tools are ready in {Folder}.", folder);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to download yt-dlp binaries.");
		}
	}

	private int _step;

	private async Task RunStepAsync(string name, string folder, Func<Task> download)
	{
		var step = Interlocked.Increment(ref _step);
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("[{Step}/3] Downloading {Name}...", step, name);

		using var heartbeatCts = new CancellationTokenSource();
		var heartbeat = Task.Run(async () =>
		{
			try
			{
				while (!heartbeatCts.IsCancellationRequested)
				{
					await Task.Delay(TimeSpan.FromSeconds(5), heartbeatCts.Token);
					var sizeMb = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
						.Sum(f => new FileInfo(f).Length) / 1024d / 1024d;
					logger.LogInformation("[{Step}/3] Still downloading {Name}... {Elapsed:F0}s elapsed, folder size {Size:F1} MB",
						step, name, stopwatch.Elapsed.TotalSeconds, sizeMb);
				}
			}
			catch (OperationCanceledException) { }
			catch { /* heartbeat is best effort */ }
		});

		try
		{
			await download();
			logger.LogInformation("[{Step}/3] {Name} ready in {Elapsed:F1}s.", step, name, stopwatch.Elapsed.TotalSeconds);
		}
		finally
		{
			heartbeatCts.Cancel();
			await heartbeat;
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

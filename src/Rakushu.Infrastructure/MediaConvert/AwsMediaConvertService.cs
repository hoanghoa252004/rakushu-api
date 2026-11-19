using Amazon.MediaConvert;
using Amazon.MediaConvert.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Infrastructure.Extensions.Options;
using System.Collections.Concurrent;

namespace Rakushu.Infrastructure.AudioClips;

internal sealed class AwsMediaConvertService : IMediaConvertService
{
	private const int MaxConcurrentJobs = 5;
	// Compensates the observed clip drift: start was ~0.5s too early, end was ~0.5s cut off.
	private const double StartPaddingSeconds = 0.1;
	private const double EndPaddingSeconds = 0.5;
	private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
	private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);
	private static readonly SemaphoreSlim JobSlots = new(MaxConcurrentJobs, MaxConcurrentJobs);

	private readonly IAmazonMediaConvert _mediaConvert;
	private readonly AwsRuntimeSettings _settings;
	private readonly IConfiguration _configuration;

	public AwsMediaConvertService(IAmazonMediaConvert mediaConvert, AwsRuntimeSettings options, IConfiguration configuration)
	{
		_mediaConvert = mediaConvert;
		_settings = options;
		_configuration = configuration;
	}

	public async Task<string> ExtractClipAsync(
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken)
	{
		await JobSlots.WaitAsync(cancellationToken);
		try
		{
			return await RunJobAsync(sourceKey, outputKeyWithoutExtension, start, end, cancellationToken);
		}
		finally
		{
			JobSlots.Release();
		}
	}

	private async Task<string> RunJobAsync(
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken)
	{
		var source = await GetSourceInfoAsync(sourceKey, cancellationToken);
		var request = BuildClipRequest(sourceKey, outputKeyWithoutExtension, start, end, source, null);

		var created = await _mediaConvert.CreateJobAsync(request, cancellationToken);
		var jobId = created.Job.Id;
		var deadline = DateTime.UtcNow + Timeout;

		while (DateTime.UtcNow < deadline)
		{
			await Task.Delay(PollInterval, cancellationToken);

			var job = (await _mediaConvert.GetJobAsync(new GetJobRequest { Id = jobId }, cancellationToken)).Job;

			if (job.Status == JobStatus.COMPLETE)
				return outputKeyWithoutExtension + ".m4a";

			if (job.Status == JobStatus.ERROR || job.Status == JobStatus.CANCELED)
				throw new InvalidOperationException($"MediaConvert job {jobId} failed: {job.ErrorMessage}");
		}

		throw new TimeoutException($"MediaConvert job {jobId} timed out.");
	}

	public async Task CreateClipJobAsync(
		Guid roleLineId,
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken)
	{
		var source = await GetSourceInfoAsync(sourceKey, cancellationToken);
		var request = BuildClipRequest(sourceKey, outputKeyWithoutExtension, start, end, source, new Dictionary<string, string>
		{
			[RoleLineIdMetadataKey] = roleLineId.ToString(),
			[PurposeMetadataKey] = ClipPurpose
		});
		await _mediaConvert.CreateJobAsync(request, cancellationToken);
	}

	public async Task<IReadOnlyList<NormalizeJobInfo>> ListClipJobsAsync(
		IReadOnlyCollection<Guid> roleLineIds,
		CancellationToken cancellationToken)
	{
		var wanted = roleLineIds.Select(i => i.ToString()).ToHashSet();
		var result = new List<NormalizeJobInfo>();
		if (wanted.Count == 0)
			return result;

		foreach (var (status, mapped) in JobStatusMap)
		{
			string? nextToken = null;
			for (var page = 0; page < 25; page++)
			{
				var response = await _mediaConvert.ListJobsAsync(new ListJobsRequest
				{
					Status = status,
					Order = Order.DESCENDING,
					MaxResults = 20,
					NextToken = nextToken
				}, cancellationToken);

				foreach (var job in response.Jobs ?? [])
				{
					if (job.UserMetadata is null
						|| !job.UserMetadata.TryGetValue(PurposeMetadataKey, out var purpose) || purpose != ClipPurpose
						|| !job.UserMetadata.TryGetValue(RoleLineIdMetadataKey, out var lineId)
						|| !wanted.Contains(lineId))
						continue;

					result.Add(new NormalizeJobInfo(Guid.Parse(lineId), mapped, job.ErrorMessage));
				}

				nextToken = response.NextToken;
				if (string.IsNullOrEmpty(nextToken))
					break;
			}
		}

		return result;
	}

	private const string ClipPurpose = "shadowing-clip";

	private static readonly (JobStatus Status, NormalizeJobStatus Mapped)[] JobStatusMap =
	[
		(JobStatus.SUBMITTED, NormalizeJobStatus.InProgress),
		(JobStatus.PROGRESSING, NormalizeJobStatus.InProgress),
		(JobStatus.COMPLETE, NormalizeJobStatus.Complete),
		(JobStatus.ERROR, NormalizeJobStatus.Failed),
		(JobStatus.CANCELED, NormalizeJobStatus.Failed)
	];

	private CreateJobRequest BuildClipRequest(
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		SourceInfo source,
		Dictionary<string, string>? userMetadata)
	{
		var clipStart = Math.Max(0, start.TotalSeconds + StartPaddingSeconds);
		var clipEnd = Math.Min(end.TotalSeconds + EndPaddingSeconds, source.DurationSeconds);

		var request = new CreateJobRequest
		{
			UserMetadata = userMetadata,
			Role = _configuration["AwsSettings:MediaConvertRoleArn"],
			Settings = new JobSettings
			{
				Inputs =
				[
					new Input
					{
						FileInput = $"s3://{_settings.BucketName}/{sourceKey}",
						AudioSelectors = new Dictionary<string, AudioSelector>
						{
							["Audio Selector 1"] = new AudioSelector { DefaultSelection = AudioDefaultSelection.DEFAULT }
						},
						InputClippings =
						[
							new InputClipping
							{
								StartTimecode = ToTimecode(clipStart, source.NominalFps, roundUp: false),
								EndTimecode = ToTimecode(clipEnd, source.NominalFps, roundUp: true)
							}
						],
						TimecodeSource = InputTimecodeSource.ZEROBASED
					}
				],
				OutputGroups =
				[
					new OutputGroup
					{
						Name = "File Group",
						OutputGroupSettings = new OutputGroupSettings
						{
							Type = OutputGroupType.FILE_GROUP_SETTINGS,
							FileGroupSettings = new FileGroupSettings
							{
								Destination = $"s3://{_settings.BucketName}/{outputKeyWithoutExtension}"
							}
						},
						Outputs =
						[
							new Output
							{
								Extension = "m4a",
								ContainerSettings = new ContainerSettings { Container = ContainerType.MP4 },
								AudioDescriptions =
								[
									new AudioDescription
									{
										AudioSourceName = "Audio Selector 1",
										CodecSettings = new AudioCodecSettings
										{
											Codec = AudioCodec.AAC,
											AacSettings = new AacSettings
											{
												Bitrate = 96000,
												CodingMode = AacCodingMode.CODING_MODE_2_0,
												SampleRate = 44100
											}
										}
									}
								]
							}
						]
					}
				]
			}
		};

		return request;
	}

	private sealed record SourceInfo(int NominalFps, double DurationSeconds);

	private static readonly ConcurrentDictionary<string, Lazy<Task<SourceInfo>>> SourceCache = new();

	private async Task<SourceInfo> GetSourceInfoAsync(string sourceKey, CancellationToken cancellationToken)
	{
		var lazy = SourceCache.GetOrAdd(sourceKey, key => new Lazy<Task<SourceInfo>>(() => ProbeAsync(key, CancellationToken.None)));
		try
		{
			return await lazy.Value.WaitAsync(cancellationToken);
		}
		catch (Exception ex) when (ex is not OperationCanceledException)
		{
			SourceCache.TryRemove(sourceKey, out _);
			throw;
		}
	}

	private async Task<SourceInfo> ProbeAsync(string sourceKey, CancellationToken cancellationToken)
	{
		var response = await _mediaConvert.ProbeAsync(new ProbeRequest
		{
			InputFiles = [new ProbeInputFile { FileUrl = $"s3://{_settings.BucketName}/{sourceKey}" }]
		}, cancellationToken);

		var container = response.ProbeResults?.FirstOrDefault()?.Container;
		var fps = container?.Tracks?
			.Select(t => t.VideoProperties?.FrameRate)
			.FirstOrDefault(f => f?.Numerator > 0 && f.Denominator > 0);

		var frameRate = fps is null ? 30.0 : (double)fps.Numerator!.Value / fps.Denominator!.Value;
		return new SourceInfo(
			Math.Max(1, (int)Math.Round(frameRate)),
			container?.Duration ?? double.MaxValue);
	}

	// Start is floored and end is ceiled to a whole frame so no audio is cut off at the clip boundaries.
	private static string ToTimecode(double seconds, int fps, bool roundUp)
	{
		var exact = Math.Max(0, seconds) * fps;
		var totalFrames = (long)(roundUp ? Math.Ceiling(exact - 1e-6) : Math.Floor(exact + 1e-6));

		var frames = (int)(totalFrames % fps);
		var totalSeconds = totalFrames / fps;
		var s = (int)(totalSeconds % 60);
		var m = (int)(totalSeconds / 60 % 60);
		var h = (int)(totalSeconds / 3600);
		return $"{h:00}:{m:00}:{s:00}:{frames:00}";
	}

	private CreateJobRequest BuildNormalizeRequest(
		string sourceKey,
		string outputKeyWithoutExtension,
		Dictionary<string, string>? userMetadata)
	{
		var inputUrl = $"s3://{_settings.BucketName}/{sourceKey}";
		var outputDestination =
			$"s3://{_settings.BucketName}/{outputKeyWithoutExtension}";

		return new CreateJobRequest
		{
			Role = _configuration["AwsSettings:MediaConvertRoleArn"],
			UserMetadata = userMetadata,

			Settings = new JobSettings
			{
				Inputs =
				[
					new Input
				{
					FileInput = inputUrl,

					AudioSelectors = new Dictionary<string, AudioSelector>
					{
						["Audio Selector 1"] = new AudioSelector
						{
							DefaultSelection = AudioDefaultSelection.DEFAULT
						}
					}
				}
				],

				OutputGroups =
				[
					new OutputGroup
				{
					Name = "Normalized WAV Output",

					OutputGroupSettings = new OutputGroupSettings
					{
						Type = OutputGroupType.FILE_GROUP_SETTINGS,

						FileGroupSettings = new FileGroupSettings
						{
							Destination = outputDestination
						}
					},

					Outputs =
					[
						new Output
						{
							Extension = "wav",

							ContainerSettings = new ContainerSettings
							{
								Container = ContainerType.RAW
							},

							AudioDescriptions =
							[
								new AudioDescription
								{
									AudioSourceName = "Audio Selector 1",

									CodecSettings = new AudioCodecSettings
									{
										Codec = AudioCodec.WAV,

										WavSettings = new WavSettings
										{
											BitDepth = 16,
											Channels = 1,
											SampleRate = 16000,
											Format = WavFormat.RIFF
										}
									}
								}
							]
						}
					]
				}
				]
			}
		};
	}

	private const string AttemptIdMetadataKey = "attemptId";
	private const string RoleLineIdMetadataKey = "roleLineId";
	private const string PurposeMetadataKey = "purpose";
	private const string NormalizePurpose = "shadowing-normalize";

	public async Task CreateNormalizeJobAsync(
		Guid attemptId,
		Guid roleLineId,
		string sourceKey,
		string outputKeyWithoutExtension,
		CancellationToken cancellationToken)
	{
		var request = BuildNormalizeRequest(sourceKey, outputKeyWithoutExtension, new Dictionary<string, string>
		{
			[AttemptIdMetadataKey] = attemptId.ToString(),
			[RoleLineIdMetadataKey] = roleLineId.ToString(),
			[PurposeMetadataKey] = NormalizePurpose
		});
		await _mediaConvert.CreateJobAsync(request, cancellationToken);
	}

	public async Task<IReadOnlyList<NormalizeJobInfo>> ListNormalizeJobsAsync(
		Guid attemptId,
		CancellationToken cancellationToken)
	{
		const int maxPagesPerStatus = 25;
		var attemptKey = attemptId.ToString();
		var result = new List<NormalizeJobInfo>();

		var statuses = new (JobStatus Status, NormalizeJobStatus Mapped)[]
		{
			(JobStatus.SUBMITTED, NormalizeJobStatus.InProgress),
			(JobStatus.PROGRESSING, NormalizeJobStatus.InProgress),
			(JobStatus.COMPLETE, NormalizeJobStatus.Complete),
			(JobStatus.ERROR, NormalizeJobStatus.Failed),
			(JobStatus.CANCELED, NormalizeJobStatus.Failed)
		};

		foreach (var (status, mapped) in statuses)
		{
			string? nextToken = null;
			for (var page = 0; page < maxPagesPerStatus; page++)
			{
				var response = await _mediaConvert.ListJobsAsync(new ListJobsRequest
				{
					Status = status,
					Order = Order.DESCENDING,
					MaxResults = 20,
					NextToken = nextToken
				}, cancellationToken);

				foreach (var job in response.Jobs ?? [])
				{
					if (job.UserMetadata is null
						|| !job.UserMetadata.TryGetValue(AttemptIdMetadataKey, out var id) || id != attemptKey
						|| !job.UserMetadata.TryGetValue(RoleLineIdMetadataKey, out var lineId)
						|| !Guid.TryParse(lineId, out var roleLineId))
						continue;

					result.Add(new NormalizeJobInfo(roleLineId, mapped, job.ErrorMessage));
				}

				nextToken = response.NextToken;
				if (string.IsNullOrEmpty(nextToken))
					break;
			}
		}

		return result;
	}

	public async Task<string> NormalizeForSpeechAssessmentAsync(
		string sourceKey,
		string outputKeyWithoutExtension,
		CancellationToken cancellationToken)
	{
		var jobRequest = BuildNormalizeRequest(sourceKey, outputKeyWithoutExtension, null);

		await JobSlots.WaitAsync(cancellationToken);

		try
		{
			var response = await _mediaConvert.CreateJobAsync(
				jobRequest,
				cancellationToken);

			var jobId = response.Job?.Id;

			if (string.IsNullOrWhiteSpace(jobId))
			{
				throw new InvalidOperationException(
					"MediaConvert did not return a Job ID.");
			}

			await WaitForJobCompletionAsync(jobId, cancellationToken);

			return $"{outputKeyWithoutExtension}.wav";
		}
		finally
		{
			JobSlots.Release();
		}
	}

	private async Task WaitForJobCompletionAsync(
	string jobId,
	CancellationToken cancellationToken)
	{
		var deadline = DateTime.UtcNow + Timeout;

		while (DateTime.UtcNow < deadline)
		{
			await Task.Delay(PollInterval, cancellationToken);

			var response = await _mediaConvert.GetJobAsync(
				new GetJobRequest
				{
					Id = jobId
				},
				cancellationToken);

			var job = response.Job;

			if (job.Status == JobStatus.COMPLETE)
			{
				return;
			}
			else if (job.Status == JobStatus.ERROR ||
					 job.Status == JobStatus.CANCELED)
			{
				throw new InvalidOperationException(
					$"MediaConvert job {jobId} failed. " +
					$"Status: {job.Status}. " +
					$"Error: {job.ErrorMessage}");
			}
		}

		throw new TimeoutException(
			$"MediaConvert job {jobId} timed out.");
	}
}

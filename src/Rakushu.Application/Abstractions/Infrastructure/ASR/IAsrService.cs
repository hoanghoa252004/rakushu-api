using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.ASR;

public interface IAsrService
{
	Task<StartTranscriptionResult> StartAsync(StartTranscriptionRequest request, CancellationToken cancellationToken = default);
	Task<TranscriptionJobResult?> GetJobAsync(string jobName, CancellationToken cancellationToken = default);
	Task<bool> CancelAsync(string jobName, CancellationToken cancellationToken = default);
}


public sealed record StartTranscriptionRequest(
	string JobName,
	string MediaUri,
	string MediaFormat,
	string LanguageCode,
	bool EnableSpeakerIdentification = true,
	int MaxSpeakerLabels = 5);

public sealed record StartTranscriptionResult(
	string JobName,
	string Status);

public sealed record TranscriptionJobResult(
	string JobName,
	string Status,
	string? TranscriptUri,
	string? FailureReason);
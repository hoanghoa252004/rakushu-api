using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure;

public interface IAiServiceClient
{
	Task<bool> SyncDictionaryEntryAsync(
		string term,
		string reading,
		string pos,
		string meaning,
		string? definitionTags = "curator-verified",
		string? originalTerm = null,
		string? status = "ADAPTED",
		CancellationToken cancellationToken = default);

	Task<bool> SyncOovStatusAsync(
		string term,
		string status,
		string? candidateId = null,
		CancellationToken cancellationToken = default);
}

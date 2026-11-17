using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Rag;

public interface IEmbeddingService
{
	string ModelId { get; }

	Task<float[]> GenerateEmbeddingAsync(
		string input,
		CancellationToken cancellationToken = default);
}

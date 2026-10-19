using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Nlp;

public interface INlpService
{
	Task<NlpAnalysisResponse> AnalyzeAsync(
		IReadOnlyCollection<NlpSegmentRequest> segments,
		CancellationToken cancellationToken);
}

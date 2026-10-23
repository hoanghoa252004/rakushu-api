using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Extentions;

public static class DomainServiceCollectionExtentions
{
	public static IServiceCollection AddRakushuDomain(this IServiceCollection services)
	{
		services.AddSingleton<TranscriptSegmentBuilder>();

		services.AddRakushuSpecifications();

		services.AddRakushuPolicies();
		
		return services;
	}
}

using Microsoft.Extensions.DependencyInjection;
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
		services.AddRakushuSpecifications();

		services.AddRakushuPolicies();
		
		return services;
	}
}

using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Entities.ContentCategory.Specifications;
using Rakushu.Domain.Entities.ProficiencyLevel.Specifications;
using Rakushu.Domain.Entities.User.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Extentions;

public static class SpecificationExtension
{
	public static IServiceCollection AddRakushuSpecifications(this IServiceCollection services)
	{
		services.AddScoped<ActiveUserSpecification>();
		services.AddScoped<ActiveProficiencyLevelSpecification>();
		services.AddScoped<ActiveContentCategorySpecification>();

		return services;
	}
}

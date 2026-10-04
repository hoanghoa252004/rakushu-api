using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Entities.ContentCategory.Specifications;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.Specifications;
using Rakushu.Domain.Entities.SupportedLanguage.Specifications;
using Rakushu.Domain.Entities.User.Profile.Policies;
using Rakushu.Domain.Entities.User.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Extentions;

public static class PolicyExtension
{
	public static IServiceCollection AddRakushuPolicies(this IServiceCollection services)
	{
		services.AddScoped<ProfileUpdatePolicy>();

		return services;
	}
}
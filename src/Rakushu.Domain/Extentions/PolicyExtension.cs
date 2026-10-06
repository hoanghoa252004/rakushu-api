using Microsoft.Extensions.DependencyInjection;
using Rakushu.Domain.Entities.User.Profile.Policies;
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
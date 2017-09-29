using System;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Staging.FareDeals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Staging
{
	public static class StagingExtensions
	{
		public static IServiceCollection AddFareDealStaging(this IServiceCollection services, IConfigurationRoot configuration)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			services.AddTransient<IStartable, FareDealStagingStartable>();

			services.Configure<FareDealStagingOptions>(configuration.GetSection("Staging:FareDeals"));

			return services;
		}
	}
}

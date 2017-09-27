using System;
using Diskordia.Columbus.Bots.FareDeals;
using Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines;
using Diskordia.Columbus.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Bots
{
	public static class BotsExtensions
	{
		public static IServiceCollection AddFareDealBots(this IServiceCollection services, IConfigurationRoot configuration)
		{
			if(services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			services.AddTransient<IFareDealService, SingaporeAirlinesFareDealService>();
			services.AddTransient<IStartable, FareDealServiceStartable>();

			services.Configure<FareDealOptions>(configuration.GetSection("FareDeals"));

			return services;
		}
	}
}

using System;
using Diskordia.Columbus.Bots.FareDeals;
using Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Bots
{
	public static class BotsExtensions
	{
		public static IServiceCollection AddFareDealBots(this IServiceCollection services, IConfiguration configuration)
		{
			if(services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			services.AddTransient<IFareDealScanService, SingaporeAirlinesFareDealService>();

			services.Configure<SingaporeAirlinesOptions>(configuration.GetSection("FareDealScan:SingaporeAirlines"));
			services.Configure<FareDealScanOptions>(configuration.GetSection("FareDealScan"));

			return services;
		}
	}
}

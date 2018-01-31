using System;
using Diskordia.Columbus.FareDealScanner.FareDeals;
using Diskordia.Columbus.FareDealScanner.FareDeals.SingaporeAirlines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.FareDealScanner
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

            services.Configure<Options>(configuration.GetSection("AirlineScanner"));

			return services;
		}
	}
}

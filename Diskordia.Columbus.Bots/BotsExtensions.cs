using System;
using Diskordia.Columbus.Bots.Services;
using Diskordia.Columbus.Bots.Services.SingaporeAirlines;
using Diskordia.Columbus.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Bots
{
	public static class BotsExtensions
	{
		public static IServiceCollection AddFareDealBots(this IServiceCollection services)
		{
			if(services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddTransient<IFareDealService, SingaporeAirlinesFareDealService>();
			services.AddTransient<IStartable, FareDealServiceStartable>();

			return services;
		}
	}
}

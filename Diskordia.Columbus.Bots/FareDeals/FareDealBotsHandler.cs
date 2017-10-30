using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public class FareDealBotsHandler : IHandleMessages<StartFareDealsScanCommand>
	{
		private readonly IBus bus;
		private readonly ILogger logger;
		private readonly IEnumerable<IFareDealScanService> fareDealServices;

		public FareDealBotsHandler(IBus bus, ILogger<FareDealBotsHandler> logger, IEnumerable<IFareDealScanService> fareDealServices)
		{
			if (bus == null)
			{
				throw new ArgumentNullException(nameof(bus));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (fareDealServices == null)
			{
				throw new ArgumentNullException(nameof(fareDealServices));
			}

			this.bus = bus;
			this.logger = logger;
			this.fareDealServices = fareDealServices;
		}

		public async Task Handle(StartFareDealsScanCommand message)
		{
			foreach (var service in this.fareDealServices)
			{
				logger.LogInformation("Start to scan singapore airlines for fare deals.");
				var fareDeals = await service.SearchFareDealsAsync();
				var result = new FareDealScanResult<SingaporeAirlinesFareDeal>
				{
					FareDeals = fareDeals.ToArray()
				};

				logger.LogInformation("Singapore airlines fare deal scan completed, sending result to service bus.");

				await this.bus.SendLocal(result);
			}
		}
	}
}

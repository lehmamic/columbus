using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;
using Rebus.Bus;
using Rebus.Handlers;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public class FareDealBotsHandler : IHandleMessages<StartFareDealsScanCommand>
	{
		readonly IBus bus;
		readonly IEnumerable<IFareDealScanService> fareDealServices;

		public FareDealBotsHandler(IBus bus, IEnumerable<IFareDealScanService> fareDealServices)
		{
			if(bus == null)
			{
				throw new ArgumentNullException(nameof(bus));
			}

			if (fareDealServices == null)
			{
				throw new ArgumentNullException(nameof(fareDealServices));
			}

			this.bus = bus;
			this.fareDealServices = fareDealServices;
		}

		public async Task Handle(StartFareDealsScanCommand message)
		{
			foreach (var scan in message.Scans)
			{
				foreach (var service in this.fareDealServices)
				{
					var fareDeals = await service.SearchFareDealsAsync(scan);
					var result = new FareDealScanResult
					{
						FareDeals = fareDeals.ToArray()
					};

					await this.bus.SendLocal(result);
				}
			}
		}
	}
}

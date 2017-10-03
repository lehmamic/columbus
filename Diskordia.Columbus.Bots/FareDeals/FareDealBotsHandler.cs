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
		readonly IEnumerable<IFareDealService> fareDealServices;

		public FareDealBotsHandler(IBus bus, IEnumerable<IFareDealService> fareDealServices)
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
			await Task.Run(() => {
				foreach (var scan in message.Bots)
				{
					foreach (var service in this.fareDealServices)
					{
						var fareDeals = service.SearchFareDeals(scan);
						var result = new FareDealScanResult
						{
							FareDeals = fareDeals.ToArray()
						};

						this.bus.SendLocal(result);
					}
				}
			});
		}
	}
}

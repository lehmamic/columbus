using System;
using Diskordia.Columbus.Contract.FareDeals;
using Rebus.Bus;

namespace Diskordia.Columbus.BackgroundWorker.Services
{
	public class FareDealBotsService : IFareDealBotsService
	{
		readonly IBus bus;

		public FareDealBotsService(IBus bus)
		{
			if ( bus == null)
			{
				throw new ArgumentNullException(nameof(bus));

			}
			this.bus = bus;
		}

		public void TriggerFareDealsScan()
		{
			var command = new StartFareDealsScanCommand
			{
				Bots = new AirlineScan[]
				{
					new AirlineScan { Airline = Airline.SingaporeAirlines, Uri = "http://www.singaporeair.com/en_UK/ch/home" }
				}
			};

			this.bus.SendLocal(command);
		}
	}
}

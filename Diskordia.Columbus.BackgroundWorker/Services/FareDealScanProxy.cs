using System;
using Diskordia.Columbus.Contract.FareDeals;
using Rebus.Bus;

namespace Diskordia.Columbus.BackgroundWorker.Services
{
	public class FareDealScanProxy : IFareDealScanProxy
	{
		readonly IBus bus;

		public FareDealScanProxy(IBus bus)
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
				Scans = new FareDealScan[]
				{
					new FareDealScan { Airline = Airline.SingaporeAirlines, Uri = "http://www.singaporeair.com/en_UK/ch/home" }
				}
			};

			this.bus.SendLocal(command);
		}
	}
}

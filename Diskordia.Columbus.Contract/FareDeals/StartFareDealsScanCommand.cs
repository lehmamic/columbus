using System.Collections.Generic;

namespace Diskordia.Columbus.Contract.FareDeals
{
	public class StartFareDealsScanCommand
	{
		public IEnumerable<AirlineScan> Bots { get; set; }
	}
}

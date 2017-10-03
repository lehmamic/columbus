using System.Collections.Generic;

namespace Diskordia.Columbus.Contract.FareDeals
{
	public class StartFareDealsScanCommand
	{
		public IEnumerable<FareDealScan> Scans { get; set; }
	}
}

using System.Collections.Generic;

namespace Diskordia.Columbus.Contract.FareDeals
{
	public class FareDealScanResult<TResult>
	{
		public IEnumerable<TResult> FareDeals { get; set; }
	}
}

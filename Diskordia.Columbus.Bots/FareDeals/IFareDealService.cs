using System.Collections.Generic;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public interface IFareDealService
	{
		IEnumerable<FareDeal> SearchFareDeals();
	}
}

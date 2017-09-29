using System.Collections.Generic;
using Diskordia.Columbus.Contract.FareDeals;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public interface IFareDealService
	{
		IEnumerable<FareDeal> SearchFareDeals();
	}
}

using System.Collections.Generic;

namespace Diskordia.Columbus.Bots.Services
{
	public interface IFareDealService
	{
		IEnumerable<FareDeal> SearchFareDeals();
	}
}

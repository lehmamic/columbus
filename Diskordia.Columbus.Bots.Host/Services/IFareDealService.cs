using System.Collections.Generic;

namespace Diskordia.Columbus.Bots.Host.Services
{
	public interface IFareDealService
	{
		IEnumerable<FareDeal> SearchFareDeals();
	}
}

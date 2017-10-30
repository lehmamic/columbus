using System.Collections.Generic;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public interface IFareDealScanService
	{
		Task<IEnumerable<SingaporeAirlinesFareDeal>> SearchFareDealsAsync();
	}
}

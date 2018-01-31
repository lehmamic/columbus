using System.Collections.Generic;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;

namespace Diskordia.Columbus.FareDealScanner.FareDeals
{
	public interface IFareDealScanService
	{
		IEnumerable<SingaporeAirlinesFareDeal> SearchFareDeals();
	}
}

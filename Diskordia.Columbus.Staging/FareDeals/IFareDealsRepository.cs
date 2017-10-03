using System.Collections.Generic;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public interface IFareDealsRepository
	{
		Task MergeFareDeals(IEnumerable<FareDeal> fareDeals);
	}
}
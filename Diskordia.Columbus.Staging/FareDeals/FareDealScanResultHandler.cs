using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Diskordia.Columbus.Contract.FareDeals;
using Rebus.Handlers;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealScanResultHandler : IHandleMessages<FareDealScanResult<SingaporeAirlinesFareDeal>>
	{
		private readonly IFareDealsRepository repository;
		private readonly IMapper mapper;

		public FareDealScanResultHandler(IFareDealsRepository repository, IMapper mapper)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			this.repository = repository;
			this.mapper = mapper;
		}

		public async Task Handle(FareDealScanResult<SingaporeAirlinesFareDeal> message)
		{
			IEnumerable<SingaporeAirlinesFareDealEntity> entities = this.mapper
			    .Map<IEnumerable<SingaporeAirlinesFareDealEntity>>(message.FareDeals);

			await this.repository.MergeFareDeals(entities);
		}
	}
}

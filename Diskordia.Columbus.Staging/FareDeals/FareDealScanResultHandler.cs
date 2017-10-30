﻿using System;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;
using Rebus.Handlers;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealScanResultHandler : IHandleMessages<FareDealScanResult<SingaporeAirlinesFareDeal>>
	{
		readonly IFareDealsRepository repository;

		public FareDealScanResultHandler(IFareDealsRepository repository)
		{
			if(repository == null)
			{
				throw new ArgumentNullException(nameof(repository));	
			}

			this.repository = repository;
		}

		public async Task Handle(FareDealScanResult<SingaporeAirlinesFareDeal> message)
		{
			await this.repository.MergeFareDeals(message.FareDeals);
		}
	}
}
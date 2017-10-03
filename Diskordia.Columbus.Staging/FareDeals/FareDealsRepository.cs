using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealsRepository : IFareDealsRepository
	{
		private readonly IOptions<FareDealStagingOptions> options;
		private readonly IMongoClient client;
		private readonly IMongoDatabase database;

		public FareDealsRepository(IOptions<FareDealStagingOptions> options)
		{
			if(options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			this.options = options;
			this.client = new MongoClient(this.options.Value.ConnectionString);
			this.database = this.client.GetDatabase(this.options.Value.Database);
		}

		public async Task MergeFareDeals(IEnumerable<FareDeal> fareDeals)
		{
			if (fareDeals.Any())
			{
				var collection = this.database.GetCollection<FareDeal>("FareDeals");
				await collection.DeleteManyAsync(Builders<FareDeal>.Filter.Eq(nameof(FareDeal.Airline), fareDeals.First().Airline));
				await collection.InsertManyAsync(fareDeals);
			}
		}
	}
}

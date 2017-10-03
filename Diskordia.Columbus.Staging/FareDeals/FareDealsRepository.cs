using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diskordia.Columbus.Contract.FareDeals;
using MongoDB.Driver;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealsRepository : IFareDealsRepository
	{
		IMongoClient client;
		IMongoDatabase database;

		public FareDealsRepository()
		{
			this.client = new MongoClient("mongodb://localhost");
			this.database = this.client.GetDatabase("COLUMBUS-FAREDEALS");
		}

		public async Task MergeFareDeals(IEnumerable<FareDeal> fareDeals)
		{
			if (fareDeals.Any())
			{
				var collection = this.database.GetCollection<FareDeal>("FareDeals");
				await collection.DeleteManyAsync(Builders<FareDeal>.Filter.Eq("Airline", fareDeals.First().Airline));
				await collection.InsertManyAsync(fareDeals);
			}
		}
	}
}

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealStagingOptions
	{
		public string ServiceBusConnectionString { get; set; }

		public string ImportQueueName { get; set; }

		public string ExportQueueName { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diskordia.Columbus.Bots.Services;
using Diskordia.Columbus.Common;
using Microsoft.Azure.ServiceBus;

namespace Diskordia.Columbus.Bots
{
	public class FareDealServiceStartable : IStartable
	{
		private readonly IEnumerable<IFareDealService> fareDealServices;
		//private readonly Lazy<IQueueClient> queueClient;

		public FareDealServiceStartable(IEnumerable<IFareDealService> fareDealServices)
		{
			if (fareDealServices == null)
			{
				throw new ArgumentNullException(nameof(fareDealServices));
			}

			this.fareDealServices = fareDealServices;
			//this.queueClient = new Lazy<IQueueClient>(() => new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);)
		}

		public void Start()
		{
			foreach(IFareDealService service in fareDealServices)
			{
				IEnumerable<FareDeal> fareDeals = service.SearchFareDeals();

				foreach (var fareDeal in fareDeals)
				{
					Console.WriteLine($"Found fare deal from {fareDeal.DepartureAirport} to {fareDeal.DestinationAirport}.");

					Console.WriteLine($"Price: {fareDeal.Price} {fareDeal.Currency}");
					Console.WriteLine($"Class: {fareDeal.Class}");
					Console.WriteLine($"Book by: {fareDeal.BookBy}");
					Console.WriteLine($"Outbound travel period: {fareDeal.OutboundStartDate} to {fareDeal.OutboundEndDate}");
					Console.WriteLine($"Travel complete date: {fareDeal.TravelCompleteDate}");
				}
			}


		}

		//private async Task SendMessageAsync(IEnumerable<FareDeal> fareDeals)
		//{
		//	var message = new Message(Encoding.UTF8.GetBytes($"Message {i}"));

		//	// Write the body of the message to the console
		//	Console.WriteLine($"Sending message: {Encoding.UTF8.GetString(message.Body)}");

		//	// Send the message to the queue
		//	await queueClient.Value.SendAsync(message);
		//}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diskordia.Columbus.Bots.FareDeals;
using Diskordia.Columbus.Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace Diskordia.Columbus.Bots.FareDeals
{
	public class FareDealServiceStartable : IStartable, IDisposable
	{
		private readonly IEnumerable<IFareDealService> fareDealServices;
		private readonly IOptionsSnapshot<FareDealOptions> options;
		private readonly ISerializer serializer;
		private readonly IQueueClient queueClient;

		private bool disposedValue = false;

		public FareDealServiceStartable(IEnumerable<IFareDealService> fareDealServices, ISerializer serializer, IOptionsSnapshot<FareDealOptions> options)
		{
			if (fareDealServices == null)
			{
				throw new ArgumentNullException(nameof(fareDealServices));
			}

			if(serializer == null)
			{
				throw new ArgumentNullException(nameof(serializer));
			}

			if(options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			this.fareDealServices = fareDealServices;
			this.serializer = serializer;
			this.options = options;

			this.queueClient = new QueueClient(this.options.Value.ServiceBusConnectionString, this.options.Value.ImportQueueName, ReceiveMode.PeekLock);
		}

		~FareDealServiceStartable()
		{
			this.Dispose(false);
		}

		public void Start()
		{
			foreach(IFareDealService service in fareDealServices)
			{
				try
				{
					IEnumerable<FareDeal> fareDeals = service.SearchFareDeals().ToArray();

					foreach (var fareDeal in fareDeals)
					{
						Console.WriteLine($"Found fare deal from {fareDeal.DepartureAirport} to {fareDeal.DestinationAirport}.");

						Console.WriteLine($"Price: {fareDeal.Price} {fareDeal.Currency}");
						Console.WriteLine($"Class: {fareDeal.Class}");
						Console.WriteLine($"Book by: {fareDeal.BookBy}");
						Console.WriteLine($"Outbound travel period: {fareDeal.OutboundStartDate} to {fareDeal.OutboundEndDate}");
						Console.WriteLine($"Travel complete date: {fareDeal.TravelCompleteDate}");
					}

					SendMessageAsync(fareDeals)
						.GetAwaiter()
						.GetResult();
				}
				catch(Exception e)
				{
					Console.WriteLine($"{DateTime.Now} > Exception: {e.Message}");
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private async Task SendMessageAsync(IEnumerable<FareDeal> fareDeals)
		{
			var message = new Message(Encoding.UTF8.GetBytes(this.serializer.Serialize(fareDeals)));

			// Write the body of the message to the console
			Console.WriteLine($"Sending message: {Encoding.UTF8.GetString(message.Body)}");

			// Send the message to the queue
			await queueClient.SendAsync(message);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.queueClient.CloseAsync()
					    .GetAwaiter()
					    .GetResult();
				}

				disposedValue = true;
			}
		}
	}
}

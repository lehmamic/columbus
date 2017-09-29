using System;
using System.Text;
using System.Threading.Tasks;
using Diskordia.Columbus.Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class FareDealStagingStartable : IStartable, IDisposable
	{
		private readonly IOptionsSnapshot<FareDealStagingOptions> options;
		private readonly IQueueClient queueClient;

		private bool disposedValue = false;

		public FareDealStagingStartable(IOptionsSnapshot<FareDealStagingOptions> options)
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			this.options = options;

			this.queueClient = new QueueClient(this.options.Value.ServiceBusConnectionString, this.options.Value.ImportQueueName, ReceiveMode.PeekLock);
		}

		~FareDealStagingStartable()
		{
			this.Dispose(false);
		}

		public void Start()
		{
			try
			{
				Console.WriteLine("Starting fare deal staging.");

				this.queueClient.RegisterMessageHandler(
					async (message, token) =>
					{
						// Process the message
						Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

						await queueClient.CompleteAsync(message.SystemProperties.LockToken);
					},
					new MessageHandlerOptions(async (e) =>
					{
						await Task.Run(() => Console.WriteLine($"{DateTime.Now} > Exception: {e.Exception.Message}"));
					})
				{
					MaxConcurrentCalls = 1,
					AutoComplete = false
				});
			}
			catch (Exception exception)
			{
				Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
			}
		}

		public void Stop()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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

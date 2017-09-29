using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Common.Hosting
{
	public class ServiceHost : IServiceHost
	{
		private readonly ServiceProvider serviceProvider;

		private bool disposedValue = false;

		public event EventHandler HostStopped; 

		public ServiceHost(ServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			this.serviceProvider = serviceProvider;
		}

		~ServiceHost()
		{
			Dispose(false);
		}


		public async Task StartAsync(CancellationToken token)
		{
			await Task.Run(() =>
			{
				IEnumerable<IStartable> modules = serviceProvider.GetServices<IStartable>();
				foreach (var module in modules)
				{
					module.Start();
				}
			});
		}

		public async Task StopAsync()
		{
			await Task.Run(() =>
			{
				IEnumerable<IStartable> modules = serviceProvider.GetServices<IStartable>();
				foreach (var module in modules)
				{
					module.Stop();
				}
			});

			RaiseHostStopped();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.serviceProvider.Dispose();
				}


				disposedValue = true;
			}
		}

		private void RaiseHostStopped()
		{
			var eventhanlder = this.HostStopped;
			if(eventhanlder != null)
			{
				eventhanlder(this, EventArgs.Empty);
			}
		}
	}
}

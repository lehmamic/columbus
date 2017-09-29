using System;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Common.Hosting
{
	public class ServiceHostBuilder : IServiceHostBuilder
	{
		private readonly IServiceCollection services = new ServiceCollection();

		public IServiceHostBuilder UseStartup<T>() where T : IStartup, new()
		{
			var startup = new T();
			startup.ConfigureServices(services);

			return this;
		}

		public IServiceHost Build()
		{
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			return new ServiceHost(serviceProvider);
		}
	}
}

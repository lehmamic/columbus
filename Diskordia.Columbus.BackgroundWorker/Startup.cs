using System;
using System.IO;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Common.Hosting;
using Diskordia.Columbus.Staging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.BackgroundWorker
{
	public class Startup : IStartup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
				.AddEnvironmentVariables();

			var configuration = builder.Build();

			services.AddOptions()
			        .AddSerializer()
					.AddFareDealStaging(configuration)
					.AddFareDealBots(configuration)
					.BuildServiceProvider();
		}
	}
}

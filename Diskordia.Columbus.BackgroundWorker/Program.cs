using System;
using System.Collections.Generic;
using System.IO;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.BackgroundWorker
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
				.AddEnvironmentVariables();
			
			var configuration = builder.Build();

			var serviceProvider = new ServiceCollection(configuration)
				.AddOptions()
				.AddFareDealBots()
				.BuildServiceProvider();

			IEnumerable<IStartable> modules = serviceProvider.GetServices<IStartable>();
			foreach(var module in modules)
			{
				module.Start();
			}
		}
	}
}

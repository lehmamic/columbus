using System;
using System.Collections.Generic;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.BackgroundWorker
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");


			var serviceProvider = new ServiceCollection()
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

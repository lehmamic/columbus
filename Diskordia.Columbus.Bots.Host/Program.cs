using System;
using System.Collections.Generic;
using Diskordia.Columbus.Bots.Host.Services;
using Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines;
using Microsoft.Extensions.Configuration;

namespace Diskordia.Columbus.Bots.Host
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			//var builder = new ConfigurationBuilder()
			//.SetBasePath(System.IO.Directory.GetCurrentDirectory())
			//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			//.AddEnvironmentVariables();

			//IConfigurationRoot configuration = builder.Build();

			//var settings = new MySettings();
			//configuration.Bind(settings);

			IFareDealService service = new SingaporeAirlinesFareDealService();
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
}

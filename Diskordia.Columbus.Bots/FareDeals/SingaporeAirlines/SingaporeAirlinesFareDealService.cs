using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects;
using Diskordia.Columbus.Contract.FareDeals;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines
{
	public class SingaporeAirlinesFareDealService : IFareDealScanService
	{
		readonly IOptionsSnapshot<SingaporeAirlinesOptions> options;

		public SingaporeAirlinesFareDealService(IOptionsSnapshot<SingaporeAirlinesOptions> options)
		{
			if(options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			this.options = options;
		}
		public async Task<IEnumerable<FareDeal>> SearchFareDealsAsync(FareDealScan scan)
		{
			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
			{
				IEnumerable<FareDeal> availableFareDeals = await this.SearchForAvailableFareDealsAsync(driver);

				var result = new List<FareDeal>();
				foreach (var fareDeal in availableFareDeals.ToArray())
				{
					var enrichedFareDeal = await ReadFareDealDetailsAsync(driver, fareDeal);
					result.Add(enrichedFareDeal);
				}

				return result;
			}
		}

		private static async Task<FareDeal> ReadFareDealDetailsAsync(ChromeDriver driver, FareDeal fareDeal)
		{
			return await Task.Run(() => ReadFareDealDetails(driver, fareDeal));
		}

		private static FareDeal ReadFareDealDetails(ChromeDriver driver, FareDeal fareDeal)
		{
			var page = new FareDealPage(driver, fareDeal.Link);
			page.NavigateTo();

			var outboundTravelPeriod = Regex.Split(page.OutboundTravelPeriod, "to")
								.Select(p => DateTime.Parse(p.Trim()));

			fareDeal.BookBy = DateTime.Parse(page.BookBy);
			fareDeal.OutboundStartDate = outboundTravelPeriod.ElementAt(0);
			fareDeal.OutboundEndDate = outboundTravelPeriod.ElementAt(1);
			fareDeal.TravelCompleteDate = DateTime.Parse(page.TravelCompleteDate);


			return fareDeal;
		}

		private async Task<IEnumerable<FareDeal>> SearchForAvailableFareDealsAsync(IWebDriver driver)
		{
			var result = new List<FareDeal>();
			foreach(var uri in this.options.Value.TargetUrls)
			{
				IEnumerable<FareDeal> fareDeals = (await this.SearchForAvailableFareDealsAsync(driver, uri)).ToArray();
				result.AddRange(fareDeals);
			}

			return result;
		}

		private async Task<IEnumerable<FareDeal>> SearchForAvailableFareDealsAsync(IWebDriver driver, Uri uri)
		{
			return await Task.Run(() => this.SearchForAvailableFareDeals(driver, uri));
		}

		private IEnumerable<FareDeal> SearchForAvailableFareDeals(IWebDriver driver, Uri uri)
		{
			var page = new HomePage(driver, uri);
			page.NavigateTo();

			var fareDealsSection = page.Sections.OfType<FareDealsSectionComponent>().SingleOrDefault();

			if (fareDealsSection != null)
			{
				foreach (string airport in fareDealsSection.AvailableDepatureAirports)
				{
					fareDealsSection.SelectDepartureAirport(airport);

					foreach (var fareDeal in fareDealsSection.FareDeals)
					{
						yield return new FareDeal
						{
							Airline = Airline.SingaporeAirlines,
							Link = new Uri(fareDeal.Link),
							DepartureAirport = fareDealsSection.DepartureAirport,
							DestinationAirport = fareDeal.DestinationAirport,
							Class = fareDeal.Class,
							Currency = Regex.Match(fareDeal.PriceLabel, @"[A-Z]{3}").Value,
							Price = Decimal.Parse(Regex.Match(fareDeal.PriceValue, @"\d+(\.\d+)?").Value)
						};
					}
				}
			}
		}
	}
}

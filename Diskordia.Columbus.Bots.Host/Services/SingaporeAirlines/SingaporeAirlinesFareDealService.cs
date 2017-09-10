using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines
{
	public class SingaporeAirlinesFareDealService : IFareDealService
	{
		public IEnumerable<FareDeal> SearchFareDeals()
		{
			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
			{
				IEnumerable<FareDeal> availableFareDeals = this.SearchForAvailableFareDeals(driver);
				foreach (var fareDeal in availableFareDeals.ToArray())
				{
					yield return ReadFareDealDetails(driver, fareDeal);
				}
			}
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

		private IEnumerable<FareDeal> SearchForAvailableFareDeals(IWebDriver driver)
		{
			var page = new HomePage(driver, new Uri("http://www.singaporeair.com/en_UK/ch/home"));
			page.NavigateTo();

			var fareDealsSection = page.Sections.OfType<FareDealsSectionComponent>().SingleOrDefault();

			if (fareDealsSection != null)
			{
				foreach (var fareDeal in fareDealsSection.FareDeals)
				{
					yield return new FareDeal
					{
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

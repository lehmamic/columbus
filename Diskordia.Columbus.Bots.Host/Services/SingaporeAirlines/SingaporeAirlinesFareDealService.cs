using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines.PageObjects;
using OpenQA.Selenium.Chrome;

namespace Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines
{
	public class SingaporeAirlinesFareDealService : IFareDealService
	{
		public IEnumerable<FareDeal> SearchFareDeals()
		{
			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
			{
				var homePage = new HomePage(driver, new Uri("http://www.singaporeair.com/en_UK/ch/home"));
				homePage.NavigateTo();

				var fareDealsSection = homePage.Sections.OfType<FareDealsSectionComponent>().SingleOrDefault();

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
}

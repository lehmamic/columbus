using System;
using System.Collections.Generic;
using System.Globalization;
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
				return (await this.SearchForAvailableFareDealsAsync(driver))
					.ToArray();
			}
		}

		private async Task<IEnumerable<FareDeal>> SearchForAvailableFareDealsAsync(IWebDriver driver)
		{
			IEnumerable<Uri> fareDealPages = await GetAvailableFareDealPages(driver);

			var result = new List<FareDeal>();
			foreach(var uri in fareDealPages)
			{
				var page = new FareDealPage(driver, uri);
				page.NavigateTo();

				Match airportsMatch = Regex.Match(page.Title, @"^[^()]+\((?<from>[A-Z]{3})\)[^()]+\((?<to>[A-Z]{3})\)$");

				Match priceMatch = Regex.Match(page.Price, @"^From\s(?<currency>[A-Z]{3})\s(?<amount>\d+(,\d+)?)$");

				Match classMatch = Regex.Match(page.Info, @"");

				IEnumerable<DateTime> outboundTravelPeriod = Regex.Split(page.OutboundTravelPeriod, "to")
					.Select(p => DateTime.Parse(p.Trim()));

				var fareDeal = new FareDeal
				{
					Link = uri,
					Airline = Airline.SingaporeAirlines,
					DepartureAirport = airportsMatch.Success ? airportsMatch.Groups["from"].Value : string.Empty,
					DestinationAirport = airportsMatch.Success ? airportsMatch.Groups["to"].Value : string.Empty,
					Price = priceMatch.Success ? decimal.Parse(priceMatch.Groups["amount"].Value, NumberStyles.AllowThousands) : 0m,
					Currency = priceMatch.Success ? priceMatch.Groups["currency"].Value : string.Empty,
					Class = classMatch.Success ? classMatch.Groups["class"].Value.Trim() : string.Empty,
					BookBy = DateTime.Parse(page.BookBy),
					OutboundStartDate = outboundTravelPeriod.ElementAt(0),
					OutboundEndDate = outboundTravelPeriod.ElementAt(1),
					TravelCompleteDate = DateTime.Parse(page.TravelCompleteDate)
				};

				result.Add(fareDeal);

			}

			return result;
		}

		private async Task<IEnumerable<Uri>> GetAvailableFareDealPages(IWebDriver driver)
		{
			var result = new List<Uri>();

			foreach (var uri in this.options.Value.TargetUrls)
			{
				IEnumerable<Uri> specialOffersByCityUrls = (await GetSpecialOfferPagesByCityAsync(driver, uri, this.options.Value.TargetUrls.First() == uri))
					.ToArray();

				IEnumerable<Uri> specialOffsersByCountryUrls = (await GetFareDealPagesByCountryAsync(driver, specialOffersByCityUrls))
					.ToArray();

				result.AddRange(specialOffsersByCountryUrls);
			}

			return result;
		}

		private async Task<IEnumerable<Uri>> GetFareDealPagesByCountryAsync(IWebDriver driver, IEnumerable<Uri> specialOfferByCityUrls)
		{
			var result = new List<Uri>();

			foreach (Uri specialOffersByCityUrl in specialOfferByCityUrls)
			{
				IEnumerable<Uri> fareDealPages = (await GetFareDealPagesByCityAsync(driver, specialOffersByCityUrl))
					.ToArray();

				result.AddRange(fareDealPages);
			}

			return result;
		}

		private async Task<IEnumerable<Uri>> GetFareDealPagesByCityAsync(IWebDriver driver, Uri specialOfferUrl)
		{
			return await Task.Run(() => GetFareDealPagesByCity(driver, specialOfferUrl));
		}

		private IEnumerable<Uri> GetFareDealPagesByCity(IWebDriver driver, Uri specialOfferUrl)
		{
			var page = new SpecialOffersPage(driver, specialOfferUrl);
			page.NavigateTo();

			var result = new List<Uri>();

			foreach (var preferredClass in page.PreferredClass.Options)
			{
				page.PreferredClass.Select(preferredClass);

				var fareDealPageLinks = page.FareDealLinks.Select(l => new Uri(l));
				result.AddRange(fareDealPageLinks);
			}

			return result;
		}

		private async Task<IEnumerable<Uri>> GetSpecialOfferPagesByCityAsync(IWebDriver driver, Uri homePageUrl, bool closeInitialPopups)
		{
			return await Task.Run(() => this.GetSpecialOfferPagesByCity(driver, homePageUrl, closeInitialPopups));
		}

		private IEnumerable<Uri> GetSpecialOfferPagesByCity(IWebDriver driver, Uri homePageUrl, bool closeInitialPopups)
		{
			var page = new HomePage(driver, homePageUrl);
			page.NavigateTo();

			if(closeInitialPopups)
			{
				page.DeclineNotifications();
				page.CloseCookiePopup();
			}

			var fareDealsSection = page.Sections.OfType<FareDealsSectionComponent>().SingleOrDefault();

			var result = new List<Uri>();
			if (fareDealsSection != null)
			{
				foreach (string airport in fareDealsSection.FareDealCities.Options)
				{
					fareDealsSection.FareDealCities.Select(airport);

					result.Add(new Uri(fareDealsSection.ViewAllByCityUrl));
				}
			}

			return result;
		}
	}
}

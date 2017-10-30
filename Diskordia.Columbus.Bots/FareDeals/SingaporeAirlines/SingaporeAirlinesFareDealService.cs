using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects;
using Diskordia.Columbus.Contract.FareDeals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines
{
	public class SingaporeAirlinesFareDealService : IFareDealScanService
	{
		private readonly IOptionsSnapshot<SingaporeAirlinesOptions> singaporeAirlinesOptions;
		private readonly IOptionsSnapshot<FareDealScanOptions> fareDealOptions;
		private readonly ILogger logger;

		public SingaporeAirlinesFareDealService(IOptionsSnapshot<SingaporeAirlinesOptions> singaporeAirlinesOptions, IOptionsSnapshot<FareDealScanOptions> fareDealOptions, ILogger<SingaporeAirlinesFareDealService> logger)
		{
			if (singaporeAirlinesOptions == null)
			{
				throw new ArgumentNullException(nameof(singaporeAirlinesOptions));
			}

			if (fareDealOptions == null)
			{
				throw new ArgumentNullException(nameof(fareDealOptions));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.singaporeAirlinesOptions = singaporeAirlinesOptions;
			this.fareDealOptions = fareDealOptions;
			this.logger = logger;
		}

		public async Task<IEnumerable<SingaporeAirlinesFareDeal>> SearchFareDealsAsync()
		{
			ChromeOptions options = new ChromeOptions();

			if (this.fareDealOptions.Value.HeadlessMode)
			{
				options.AddArgument("--headless");
			}

			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options))
			{
				return (await this.SearchForAvailableFareDealsAsync(driver))
					.ToArray();
			}
		}

		private async Task<IEnumerable<SingaporeAirlinesFareDeal>> SearchForAvailableFareDealsAsync(IWebDriver driver)
		{
			IEnumerable<Uri> fareDealPages = await GetAvailableFareDealPages(driver);

			var result = new List<SingaporeAirlinesFareDeal>();
			foreach(var uri in fareDealPages.Distinct().ToArray())
			{
				SingaporeAirlinesFareDeal fareDeal = await this.ExtractFareDealFromPageAsync(driver, uri);
				result.Add(fareDeal);
			}

			return result;
		}

		private async Task<IEnumerable<Uri>> GetAvailableFareDealPages(IWebDriver driver)
		{
			var result = new List<Uri>();

			foreach (var uri in this.singaporeAirlinesOptions.Value.TargetUrls)
			{
				logger.LogTrace("Scanning url {0} for special offer pages.", uri);

				IEnumerable<Uri> specialOffersByCityUrls = (await GetSpecialOfferPagesByCityAsync(driver, uri, this.singaporeAirlinesOptions.Value.TargetUrls.First() == uri))
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
				logger.LogTrace("Scanning special offer page from url {0} for class {1}", specialOfferUrl, preferredClass);
				page.PreferredClass.Select(preferredClass);

				var fareDealPageLinks = page.FareDealLinks.Select(l =>
				{
					logger.LogDebug("Fare deal url found {0} for class {1}", l, preferredClass);
					return new Uri(l);
				});
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

					string url = fareDealsSection.ViewAllByCityUrl;
					this.logger.LogDebug("Found special offer pages {0}.", url);
					
					result.Add(new Uri(url));
				}
			}

			return result;
		}

		private async Task<SingaporeAirlinesFareDeal> ExtractFareDealFromPageAsync(IWebDriver driver, Uri url)
		{
			return await Task.Run(() => this.ExtractFareDealFromPage(driver, url));
		}

		private SingaporeAirlinesFareDeal ExtractFareDealFromPage(IWebDriver driver, Uri url)
		{
			logger.LogInformation("Scanning fare deal from url {0}", url);

			var page = new FareDealPage(driver, url);
			page.NavigateTo();

			//Match airportsMatch = Regex.Match(page.Title, @"^[^()]+\((?<from>[A-Z]{3})\)[^()]+\((?<to>[A-Z]{3})\)$");

			//Match priceMatch = Regex.Match(page.Price, @"^From\s(?<currency>[A-Z]{3})\s(?<amount>\d+(,\d+)?)$");

			//Match classMatch = Regex.Match(page.Info, @"");

			//IEnumerable<DateTime> outboundTravelPeriod = Regex.Split(page.OutboundTravelPeriod, "to")
			//	.Select(p => DateTime.Parse(p.Trim()));

			//var fareDeal = new SingaporeAirlinesFareDeal
			//{
			//	Link = uri,
			//	Airline = Airline.SingaporeAirlines,
			//	DepartureAirport = airportsMatch.Success ? airportsMatch.Groups["from"].Value : string.Empty,
			//	DestinationAirport = airportsMatch.Success ? airportsMatch.Groups["to"].Value : string.Empty,
			//	Price = priceMatch.Success ? decimal.Parse(priceMatch.Groups["amount"].Value, NumberStyles.AllowThousands) : 0m,
			//	Currency = priceMatch.Success ? priceMatch.Groups["currency"].Value : string.Empty,
			//	Class = classMatch.Success ? classMatch.Groups["class"].Value.Trim() : string.Empty,
			//	BookBy = DateTime.Parse(page.BookBy),
			//	OutboundStartDate = outboundTravelPeriod.ElementAt(0),
			//	OutboundEndDate = outboundTravelPeriod.ElementAt(1),
			//	TravelCompleteDate = DateTime.Parse(page.TravelCompleteDate)
			//};
			return new SingaporeAirlinesFareDeal
			{
				Link = url,
				Titel = page.Title,
				TravelInfo = page.Info,
				Price = page.Price,
				BookBy = page.BookBy,
				OutboundStartDate = page.OutboundStartDate,
				OutboundEndDate = page.OutboundEndDate,
				TravelCompleteDate = page.TravelCompleteDate,
			};
		}
	}
}

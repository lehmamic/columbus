using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects;
using Diskordia.Columbus.Contract.FareDeals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Polly;
using Polly.Timeout;

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

		public IEnumerable<SingaporeAirlinesFareDeal> SearchFareDeals()
		{
			ChromeOptions options = new ChromeOptions();
			if (this.fareDealOptions.Value.HeadlessMode)
			{
				options.AddArgument("--headless");
			}

			IEnumerable<Uri> fareDealPages;
			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options))
			{
				fareDealPages = GetAvailableFareDealPages(driver);
			}

			var result = new List<SingaporeAirlinesFareDeal>();

			foreach (var uri in fareDealPages.Distinct().ToArray())
			{
				using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options))
				{
					SingaporeAirlinesFareDeal fareDeal = Policy.Wrap(
						Policy.Handle<WebDriverException>().Or<InvalidOperationException>().Or<TimeoutRejectedException>().WaitAndRetry(2, retryAttempts => TimeSpan.FromSeconds(1)),
						Policy.Timeout(TimeSpan.FromSeconds(20), TimeoutStrategy.Pessimistic)
					).Execute(() => this.ExtractFareDealFromPage(driver, uri));

					result.Add(fareDeal);
				}
			}

			return result;
		}

		private IEnumerable<Uri> GetAvailableFareDealPages(IWebDriver driver)
		{
			var result = new List<Uri>();

			foreach (var uri in this.singaporeAirlinesOptions.Value.TargetUrls)
			{
				logger.LogTrace("Scanning url {0} for special offer pages.", uri);

				IEnumerable<Uri> specialOffersByCityUrls = GetSpecialOfferPagesByCity(driver, uri, this.singaporeAirlinesOptions.Value.TargetUrls.First() == uri)
					.ToArray();

				IEnumerable<Uri> specialOffsersByCountryUrls = GetFareDealPagesByCountry(driver, specialOffersByCityUrls)
					.ToArray();

				result.AddRange(specialOffsersByCountryUrls);
			}

			return result;
		}

		private IEnumerable<Uri> GetFareDealPagesByCountry(IWebDriver driver, IEnumerable<Uri> specialOfferByCityUrls)
		{
			var result = new List<Uri>();

			foreach (Uri specialOffersByCityUrl in specialOfferByCityUrls)
			{
				IEnumerable<Uri> fareDealPages = Policy.Wrap(
						Policy.Handle<WebDriverException>().Or<InvalidOperationException>().Or<TimeoutRejectedException>().WaitAndRetry(2, retryAttempts => TimeSpan.FromSeconds(1)),
					Policy.Timeout(TimeSpan.FromMinutes(10), TimeoutStrategy.Pessimistic)
				).Execute(() => GetFareDealPagesByCity(driver, specialOffersByCityUrl))
				 .ToArray();

				result.AddRange(fareDealPages);
			}

			return result;
		}

		private IEnumerable<Uri> GetFareDealPagesByCity(IWebDriver driver, Uri specialOfferUrl)
		{
			var page = new SpecialOffersPage(driver, specialOfferUrl);
			page.NavigateTo();

			driver.DeclineNotifications();
			driver.CloseCookiePopup();

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

		private IEnumerable<Uri> GetSpecialOfferPagesByCity(IWebDriver driver, Uri homePageUrl, bool closeInitialPopups)
		{
			var page = new HomePage(driver, homePageUrl);
			page.NavigateTo();

			driver.DeclineNotifications();
			driver.CloseCookiePopup();

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

		private SingaporeAirlinesFareDeal ExtractFareDealFromPage(IWebDriver driver, Uri url)
		{
			logger.LogInformation("Scanning fare deal from url {0}", url);

			var page = new FareDealPage(driver, url);
			page.NavigateTo();

			driver.DeclineNotifications();
			driver.CloseCookiePopup();

			//Match airportsMatch = Regex.Match(page.Title, @"^[^()]+\((?<from>[A-Z]{3})\)[^()]+\((?<to>[A-Z]{3})\)$");

			//Match priceMatch = Regex.Match(page.Price, @"^From\s(?<currency>[A-Z]{3})\s(?<amount>\d+(,\d+)?)$");

			//Match classMatch = Regex.Match(page.Info, @"");

			//IEnumerable<DateTime> outboundTravelPeriod = Regex.Split(page.OutboundTravelPeriod, "to")
			//	.Select(p => DateTime.Parse(p.Trim()));

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

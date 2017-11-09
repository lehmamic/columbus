using System;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects
{
	public class FareDealPage
	{
		private readonly IWebDriver driver;
		private readonly Uri uri;

		public FareDealPage(IWebDriver driver, Uri uri)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			this.driver = driver;
			this.uri = uri;
		}

		public string Title
		{
			get
			{
				return this.driver.FindElement(By.ClassName("main-heading")).Text;

			}
		}

		public string Info
		{
			get
			{
				IWebElement element = this.driver.FindElements(By.CssSelector(".info-promotions span"))
					       .FirstOrDefault(e => Regex.IsMatch(e.Text, "Round Trip"));

				string info = string.Empty;
				if (element != null)
				{
					info = element.Text;
				}

				return info;
			}
		}

		public string Price
		{
			get
			{
				IWebElement element = this.driver.FindElement(By.CssSelector(".flight-item__info-2 h3"));

				string price = string.Empty;
				if(element != null)
				{
					//string currency = element.FindElement(By.CssSelector("span")).Text;
					price = element.Text;

					//price = $"{currency}{amount}";
				}

				return price;
			}
		}

		public string BookBy
		{
			get
			{
				IWebElement element = this.driver.FindElements(By.CssSelector(".promotion-choice li"))
				                  .FirstOrDefault(e => string.Equals(e.FindElement(By.ClassName("title")).Text, "Book\nby", StringComparison.OrdinalIgnoreCase));

				var bookBy = string.Empty;
				if(element != null)
				{
					string bookByDay = element.FindElement(By.ClassName("order")).Text;
					string bookYear = element.FindElement(By.ClassName("text")).Text;

					bookBy = $"{bookByDay} {bookYear}";
				}

				return bookBy;
			}
		}

		public string OutboundTravelPeriod
		{
			get
			{
				IWebElement element = this.driver.FindElements(By.CssSelector("dl"))
				    .FirstOrDefault(e => string.Equals(e.FindElement(By.ClassName("title")).Text, "Outbound travel period", StringComparison.OrdinalIgnoreCase));

				var outboundTravelPeriod = string.Empty;
				if (element != null)
				{
					outboundTravelPeriod = element.FindElement(By.ClassName("value")).Text;
				}

				return outboundTravelPeriod;
			}
		}

		public string OutboundStartDate
		{
			get
			{
				IWebElement element = this.driver.FindElement(By.Id("outboundStartDate"));

				IJavaScriptExecutor js = (IJavaScriptExecutor)this.driver;
				js.ExecuteScript("arguments[0].setAttribute('type', '')", element);

				return element.GetAttribute("value");
			}
		}

		public string OutboundEndDate
		{
			get
			{
				IWebElement element = this.driver.FindElement(By.Id("outboundEndDate"));

				IJavaScriptExecutor js = (IJavaScriptExecutor)this.driver;
				js.ExecuteScript("arguments[0].setAttribute('type', '')", element);

				return element.GetAttribute("value");
			}
		}

		public string TravelCompleteDate
		{
			get
			{
				IWebElement element = this.driver.FindElements(By.CssSelector("dl"))
					.FirstOrDefault(e => string.Equals(e.FindElement(By.ClassName("title")).Text, "Travel completion date", StringComparison.OrdinalIgnoreCase));

				var travelCompleteDate = string.Empty;
				if (element != null)
				{
					travelCompleteDate = element.FindElement(By.ClassName("value")).Text;
				}

				return travelCompleteDate;
			}
		}

		public void NavigateTo()
		{
			this.driver.Navigate().GoToUrl(this.uri);
			this.driver.WaitUntilLoadingOverlayClosed();
			this.driver.Wait(TimeSpan.FromSeconds(1));
		}
	}
}

using System;
using System.Linq;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.Services.SingaporeAirlines.PageObjects
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
			driver.Navigate().GoToUrl(this.uri);
		}
	}
}

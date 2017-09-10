using System;
using System.Linq;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines.PageObjects
{
	public class FareDealsListItemComponent
	{
		private readonly IWebDriver driver;
		private readonly IWebElement element;

		public FareDealsListItemComponent(IWebDriver driver, IWebElement element)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.driver = driver;
			this.element = element;
		}

		public string DestinationAirport
		{
			get
			{
				return this.element.FindElements(By.CssSelector("span")).ElementAt(0).Text;	
			}
		}

		public string Class
		{
			get
			{
				return this.element.FindElements(By.CssSelector("span")).ElementAt(1).Text;
			}
		}

		public string PriceLabel
		{
			get
			{
				return this.element.FindElements(By.CssSelector("span")).ElementAt(2).Text;
			}
		}

		public string PriceValue
		{
			get
			{
				return this.element.FindElements(By.CssSelector("span")).ElementAt(3).Text;
			}
		}

		public string Link
		{
			get
			{
				return this.element.FindElement(By.CssSelector("a")).GetAttribute("href");
			}
		}
	}
}

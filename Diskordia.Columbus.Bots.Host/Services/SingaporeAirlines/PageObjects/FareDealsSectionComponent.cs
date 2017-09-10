using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines.PageObjects
{
	public class FareDealsSectionComponent
	{
		private readonly IWebDriver driver;
		private readonly IWebElement element;

		public FareDealsSectionComponent(IWebDriver driver, IWebElement element)
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

		public string DepartureAirport
		{
			get
			{
				return this.driver.FindElement(By.ClassName("select__text")).Text;
			}
		}

		public IEnumerable<FareDealsListItemComponent> FareDeals
		{
			get
			{
				return this.driver.FindElements(By.CssSelector(".fare-deals-list li"))
					       		  .Where(e => !string.IsNullOrWhiteSpace(e.FindElement(By.ClassName("link")).Text))
					       		  .Select(e => new FareDealsListItemComponent(this.driver, e));
			}
		}
	}
}

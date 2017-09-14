using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.Services.SingaporeAirlines.PageObjects
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
				return this.element.FindElement(By.ClassName("select__text")).Text;
			}
		}

		public IEnumerable<string> AvailableDepatureAirports
		{
			get
			{
				return this.element.FindElements(By.CssSelector("#fare-deal-city option"))
					       .Select(e => e.GetAttribute("value"));
			}
		}

		public IEnumerable<FareDealsListItemComponent> FareDeals
		{
			get
			{
				return this.element.FindElements(By.CssSelector(".fare-deals-list li"))
					       		  .Where(e => !string.IsNullOrWhiteSpace(e.FindElement(By.ClassName("link")).Text))
					       		  .Select(e => new FareDealsListItemComponent(this.driver, e));
			}
		}

		public void SelectDepartureAirport(string airport)
		{
			IWebElement customSelectElement = this.element.FindElement(By.ClassName("custom-select"));

			// e.g. customSelect-19-combobox
			string inputOverlayId = customSelectElement.FindElement(By.ClassName("input-overlay")).GetAttribute("id");

			// in this case customSelect-19-listbox
			string optionElementId = inputOverlayId.Replace("combobox", "listbox");

			customSelectElement.Click();

			var optionElement = this.driver.FindElements(By.CssSelector($"#{optionElementId} li"))
				.FirstOrDefault(e => string.Equals(e.GetAttribute("data-value"), airport, StringComparison.OrdinalIgnoreCase));

			if(optionElement !=null)
			{
				optionElement.Click();
			}
		}
	}
}

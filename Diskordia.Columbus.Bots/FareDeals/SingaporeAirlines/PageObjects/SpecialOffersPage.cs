using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects
{
	public class SpecialOffersPage
	{
		private readonly IWebDriver driver;
		private readonly Uri uri;

		public SpecialOffersPage(IWebDriver driver, Uri uri)
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

			this.PreferredClass = new CustomSelectElement(this.driver, "fare-filter-3");
		}

		public CustomSelectElement PreferredClass { get; }

		public IEnumerable<string> FareDealLinks
		{
			get
			{
				var result = new List<string>();

				try
				{
					IWebElement seeMoreButton = this.driver.FindElement(By.ClassName("see-more-btn"));
					while (seeMoreButton != null && seeMoreButton.Displayed)
					{
						WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
						wait.Until(ExpectedConditions.ElementToBeClickable(seeMoreButton));

						seeMoreButton.Click();

						seeMoreButton = this.driver.FindElement(By.ClassName("see-more-btn"));
					}
				}
				catch(WebDriverTimeoutException)
				{
					// just swallow
				}

				IEnumerable<IWebElement> promotionItemElements = this.driver.FindElements(By.ClassName("promotion-item"));
				foreach(IWebElement promotionItemElement in promotionItemElements)
				{
					IWebElement promotionItemContentElement = promotionItemElement.FindElement(By.ClassName("flight-item"));
					if(promotionItemContentElement != null)
					{
						Actions actions = new Actions(driver);

						actions.MoveToElement(promotionItemContentElement)
						       .Click()
						       .Perform();


						//promotionItemContentElement.Click();
						WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
						wait.Until(d => promotionItemElement.FindElement(By.ClassName("promotion-item__detail")));

						IEnumerable<string> links = promotionItemElement.FindElements(By.ClassName("btn-1"))
																		.Where(e => string.Equals(e.Text, "Book Now", StringComparison.OrdinalIgnoreCase))
																		.Select(e => e.GetAttribute("href"))
																		.ToArray();

						result.AddRange(links);
					}
				}

				return result;
						   

			}	
		}

		public void NavigateTo()
		{
			this.driver.Navigate().GoToUrl(this.uri);
		}
	}
}

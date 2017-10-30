using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Polly;

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
				WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));


				var result = new List<string>();

				IWebElement seeMoreButton = this.driver.FindElement(By.ClassName("see-more-btn"));
				while (seeMoreButton != null && seeMoreButton.Displayed)
				{
					wait.Until(ExpectedConditions.ElementToBeClickable(seeMoreButton));
					seeMoreButton.Click();

					seeMoreButton = this.driver.FindElement(By.ClassName("see-more-btn"));
				}

				IEnumerable<IWebElement> promotionItemElements = this.driver.FindElements(By.ClassName("promotion-item"));
				foreach (IWebElement promotionItemElement in promotionItemElements)
				{
					wait.Until(ExpectedConditions.ElementToBeClickable(promotionItemElement));

					IEnumerable<string> links = Policy.Handle<WebDriverException>()
					                                  .WaitAndRetry(2, retryCount => TimeSpan.FromSeconds(1))
					                                  .Execute(() => ReadFareDealUrlsFromPromotionItem(promotionItemElement)) ;
					result.AddRange(links);

				}

				return result;
			}
		}

		private IEnumerable<string> ReadFareDealUrlsFromPromotionItem(IWebElement promotionItemElement)
		{
			IWebElement detailElement = promotionItemElement.FindElements(By.ClassName("promotion-item__detail"))
			                                                .FirstOrDefault();
			if (detailElement == null || !detailElement.Displayed)
			{
				IWebElement promotionItemContentElement = promotionItemElement.FindElement(By.ClassName("flight-item"));

				IJavaScriptExecutor executor = (IJavaScriptExecutor)this.driver;
				executor.ExecuteScript("arguments[0].click();", promotionItemContentElement);

				this.driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));
			}
			//Actions actions = new Actions(driver);
			//actions.MoveToElement(promotionItemContentElement);
			//actions.Click();
			//actions.Perform();

			//actions.MoveToElement(promotionItemContentElement)
			//.Click()
			//.Perform();

			//wait.Until(ExpectedConditions.ElementToBeClickable(promotionItemContentElement));
			//promotionItemContentElement.Click();

			//IWebElement promotionItemDetailElement = promotionItemElement.FindElement(By.ClassName("promotion-item__detail"));
			//wait.Until(ExpectedConditions.(promotionItemDetailElement));

			return promotionItemElement.FindElements(By.ClassName("btn-1"))
															.Where(e => string.Equals(e.Text, "Book Now", StringComparison.OrdinalIgnoreCase))
															.Select(e => e.GetAttribute("href"))
															.ToArray();
		}


		public void NavigateTo()
		{
			this.driver.Navigate().GoToUrl(this.uri);

			WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
			wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay-loading")));
		}
	}
}

using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects
{
	public class HomePage
	{
		private readonly IWebDriver driver;
		private readonly Uri uri;

		public HomePage(IWebDriver driver, Uri uri)
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

		public IEnumerable<object> Sections
		{
			get
			{
				IWebElement fareDealSectionsElement = this.driver.FindElement(By.ClassName("fare-deals"));
				if (fareDealSectionsElement != null)
				{
					yield return new FareDealsSectionComponent(this.driver, fareDealSectionsElement);
				}
			}
		}

		public void NavigateTo()
		{
			this.driver.Navigate().GoToUrl(this.uri);
		}

		public void DeclineNotifications()
		{
			WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
			wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("insider-opt-in-disallow-button")));

			IWebElement notificatinsDeclineButton = this.driver.FindElement(By.ClassName("insider-opt-in-disallow-button"));
			if (notificatinsDeclineButton != null && notificatinsDeclineButton.Displayed)
			{
				notificatinsDeclineButton.Click();
			}
		}

		public void CloseCookiePopup()
		{
			WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
			wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".popup--cookie a")));

			IWebElement closePopupButton = this.driver.FindElement(By.CssSelector(".popup--cookie a"));
			if (closePopupButton != null && closePopupButton.Displayed)
			{
				closePopupButton.Click();
			}
		}
	}
}

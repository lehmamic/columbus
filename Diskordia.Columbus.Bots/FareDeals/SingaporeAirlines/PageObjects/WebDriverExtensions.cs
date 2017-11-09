using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Diskordia.Columbus.Bots.FareDeals.SingaporeAirlines.PageObjects
{
	public static class WebDriverExtensions
	{
		public static void WaitUntilLoadingOverlayClosed(this IWebDriver driver)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			IWebElement loadOverlayElement = driver.FindElements(By.ClassName("overlay-loading"))
									 .FirstOrDefault();
			if (loadOverlayElement != null && loadOverlayElement.Displayed)
			{
				WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
				wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay-loading")));
			}
		}

		public static void DeclineNotifications(this IWebDriver driver)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			IWebElement element = driver.FindElements(By.ClassName("insider-opt-in-disallow-button"))
						 .FirstOrDefault();

			if (element != null && element.Displayed)
			{
				WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
				wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("insider-opt-in-disallow-button")));

				IWebElement notificatinsDeclineButton = driver.FindElement(By.ClassName("insider-opt-in-disallow-button"));
				if (notificatinsDeclineButton != null && notificatinsDeclineButton.Displayed)
				{
					notificatinsDeclineButton.Click();
				}
			}
		}

		public static void CloseCookiePopup(this IWebDriver driver)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			IWebElement element = driver.FindElements(By.CssSelector(".popup--cookie a"))
			 .FirstOrDefault();

			if (element != null && element.Displayed)
			{
				WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
				wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".popup--cookie a")));

				IWebElement closePopupButton = driver.FindElement(By.CssSelector(".popup--cookie a"));
				if (closePopupButton != null && closePopupButton.Displayed)
				{
					closePopupButton.Click();
				}
			}
		}

		public static void Wait(this IWebDriver driver, TimeSpan waitTime)
		{
			if (driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			driver.Manage().Timeouts().ImplicitlyWait(waitTime);
		}
	}
}

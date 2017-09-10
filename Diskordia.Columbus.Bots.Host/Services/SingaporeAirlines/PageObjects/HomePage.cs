using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace Diskordia.Columbus.Bots.Host.Services.SingaporeAirlines.PageObjects
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
			driver.Navigate().GoToUrl(this.uri);
		}
	}
}

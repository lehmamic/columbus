using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Polly;

namespace Diskordia.Columbus.FareDealScanner.FareDeals.SingaporeAirlines.PageObjects
{
	public class CustomSelectElement
	{
		private readonly IWebDriver driver;
		private readonly string id;

		public CustomSelectElement(IWebDriver driver, string id)
		{
			if(driver == null)
			{
				throw new ArgumentNullException(nameof(driver));
			}

			if(id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}

			this.id = id;
			this.driver = driver;
		}

		public IEnumerable<string> Options
		{
			get
			{
				return this.driver.FindElements(By.CssSelector($"#{this.id} option"))
						   .Select(e => e.GetAttribute("value"))
					       .ToArray();
			}
		}

		public void Select(string value)
		{
			IEnumerable<IWebElement> allCustomSelectElements = this.driver.FindElements(By.ClassName("custom-select"));
			IWebElement customSelectElement = allCustomSelectElements.FirstOrDefault(e => IsCustomSelectWithId(e, this.id));

			Policy.Handle<WebDriverException>()
			      .Or<InvalidOperationException>() // can happen when the notifications popup is there
				  .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(2))
			      .Execute(() =>
					{
						this.ClickOption(customSelectElement, value);
					});
		}

		private static bool IsCustomSelectWithId(IWebElement element, string id)
		{
			return element.FindElements(By.Id(id)).Any();
		}

		private void ClickOption(IWebElement customSelectElement, string value)
		{
			WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
			//Actions actions = new Actions(driver);

			// e.g. customSelect-19-combobox
			string inputOverlayId = customSelectElement.FindElement(By.ClassName("input-overlay")).GetAttribute("id");

			// in this case customSelect-19-listbox
			string optionElementId = inputOverlayId.Replace("combobox", "listbox");

			IJavaScriptExecutor executor = (IJavaScriptExecutor)this.driver;
			executor.ExecuteScript("arguments[0].click();", customSelectElement);

			this.driver.Wait(TimeSpan.FromSeconds(1));

			var optionElement = this.driver.FindElements(By.CssSelector($"#{optionElementId} li"))
				.FirstOrDefault(e => string.Equals(e.GetAttribute("data-value"), value, StringComparison.OrdinalIgnoreCase));

			if (optionElement != null)
			{
				executor.ExecuteScript("arguments[0].click();", optionElement);
			}
		}
	}
}

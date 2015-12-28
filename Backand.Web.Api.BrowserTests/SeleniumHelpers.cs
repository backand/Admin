using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Backand.Web.Api.BrowserTests
{
    public static class SeleniumHelpers
    {
        public static void MoveTo(this IWebDriver driver,By locator)
        {
            var element = driver.FindElement(locator);
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        
    }
}

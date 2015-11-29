using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Backand.Web.Api.BrowserTests
{
    [TestClass]
    public class UnitTest1
    {
        IWebDriver driver = new ChromeDriver();

        [TestCleanup]
        public void Cleanup()
        {
            driver.Dispose();
        }

        public void TestLoginWithValidUsernameAndPasswordArriveToAppPage()
        {
            string username = "relly@backand.com";
            string password = "123456";
            string url = "http://localhost:3001/#/";


            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
            driver.Navigate().GoToUrl(url);

            wait.Until((a) => { return a.FindElement(By.Name("uEmail")); });

            // Thread.Sleep(2 * 1000);

            IWebElement email = driver.FindElement(By.Name("uEmail"));
            email.SendKeys(username);
            IWebElement pwd = driver.FindElement(By.Name("uPassword"));
            pwd.SendKeys(password);

            var submit = driver.FindElement(By.ClassName("auth-button"));
            submit.Click();

            wait.Until((a) => { return a.FindElement(By.ClassName("bknd-username")); });

            //Thread.Sleep(2 * 1000);

            var userLabel = driver.FindElement(By.ClassName("bknd-username"));
            //            var labelText = userLabel.Text;
            var labelText = userLabel.Text;

            Assert.AreEqual(labelText, username, "username label is not same");
        }
    }
}

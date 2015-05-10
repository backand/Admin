using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;

namespace BackAnd.Web.Api.Test
{
    [TestClass]
    public class AngulatTest
    {

        private static IWebDriver GetChromeDriver()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(ass.Location);
            //!Make sure to add the path to where you extracting the chromedriver.exe:
            return new ChromeDriver(path); //<-Add your path
            
        }
        private static System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> GetLoginButtons(IWebDriver browser)
        {
            browser.Navigate().GoToUrl("http://localhost:8000/app/index-lte.html#/login");
            Thread.Sleep(2 * 1000);
            var loginElements = browser.FindElements(By.ClassName("btn-lg"));
            return loginElements;
        }


        [TestMethod]
        public void TestExternalLoginGoogleExist()
        {
            var browser = GetChromeDriver();
            var loginElements = GetLoginButtons(browser);
            var googleElement = loginElements.Where(a => a.Text.Contains("Google"));
            Assert.AreEqual(googleElement.Count(), 1);
            
        }

        [TestMethod]
        public void TestExternalLoginGitHubExist()
        {
            var browser = GetChromeDriver();
            var loginElements = GetLoginButtons(browser);
            var gitHubElement = loginElements.Where(a => a.Text.Contains("GitHub"));
            Assert.AreEqual(gitHubElement.Count(), 1);

        }

        [TestMethod]
        public void TestExternalLoginCantLoginWithoutApplication()
        {
            var browser = GetChromeDriver();
            var loginElements = GetLoginButtons(browser);
            var googleElement = loginElements.Where(a => a.Text.Contains("Google")).FirstOrDefault();
            googleElement.Click();
            Thread.Sleep(1 * 1000);

            Assert.IsTrue(browser.FindElements(By.ClassName("has-error")).Any());

        }
    }
}

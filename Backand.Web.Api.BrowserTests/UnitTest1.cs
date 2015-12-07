using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Backand.Config;
using System.Linq;

namespace Backand.Web.Api.BrowserTests
{
    [TestClass]
    public class SeleniumSignup
    {
        IWebDriver driver = new ChromeDriver();

        public string WebAddress
        {
            get
            {
                return "https://www.backand.com";
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Dispose();
        }

        public void TestLoginWithValidUsernameAndPasswordArriveToAppPage()
        {
            string username = "relly@backand.com";
            string password = "123456";
            string url = WebAddress;

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

    public class AutomationTestContext
    {
        public IWebDriver driver;
        public WebDriverWait wait;

        public ServerConfig config;

        public string username
        {

            get
            {
                if (_username == null)
                {
                    _username = GetUserName();
                }

                return _username;
            }
        }

        private string _username;


        public string Password = "12345678";
        public AutomationTestContext()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
            WebPage = "https://www.backand.com/apps/#/sign_up";
            config = ConfigStore.GetConfig();
        }

        public string WebPage { get; set; }

        private string GetUserName()
        {
            return "relly+" + DateTime.Now.Ticks + "@backand.com";
        }
    }

    public static class AutomationTestRunner
    {
        public static AutomationTestContext GoToHomePage(this AutomationTestContext context)
        {
            // 
            context.driver.Navigate().GoToUrl(context.WebPage);

            // Todo Assert
            return context;
        }

        public static AutomationTestContext ClicktOnTryMeButton(this AutomationTestContext context)
        {
            return context;
        }

        public static AutomationTestContext CloseIntercom(this AutomationTestContext context)
        {
            Console.WriteLine("Wait for intercom");
            context.wait.Until((a) => { return a.FindElement(By.ClassName("intercom-launcher-hovercard-close")); }).Click();
            return context;
        }

        public static AutomationTestContext FillSignUpPage(this AutomationTestContext context)
        {
            context.wait.Until((a) => { return a.FindElement(By.Name("uFullFirst")); }).SendKeys("Ygal");
            context.driver.FindElement(By.Name("uEmail")).SendKeys(context.username);
            context.driver.FindElement(By.Name("Upassword")).SendKeys(context.Password);
            context.driver.FindElement(By.Name("confirm_password")).SendKeys(context.Password);
            context.driver.FindElements(By.ClassName("auth-button")).First(a => a.Enabled).Click();
            return context;
        }


    }
}

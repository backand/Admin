using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Backand.Config;
using System.Linq;
using System.Drawing.Imaging;

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
        public WebDriverWait tinyWait;
        public WebDriverWait longWait;


        public ServerConfig config;

        public string username;
        

        public string Password = "12345678";
        public AutomationTestContext()
        {
            driver = new ChromeDriver();
            tinyWait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
            longWait = new WebDriverWait(driver, new TimeSpan(0, 3, 0));

            WebPage = "https://www.backand.com/apps/#/sign_up";
            config = ConfigStore.GetConfig();
            username = GetUserName();
            AppName = GetAppName();
        }

        private string GetAppName()
        {
            return "testApp" + DateTime.Now.Ticks;
        }

        public string WebPage { get; set; }

        private string GetUserName()
        {
            return "relly+" + DateTime.Now.Ticks + "@backand.com";
        }

        public string AppName { get; set; }

        public void TakeScreenshot()
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            //Use it as you want now
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            ss.SaveAsFile(AppName + ".png", ImageFormat.Png); //use any of the built in image formating
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
            context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("intercom-launcher-hovercard-close")));
            context.tinyWait.Until((a) => { return a.FindElement(By.ClassName("intercom-launcher-hovercard-close")); }).Click();

            return context;
        }

        public static AutomationTestContext FillSignUpPage(this AutomationTestContext context)
        {
            context.tinyWait.Until(ExpectedConditions.ElementExists(By.Name("uFullFirst")));

            //todo
            context.tinyWait.Until((a) => { return a.FindElement(By.Name("uFullFirst")); }).SendKeys("Ygal");
            context.driver.FindElement(By.Name("uEmail")).SendKeys(context.username);
            context.driver.FindElement(By.Name("Upassword")).SendKeys(context.Password);
            context.driver.FindElement(By.Name("confirm_password")).SendKeys(context.Password);
            context.tinyWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("auth-button")));
            context.driver.FindElements(By.ClassName("auth-button")).First(c => c.Enabled).Click();
            return context;
        }

        public static AutomationTestContext CreateApp(this AutomationTestContext context)
        {
            context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.Name("appName")));
            context.driver.FindElement(By.Name("appName")).SendKeys(context.AppName);
            context.tinyWait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[test-hook='apps.new-app.create']")));
            context.driver.FindElement(By.CssSelector("[test-hook='apps.new-app.create']")).Click();

            // we arrived to intern app page
            // check "next" button exist
            // thanks to shmuela
            
            context.tinyWait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[test-hook='database-edit.create']")));
            context.driver.FindElement(By.CssSelector("[test-hook='database-edit.create']")).Click();
            return context;
        }

    

        public static AutomationTestContext EnsureAppCreated(this AutomationTestContext context)
        {
            var res = context.longWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("ba-icon-objects")));
            
            if(res == null)
            {
                throw new ElementNotVisibleException("Can't find object menu in tab");
            }

            return context;
        }
                
        public static AutomationTestContext Finish(this AutomationTestContext context)
        {
            context.driver.Dispose();
            return context;
        }



    }
}

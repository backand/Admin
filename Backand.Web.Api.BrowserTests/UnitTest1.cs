using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Backand.Config;
using System.Linq;
using System.Drawing.Imaging;
using System.Data.SqlClient;

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
                return "http://localhost:4110";
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
        //public Logger logger;


        public ServerConfig config;

        public string username;

        public bool hasIntercom;

        public string Password = "12345678";
        public AutomationTestContext()
        {
            driver = new ChromeDriver();
            tinyWait = new WebDriverWait(driver, new TimeSpan(0, 0, 60));
            longWait = new WebDriverWait(driver, new TimeSpan(0, 5, 0));

            //WebPage = "https://www.backand.com/apps/#/sign_up";
            WebPage = GetWebPage();
            hasIntercom = false;
            config = ConfigStore.GetConfig();
            username = GetUserName();
            AppName = GetAppName();
            //logger = new Logger();
        }

        private string GetWebPage()
        {
            return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["WebPage"] ?? "http://localhost:3001/#/sign_up");
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

        public string TakeScreenshot()
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            //Use it as you want now
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            string filename = AppName + ".png";
            ss.SaveAsFile(filename, ImageFormat.Png); //use any of the built in image formating
            return filename;
        }

        public void SendEmail(string screenshot, Exception e)
        {
            //string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            //int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            //string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            //string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);
            
            //string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
            //string defaultTo = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);
            //string[] to = new string[1] { defaultTo };
            //string[] cc = new string[1] { defaultTo };
            //string[] files = new string[1] { screenshot };


            //string message = "The following error occurred:\n\r" + e.ToString();
            
            //Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to, cc, null, " Auto test error", message, from, null, null, false, files, logger);
        }

        public void SendSMS()
        {
            //throw new NotImplementedException();
        }

        public void WriteToEventViewer(Exception e)
        {
            //throw new NotImplementedException();
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
            if (context.hasIntercom)
            {
                Console.WriteLine("Wait for intercom");
                context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("intercom-launcher-hovercard-close")));
                context.tinyWait.Until((a) => { return a.FindElement(By.ClassName("intercom-launcher-hovercard-close")); }).Click();
            }
            
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
            
            //context.tinyWait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[test-hook='database-edit.create']")));
            //context.driver.FindElement(By.CssSelector("[test-hook='database-edit.create']")).Click();
            return context;
        }

        private static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["LogConnectionString"].ConnectionString;
        }

        private static int GetFailureLogType()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FailureLogType"] ?? "1"); 
        }

        private static int GetSuccessLogType()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SuccessLogType"] ?? "3");
        }

        public static AutomationTestContext Log(this AutomationTestContext context, Exception e = null, string freeText = null)
        {
            bool failure = e != null;
            int failureLogType = GetFailureLogType();
            int successLogType = GetSuccessLogType();

            //context.logger.Log("BrowserTest", "app: " + context.AppName, "username: " + context.username, e, failure ? 1 : 3, "");
            string connectionString = GetConnectionString();
            string cmd = "insert into durados_log ([ApplicationName], [Username], [MachineName], [Time], [Controller], [Action], [MethodName], [LogType], [ExceptionMessage], [Trace], [FreeText]) values (@ApplicationName, @Username, @MachineName, @Time, @Controller, @Action, @MethodName, @LogType, @ExceptionMessage, @Trace, @FreeText)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("ApplicationName", context.AppName);
                    command.Parameters.AddWithValue("Username", context.username);
                    command.Parameters.AddWithValue("MachineName", System.Environment.MachineName);
                    command.Parameters.AddWithValue("Time", DateTime.Now);
                    command.Parameters.AddWithValue("Controller", "auto test");
                    command.Parameters.AddWithValue("Action", "Action");
                    command.Parameters.AddWithValue("MethodName", "MethodName");
                    command.Parameters.AddWithValue("LogType", failure ? failureLogType : successLogType);
                    command.Parameters.AddWithValue("ExceptionMessage", e == null ? string.Empty : e.Message);
                    command.Parameters.AddWithValue("Trace", e == null ? string.Empty : e.ToString());
                    command.Parameters.AddWithValue("freeText", freeText == null ? string.Empty : freeText);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            } 
            return context;
        }

        public static AutomationTestContext EnsureAppCreated(this AutomationTestContext context)
        {
            //var res = context.longWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("ba-icon-objects")));
            var res = context.longWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//strong[text()='Your app " + context.AppName + " is ready']")));
            
            if(res == null)
            {
                throw new ElementNotVisibleException("Can't find object menu in tab");
            }

            return context;
        }

        public static AutomationTestContext LogOut(this AutomationTestContext context)
        {
            try
            {
                context.longWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[ng-bind='okBtnText']"))).Click();
                // wait for fadeout animation
                Thread.Sleep(1000);
            }
            catch (Exception exception)
            {
                throw new ElementNotVisibleException("Element probably disappeared because app was not created and logged out", exception);
            }

           

            var res = context.longWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("dropdown-toggle")));

            if (res == null)
            {
                throw new ElementNotVisibleException("Can't find user tag");
            }
            try
            {
                res.Click();
            }
            catch (Exception exception)
            {
                throw new ElementNotVisibleException("Element probably disappeared because app was not created and logged out", exception);
            }

            context.longWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[ng-click='header.logout()']"))).Click();

            return context;
        }

        public static AutomationTestContext SignIn(this AutomationTestContext context)
        {
            context.tinyWait.Until((a) => { return a.FindElement(By.Name("uEmail")); }).SendKeys(context.username);
            context.driver.FindElement(By.Name("uPassword")).SendKeys(context.Password);
            context.driver.FindElements(By.ClassName("auth-button")).First(c => c.Enabled).Click();
            
            return context;
        }

        public static AutomationTestContext OpenApp(this AutomationTestContext context)
        {
            try
            {
                context.longWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[text()[contains(.,'Manage')]]"))).Click();

                
                //context.longWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[test-hook='apps.app-panel.manage-button']"))).Click();
            }
            catch (Exception exception)
            {
                throw new ElementNotVisibleException("Element probably disappeared because app was not created and logged out", exception);
            }

            return context;
        }

        public static AutomationTestContext DeleteApp(this AutomationTestContext context)
        {
            // close the object to ensure settings visibilty
            var res = context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("ba-icon-objects")));
            res.Click();

            res = context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("ba-icon-settings")));
            context.driver.MoveTo(By.ClassName("ba-icon-settings"));
            res.Click();

            // wait for fadein animation
            Thread.Sleep(500);
            context.driver.FindElements(By.CssSelector("[ng-click=\"nav.goTo('app.edit')\"]")).First(c => c.Enabled).Click();

            context.driver.FindElements(By.CssSelector("[ng-click='settings.delete()']")).First(c => c.Enabled).Click();

            // wait for fadein animation
            Thread.Sleep(500);

            context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[ng-bind='okBtnText']"))).Click();

            // wait for fadeout animation
            Thread.Sleep(1000);

            return context;
        }

        public static AutomationTestContext SelectItems(this AutomationTestContext context)
        {
            var res = context.tinyWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("ba-icon-objects")));
            res.Click();

            // wait for fadein animation
            Thread.Sleep(1000);
            context.driver.FindElements(By.CssSelector("[ng-click='nav.showTable(table)']")).First(c => c.Enabled).Click();

            res = context.longWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//strong[text()=' Object - items ']")));

            return context;
        }
                
        public static AutomationTestContext Finish(this AutomationTestContext context)
        {
            context.driver.Dispose();
            return context;
        }

        public static AutomationTestContext GoToAppsPage(this AutomationTestContext context)
        {
            // goto appPage
            context.driver.Navigate().GoToUrl(context.WebPage + "/#/");
            return context;
        }

        public static AutomationTestContext AssertCreatedAppNotExist(this AutomationTestContext context)
        {
            //res = context.tinyWait.Until(ExpectedConditions.El(By.ClassName("ba-icon-settings")));

            Thread.Sleep(10 * 1000);
            var res = new AppPageHelper(context).AppExist(context.AppName);

            if (!res)
            {
                throw new Exception("find app " + context.AppName + " but shouldn't");
 
            }
            Assert.IsFalse(res);

            return context;
        }



    }
}

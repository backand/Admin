using Backand.Web.Api.BrowserTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand.BrowserRunner
{
    class Program
    {
        private static int GetSeqRuns()
        {
            return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SeqRuns"] ?? "5");
        }

        static void Main(string[] args)
        {
            int seqRuns = GetSeqRuns();

            for (int i = 1; i <= seqRuns; i++)
            {
                RunTest(i);
            }
        }

        static void RunTest(int i)
        {
            string processId = Process.GetCurrentProcess().Id.ToString();
            var context = new AutomationTestContext();
            try
            {
                context.Log(null, "P" + processId + "; " + i.ToString() + " - started");
                context
                .GoToHomePage()
                .ClicktOnTryMeButton()
                .CloseIntercom()
                .FillSignUpPage()
                .CreateApp()
                .EnsureAppCreated()
                .LogOut()
                .SignIn()
                .OpenApp()
                .SelectItems();
                context.Log(null, "P" + processId + "; " + i.ToString() + " - ended");
            }
            catch (Exception e)
            {
                try
                {
                    context.Log(e);
                }
                catch (Exception exception)
                {
                    try
                    {
                        context.WriteToEventViewer(exception);
                    }
                    catch { }
                }
                string screenshot = context.TakeScreenshot();
                try
                {
                    context.SendEmail(screenshot, e);
                }
                catch (Exception exception)
                {
                    try
                    {
                        context.WriteToEventViewer(exception);
                    }
                    catch { }
                }
                try
                {
                    context.SendSMS();
                }
                catch (Exception exception)
                {
                    try
                    {
                        context.WriteToEventViewer(exception);
                    }
                    catch { }
                }

            }
            finally
            {
                context.Finish();
            }
            
        }
        
    }
}

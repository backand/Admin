using Backand.Web.Api.BrowserTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand.BrowserRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new AutomationTestContext();
            try
            {
                context
                .GoToHomePage()
                .ClicktOnTryMeButton()
                .CloseIntercom()
                .FillSignUpPage()
                .CreateApp()
                .EnsureAppCreated();
                context.Log();
            }
            catch(Exception e)
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

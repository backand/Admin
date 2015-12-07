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
            }
            catch(Exception e)
            {
                context.TakeScreenshot();
                   
            }
            finally
            {
                context.Finish();
            }
            
        }
    }
}

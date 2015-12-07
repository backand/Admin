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
            new AutomationTestContext()
                .GoToHomePage()
                .ClicktOnTryMeButton()
                .CloseIntercom()
                .FillSignUpPage();
        }
    }
}

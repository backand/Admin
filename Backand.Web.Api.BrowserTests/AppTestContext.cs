using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backand.Web.Api.BrowserTests
{
    public class AppPageHelper
    {
        AutomationTestContext current;


        public AppPageHelper(AutomationTestContext current)
        {
            this.current = current;

            // create availableApps
            _availableApps = GetAllApps(current);
        }

          public List<AppViewModel> AvailableApps
        {
            get
            {
                return _availableApps;
            }
        }

        private List<AppViewModel> GetAllApps(AutomationTestContext current)
        {
            List<AppViewModel> model = new List<AppViewModel>();
            var appsNames = current.driver.FindElements(By.ClassName("body-height")).Select(a => a.Text).ToList();

            // skip first element because "add a new app" button have a ribbon
            var statuses = current.driver.FindElements(By.ClassName("ui-ribbon")).Select(a => a.Text).Skip(1).ToList();

            for (int i = 0; i < appsNames.Count(); i++)
            {
                model.Add(new AppViewModel { Name = appsNames[i], Status = statuses[i] });
            }

            return model;
        }

        public bool AppExist(string appName)
        {
            return AvailableApps.Any(a => a.Name == appName);
        }

        private List<AppViewModel> _availableApps { get; set; }



        public AutomationTestContext Finish()
        {
            return current;
        }
    }

    public class AppViewModel 
    {
        public string Name { get; set; }   

        public string Status {get; set;}
    }

}

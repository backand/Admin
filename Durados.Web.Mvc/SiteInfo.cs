using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc
{
    public class SiteInfo
    {
        public SiteInfo()
        {
            LogOnPath = "~/Views/Account/LogOn.aspx";
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Company { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Product { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Version { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Logo { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LogoHref { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowCompany { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowProduct { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowVersion { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowLogo { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LogOnPath { get; set; }

        public string GetTitle()
        {
            if (Maps.Instance.GetMap().IsMainMap)
                return Durados.Database.ShortProductName;

            string title = Product;
            if (Maps.MultiTenancy && Product == string.Empty)
                title = Maps.GetCurrentAppName();
            return title;
        }

    }
}

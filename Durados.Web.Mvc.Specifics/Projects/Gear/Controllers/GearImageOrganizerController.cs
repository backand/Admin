using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Projects.Gear;

namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class GearImageOrganizerController : Durados.Web.Mvc.Controllers.ImageOrganizerController
    {
        protected override void AddImageValues(View imageView, System.Xml.XmlElement element, Dictionary<string, object> values)
        {
            base.AddImageValues(imageView, element, values);

            string name = element.Attributes["name"].Value;

            string[] foldersAndFileName = name.Split('/');

            string fileName = foldersAndFileName.Last();

            string[] fileNameFragments = fileName.Split('-');

            string lastFilenameFragment = fileNameFragments.Last();

            int galleryID = -1;

            if (lastFilenameFragment.Contains("in"))
                galleryID = 2;
            else if (lastFilenameFragment.Contains("out"))
                galleryID = 3;
            else if (lastFilenameFragment.Contains("spec"))
                galleryID = 4;

            values.Add(gear_ModelImage.FK_gear_ModelImage_gear_Galery_Parent.ToString(), galleryID);

        }

        protected override void AddAdditionals(View view, XmlDocument xml, System.Xml.XmlElement element, DataRow row)
        {
            XmlAttribute names = xml.CreateAttribute("AdditionalsNames");
            names.Value = "versions";
            element.Attributes.Append(names);

            string cars = string.Empty;

            View carView = (Durados.Web.Mvc.View)Map.Database.Views[GearViews.gear_Car.ToString()];

            Dictionary<string, object> filter = new Dictionary<string, object>();

            filter.Add(gear_Car.CarID.ToString(), row[gear_Car.CarID.ToString()].ToString());
            filter.Add(gear_Car.VERSION.ToString(), row[gear_Car.VERSION.ToString()].ToString());

            int rowCount = -1;
            DataView dataView = carView.FillPage(1, 1000, filter, false, null, SortDirection.Asc, out rowCount, null, null);

            foreach (DataRow carRow in dataView.Table.Rows)
            {
                cars += carRow[gear_Car.NAME.ToString()] + ", ";
            }

            XmlAttribute values = xml.CreateAttribute("AdditionalsValues");
            values.Value = cars;
            element.Attributes.Append(values);

        }

        protected override string GetDisplayName(View view, DataRow row)
        {
            return base.GetDisplayName(view, row) + " שנים(" + (row.IsNull(gear_Model.FirstYear.ToString()) ? string.Empty : row[gear_Model.FirstYear.ToString()].ToString()) + "-" + (row.IsNull(gear_Model.LastYear.ToString()) ? DateTime.Now.Year.ToString() : row[gear_Model.LastYear.ToString()].ToString()) + ")";
        }
    }

}

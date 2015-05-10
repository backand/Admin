using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Specifics.Shade.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.Controllers
{

    public class CRMShadeWebSiteController : Controller
    {
        [Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute(true, true)]
        public void AddJob(FormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["firstname"]) || string.IsNullOrEmpty(collection["lastname"]))
                return;

            SqlTransaction transaction = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;


                    bool rollback = false;
                    List<d_Field> fields = new List<d_Field>();
                    string orgID = string.Empty;

                    if (!string.IsNullOrEmpty(collection["company"]))
                    {
                        fields.Add(new d_Field() { Name = "Name", Value = collection["company"] });
                        fields.Add(new d_Field() { Name = "OrgStatusId", Value = "1" });
                        fields.Add(new d_Field() { Name = "OrgTypeId", Value = "2" });

                        orgID = ViewHelper.AddRow("Organizations", fields, command, out rollback);
                        if (rollback)
                            return;
                    }

                    fields = new List<d_Field>();
                    fields.Add(new d_Field() { Name = "FirstName", Value = collection["firstname"] });
                    fields.Add(new d_Field() { Name = "LastName", Value = collection["lastname"] });
                    fields.Add(new d_Field() { Name = "OrgId", Value = orgID });
                    fields.Add(new d_Field() { Name = "OwnerUserId", Value = "8" });
                    //fields.Add(new d_Field() { Name = "Status", Value = "1" });
                    fields.Add(new d_Field() { Name = "Email", Value = collection["email"] });
                    fields.Add(new d_Field() { Name = "Phone", Value = collection["phone"] });
                    fields.Add(new d_Field() { Name = "Cellular", Value = collection["cellphone"] });

                    string contactID = ViewHelper.AddRow("Contacts", fields, command, out rollback);
                    if (rollback)
                        return;


                    fields = new List<d_Field>();
                    fields.Add(new d_Field() { Name = "Name", Value = collection["firstname"] + " " + collection["lastname"] });
                    fields.Add(new d_Field() { Name = "JobStatusId", Value = "0" });
                    fields.Add(new d_Field() { Name = "SalesUserId", Value = "8" });
                    fields.Add(new d_Field() { Name = "ContactId", Value = contactID });
                    fields.Add(new d_Field() { Name = "OrgId", Value = orgID });
                    fields.Add(new d_Field() { Name = "LeadAddress", Value = collection["address"] + " " + collection["city"] + " " + collection["state"] + " " + collection["zip"] });
                    fields.Add(new d_Field() { Name = "ProductCategoryId", Value = ConvertProductCategoryId(collection["installdate"]) });
                    fields.Add(new d_Field() { Name = "InstallationTypeId", Value = ConvertInstallationTypeId(collection["installtype"]) });
                    fields.Add(new d_Field() { Name = "LeadSourceTypeId", Value = ConvertLeadSourceTypeId(collection["hearaboutus1"]) });
                    fields.Add(new d_Field() { Name = "LeadInfoText", Value = collection["hearaboutus"] });
                    fields.Add(new d_Field() { Name = "MotorizedSolutionId", Value = ConvertMotorizedSolutionId(collection["motorized"]) });
                    fields.Add(new d_Field() { Name = "Coupon", Value = collection["promotion"] });

                    string jobID = ViewHelper.AddRow("Jobs", fields, command, out rollback);
                    if (rollback)
                        return;
                    command.Transaction.Commit();

                    string notes = collection["notes"];
                    if (!string.IsNullOrEmpty(notes))
                    {
                        fields = new List<d_Field>();
                        fields.Add(new d_Field() { Name = "Note", Value = notes });
                        fields.Add(new d_Field() { Name = "Auto", Value = "true" });
                        fields.Add(new d_Field() { Name = "JobID", Value = jobID });
                        string noteID = ViewHelper.AddRow("JobNotes", fields);
                        if (rollback)
                            return;

                    }


                    

                }
            }
            catch (Exception exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch { }
                Durados.Web.Mvc.Logging.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
            }

        }

        private string ConvertProductCategoryId(string val)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            switch (val)
            {
                case "Window Shades":
                    return "22";

                case "Window Blinds":
                    return "23";

                case "Skylight Shades":
                    return "24";

                case "Window Shutters":
                    return "25";

                case "Soft Treatment":
                    return "26";
                
                default:
                    return string.Empty;
            }
        }

        private string ConvertInstallationTypeId(string val)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            switch (val)
            {
                case "Residential":
                    return "1";

                case "Office":
                    return "2";

                case "Retail Store":
                    return "3";

                case "Hotel":
                    return "4";

                case "Institution":
                    return "5";

                default:
                    return string.Empty;
            }
        }

        private string ConvertMotorizedSolutionId(string val)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            switch (val)
            {
                case "Yes":
                    return "1";

                case "No":
                    return "2";

                case "Not Sure":
                    return "3";

                default:
                    return string.Empty;
            }
        }

        private string ConvertLeadSourceTypeId(string val)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            switch (val)
            {
                case "From friends":
                    return "9";

                case "Ads":
                    return "6";

                case "Other":
                    return "8";

                default:
                    return string.Empty;
            }
        }
    }
}



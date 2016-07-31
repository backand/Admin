#define Debug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Reflection;

using Durados.Web.Mvc.Controllers;

namespace Durados.Web.ASL.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public class ASLAPIAdGroupController : DuradosController
    {

        protected override void BeforeCreate(CreateEventArgs e)
        {
            string result = null;

            try
            {
                ASLAPI.CampaignWrapper camp = new ASLAPI.CampaignWrapper(e.Values["AuthToken"].ToString(), e.Values["ClientCustomerId"].ToString());
                switch (e.Values["FK_CampaignService_function_Parent"].ToString())
                {
                    case "1": //GET
                        camp.Get(e.Values["Status"].ToString(),
                                Convert.ToInt32(e.Values["paging"].ToString()),
                                e.Values["id"].ToString());
                        break;
                    case "2": //mutate - ADD
                        //result = camp.Create(e.Values["name"].ToString(),
                        //    Convert.ToDouble(e.Values["budgetAmount"]),
                        //    e.Values["budgetPeriod"].ToString(),
                        //    e.Values["biddingStrategy"].ToString(),
                        //    Convert.ToDateTime(e.Values["startDate"]),
                        //    Convert.ToDateTime(e.Values["endDate"]),
                        //    e.Values["status"].ToString());

                        break;
                }
            }
            catch (Exception ex)
            {
                result = "{error}" + ex.ToString();
            }
            finally
            {
                if (e.Values.ContainsKey("result"))
                    e.Values["result"] = result;
                else
                    e.Values.Add("result", result);

                base.BeforeCreate(e);
            }
            //e.Cancel = true;
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            base.BeforeEdit(e);
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            throw new Exception(e.Values["result"].ToString());

            //base.AfterCreateAfterCommit(e);
        }

        protected override string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            string message = null;

            if (exception.Message.StartsWith("{error}"))
            {
                message = exception.Message.Replace("{error}", "");
            }
            else
                message = "Test Result: \n" + exception.Message;

            return message;
        }

    }
}

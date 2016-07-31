using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers
{
    public class MessageBoardController : CrmController
    {
        //string sql;
        //Durados.DataAccess.Filter filter;

        /*
        protected override void SetSql(SelectEventArgs e)
        {
            base.SetSql(e);

            string sql1 = "FROM (SELECT durados_v_MessageBoard.Id, durados_v_MessageBoard.Subject, durados_v_MessageBoard.Message, durados_v_MessageBoard.OriginatedUserId, " +
                  "durados_v_MessageBoard.ViewName, durados_v_MessageBoard.ViewDisplayName, durados_v_MessageBoard.PK, durados_v_MessageBoard.RowDisplayName, " +
                  "durados_v_MessageBoard.CreatedDate, durados_v_MessageBoard.RowLink, durados_v_MessageBoard.ViewLink, derivedtbl_1.UserId, ISNULL(derivedtbl_1.Deleted, 0) " +
                  "AS Deleted, ISNULL(derivedtbl_1.Reviewed, 0) AS Reviewed, ISNULL(derivedtbl_1.Important, 0) AS Important, ISNULL(derivedtbl_1.ActionRequired, 0) AS ActionRequired, " + 
                  "CASE WHEN Reviewed = 1 THEN '' ELSE 'Bold' END AS Css " +
                    "FROM     durados_MessageBoard AS durados_v_MessageBoard WITH (nolock) LEFT OUTER JOIN " +
                      "(SELECT MessageId, UserId, Deleted, Reviewed, Important, ActionRequired " +
                       "FROM      durados_MessageStatus WITH (nolock) " +
                       "WHERE   (UserId = " + GetUserID() + ")) AS derivedtbl_1 ON durados_v_MessageBoard.Id = derivedtbl_1.MessageId " +
                    "WHERE  (ISNULL(derivedtbl_1.Deleted, 0) = 0)) as [durados_v_MessageBoard] ";

            string replace1 = "FROM [durados_v_MessageBoard] with(nolock) ";
            if (e.Sql.Contains(replace1))
                e.Sql = e.Sql.Replace(replace1, sql1);

            filter = (Durados.DataAccess.Filter) e.Filter;
            sql = e.Sql;
        }
        */

        /*
        protected override int? GetRowCount()
        {
            object scalar = null;

            string sql2 = "select count([aaa].[Id]) from (" + sql.Remove(sql.IndexOf("RowNum BETWEEN ")) + " RowNum BETWEEN 1 and 10000000000 " + ") as aaa";

            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql2, connection))
                {
                    foreach (SqlParameter parameter in filter.Parameters)
                    {
                        command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    } 
                    scalar = command.ExecuteScalar();

                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(scalar);
        }
        */

        public override ActionResult Delete(string viewName, string pk, string guid)
        {
            try
            {
                View view = GetView(viewName);
                Dictionary<string, object> values = new Dictionary<string,object>();
                values.Add("Deleted", true);
                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                //SqlAccess sqlAccess = new SqlAccess();
                //Dictionary<string, object> parameters = new Dictionary<string,object>();
                //parameters.Add("@UserId", Convert.ToInt32(GetUserID()));
                //parameters.Add("@MessageId", Convert.ToInt32(pk));
                //parameters.Add("@ActionId", MessageBoardAction.Delete.GetHashCode());
                //parameters.Add("@ActionValue", true);

                //sqlAccess.ExecuteProcedure(Map.Database.ConnectionString, "Durados_MessageBoard_Action", parameters, null);
                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message);
            }
        }

        /*
        public override ActionResult Edit(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                Database.Logger.Log(viewName, "Start", "Edit", "Controller", "", 11, Map.Logger.NowWithMilliseconds(), DateTime.Now);
                Durados.Web.Mvc.View view = GetView(viewName, "Edit");


                Durados.Web.Mvc.UI.Json.View jsonView = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.View>(collection["jsonView"]);

                SqlAccess sqlAccess = new SqlAccess();
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@UserId", Convert.ToInt32(GetUserID()));
                parameters.Add("@MessageId", Convert.ToInt32(pk));
                    
                foreach (Durados.Web.Mvc.UI.Json.Field jsonField in jsonView.Fields.Values)
                {
                        
                    if (jsonField.Name == "Reviewed")
                    {
                        if (parameters.ContainsKey("@ActionId"))
                        {
                            parameters["@ActionId"] = MessageBoardAction.Review.GetHashCode();
                        }
                        else
                        {
                            parameters.Add("@ActionId", MessageBoardAction.Review.GetHashCode());
                        }
                        if (parameters.ContainsKey("@ActionValue"))
                        {
                            parameters["@ActionValue"] = ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString());
                        }
                        else
                        {
                            parameters.Add("@ActionValue", ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString()));
                        }
                        


                        sqlAccess.ExecuteProcedure(Map.Database.ConnectionString, "Durados_MessageBoard_Action", parameters, null);
                
                    }
                    else if (jsonField.Name == "Important")
                    {
                        if (parameters.ContainsKey("@ActionId"))
                        {
                            parameters["@ActionId"] = MessageBoardAction.Important.GetHashCode();
                        }
                        else
                        {
                            parameters.Add("@ActionId", MessageBoardAction.Important.GetHashCode());
                        }
                        if (parameters.ContainsKey("@ActionValue"))
                        {
                            parameters["@ActionValue"] = ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString());
                        }
                        else
                        {
                            parameters.Add("@ActionValue", ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString()));
                        }
                        sqlAccess.ExecuteProcedure(Map.Database.ConnectionString, "Durados_MessageBoard_Action", parameters, null);

                    }
                    else if (jsonField.Name == "ActionRequired")
                    {
                        if (parameters.ContainsKey("@ActionId"))
                        {
                            parameters["@ActionId"] = MessageBoardAction.ActionRequired.GetHashCode();
                        }
                        else
                        {
                            parameters.Add("@ActionId", MessageBoardAction.ActionRequired.GetHashCode());
                        }
                        if (parameters.ContainsKey("@ActionValue"))
                        {
                            parameters["@ActionValue"] = ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString());
                        }
                        else
                        {
                            parameters.Add("@ActionValue", ((ColumnField)view.Fields["Reviewed"]).ConvertFromString(jsonField.Value.ToString()));
                        }
                        sqlAccess.ExecuteProcedure(Map.Database.ConnectionString, "Durados_MessageBoard_Action", parameters, null);

                    }
                }
                Database.Logger.Log(view.Name, "End", "Edit", "Controller", "", 11, Map.Logger.NowWithMilliseconds(), DateTime.Now);

                return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName, ajax = true, guid = guid });
            }
            catch (MessageException exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return PartialView("~/Views/Shared/Controls/Message.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Edit", exception));

            }
        }
        */
        /*
        public override JsonResult EditOnly(string viewName, FormCollection collection, string pk, string guid)
        {
            try
            {
                Durados.Web.Mvc.View view = GetView(viewName, "EditOnly");

                if (!view.IsEditable(guid))
                    throw new AccessViolationException();


                
                Durados.Web.Mvc.UI.Json.View jsonView = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.View>(collection["jsonView"]);

                
                foreach (Durados.Web.Mvc.UI.Json.Field jsonField in jsonView.Fields.Values)
                {

                    if (jsonField.Name == "Important")
                    {
                        SaveMessageAction(view, pk, jsonField, MessageBoardAction.Important);
                    }
                    else if (jsonField.Name == "ActionRequired")
                    {
                        SaveMessageAction(view, pk, jsonField, MessageBoardAction.ActionRequired);
                
                
                    }
                }
                return Json("success");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditOnly", exception));
            }
        }
        */
        public override JsonResult GetJsonView(string viewName, DataAction dataAction, string pk, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName);

            if (viewName == "durados_v_MessageBoard")
            {
                try
                {
                    //SqlAccess sqlAccess = new SqlAccess();
                    //Dictionary<string, object> parameters = new Dictionary<string, object>();
                    //parameters.Add("@UserId", Convert.ToInt32(GetUserID()));
                    //parameters.Add("@MessageId", Convert.ToInt32(pk));
                    //parameters.Add("@ActionId", MessageBoardAction.Review.GetHashCode());
                    //parameters.Add("@ActionValue", true);

                    //sqlAccess.ExecuteProcedure(Map.Database.ConnectionString, "Durados_MessageBoard_Action", parameters, null);
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add("Reviewed", true);
                    view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                
                }
                catch (Exception exception)
                {
                    //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                }
            }

            
            UI.Json.View jsonView;
            if (string.IsNullOrEmpty(pk))
                jsonView = view.GetJsonViewNotSerialized(dataAction, guid);
            else
                jsonView = view.GetJsonViewNotSerialized(dataAction, pk, GetDataRow(view, pk), guid, view_BeforeSelect, null);

            return Json(GetJsonViewSerialized(view, dataAction, jsonView));
        }


        public override void EditOnlyAdditionalSpecificProcess(Durados.Web.Mvc.View view, string pk)
        {
            if (view.Name == "durados_v_MessageBoard")            
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Reviewed", true);
                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
            }
        }

        protected override Durados.Web.Mvc.UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new MessageBoardStyler(view, dataView);
        }
       

        public ActionResult FilterByView(string viewName, string guid)
        {
            //string viewPK = (new Durados.DataAccess.ConfigAccess()).GetViewPK(viewName, Map.GetConfigDatabase().ConnectionString);

            System.Collections.Specialized.NameValueCollection queryString = new System.Collections.Specialized.NameValueCollection();
            queryString.Add("ViewName", viewName);
            queryString.Add("isMainPage", "True");

            Durados.Web.Mvc.UI.Helpers.ViewHelper.SetSessionState(guid + "PageFilterState", queryString);

            //if (!string.IsNullOrEmpty(filter))
            //{
            //    SetSessionFilter("Field", guid, filter, false);
            //}

            string viewDisplayName = string.Empty;
            if (Map.Database.Views.ContainsKey(viewName))
            {
                viewDisplayName = Map.Database.Views[viewName].DisplayName;
            }

            return RedirectToAction("Index", "MessageBoard", new { viewName = "durados_v_MessageBoard", isMainPage = true, ViewDisplayName = viewDisplayName, guid = guid, firstTime = true, SortColumn = "CreatedDate", direction = "Desc" });
        }
    }


    public enum MessageBoardAction
    {
        Delete = 1,
        Review = 2,
        Important = 3,
        ActionRequired = 4
    }

            

    public class MessageBoardStyler : Durados.Web.Mvc.UI.Styler
    {
        public MessageBoardStyler(View view, DataView dataView)
            : base(view, dataView)
        {

        }


        public override string GetRowCss(View view, DataRow row, int rowIndex)
        {
            string CssClassName = string.Empty;

            if (view.Fields.ContainsKey("Css"))
            {
                CssClassName = view.Fields["Css"].GetValue(row);

                if (CssClassName == null) 
                    CssClassName = string.Empty;
                else
                    CssClassName = " " + CssClassName;
            }

            if (rowIndex % 2 == 0)
                return view.ContainerGraphicProperties + " d_fix_row " + CssClassName;
            else
                return "d_alt_row" + CssClassName; 

        }
    }

}




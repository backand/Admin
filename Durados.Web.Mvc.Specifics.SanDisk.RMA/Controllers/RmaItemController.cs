using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Web.Mvc;

namespace Durados.Web.Mvc.Specifics.SanDisk.RMA.Controllers
{
    public class RmaItemController : RmaBaseController
    {
        protected override void RegisterDropDownFilterEvents(Durados.Web.Mvc.View view)
        {
            if (view.Name == "v_Item")
            {
                HashSet<string> parents = new HashSet<string>() { "v_Location", "v_Failure", "v_Correction", "v_RemoteDiagnostic", "v_Repair", "v_EngineeringGroup" };
                foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                {
                    if (parents.Contains(parentField.ParentView.Name))
                    {
                        parentField.DependencyFieldName = "";
                        parentField.InsideTriggerFieldName = "";
                    }
                }
            }

            base.RegisterDropDownFilterEvents(view);
        }

        protected override void DropDownFilter(ParentField parentField, ref string sql)
        {
            HashSet<string> parents = new HashSet<string>() { "v_Location", "v_Failure", "v_Correction", "v_RemoteDiagnostic", "v_Repair", "v_EngineeringGroup" };
            if (parents.Contains(parentField.ParentView.Name))
            {
                SetFilter(ref sql);
            }
        }

        public JsonResult GetParentPart(string viewName, string topMarking,string SKU,string ItemPK,string ProductLine)
        {
            
            View view = GetView(viewName);
            string currentUserId = GetUserID();
            string _54_90 = "";
            string mfgDate = "";
            //string ParentPartId = "";
            try
            {
                bool success= SetParentPart(topMarking, SKU, ItemPK, ProductLine, out _54_90, out mfgDate);//, out ParentPartId, out ParentPartName

                
                    var json = new { ParentPartName = _54_90??"", mfgDate = mfgDate??"" };//, ParentPartId = ParentPartId };
                    return Json(json);
                
                
            }
            catch (DuradosException ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "GetParentPart", ex));
            }
        
           
         
        }

        private bool SetParentPart(string topMarking, string SKU, string ItemPK, string ProductLine,out string  _54_90 ,out string mfgDate  )//, out string ParentPartId, out string ParentPartName)
        {
             _54_90 = "";
             mfgDate = "";
             bool success = true;
           
            try
            {
                SqlCommand command = new SqlCommand();
                SqlConnection connection = new SqlConnection(Map.Database.ConnectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.CommandText = "GetSKUFromDeviceMarking";
                    command.Parameters.Clear();

                    object scalar = null;

                    if (!string.IsNullOrEmpty(topMarking))
                    { 
                        command.Parameters.Add( new SqlParameter {
                                ParameterName = "@Out_54_90",
                                SqlDbType = SqlDbType.NVarChar,
                                Size = 1000,
                                Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter { 
                                ParameterName = "@OutManufacDate",
                                SqlDbType = SqlDbType.NVarChar,
                                Size = 1000,
                                Direction = ParameterDirection.Output });
                        command.Parameters.AddWithValue("@TopMarkingNo", topMarking);
                        command.Parameters.AddWithValue("@SKU", SKU);
                        command.Parameters.AddWithValue("@ProductLineId", ProductLine);
                        command.Parameters.AddWithValue("@ID", ItemPK);
                        
                       
                        scalar = command.ExecuteScalar();


                        if (scalar == null || scalar == DBNull.Value)
                        {
                            success= false;
                        }
                        else
                        {
                            //if (command.Parameters.Contains["@Out_54_90"])
                            _54_90 = Convert.ToString(command.Parameters["@Out_54_90"].Value);
                            mfgDate = Convert.ToString(command.Parameters["@OutManufacDate"].Value);
                        }
                    }
                }
                if (command != null)
                {
                    if (command.Connection != null)
                    {
                        command.Connection.Close();
                    }
                    command.Dispose();
                }
                return success;
            }
            catch (SqlException exception)
            {
                throw new DuradosException(exception.Message, exception);
            }
            catch (Exception ex)
            {
                throw new DuradosException(ex.Message, ex);

            }
        }

        
        private void SetFilter(ref string sql)
        {
            int? rmaId = GetRmaId();

            if (rmaId.HasValue)
            {
                string filter = GetFilter(rmaId.Value, sql);

                if (!string.IsNullOrEmpty(filter))
                {
                    sql += filter;
                }

            }
        }

        private string GetFilter(int rmaId, string sql)
        {
            if (sql.ToLower().Contains(" where "))
                return string.Format(" and RmaId = {0}", rmaId);
            else
                return string.Format(" where RmaId = {0}", rmaId);
        }

        private int? GetRmaId()
        {
            Durados.Web.Mvc.UI.Json.Filter jsonFilter = ViewHelper.ConvertQueryStringToJsonFilter(currentGuid);

            if (jsonFilter == null)
                return null;

            string fkFieldName = "RmaId";
            if (!jsonFilter.Fields.ContainsKey(fkFieldName))
            {
                return null;
            }
            object value = jsonFilter.Fields[fkFieldName].Value;

            if (value == null || value.ToString() == string.Empty)
                return null;

            return Convert.ToInt32(jsonFilter.Fields[fkFieldName].Value);
        }

    }
}

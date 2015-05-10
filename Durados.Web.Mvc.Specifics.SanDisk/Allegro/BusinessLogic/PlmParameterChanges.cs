using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.DataAccess;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic
{
    public class PlmParameterChanges : BETchnicalChanges
    {

        public override Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> GetPlmChangesFromLastChangeRequest(View view, DataView dataView)
        {
            return GetPlmChanges(view, dataView, "vc_PlmParameterChangesAfterLastChangeRequest");
        }

        public override Dictionary<string, Dictionary<string, SortedList<DateTime, PlmChange>>> GetPlmChangesFromLastNewRequest(View view, DataView dataView)
        {
            return GetPlmChanges(view, dataView, "vc_PlmParameterChangesAfterLastNewRequest");
        }

        protected override string GetPlmIDColumnName()
        {
            return "Id";
        }


        public override Dictionary<string, string> GetPlmParametersChangesFromLastChangeRequest(View view, DataView dataView)
        {
            return new Dictionary<string, string>();
        }

        public override Dictionary<string, string> GetPlmParametersChangesFromLastNewRequest(View view, DataView dataView)
        {
            return new Dictionary<string, string>();
        }
    }
    public class PlmChangesUtil
    {
        public virtual  bool IsInChangeRequest(EditEventArgs e)
        {

            int? status = GetStatus(e);
            if (status.HasValue)
            {
                if (status.Value == PLMBEStatus.ChangeRequest || status.Value == PLMBEStatus.NewRequestCR)
                    return true;
            }
           
            return false;

        }

        public virtual void UpdateBEResponseStatus(EditEventArgs e)
        {
            UpdateBEResponseStatus(e, "Id", string.Empty);
        }

        public virtual void UpdateBEResponseStatus(EditEventArgs e, string status)
        {
            UpdateBEResponseStatus(e,"Id",status );
        }

        public virtual bool IsParametersChanged(Durados.View view, OldNewValue[] oldNewValues)
        {
            foreach (OldNewValue oldNewValue in oldNewValues)
            {
                if (view.Fields.ContainsKey(oldNewValue.FieldName))
                {
                    Field field = view.Fields[oldNewValue.FieldName];

                    if (field.Category != null && field.Category.Name.ToLower().Contains("parameter"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool HasParametersChanged(Durados.View view, DataRow dataRow, Dictionary<string, object> values)
        {
            foreach (KeyValuePair<string, object> value in values)
            {
                Field field = view.Fields[value.Key];
                if (field != null)
                {
                    string drVal = field.GetValue(dataRow);
                    string valVal = (value.Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(valVal) || !string.IsNullOrEmpty(drVal))
                    {

                        if ((!string.IsNullOrEmpty(valVal) && string.IsNullOrEmpty(drVal)) || (string.IsNullOrEmpty(valVal) && !string.IsNullOrEmpty(drVal)) || drVal != valVal)
                            return true;
                    }
                }
            }

            return false;
        }
       
        protected virtual int? GetStatus(EditEventArgs e)
        {
            if (e.View.Fields.ContainsKey("FK_PLMBEStatus_v_PLM_Parent"))
            {
                Field statusField = e.View.Fields["FK_PLMBEStatus_v_PLM_Parent"];

                string statusId = statusField.GetValue(e.PrevRow);
                if (string.IsNullOrEmpty(statusId))
                    return null;


                int status = Convert.ToInt32(statusId);

                return status;
            }
            return null;
        }

        protected string[] GetBETechnicalIds(string plmId, SqlCommand command)
        {
            List<string> ids = new List<string>();
            command.Parameters.Clear();
            command.CommandText = string.Format("select Id from BETechnical with(nolock) WHERE BETechnical.PLMId={0} and versionNum=1", plmId);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string id = reader.GetInt32(reader.GetOrdinal("Id")).ToString();
                    ids.Add(id);
                }
                reader.Close();
            }

            return ids.ToArray();
        }

        protected virtual void UpdateBEResponseStatus(EditEventArgs e, string plmIdFieldName, string status)
        {
            SqlCommand command = (SqlCommand)e.Command;
            Field field = (e.View.Fields.ContainsKey(plmIdFieldName) ? e.View.Fields[plmIdFieldName] : null);
            string plmId = field.GetValue(e.PrevRow);
            if (command != null && field != null && plmId != string.Empty)
            {

                SqlAccess sqlAccess = new SqlAccess();
                string v_BETechnicalViewName = "v_BETechnical";
                Dictionary<string, Durados.View> views = e.View.Database.Views;

                if (!views.ContainsKey(v_BETechnicalViewName))
                {
                    throw new DuradosException("BETechnical View is missing");
                }
                View v_BETechnicalView = (View)views[v_BETechnicalViewName];
                Dictionary<string, object> values = new Dictionary<string, object>();

                // find its name
                string BEResponseStatusFieldName = "FK_BEResponseStatus_v_BETechnical_Parent";
                values.Add(BEResponseStatusFieldName, status);

                string id = field.GetValue(e.PrevRow);
                string[] beTechnicalIds = GetBETechnicalIds(plmId, command);

                foreach (string beTechnicalId in beTechnicalIds)
                {
                    sqlAccess.Edit(v_BETechnicalView, values, beTechnicalId, null, null, null, null, (SqlCommand)e.Command, e.History, e.UserId);

                }
            }
        }
    }

    public class PlmParameterChangesUtil : PlmChangesUtil
    {
        protected override int? GetStatus(EditEventArgs e)
        {
            
                if (e.PrevRow == null)
                    throw new DuradosException("Missing previous row in update BE response status.");

                AllegroDataSet1.v_PLMParameterModeRow v_PLMParameterModeRow;
                try
                {
                    v_PLMParameterModeRow = (AllegroDataSet1.v_PLMParameterModeRow)e.PrevRow;
                }
                catch (Exception ex)
                {
                    throw new DuradosException("Could not cast specific PLM Paramerer Row", ex);
                }

                View plmView = (e.View.Database.Views.ContainsKey("v_PLM") ? (View)e.View.Database.Views["v_PLM"] : null);
                if (plmView == null)
                    throw new DuradosException("Missing PLM View, check configuration");

                
                Field statusField = (plmView.Fields.ContainsKey("FK_PLMBEStatus_v_PLM_Parent")?plmView.Fields["FK_PLMBEStatus_v_PLM_Parent"]:null);
                if (statusField != null)
                {
                    DataRow plmRow = null;
                    if (v_PLMParameterModeRow.v_PLMRow == null)
                    {
                        Field plmParentField = e.View.Fields[""];
                        string plmFk = plmParentField.GetValue(v_PLMParameterModeRow);
                        if (string.IsNullOrEmpty(plmFk))
                        {
                            throw new DuradosException("PLM Parent is null.");
                        }
                        plmRow = plmView.GetDataRow(plmFk);
                        if (plmRow == null)
                        {
                            throw new DuradosException("missing PLM Parent row with id=" + plmFk);
                        }
                    }
                    else
                    {
                        plmRow = v_PLMParameterModeRow.v_PLMRow;
                    }
                    string status = statusField.GetValue(plmRow);
                    if (string.IsNullOrEmpty(status))
                        return null;
                    int statusId;
                    if (int.TryParse(status, out statusId))
                        return statusId;
                    else
                        throw new DuradosException("Could not parse status from " + statusId + " to identifier");
                }
                else
                {
                    throw new DuradosException("Missing PLM BE Status field, check configuration");
                }
                
        }

        public override void UpdateBEResponseStatus(EditEventArgs e)
        {
            UpdateBEResponseStatus(e, "FK_v_PLM_PLMParameterMode_Parent",string.Empty);
        }
    
    }
}

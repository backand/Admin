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
using Durados.Web.Membership;

using Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroPLMController : AllegroHomeController
    {
        
        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            base.AfterEditBeforeCommit(e);

            PlmChangesUtil plmUtil = new PlmChangesUtil();
            if (plmUtil.IsInChangeRequest(e) && plmUtil.IsParametersChanged(e.View, e.OldNewValues))
            {
                plmUtil.UpdateBEResponseStatus(e, "11");

                e.Values.Add("ParameterChanged", "1");
            }
        }

        // make sure the View has open rules
        protected override void SetPermanentFilter(View view, Filter filter)
        {
            base.SetPermanentFilter(view, filter);

            AddOpenRule(view);
        }

        private void AddOpenRule(View view)
        {
            if (!view.HasOpenRules)
            {
                Rule rule = new Rule();

                rule.Name = view.Name + " Open Rule";
                rule.WhereCondition = " 1=2 ";
                rule.UseSqlParser = false;
                rule.WorkflowAction = WorkflowAction.Validate;
                rule.DataAction = TriggerDataAction.Open;

                view.Rules.Add(rule.Name, rule);
            }
        }

        public override JsonResult IsDisabled(string viewName, string pk, string guid)
        {
            //return base.IsDisabled(viewName, pk, guid);

            string message = "The original records are sealed";

            View view = GetView(viewName);

            bool disabled = pk != null && GetPLMParent(view, pk) != null;

            var json = new { disabled = disabled, message = message };

            return Json(json);
        }

        public string GetPLMParent(View view, string pk)
        {
            object scalar = null;

            string sql = "SELECT ParentPLMId FROM PLM WHERE Id = " + pk;

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    scalar = command.ExecuteScalar();

                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();

        }

        private string GetCheckListValues(Field field,DataRow row)
        {
            return string.Join(",",((ChildrenField)field).GetCheckListKeys(row).ToArray());
        }

        private bool IsParameterField(Field field)
        {
            if ( field.View.EditableTableName== "PLM" )
            {
                return field.Category != null && field.Category.Name.ToLower() == "parameters";
            }

            return false;
        }

        private void ExecProc(string sqName, SqlCommand command, string plmIdParameterName, int plmId)
        {
            command.CommandText = sqName;

            SqlParameter plmIdParameter = new SqlParameter(plmIdParameterName, plmId);

            command.Parameters.Clear();

            command.Parameters.Add(plmIdParameter);
            
            command.CommandType = CommandType.StoredProcedure;

            command.ExecuteNonQuery();

            command.CommandType = CommandType.Text;
                    
        }

        private bool IsChangeRequestToPor(int newStatus, int oldStatus)
        {
            return (newStatus == PLMBEStatus.POR && oldStatus == PLMBEStatus.ChangeRequest);
        }

        private bool IsNewRequestCRToPOR(int newStatus, int oldStatus)
        {
            return (newStatus == PLMBEStatus.POR && oldStatus == PLMBEStatus.NewRequestCR);
        }

        protected override void AfterCompleteStepAfterCommit(EditEventArgs e)
        {
            base.AfterCompleteStepAfterCommit(e);

            int plmId = Convert.ToInt32(e.PrimaryKey);

            using (SqlConnection connection = new SqlConnection(e.View.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    if (HasPLMBEStatusChangedFromCRToPOR(e))
                    {
                        ExecProc("Durados_Allegro_PLM_CR_to_POR_State", command, "@PLMId", plmId);
                    }
                }
            }
            int? newPLMId;
            Duplicate(e,out newPLMId);
            
            if (HasPLMBEStatusChangedToCR(e))
            {
                UpdatePLMBEStatusToBeforeCR(e, newPLMId);
            }
        }

        private void UpdatePLMBEStatusToBeforeCR(EditEventArgs e,int? newPLMId)
        {
            string plmBEStatusFieldName = "FK_PLMBEStatus_v_PLM_Parent";
            
            Field field = (e.View.Fields.ContainsKey(plmBEStatusFieldName) ? e.View.Fields[plmBEStatusFieldName] : null);
            
            string prevPlmBEStatusId = field.GetValue(e.PrevRow);

            if (newPLMId.HasValue && field != null && prevPlmBEStatusId != string.Empty)
            {
                
                SqlAccess sqlAccess = new SqlAccess();
                
                string v_PLMViewName = "v_PLM";
                Dictionary<string, Durados.View> views = e.View.Database.Views;

                if (!views.ContainsKey(v_PLMViewName))
                {
                    throw new DuradosException("v_PLM View is missing");
                }
                View v_PLMView = (View)views[v_PLMViewName];
                Dictionary<string, object> values = new Dictionary<string, object>();
                

                values.Add(plmBEStatusFieldName, prevPlmBEStatusId);

                string id = newPLMId.Value.ToString(); //field.View.GetPkValue(e.PrevRow).

                sqlAccess.Edit(v_PLMView, values,id, null, null, null, null, (SqlCommand)e.Command, e.History, e.UserId);

            }

        }

        protected override void BeforeCompleteStep(EditEventArgs e)
        {
            
            base.BeforeCompleteStep(e);

           
        }

        private void Duplicate(EditEventArgs e,out int? newPLMId)
        {
            newPLMId = null;
            if (HasPLMBEStatusChangedToCR(e))
            {
                int? plmIdDuplicatedOutput = null;
                int? beTechnicalIdToDuplicateOutput = null;
                int? beTechnicalIdDuplicatedOutput = null;

                using (SqlConnection connection = new SqlConnection(e.View.Database.ConnectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;

                            DuplicateShellow(e, command, out plmIdDuplicatedOutput, out beTechnicalIdToDuplicateOutput, out beTechnicalIdDuplicatedOutput);
                            newPLMId = plmIdDuplicatedOutput;

                        }
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        throw exception;
                    }
                }

                //if (plmIdDuplicatedOutput.HasValue)
                //{
                //    DuplicatePLM(e, plmIdDuplicatedOutput.Value);
                //    DuplicateBETechnical(beTechnicalIdToDuplicateOutput, beTechnicalIdDuplicatedOutput);

                //}
            }
            // do only  if status is change request or por change request!!!!
            PlmChangesUtil plmUtil = new PlmChangesUtil();
            if (plmUtil.IsInChangeRequest(e) && plmUtil.HasParametersChanged(e.View, e.PrevRow, e.Values))
            {
                plmUtil.UpdateBEResponseStatus(e);

            }
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            base.BeforeEdit(e);
            
        }

        private void DuplicateShellow(EditEventArgs e, SqlCommand command, out int? plmIdDuplicatedOutput, out int? beTechnicalIdToDuplicateOutput, out int? beTechnicalIdDuplicatedOutput)
        {
            int plmIdToDuplicate = Convert.ToInt32(e.PrimaryKey);
            string sqName = "Durados_Allegro_DuplicatePLM";
            string plmIdToDuplicateParameterName = "@PLMId";
            string plmIdDuplicatedOutputParameterName = "@NewPLMId";
            string beTechnicalIdDuplicatedOutputParameterName = "@NewBETechnicalId";
            string beTechnicalIdToDuplicateOutputParameterName = "@BETechnicalId";

            DuplicateShellow(plmIdToDuplicate, command, sqName, plmIdToDuplicateParameterName, plmIdDuplicatedOutputParameterName, beTechnicalIdToDuplicateOutputParameterName, beTechnicalIdDuplicatedOutputParameterName, out plmIdDuplicatedOutput, out beTechnicalIdToDuplicateOutput, out beTechnicalIdDuplicatedOutput);

        }

        private void DuplicateShellow(int plmIdToDuplicate, SqlCommand command, string sqName, string plmIdToDuplicateParameterName, string plmIdDuplicatedOutputParameterName, string beTechnicalIdToDuplicateOutputParameterName, string beTechnicalIdDuplicatedOutputParameterName, out int? plmIdDuplicatedOutput, out int? beTechnicalIdToDuplicateOutput, out int? beTechnicalIdDuplicatedOutput)
        {

            command.CommandText = sqName;
            
            SqlParameter plmIdToDuplicateParameter = new SqlParameter(plmIdToDuplicateParameterName, plmIdToDuplicate);

            SqlParameter plmIdDuplicatedOutputParameter = new SqlParameter();
            plmIdDuplicatedOutputParameter.Direction = ParameterDirection.Output;
            plmIdDuplicatedOutputParameter.ParameterName = plmIdDuplicatedOutputParameterName;
            plmIdDuplicatedOutputParameter.Size = 4;

            SqlParameter beTechnicalIdDuplicatedOutputParameter = new SqlParameter();
            beTechnicalIdDuplicatedOutputParameter.Direction = ParameterDirection.Output;
            beTechnicalIdDuplicatedOutputParameter.ParameterName = beTechnicalIdDuplicatedOutputParameterName;
            beTechnicalIdDuplicatedOutputParameter.Size = 4;

            SqlParameter beTechnicalIdToDuplicateOutputParameter = new SqlParameter();
            beTechnicalIdToDuplicateOutputParameter.Direction = ParameterDirection.Output;
            beTechnicalIdToDuplicateOutputParameter.ParameterName = beTechnicalIdToDuplicateOutputParameterName;
            beTechnicalIdToDuplicateOutputParameter.Size = 4;

            command.Parameters.Add(plmIdToDuplicateParameter);
            command.Parameters.Add(plmIdDuplicatedOutputParameter);
            command.Parameters.Add(beTechnicalIdToDuplicateOutputParameter);
            command.Parameters.Add(beTechnicalIdDuplicatedOutputParameter);
            
            command.CommandType = CommandType.StoredProcedure;

            command.ExecuteNonQuery();

            if (plmIdDuplicatedOutputParameter.Value != null && plmIdDuplicatedOutputParameter.Value != DBNull.Value)
                plmIdDuplicatedOutput = Convert.ToInt32(plmIdDuplicatedOutputParameter.Value);
            else
                plmIdDuplicatedOutput = null;

            if (beTechnicalIdToDuplicateOutputParameter.Value != null && beTechnicalIdToDuplicateOutputParameter.Value != DBNull.Value)
                beTechnicalIdToDuplicateOutput = Convert.ToInt32(beTechnicalIdToDuplicateOutputParameter.Value);
            else
                beTechnicalIdToDuplicateOutput = null;

            if (beTechnicalIdDuplicatedOutputParameter.Value != null && beTechnicalIdDuplicatedOutputParameter.Value != DBNull.Value)
                beTechnicalIdDuplicatedOutput = Convert.ToInt32(beTechnicalIdDuplicatedOutputParameter.Value);
            else
                beTechnicalIdDuplicatedOutput = null;
                
        }
        
        private bool HasPLMBEStatusChangedToCR(EditEventArgs e)
        {
            
            Field statusField = e.View.Fields["FK_PLMBEStatus_v_PLM_Parent"];
            if (!e.Values.ContainsKey(statusField.Name))
                return false;
            if (string.IsNullOrEmpty(statusField.GetValue(e.PrevRow)) || e.Values[statusField.Name] == null || e.Values[statusField.Name].Equals(string.Empty))
                return false;

            int newStatus = Convert.ToInt32(e.Values[statusField.Name]);
            int oldStatus = Convert.ToInt32(statusField.GetValue(e.PrevRow));

            bool isPorToChangeRequest = IsPorToChangeRequest(newStatus, oldStatus);
            bool isNewRequestToNewRequestCR = IsNewRequestToNewRequestCR(newStatus, oldStatus);

          
            return isPorToChangeRequest || isNewRequestToNewRequestCR;
        }
   
        private bool HasPLMBEStatusChangedFromCRToPOR(EditEventArgs e)
        {

            Field statusField = e.View.Fields["FK_PLMBEStatus_v_PLM_Parent"];
            if (!e.Values.ContainsKey(statusField.Name))
                return false;
            if (string.IsNullOrEmpty(statusField.GetValue(e.PrevRow)) || e.Values[statusField.Name] == null || e.Values[statusField.Name].Equals(string.Empty))
                return false;

            int newStatus = Convert.ToInt32(e.Values[statusField.Name]);
            int oldStatus = Convert.ToInt32(statusField.GetValue(e.PrevRow));

            bool isChangeRequestToPor = IsChangeRequestToPor(newStatus, oldStatus);
            bool isNewRequestCRToPOR = IsNewRequestCRToPOR(newStatus, oldStatus);
            bool isNewRequestCRToCancel = IsNewRequestCRToCancel(newStatus, oldStatus);

            return isChangeRequestToPor || isNewRequestCRToPOR || isNewRequestCRToCancel;
        }

        private bool IsNewRequestCRToCancel(int newStatus, int oldStatus)
        {
            bool newRequestCRToCanceled= (newStatus == PLMBEStatus.Canceled && oldStatus == PLMBEStatus.NewRequestCR);
            bool changeRequestToCanceled = (newStatus == PLMBEStatus.Canceled && oldStatus == PLMBEStatus.ChangeRequest);
            return newRequestCRToCanceled || changeRequestToCanceled;
        }
  
        private bool IsPorToChangeRequest(int newStatus, int oldStatus)
        {
            return (newStatus == PLMBEStatus.ChangeRequest && oldStatus == PLMBEStatus.POR);
        }

        private bool IsNewRequestToNewRequestCR(int newStatus, int oldStatus)
        {
            return (newStatus == PLMBEStatus.NewRequestCR && oldStatus == PLMBEStatus.NewRequest);
        }

        protected override Durados.Web.Mvc.UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.PLMStyler(view, dataView);
        }

        
    }
}
//private void UpdateBEResponseStatus(EditEventArgs e)
//{

//    SqlCommand command = (SqlCommand)e.Command;
//    Field field = (e.View.Fields.ContainsKey("Id")?e.View.Fields["Id"]:null);
//    string plmId=field.GetValue(e.PrevRow);
//    if (command != null && field != null && plmId != string.Empty)
//    {

//        SqlAccess sqlAccess = new SqlAccess();
//        string v_BETechnicalViewName = "v_BETechnical";

//        if (!Database.Views.ContainsKey(v_BETechnicalViewName))
//        {
//            throw new DuradosException("BETechnical View is missing");
//        }
//        View v_BETechnicalView = (View)Database.Views[v_BETechnicalViewName];
//        Dictionary<string, object> values = new Dictionary<string, object>();

//        // find its name
//        string BEResponseStatusFieldName = "FK_BEResponseStatus_v_BETechnical_Parent";
//        values.Add(BEResponseStatusFieldName, string.Empty);

//        string id = field.GetValue(e.PrevRow);
//        string[] beTechnicalIds = GetBETechnicalIds(plmId, command);

//        foreach (string beTechnicalId in beTechnicalIds)
//        {
//            sqlAccess.Edit(v_BETechnicalView, values, beTechnicalId, null, null, null, e.History, e.UserId);  

//        }
//    }
//}

//private void UpdateBEResponseStatus_old(EditEventArgs e)
//{

//    SqlCommand command = (SqlCommand)e.Command;
//    Field field = (e.View.Fields.ContainsKey("Id") ? e.View.Fields["Id"] : null);
//    string id = field.GetValue(e.PrevRow);
//    if (command != null && field != null && id != string.Empty)
//    {
//        int plmId = Convert.ToInt32(id);
//        string sql = string.Format("Update BETechnical Set BEResponseStatusId=NULL WHERE BETechnical.PLMId={0} AND VersionNum=1", plmId);

//        command.CommandText = sql;
//        command.CommandType = CommandType.Text;
//        int? rowsEffected = null;
//        try
//        {
//            rowsEffected = command.ExecuteNonQuery();
//            // write to history!!!
//        }
//        catch (Exception)
//        {
//            throw new DuradosException("Faild To update BE Technical Response Status");
//        }
//        if (rowsEffected != null && rowsEffected == 0) throw new DuradosException("Faild To update BE Technical Response Status,BE Technical missing version 1");

//    }
//}


//private bool IsInChangeRequest(EditEventArgs e)
//{
//    Field statusField = e.View.Fields["FK_PLMBEStatus_v_PLM_Parent"];

//    if (string.IsNullOrEmpty(statusField.GetValue(e.PrevRow)))
//        return false;


//    int status =  Convert.ToInt32(statusField.GetValue(e.PrevRow));

//    if (status == PLMBEStatus.ChangeRequest || status == PLMBEStatus.NewRequestCR)
//        return true;
//    return false;

//}

//private void DuplicateBETechnical(int? beTechnicalIdToDuplicateOutput, int? beTechnicalIdDuplicatedOutput)
//{
//    if (beTechnicalIdToDuplicateOutput.HasValue && beTechnicalIdDuplicatedOutput.HasValue)
//        Duplicate(Map.Database.Views["v_BETechnical"], beTechnicalIdToDuplicateOutput.Value.ToString(), beTechnicalIdDuplicatedOutput.Value.ToString()); 
//}

//private void DuplicatePLM(EditEventArgs e, int toPlmId)
//{
//    Duplicate(e.View, e.PrimaryKey, toPlmId.ToString());
//}

//private void Duplicate(Durados.View view, string fromPK, string toPK)
//{
//    DuplicateChecklists(view);
//    //Duplicator duplicator = new Duplicator();

//    //duplicator.Duplicate(view, fromPK, toPK);
//}

//private void DuplicateChecklists(Durados.View view)
//{
//    foreach (ChildrenField childrenField in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && !f.Excluded))
//    {
//        if (childrenField.ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
//        {
//            Durados.ParentField parentField = childrenField.GetRelatedParentField();
//            if (parentField != null)
//            {
//                if ((childrenField.ExcludeInInsert && childrenField.ExcludeInUpdate) || childrenField.Name == "FK_v_PLM_v_POR_PORChannel_PLM_Children" || childrenField.Name == "FK_v_BETechnical_v_POR_PORChannel_BETEchnical_Children" || childrenField.Name == "FK_v_BETechnical_v_PLMSDA_MMCA_BETechnical_Children")
//                    parentField.Integral = false;
//                else
//                    parentField.Integral = true;
//            }
//        }
//    }
//}
//private bool ShouldAcceptChange(EditEventArgs e)
//{
//    Field statusField = e.View.Fields["FK_PLMBEStatus_v_PLM_Parent"];
//    if (!e.Values.ContainsKey(statusField.Name))
//        return false;
//    if (string.IsNullOrEmpty(statusField.GetValue(e.PrevRow)) || e.Values[statusField.Name] == null || e.Values[statusField.Name].Equals(string.Empty))
//        return false;

//    int newStatus = Convert.ToInt32(e.Values[statusField.Name]);
//    int oldStatus = Convert.ToInt32(statusField.GetValue(e.PrevRow));

//    bool isChangeRequestToPor = IsChangeRequestToPor(newStatus, oldStatus);
//    bool isNewRequestCRToPOR = IsNewRequestCRToPOR(newStatus, oldStatus);

//    return isChangeRequestToPor || isNewRequestCRToPOR;
//}

       //protected override void AfterEditBeforeCommit(EditEventArgs e)
        //{
        //    base.AfterEditBeforeCommit(e);
            
        //    int plmId = Convert.ToInt32(e.PrimaryKey);

        //    if (IsPlmBESydConfigMatch(e, plmId))
        //    {
        //        ExecProc("Durados_Allegro_SetPLMBEStatus", (SqlCommand)e.Command, "@PLMId", plmId);
        //    }
            
        
        //    if (ShouldAcceptChange(e))
        //    {
        //        ExecProc("Durados_Allegro_PLM_CR_to_POR_State", (SqlCommand)e.Command, "@PLMId", plmId);
        //    }
        //}

        //private bool HasParametersChanged(Durados.View view, DataRow dataRow, Dictionary<string, object> values)
        //{
        //    foreach (KeyValuePair<string,object> value in values)
        //    {
        //        Field field=view.Fields[value.Key];
        //        if (field != null && IsParameterField(field))
        //        {
        //            string drVal = (field.IsCheckList()==false?field.GetValue(dataRow):GetCheckListValues(field,dataRow));
        //            string valVal = (value.Value ?? string.Empty).ToString();
        //            if (!string.IsNullOrEmpty(valVal) || !string.IsNullOrEmpty( drVal))
        //            {
                        
        //                if ((!string.IsNullOrEmpty(valVal) && string.IsNullOrEmpty(drVal)) || (string.IsNullOrEmpty(valVal) && !string.IsNullOrEmpty(drVal)) || drVal != valVal)
        //                    return true;
        //            }
        //        }
        //    }
            
        //    return false;
        //}       //protected override void AfterEditBeforeCommit(EditEventArgs e)
        //{
        //    base.AfterEditBeforeCommit(e);
            
        //    int plmId = Convert.ToInt32(e.PrimaryKey);

        //    if (IsPlmBESydConfigMatch(e, plmId))
        //    {
        //        ExecProc("Durados_Allegro_SetPLMBEStatus", (SqlCommand)e.Command, "@PLMId", plmId);
        //    }
            
        
        //    if (ShouldAcceptChange(e))
        //    {
        //        ExecProc("Durados_Allegro_PLM_CR_to_POR_State", (SqlCommand)e.Command, "@PLMId", plmId);
        //    }
        //}

        //private bool HasParametersChanged(Durados.View view, DataRow dataRow, Dictionary<string, object> values)
        //{
        //    foreach (KeyValuePair<string,object> value in values)
        //    {
        //        Field field=view.Fields[value.Key];
        //        if (field != null && IsParameterField(field))
        //        {
        //            string drVal = (field.IsCheckList()==false?field.GetValue(dataRow):GetCheckListValues(field,dataRow));
        //            string valVal = (value.Value ?? string.Empty).ToString();
        //            if (!string.IsNullOrEmpty(valVal) || !string.IsNullOrEmpty( drVal))
        //            {
                        
        //                if ((!string.IsNullOrEmpty(valVal) && string.IsNullOrEmpty(drVal)) || (string.IsNullOrEmpty(valVal) && !string.IsNullOrEmpty(drVal)) || drVal != valVal)
        //                    return true;
        //            }
        //        }
        //    }
            
        //    return false;
        //}

//private bool IsPlmBESydConfigMatch(SqlCommand command, int plmId)
//{

//    bool isCompare=false;

//    command.CommandText = "Durados_Allegro_PLM_CompareConfig_To_BESysCap";

//    SqlParameter plmIdParameter = new SqlParameter("@PLMId", plmId);

//    SqlParameter isCompareParameter = new SqlParameter("@IsCompare", DbType.Boolean);

//    isCompareParameter.Direction = ParameterDirection.Output;

//    command.Parameters.Clear();

//    command.Parameters.Add(plmIdParameter);

//    command.Parameters.Add(isCompareParameter);

//    command.CommandType = CommandType.StoredProcedure;

//    command.ExecuteNonQuery();

//    command.CommandType = CommandType.Text;

//    if (command.Parameters["@IsCompare"] != null && command.Parameters["@IsCompare"].Value != null && command.Parameters["@IsCompare"].Value != DBNull.Value && Convert.ToBoolean(command.Parameters["@IsCompare"].Value) == true)

//        isCompare = true;

//    return isCompare;


//}
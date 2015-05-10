using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class v_WorkOrderController : SamplesBaseController
    {
        Durados.Web.Mvc.Workflow.Notifier notifier = new Durados.Web.Mvc.Workflow.Notifier();

        protected override void BeforeCompleteStep(EditEventArgs e)
        {

            base.BeforeCompleteStep(e);

            Checkout(e);
        }

        private void Checkout(EditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PrimaryKey))
                return;
            int id = Convert.ToInt32(e.PrimaryKey);

            object category = null;
            int? categoryId = null;
            if (e.Values.ContainsKey("FK_WorkOrderCategory_v_WorkOrder_Parent"))
            {
                category = e.Values["FK_WorkOrderCategory_v_WorkOrder_Parent"];
            }
            else
            {
                Field categoryField = e.View.Fields["FK_WorkOrderCategory_v_WorkOrder_Parent"];
                category = categoryField.GetValue(e.PrevRow);
            }

            if (category != null && !category.Equals(string.Empty))
            {
                categoryId = Convert.ToInt32(category);
            }

            bool isCustomer = categoryId.HasValue && categoryId.Value == 2; // Customer


            if (e.Values.ContainsKey("FK_WorkOrder_WorkOrderStatus_Parent") && e.Values["FK_WorkOrder_WorkOrderStatus_Parent"] != null)
            {
                int status = Convert.ToInt32(e.Values["FK_WorkOrder_WorkOrderStatus_Parent"]);
                if (status == 5 && isCustomer) // 5 = Completed
                {
                    Completed(e.Command, (View)e.View, e.Values, e.PrevRow,id,isCustomer);
                }
            }
            
        }

        private void Completed(IDbCommand command, View view, Dictionary<string, object> values, DataRow prevRow, int id, bool isCustomer)
        {
            //Create approval process and send LRC
            //exec Samples_CreateApprovalProcess  @WOId,1, @LRCDocumentTemplate, @ApprovalProcessId output

            if (isCustomer)
            {
                string sp = "Samples_CreateApprovalProcess";

                command.Parameters.Clear();

                SqlParameter idParameter = new SqlParameter("WOId", id);
                SqlParameter pidParameter = new SqlParameter("PROCESS_ID", 1);
                SqlParameter templateParameter = new SqlParameter("LRCDocumentTemplate", 1);
                SqlParameter outParameter = new SqlParameter();
                outParameter.Direction = ParameterDirection.Output;
                outParameter.ParameterName = "TempID";
                outParameter.Size = 4;
                command.Parameters.Add(idParameter);
                command.Parameters.Add(pidParameter);
                command.Parameters.Add(templateParameter);
                command.Parameters.Add(outParameter);

                command.CommandText = sp;
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    throw new Durados.Workflow.WorkflowEngineException(exception.Message);
                }

                command.Parameters.Clear();
                command.CommandType = CommandType.Text;

                if (outParameter.Value != null && !((System.Data.SqlTypes.SqlString)(outParameter.SqlValue)).IsNull)
                {
                    View approvalProcessView = (View)((ChildrenField)view.Fields["FK_ApprovalProcess_WorkOrder_Children"]).ChildrenView;
                    int approvalProcessId = Convert.ToInt32(outParameter.Value);
                    string document = CreateDocument(approvalProcessId, command, approvalProcessView);
                    Notify(approvalProcessId, approvalProcessView, document);
                }
            }

            //View approvalProcessView = (View)((ChildrenField)view.Fields["FK_ApprovalProcess_WorkOrder_Children"]).ChildrenView;

            //object value = view.Fields["v_ApprovalProcessForDoc_v_WorkOrder_Parent"].GetValue(prevRow);
            //if (value == null || value.Equals(string.Empty))
            //    throw new Durados.Workflow.WorkflowEngineException("Can not move to Completed status. Missing Approval Process.");
            //else
            //{
            //    int approvalProcessId = Convert.ToInt32(value);

            //    string document = CreateDocument(approvalProcessId, command, approvalProcessView);
            //}
                //if (values.ContainsKey("v_ApprovalProcessForDoc_v_WorkOrder_Parent") && values["v_ApprovalProcessForDoc_v_WorkOrder_Parent"] != null && values["v_ApprovalProcessForDoc_v_WorkOrder_Parent"].ToString() != string.Empty)
                //{

                //    int approvalProcessId = Convert.ToInt32(values["v_ApprovalProcessForDoc_v_WorkOrder_Parent"]);

                //    string document = CreateDocument(approvalProcessId, command, approvalProcessView);
                //}
                //else
                //{
                //    throw new Durados.Workflow.WorkflowEngineException("Can not move to Completed status. Missing Approval Process.");
                //}
        }

        //private void Checkout(int id, IDbCommand command, View view, Dictionary<string, object> values, DataRow prevRow, bool isCustomer)
        //{
        //    string sp = "Samples_ItemTransactionCheckout";

        //    command.Parameters.Clear();

        //    SqlParameter idParameter = new SqlParameter("WOId", id);
        //    SqlParameter templateParameter = new SqlParameter("LRCDocumentTemplate", 1);

        //    command.Parameters.Add(idParameter);
        //    command.Parameters.Add(templateParameter);

        //    command.CommandText = sp;
        //    command.CommandType = CommandType.StoredProcedure;

        //    try
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Durados.Workflow.WorkflowEngineException(exception.Message);
        //    }

        //    command.Parameters.Clear();
        //    command.CommandType = CommandType.Text;
            
        //}

        private string CreateDocument(int id, IDbCommand command, View view)
        {
            Durados.Workflow.Document Document = new Durados.Workflow.Document();

            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();

            parameters.Add(Durados.Workflow.DocumentParameters.DocumentFileNameKey.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.DocumentFileNameKey.ToString(), Value = "LRC" });
            parameters.Add(Durados.Workflow.DocumentParameters.TemplateViewName.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.TemplateViewName.ToString(), Value = "DocumentTemplate" });
            parameters.Add(Durados.Workflow.DocumentParameters.TemplateViewFileNameFieldName.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.TemplateViewFileNameFieldName.ToString(), Value = "DocumentName" });
            parameters.Add(Durados.Workflow.DocumentParameters.DocumentFieldName.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.DocumentFieldName.ToString(), Value = "LRCDocument" });
            parameters.Add(Durados.Workflow.DocumentParameters.TemplateFieldName.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.TemplateFieldName.ToString(), Value = "FK_ApprovalProcess_DocumentTemplate_Parent" });
            parameters.Add(Durados.Workflow.DocumentParameters.OverrideExistingFile.ToString(), new Parameter() { Name = Durados.Workflow.DocumentParameters.OverrideExistingFile.ToString(), Value = "False" });
            
            Dictionary<string, object> values = new Dictionary<string, object>();

            DataRow prevRow = view.GetDataRow(id.ToString());
            
            foreach (Field field in view.Fields.Values)
            {
                values.Add(field.Name, field.GetValue(prevRow));
            }

            return Document.Create(this, parameters, view, values, prevRow, id.ToString(), view.Database.ConnectionString, command);
        }



        private void Notify(int id, View view, string document)
        {
            ChildrenField childrenField = (ChildrenField)view.Fields["FK_ApprovalProcessUser_ApprovalProcess_Children"];

            DataView approvalProcessUserDataView = childrenField.GetDataView(id.ToString());
            
            foreach (System.Data.DataRowView row in approvalProcessUserDataView)
            {
                int userId = (int)row.Row["UserId"];
                int approvalProcessUserId = (int)row.Row["Id"];
                string subject = GetSubject((View)childrenField.ChildrenView, approvalProcessUserId);
                string message = GetMessage((View)childrenField.ChildrenView, approvalProcessUserId);

                string to = string.Empty;
                string name = string.Empty;
                GetEmailInfo(userId.ToString(), out to, out name);
                SendEmail(GetDefaultFrom(), to, string.Empty, message, subject, new string[1]{document});
   
            }

            
        }

        private string GetMessage(View view, int id)
        {
            return notifier.GetMessage(this, "Work Order Approval Message", view, null, id.ToString(), GetSiteWithoutQueryString());
        }

        private string GetSubject(View view, int id)
        {
            return notifier.GetSubject(this, "Work Order Approval Subject", view, null, id.ToString());
        }

        
        
    }
}

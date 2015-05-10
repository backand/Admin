using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Web;
using System.Web.Mvc;

namespace Durados.Web.Mvc.Specifics.SanDisk.RMA.Controllers
{
    public class RmaReportController : RmaBaseController
    {
       

        protected override ParentField GetOwnerField(View view, string pk)
        {
            return (ParentField)view.Fields["FK_v_User_v_Report_Parent"];
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);
            CreatePdf(e);
        }
        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            CreatePdf(e);
        }


        protected virtual string GetWordFieldName()
        {
            return "DownloadReport";
        }

        protected virtual string GetPdfFieldName()
        {
            return "PDFDocument";
        }

        protected override string GetDocumentTemplatePK(View view, DataRow row)
        {
            if (sendToCustomer)
                return base.GetDocumentTemplatePK(view, row);
            else
                return string.Empty;
        } 

        SqlCommand command = null;
        SqlTransaction transaction = null;

        protected override void PrepareAttachedFile(string physicalPath, string viewName, string pk)
        {
            if (!System.IO.File.Exists(physicalPath))
            {
                throw new DuradosException("The PDF report is missing. Please save the Report row to generate the PDF file. Then email the report.");
            }

            UpdateReportSentToCustomer(viewName, pk);
        }

        private void UpdateReportSentToCustomer(string viewName, string pk)
        {
            try
            {
                command = new SqlCommand();
                SqlConnection connection = new SqlConnection(Map.Database.ConnectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.CommandText = "durados_RMA_RMAAndItemPromoteManager";
                    command.Parameters.Clear();

                    int id;
                    if (int.TryParse(pk, out id))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@ViewName", viewName);
                        int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
                        command.Parameters.AddWithValue("@UserId", currentUserId);
                        command.ExecuteNonQuery();

                    }
                    else
                    {
                        throw new DuradosException("Failed to parse report Id to int! ");
                    }
                }
                else
                {
                    throw new DuradosException("Unable to Connect to Database! ");
                }
                
            }
            catch (Exception exception)
            {
                throw new DuradosException(exception.Message, exception);
            }
        }

        protected override void AfterEmailSent(string Subject, string To, string body, string file, string viewName, string pk)
        {
            if (transaction != null)
            {
                transaction.Commit();
            }
            if (command != null)
            {
                if (command.Connection != null)
                {
                    command.Connection.Close();
                }
                command.Dispose();
            }
        }

        protected override void HandleSendException(string viewName, string pk)
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
            if (command != null)
            {
                if (command.Connection != null)
                {
                    command.Connection.Close();
                }
                command.Dispose();
            }
        }

        protected virtual void CreatePdf(DataActionEventArgs e)
        {
            ColumnField wordField = (ColumnField)e.View.Fields[GetWordFieldName()];

            string word = null;

            string sql = "select " + wordField.Name + " from " + e.View.Name + " where Id = " + e.PrimaryKey;

            word = (new SqlAccess()).ExecuteScalar(e.View.Database.ConnectionString, sql);

            string virtualPath = wordField.Upload.UploadVirtualPath;
            string path = HttpContext.Server.MapPath(virtualPath);
            if (!string.IsNullOrEmpty(word))
            {
                CreatePdf(e, path, word);
            }
        }

        protected virtual void CreatePdf(DataActionEventArgs e, string path, string word)
        {
            string pdf = CreatePdf(path + word);

            UpdateRow(e, pdf.Replace(path, ""));
        }

        private void UpdateRow(DataActionEventArgs e, string pdf)
        {
            string sql = "update " + e.View.EditableTableName + " set " + GetPdfFieldName() + " = @pdf where Id = @pk";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("pdf", pdf);
            parameters.Add("pk", Convert.ToInt32(e.PrimaryKey));
            (new SqlAccess()).ExecuteNonQuery(e.View.Database.ConnectionString, sql, parameters, null);
        }

        protected override ColumnField GetAttachmentField(View view, string pk)
        {
            return (ColumnField)view.Fields[GetPdfFieldName()];
        }

        bool sendToCustomer = false;

        public override JsonResult GetJsonLetter(string viewName, string pks)
        {
            return base.GetJsonLetter(viewName, pks);
        }

        public override ActionResult GetLetter(string viewName, string pk)
        {
            sendToCustomer = true;

            return base.GetLetter(viewName, pk);
        }

        public JsonResult IsInitiator(string viewName, string pk)
        {
            View view = GetView(viewName);
            string currentUserId = GetUserID();
                
            DataRow reportRow = view.GetDataRow(pk);
            if (reportRow == null)
                return Json(false);
            DataRow rmaRow = reportRow.GetParentRow(((ParentField)view.Fields["FK_v_Rma_v_Report_Parent"]).DataRelation.RelationName);
            if (rmaRow == null)
                return Json(false);

            string initiatorId = rmaRow["RmaInitiatorUserId"].ToString();

            string role = GetUserRow()["Role"].ToString();
            if (initiatorId != currentUserId && role != "CQE" && role != "Developer")
                return Json(false);

            return Json(true);
        }

        protected override string GetTo(View view, string pk, Dictionary<string, object> values)
        {
            if (values == null)
                return string.Empty;
            return base.GetTo(view, pk, values);
        }
    }
}

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


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroBETechnicalController : AllegroHomeController
    {
        

        

        protected override Durados.Web.Mvc.UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.BETechnicalStyler(view, dataView);
        }

        protected override Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.BETechnicalTableViewer(this);
        }

        protected override void BeforeCreate(CreateEventArgs e)
        {
            string plmParentFieldName = "FK_v_PLM_v_BETechnical_Parent";
            if (!e.Values.ContainsKey(plmParentFieldName))
                return;
            string plmFieldValue = e.Values[plmParentFieldName].ToString();

            string plmParentFieldValue = GetPLMParent((View)e.View, plmFieldValue);

            if (string.IsNullOrEmpty(plmParentFieldValue))
                plmParentFieldValue = plmFieldValue;

            e.Values[plmParentFieldName] = plmParentFieldValue;

            base.BeforeCreate(e);
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


        //internal protected bool IsDisabled(View view, string pk)
        //{
        //    return pk != null && GetBEParent(view, pk) == null && HasChildren(view, pk);

        //}

        public override JsonResult IsDisabled(string viewName, string pk, string guid)
        {
            //return base.IsDisabled(viewName, pk, guid);

            string message = "The original records are sealed";

            View view = GetView(viewName);

            bool disabled = IsDisabled(view, pk);
            
            var json = new { disabled = disabled, message = message };

            return Json(json);
        }

        internal protected bool IsDisabled(View view, DataRow row)
        {
            return !row.IsNull("PLMBEChangeRequest") && row["PLMBEChangeRequest"].Equals(1);
        }

        internal protected bool IsDisabled(View view, string pk)
        {
            if (string.IsNullOrEmpty(pk))
                return false;

            object scalar = null;

            string sql = "SELECT PLMBEChangeRequest FROM v_BETechnical WHERE Id = " + pk;

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    scalar = command.ExecuteScalar();

                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return true;
            else
                return scalar.Equals(1);

        }

        //protected string GetBEParent(View view, string pk)
        //{
        //    object scalar = null;

        //    string sql = "SELECT ParentBETechnicalID FROM BETechnical WHERE Id = " + pk;

        //    using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            scalar = command.ExecuteScalar();

        //        }
        //    }

        //    if (scalar == null || scalar == DBNull.Value)
        //        return null;
        //    else
        //        return scalar.ToString();

        //}

        //public bool HasChildren(View view, string pk)
        //{
        //    object scalar = null;

        //    string sql = "SELECT Id FROM BETechnical WHERE ParentBETechnicalID = " + pk;

        //    using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            scalar = command.ExecuteScalar();

        //        }
        //    }

        //    if (scalar == null || scalar == DBNull.Value)
        //        return false;
        //    else
        //        return true;

        //}
    }
}

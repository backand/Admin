using System;
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

namespace Durados.Web.Mvc.Controllers
{
    public class BlockController : CrmController
    {
        public override ActionResult Index(string viewName, int? page, string SortColumn, string direction, string pks, bool? ajax, string pk, string guid, bool? children, string newPk, bool? firstTime, bool? isMainPage, bool? safety, bool? disabled, string path, bool? needChangeDisplayType)
        {
            string targetView;
            if (Request.QueryString["parameters"] != null && Map.Database.Views.ContainsKey(Request.QueryString["parameters"]))
            {
                targetView = Request.QueryString["parameters"];
            }
            else
            {
                targetView = GetViewName();
            }
            if (Request.QueryString["pk"] != null)
            {
                pk = Request.QueryString["parameters"];
            }


            if (string.IsNullOrEmpty(pk))
            {
                pk = GetFirstPK(targetView);
            }

            if (string.IsNullOrEmpty(pk))
                throw new DuradosException("The view " + targetView + " has no rows, or was not found.");

            return RedirectToAction("BlocksNamesIndex", new { viewName = targetView, pk = pk });
        }

        private string GetFirstPK(string viewName)
        {
            return GetFirstPK(GetView(viewName));
        }
        private string GetFirstPK(View view)
        {
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(view.ConnectionString);
            if (Map.Database.Views.ContainsKey(view.Name))
                return sqlAccess.GetFirstPK(Map.Database.Views[view.Name]);
            else
                return null;
        }


        protected virtual string GetViewName()
        {
            return "v_Proposal";
        }

        public virtual ActionResult BlocksNamesIndex(string viewName, string pk)
        {
            View view = GetView(viewName);

              if (view == null)
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "View was not found");
            DataView dataView = BuildDictionary(view, pk);

            //dataView.Table.ExtendedProperties.Add("guid", GetUniqueName(view));
            //ViewData["jsonView"] = GetJsonViewSerialized(view, DataAction.Create, view.GetJsonViewNotSerialized(DataAction.Create));

            return View("Block", dataView);

        }

        private DataView BuildDictionary(View view, string pk)
        {

            View blockView = GetView("Block");

            DataTable dataTable = blockView.DataTable.Copy();
            dataTable.CaseSensitive = true;

            List<string> names = new List<string>();
            Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
            LoadNames(names, view, null, view, view.DisplayName + ".", string.Empty, string.Empty, pk, dicFields, string.Empty);

            //dataTable.Rows.Clear();
            foreach (string name in names)
            {
                dataTable.Rows.Add(new object[1] { name });
            }

            DataView dataView = new DataView(dataTable);
            return dataView;
        }

        public ActionResult GetDictionary(string viewName, string dicType)
        {
            if (string.IsNullOrEmpty(viewName)) { return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "View was not found"); }

            View view = GetView(viewName);

            if (view == null)
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "View was not found");

            string pk = GetFirstPK(view);

            DictionaryType dictionaryType;
            try
            {
                dictionaryType = (DictionaryType)Enum.Parse(typeof(DictionaryType), dicType);
            }
            catch (Exception)
            {

                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "Dictionary type is not recognize");
            }

            if (dictionaryType != DictionaryType.PlaceHolders && string.IsNullOrEmpty(pk))
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", "View was not found. or has no rows");


            if (dictionaryType == DictionaryType.InternalNamesPlaceHolders)
            { 
                ViewData["PlaceHolders"] = GetDataViewForDictionary(view, pk, DictionaryType.PlaceHolders);
                dictionaryType = DictionaryType.InternalNames;
            }
            DataView dataView = GetDataViewForDictionary(view, pk, dictionaryType);
            ViewData["DictionaryType"] = dictionaryType;
           
            return PartialView("~/Views/Shared/Controls/Block.ascx", dataView);
        }

        private DataView GetDataViewForDictionary(View view, string pk, DictionaryType dictionaryType)
        {

            switch (dictionaryType)
            {

                case DictionaryType.PlaceHolders:
                    return GetPlaceHolderDictionary();

                case DictionaryType.InternalNames:
                    return GetInternalNameDictionary(view, null);

              
                default:
                    return GetDisplayNameDictionary(view, pk);
            }


        }

        private DataView GetInternalNameDictionary(View view, string pk)
        {
            DataTable dt = GetDictionarySchema();
            string dynastyPath = view.DisplayName + ".";
            foreach (Field field in view.Fields.Values.Where(f => !f.Excluded && (f.FieldType == FieldType.Column || f.FieldType == FieldType.Parent)))
            {

                dt.Rows.Add(dynastyPath + field.DisplayName, field.Name, field.DataType);
            }

            DataView dataView = new DataView(dt);
            return dataView;
        }


        private DataView GetPlaceHolderDictionary()
        {
            DataTable dt = GetDictionarySchema();
            dt.Rows.Add("User Id", Durados.Database.SysUserPlaceHolder, DataType.Numeric);
            dt.Rows.Add("User Role", Durados.Database.SysRolePlaceHolder, DataType.ShortText);
            dt.Rows.Add("Username", Durados.Database.SysUsernamePlaceHolder, DataType.ShortText);
            //   dt.Rows.Add("Current Date", Durados.Database.CurrentDatePlaceHolder);
            DataView dataView = new DataView(dt);
            return dataView;
        }

        private static DataTable GetDictionarySchema()
        {

            DataTable dt = new DataTable("View");
            dt.Columns.Add("Tag", typeof(string));
            dt.Columns.Add("Token", typeof(string));
            dt.Columns.Add("DataType", typeof(DataType));
            return dt;
        }

        private DataView GetDisplayNameDictionary(View view, string pk)
        {
            View blockView = GetView("Block");

            DataTable dt = GetDictionarySchema();
            dt.CaseSensitive = true;

            List<string> names = new List<string>();
            Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
            LoadNames(names, view, null, view, view.DisplayName + ".", string.Empty, string.Empty, pk, dicFields, view.Name + ".");

            //dataTable.Rows.Clear();
            //foreach (string name in names)
            //{
            //    dt.Rows.Add( name, name );
            //}
            foreach (string name in dicFields.Keys)
            {
                dt.Rows.Add(dicFields[name].DisplayName, name, dicFields[name].Type);

            }
            DataView dataView = new DataView(dt);
            return dataView;
        }

        protected void LoadNames(List<string> names, View view, ParentField parentField, View rootView, string dynastyPath, string prefix, string postfix, string pk, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (pk == null)
            {
                LoadNames(names, view, parentField, rootView, dynastyPath, prefix, postfix);
            }
            else
            {
                DataRow row = view.GetDataRow(pk);
                Dictionary<string, object> values = new Dictionary<string, object>();
                LoadValues(values, row, view, parentField, rootView, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
                
                foreach (string name in values.Keys)
                {
                    names.Add(name);
                }
            }
        }

        protected void LoadNames(List<string> names, View view, ParentField parentField, View rootView, string dynastyPath, string prefix, string postfix)
        {
            if (view.Equals(rootView))
                dynastyPath = view.DisplayName + ".";

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                LoadName(names, view, field, dynastyPath, prefix, postfix);
            }

            foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate))
            {
                LoadName(names, view, field, dynastyPath, prefix, postfix);
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (view.Equals(rootView))
                    dynastyPath = view.DisplayName + ".";

                LoadName(names, view, field, dynastyPath, prefix, postfix);

                View parentView = (View)field.ParentView;

                if (parentView != rootView && !IsRecursive(dynastyPath + field.DisplayName))
                {
                    //dynastyPath += field.DisplayName + ".";
                    dynastyPath = GetDynastyPath(dynastyPath, parentField, field);
                    LoadNames(names, parentView, field, rootView, dynastyPath, prefix, postfix);
                }

            }
        }

        protected override string GetDynastyPath(string dynastyPath, ParentField parentField, ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.DisplayName + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.DisplayName)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.DisplayName + ".";
                }
            }

            return dynastyPath += field.DisplayName + ".";
        }

        protected bool IsRecursive(string dynastyPath)
        {
            string[] s = dynastyPath.Split('.');

            int l = s.Length;

            for (int rank = 2; rank <= l / 2; rank++)
            {
                if (IsRecursive(dynastyPath, rank))
                    return true;
            }

            return false;
        }

        protected bool IsRecursive(string dynastyPath, int rank)
        {
            string[] s = dynastyPath.Split('.');

            string last = string.Empty;

            if (s.Length > rank)
            {
                for (int i = s.Length - 1; i >= s.Length - rank; i--)
                {
                    last += s[i] + ".";
                }

                return dynastyPath.Contains(last);
            }

            return false;
        }

        protected void LoadName(List<string> names, View view, Field field, string dynastyPath, string prefix, string postfix)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            if (!names.Contains(name))
                names.Add(name);
        }

    }
}

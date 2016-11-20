using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.Controllers.Api;
using System.Reflection;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class viewConfigController : wfController
    {
        
        #region config

        public IHttpActionResult Get(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }

                ConfigAccess ConfigAccess = new ConfigAccess();
                var pk = ConfigAccess.GetViewPK(view.Name, Map.GetConfigDatabase().ConnectionString);
                var item = RestHelper.Get(GetView(GetConfigViewName()), pk, true, view_BeforeSelect, view_AfterSelect);
                
                return Ok(item);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                
            }

            
        }

        protected virtual string GetConfigViewName()
        {
            return "View";
        }

        [Route("1/table/config")]
        [Route("1/view/config")]
        [HttpGet]
        public IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {

            try
            {
                View view = (View)Map.GetConfigDatabase().Views[GetConfigViewName()];

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;
                if (!string.IsNullOrEmpty(filter))
                {
                    filterArray = JsonConverter.DeserializeArray(filter);
                }

                Dictionary<string, object>[] sortArray = null;
                if (!string.IsNullOrEmpty(sort))
                {
                    sortArray = JsonConverter.DeserializeArray(sort);
                }

                var items = RestHelper.Get(view, withSelectOptions ?? false, false, pageNumber ?? 1, pageSize ?? 20, filterArray, search, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect);

                return Ok(items);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        const string durados_Schema = "durados_Schema";
        const string EditableTableName = "EditableTableName";
        const string Name = "Name";
        
        protected override void AfterCreateAfterCommit(Durados.CreateEventArgs e)
        {
           
            base.AfterCreateAfterCommit(e);
            
            if (e.View.Name == "View")
            {
                if (e.Values.ContainsKey(Name))
                {
                    string name = e.Values[Name].ToString();
                    if (!e.Values.ContainsKey(EditableTableName))
                    {
                        e.Values.Add(EditableTableName, name);
                    }
                    else if (e.Values[EditableTableName] == null)
                    {
                        e.Values[EditableTableName] = string.Empty;
                    }
                    string editableTableName = e.Values[EditableTableName].ToString();
                    Map.CreateView(name, e.PrimaryKey, editableTableName, (View)e.View);
                    Initiate();
                }
            }
        }

        protected void Initiate()
        {
            lock (Map)
            {
                string fileName = Map.GetConfigDatabase().ConnectionString;
                Database configDatabase = Map.GetConfigDatabase();
                Map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(Map.Database.ConfigVersion, fileName);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Map.Logger);
                Map.SaveDynamicMapping();
                Map.Initiate();
                ConfigAccess.Refresh(fileName);
                configDatabase = Map.GetConfigDatabase();
            }
        }


        const string NewFieldToken = "NewField$$";

        protected override void BeforeCreate(Durados.CreateEventArgs e)
        {
            base.BeforeCreate(e);

            if (e.View.Name == "View")
            {
                if (e.Values.ContainsKey("name") && !e.Values.ContainsKey(Name))
                {
                    e.Values.Add(Name, e.Values["name"]);
                    e.Values.Remove("name");
                }
                if (e.Values.ContainsKey(Name) && e.Values[Name] != null && e.Values[Name].ToString() != string.Empty)
                {
                    string name = e.Values[Name].ToString();
                    if (Map.IsViewAlreadyExists(name))
                    {
                        throw new Durados.DuradosException("The table already exists");
                    }
                }
                else
                {
                    throw new Durados.DuradosException("Name is required!");
                }

                string fileName = Map.GetConfigDatabase().ConnectionString;
                e.Values["Views_Parent"] = Durados.DataAccess.ConfigAccess.GetDatabasePK(fileName);
                SetViewDefaultsIfValueIsNull(e);
                if (e.Values["Views_Parent"] == null || e.Values["Views_Parent"].ToString() == string.Empty)
                {
                    throw new Durados.DuradosException("Database configuration is missing.");
                }
            }
            else if (e.View.Name == "Field")
            {
                UpdateViewId(e);
                if (e.Values.ContainsKey("DisplayName") && e.Values["DisplayName"] != null && !e.Values["DisplayName"].Equals(string.Empty))
                {
                    if (e.Values["DisplayName"].Equals(NewFieldToken))
                    {
                        string configViewPk = string.Empty;
                        configViewPk = e.Values["Fields_Parent"].ToString();
                        Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                        string viewName = configAccess.GetViewNameByPK(configViewPk, Map.GetConfigDatabase().ConnectionString);
                        View view = (View)Map.Database.Views[viewName];
                        e.Values["DisplayName"] = GetNewFieldName(view);
                    }
                }
                else
                {
                    throw new Durados.DuradosException("Display Name is required!");
                }

                CreateField(e);
                //e.Values["Name"] = e.Values["DisplayName"].ToString().Replace(" ", "_");

            }
        }

        protected virtual string CreateField(Durados.DataActionEventArgs e)
        {
            return CreateField(e, e.Values);
        }

        protected string newFieldPk = null;

        protected virtual string CreateField(Durados.DataActionEventArgs e, Dictionary<string, object> values2)
        {
            lock (this)
            {
                if (!values2.ContainsKey("JsonName"))
                {
                    values2.Add("JsonName", values2["DisplayName"].ToString().ReplaceNonAlphaNumeric2());
                }
                string configViewPk = e.Values["Fields_Parent"].ToString();
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(configViewPk, Map.GetConfigDatabase().ConnectionString);
                ((View)Map.Database.Views[viewName]).ValidateUniqueFieldJsonName(values2["JsonName"].ToString());


                string manyToManyViewName = null;
                string columnName = Map.CreateField(viewName, configViewPk, values2, out manyToManyViewName);
                e.Cancel = true;
                Initiate();

                UpdateDisplayFormatForNewColumn(e, configViewPk, viewName, columnName);
                SetAutoIncrement(e.Values, e);
                string newDataType = e.Values["DataType"].ToString();

                string newFieldName = e.Values["DisplayName"].ToString();
                if (newFieldName.StartsWith(NewFieldToken.TrimEnd("$$".ToCharArray())))
                {
                    View view = (View)Map.Database.Views[viewName];
                    Durados.Field field = view.GetFieldByColumnNames(columnName);
                    string fieldName = field.Name;
                    newFieldPk = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);

                }

                if (newDataType == Durados.DataType.SingleSelect.ToString() || newDataType == Durados.DataType.ImageList.ToString())
                {
                    View view = (View)Map.Database.Views[viewName];
                    Durados.Field field = view.GetFieldByColumnNames(columnName);
                    string fieldName = field.Name;
                    string newFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);

                    Dictionary<string, object> values = new Dictionary<string, object>();
                    //if (e.Values.ContainsKey("DisplayName"))
                    //    values.Add("DisplayName", e.Values["DisplayName"]);
                    //if (e.Values.ContainsKey("Description"))
                    //    values.Add("Description", e.Values["Description"]);
                    LoadCreateValues(e, values);
                    if (e.Values.ContainsKey("Order"))
                        values.Add("Order", e.Values["Order"]);
                    if (e.Values.ContainsKey("OrderForCreate"))
                        values.Add("OrderForCreate", e.Values["OrderForCreate"]);
                    if (e.Values.ContainsKey("OrderForEdit"))
                        values.Add("OrderForEdit", e.Values["OrderForEdit"]);

                    if (!e.Values.ContainsKey("InlineAdding"))
                    {
                        values.Add("InlineAdding", true);
                    }

                    if (!e.Values.ContainsKey("InlineEditing"))
                    {
                        values.Add("InlineEditing", true);
                    }

                    e.View.Edit(values, newFieldPK, null, null, null, null);

                    View parentView = (View)((ParentField)field).ParentView;
                    if (field.FieldType == Durados.FieldType.Parent && ((ParentField)field).ParentView.Fields.ContainsKey("Ordinal"))
                    {
                        string configOrdinalViewPk = configAccess.GetViewPK(parentView.Name, Map.GetConfigDatabase().ConnectionString);

                        string ordinalFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configOrdinalViewPk, "Ordinal");
                        if (!string.IsNullOrEmpty(ordinalFieldPK))
                        {
                            values = new Dictionary<string, object>();
                            values.Add("HideInTable", true);
                            values.Add("HideInCreate", true);
                            values.Add("HideInEdit", true);
                            values.Add("InlineAdding", true);
                            values.Add("InlineEditing", true);
                            e.View.Edit(values, ordinalFieldPK, null, null, null, null);

                        }
                    }

                    // if new view
                    if (!e.Values.ContainsKey("RelatedViewName"))
                        UpdateNewRelatedViewWorkspace(view, parentView);
                    UpdateNewRelatedViewLayout(view, parentView);

                    RefreshConfigCache();
                }
                else if (newDataType == Durados.DataType.MultiSelect.ToString())
                {

                    //Initiate();


                    View view = (View)Map.Database.Views[manyToManyViewName];
                    View view2 = (View)Map.Database.Views[viewName];

                    ParentField field = null;
                    ParentField field2 = null;
                    foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Parent))
                    {
                        if (parentField.ParentView.Name.Equals(viewName))
                        {
                            field = parentField;

                        }
                        else
                            field2 = parentField;
                    }
                    if (field == null)
                        return null;

                    ChildrenField childrenField = (ChildrenField)field.GetEquivalentChildrenField();

                    string fieldName = childrenField.Name;
                    string newFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);

                    Dictionary<string, object> values = new Dictionary<string, object>();
                    //if (e.Values.ContainsKey("DisplayName"))
                    //    values.Add("DisplayName", e.Values["DisplayName"]);
                    //if (e.Values.ContainsKey("Description"))
                    //    values.Add("Description", e.Values["Description"]);
                    LoadCreateValues(e, values);
                    if (e.Values.ContainsKey("Order"))
                        values.Add("Order", e.Values["Order"]);
                    if (e.Values.ContainsKey("OrderForCreate"))
                        values.Add("OrderForCreate", e.Values["OrderForCreate"]);
                    if (e.Values.ContainsKey("OrderForEdit"))
                        values.Add("OrderForEdit", e.Values["OrderForEdit"]);

                    int? categoryId = ConfigAccess.GetFirstCategoryId(configViewPk, Map.GetConfigDatabase().ConnectionString);
                    if (!categoryId.HasValue)
                    {
                        categoryId = ConfigAccess.AddCategory(Map.GetConfigDatabase().ConnectionString);
                    }
                    if (categoryId.HasValue)
                    {
                        values.Add("Category_Parent", categoryId.ToString());
                    }
                    e.View.Edit(values, newFieldPK, null, null, null, null);

                    try
                    {
                        UpdateNewRelatedViewWorkspace(view2, view);
                        UpdateNewRelatedViewWorkspace(view2, (View)field2.ParentView);
                        UpdateNewRelatedViewLayout(view2, view);
                        UpdateNewRelatedViewLayout(view2, (View)field2.ParentView);

                    }
                    catch { }
                    Initiate();
                }
                else
                {
                    View view = (View)Map.Database.Views[viewName];
                    Durados.Field field = view.GetFieldByColumnNames(columnName);
                    string fieldName = field.Name;
                    string newFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);

                    Dictionary<string, object> values = new Dictionary<string, object>();
                    //if (e.Values.ContainsKey("DisplayName"))
                    //    values.Add("DisplayName", e.Values["DisplayName"]);
                    //if (e.Values.ContainsKey("Description"))
                    //    values.Add("Description", e.Values["Description"]);
                    LoadCreateValues(e, values);
                    if (newDataType == "LongText")
                    {
                        values.Add("DataType", newDataType);
                    }

                    e.View.Edit(values, newFieldPK, null, null, null, null);
                    RefreshConfigCache();
                }

                return columnName;
            }
        }

        
        private void LoadCreateValues(Durados.DataActionEventArgs e, Dictionary<string, object> values)
        {
            IEnumerable<Durados.Field> fields = e.View.Fields.Values.Where(f => !f.HideInCreate && !f.ExcludeInInsert && !f.Excluded);

            HashSet<string> names = new HashSet<string>() { "Fields_Parent", "RelatedViewName", "Name", "DataType", "Formula", "DatabaseNames", "DisplayFormat" };

            //Database db = Map.GetDefaultDatabase();
            //Field f = db.Views["Table"].Fields["Column"];

            foreach (Durados.Field field in fields)
            {
                if (!values.ContainsKey(field.Name) && !names.Contains(field.Name))
                {
                    if (e.Values.ContainsKey(field.Name))
                        values.Add(field.Name, e.Values[field.Name]);
                }
            }

            if (!values.ContainsKey("JsonName"))
                values.Add("JsonName", e.Values["JsonName"]);
            else
                values["JsonName"] = e.Values["JsonName"];
        }
        private void UpdateDisplayFormatForNewColumn(Durados.DataActionEventArgs e, string configViewPk, string viewName, string columnName)
        {
            string displayFormatFieldName = "DisplayFormat";
            View view = (View)Map.Database.Views[viewName];
            Durados.Field field = view.GetFieldByColumnNames(columnName);
            if (field == null)//children
                return;
            string fieldName = field.Name;
            string newFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (e.Values.ContainsKey(displayFormatFieldName))
            {

                object formatVal = e.Values[displayFormatFieldName];

                if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(Durados.DataType.Image.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(Durados.DisplayFormat.None.ToString())))
                {
                    formatVal = Durados.DisplayFormat.Fit.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(Durados.DataType.Url.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(Durados.DisplayFormat.None.ToString())))
                {
                    formatVal = Durados.DisplayFormat.Hyperlink.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(Durados.DataType.Email.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(Durados.DisplayFormat.None.ToString())))
                {
                    formatVal = Durados.DisplayFormat.Email.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(Durados.DataType.Html.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(Durados.DisplayFormat.None.ToString())))
                {
                    formatVal = Durados.DisplayFormat.Html.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(Durados.DataType.DateTime.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(Durados.DisplayFormat.None.ToString())))
                {
                    Durados.DisplayFormat displayFormat = Durados.DisplayFormat.Date_Custom;
                    string dateFormat = Map.Database.DateFormat;

                    if (dateFormat == "MM/dd/yyyy")
                    {
                        displayFormat = Durados.DisplayFormat.Date_mm_dd;
                    }
                    else if (dateFormat == "dd/MM/yyyy")
                    {
                        displayFormat = Durados.DisplayFormat.Date_dd_mm;
                    }
                    formatVal = displayFormat.ToString();
                }

                values.Add(displayFormatFieldName, formatVal);
                if (!string.IsNullOrEmpty(formatVal.ToString()))
                    SetDisplayFormatBehaviors(values, formatVal.ToString(), typeof(Durados.DisplayFormat), e);
            }

            HandleCategory(field, values);

            e.View.Edit(values, newFieldPK, null, null, null, null);

        }

        private void SetDisplayFormatBehaviors(Dictionary<string, object> values, string formatVal, Type type, Durados.DataActionEventArgs e)
        {
            Durados.DisplayFormat displayFormat = (Durados.DisplayFormat)Enum.Parse(type, formatVal);
            switch (displayFormat)
            {
                case Durados.DisplayFormat.SingleLine:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.Text);
                    break;
                case Durados.DisplayFormat.MultiLines:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.TextArea);
                    break;
                case Durados.DisplayFormat.Email:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.Email);
                    break;
                case Durados.DisplayFormat.Password:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.Password);
                    break;
                case Durados.DisplayFormat.SSN:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.SSN);
                    break;
                case Durados.DisplayFormat.Phone:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.Phone);
                    break;
                case Durados.DisplayFormat.Html:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.Html);
                    break;
                case Durados.DisplayFormat.MultiLinesEditor:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.TextArea);
                    UpdateFormatFields(values, "Rich", true);
                    break;
                case Durados.DisplayFormat.Currency:
                    UpdateFormatFields(values, "SpecialColumn", Durados.SpecialColumn.Currency);
                    UpdateFormatFields(values, "Format", "c");
                    break;
                case Durados.DisplayFormat.NumberWithSeparator:
                    UpdateFormatFields(values, "Format", "#,##0.00");
                    break;
                case Durados.DisplayFormat.Percentage:
                    UpdateFormatFields(values, "Format", "p");
                    break;
                case Durados.DisplayFormat.DropDown:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.DropDown);
                    UpdateFormatFields(values, "AutocompleteFilter", false);
                    break;
                case Durados.DisplayFormat.AutoCompleteStratWith:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Autocomplete);
                    UpdateFormatFields(values, "AutocompleteMathing", Durados.AutocompleteMathing.StartsWith);
                    UpdateFormatFields(values, "AutocompleteFilter", true);
                    break;
                case Durados.DisplayFormat.AutoCompleteMatchAny:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Autocomplete);
                    UpdateFormatFields(values, "AutocompleteMathing", Durados.AutocompleteMathing.Contains);
                    UpdateFormatFields(values, "AutocompleteFilter", true);
                    break;
                case Durados.DisplayFormat.Tile:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Tile);
                    break;
                case Durados.DisplayFormat.Slider:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Slider);
                    break;
                case Durados.DisplayFormat.Date_mm_dd:
                    UpdateFormatFields(values, "Format", "MM/dd/yyyy");
                    break;
                case Durados.DisplayFormat.Date_dd_mm:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy");
                    break;
                case Durados.DisplayFormat.Date_mm_dd_12:
                    UpdateFormatFields(values, "Format", "MM/dd/yyyy hh:mm:ss tt");
                    break;
                case Durados.DisplayFormat.Date_dd_mm_12:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss tt");
                    break;
                case Durados.DisplayFormat.Date_mm_dd_24:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss");
                    break;
                case Durados.DisplayFormat.Date_dd_mm_24:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss");
                    break;
                //case DisplayFormat.Date_Custom:
                //    UpdateFormatFields(values, "Format", Map.Database.DateFormat);
                //    break;
                case Durados.DisplayFormat.TimeOnly:
                    UpdateFormatFields(values, "Format", "hh:mm:ss tt");
                    break;
                case Durados.DisplayFormat.GeneralNumeric:
                    UpdateFormatFields(values, "Format", string.Empty);
                    break;

                case Durados.DisplayFormat.Fit:
                case Durados.DisplayFormat.Crop:

                    if (IsToSetDefaultUpload(values, e))
                    {
                        string upload = GetDefaultUpload();
                        UpdateFormatFields(values, "FtpUpload_Parent", upload);
                    }

                    UpdateFormatFields(values, "GridEditable", true);
                    UpdateFormatFields(values, "GridEditableEnabled", true);
                    UpdateFormatFields(values, "DataType", Durados.DataType.Image.ToString());
                    break;

                case Durados.DisplayFormat.Hyperlink:
                case Durados.DisplayFormat.ButtonLink:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.Url.ToString());
                    break;

                case Durados.DisplayFormat.Checklist:
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.CheckList);
                    break;

                case Durados.DisplayFormat.SubGrid:
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.Grid);
                    break;

                default:
                    break;
            }

        }

        private string GetDefaultUpload()
        {
            int? defaultUploadName = ConfigAccess.GetFirstId("FtpUpload", "Title", Maps.DefaultUploadName, Map.GetConfigDatabase().ConnectionString);

            if (!defaultUploadName.HasValue)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                View ftpUploadView = (View)Map.GetConfigDatabase().Views["FtpUpload"];

                if (Maps.PrivateCloud)
                {
                    values.Add("StorageType", "Ftp");
                    throw new Durados.DuradosException("Implement default FTP storage!");
                }
                else
                {

                    string appId = Maps.Instance.GetCurrentAppId();
                    values.Add("Title", Maps.DefaultUploadName);
                    values.Add("FileMaxSize", "2");
                    values.Add("UploadFileType", "Image");
                    values.Add("FileAllowedTypes", "jpg,jpeg,gif,png,JPEG,JPG,GIF,PNG");
                    values.Add("DirectoryVirtualPath", string.Format(Maps.AzureStorageUrl, Maps.AzureStorageAccountName, Maps.AzureAppPrefix + appId));
                    values.Add("DirectoryBasePath", Maps.AzureAppPrefix + appId);
                    values.Add("AzureAccountName", Maps.AzureStorageAccountName);
                    values.Add("AzureAccountKey", Maps.AzureStorageAccountKey);
                    values.Add("Height", Maps.DefaultImageHeight);
                    values.Add("StorageType", "Azure");
                }

                System.Data.DataRow row = ftpUploadView.Create(values, null, null, null, null, null);

                defaultUploadName = (int)row["ID"];
            }


            return defaultUploadName.Value.ToString();
        }

        private static bool IsToSetDefaultUpload(Dictionary<string, object> values, Durados.DataActionEventArgs e)
        {
            if (!(e is Durados.CreateEventArgs))
                return false;

            string upload = null;
            if (values.Keys.Contains("FtpUpload_Parent"))
            {
                upload = Convert.ToString(values["FtpUpload_Parent"]);
            }
            else if (e is Durados.EditEventArgs && ((Durados.EditEventArgs)e).PrevRow.Table.Columns.Contains("FtpUpload") && !((Durados.EditEventArgs)e).PrevRow.IsNull("FtpUpload"))
            {
                upload = Convert.ToString(((Durados.EditEventArgs)e).PrevRow["FtpUpload"]);
            }

            return string.IsNullOrEmpty(upload); ;


        }

        private void UpdateFormatFields(Dictionary<string, object> values, string key, object val)
        {
            if (values.ContainsKey(key))
                values[key] = val;
            else
                values.Add(key, val);
        }

        private void HandleCategory(Durados.Field field, Dictionary<string, object> values)
        {
            if (field.Category == null && field.View.Categories.Count > 0)
            {
                Durados.Category category = field.View.Categories.Values.OrderBy(c => c.Ordinal).FirstOrDefault();
                if (category != null)
                {
                    int? categoryId = ConfigAccess.GetCategoryId(category.Name, Map.GetConfigDatabase().ConnectionString);
                    if (categoryId.HasValue)
                    {
                        values.Add("Category_Parent", categoryId.ToString());
                    }
                }
            }
        }

        private string GetNewFieldName(View view)
        {
            HashSet<string> fieldsNames = GetColumnsNames(view);
            int i = 1;
            string fieldNamePrefix = NewFieldToken.TrimEnd("$$".ToCharArray());
            string newFieldName = fieldNamePrefix + i;
            while (fieldsNames.Contains(newFieldName))
            {
                newFieldName = fieldNamePrefix + (i++);
            }

            return newFieldName;
        }

        private HashSet<string> GetColumnsNames(View view)
        {
            Durados.DataAccess.AutoGeneration.Generator systemGenerator = new Durados.DataAccess.AutoGeneration.Generator();
            System.Data.DataTable table = systemGenerator.CreateTable(view.DataTable.TableName, string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName, Map.GetConfigDatabase().ConnectionString);

            HashSet<string> fieldsNames = new HashSet<string>();

            foreach (System.Data.DataColumn column in table.Columns)
            {
                fieldsNames.Add(column.ColumnName);
            }

            return fieldsNames;
        }

        protected void UpdateViewId(Durados.DataActionEventArgs e)
        {
            if (e.Values.ContainsKey("Fields_Parent") && e.Values["Fields_Parent"] != null && !e.Values["Fields_Parent"].Equals(string.Empty))
            {
            }
            else
            {
                if (e.Values.ContainsKey("ViewName") && e.Values["ViewName"] != null)
                {
                    string mainViewName = e.Values["ViewName"].ToString();
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    string viewPk = configAccess.GetViewPK(mainViewName, Map.GetConfigDatabase().ConnectionString);
                    if (!e.Values.ContainsKey("Fields_Parent"))
                    {
                        e.Values.Add("Fields_Parent", viewPk);
                    }
                    else
                    {
                        e.Values["Fields_Parent"] = viewPk;
                    }
                }
                else
                    throw new Durados.DuradosException("View is required!");
            }

        }
        private void SetViewDefaultsIfValueIsNull(Durados.CreateEventArgs e)
        {
            Durados.Web.Mvc.Database db = Map.GetDefaultDatabase();

            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)db.Views["Table"];

            Type type = view.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {

                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
                Durados.Config.Attributes.PropertyAttribute propertyAttribute = null;
                if (propertyAttributes.Length == 1)
                {
                    propertyAttribute = (Durados.Config.Attributes.PropertyAttribute)propertyAttributes[0];

                }

                if (propertyAttribute != null)
                {
                    switch (propertyAttribute.PropertyType)
                    {
                        case Durados.Config.Attributes.PropertyType.Column:
                            try
                            {

                                if (propertyInfo.Name == "Order")
                                {
                                    if (!e.Values.ContainsKey("Order"))
                                    {

                                        string order = "10";
                                        if (Map.Database.Views.Count() > 0)
                                        {
                                            View lastView = (View)Map.Database.Views.Values.OrderByDescending(v => v.Order).FirstOrDefault();
                                            if (lastView != null)
                                                order = (lastView.Order + 10).ToString();
                                        }
                                        e.Values.Add(propertyInfo.Name, order);
                                    }
                                }
                                else if (propertyInfo.Name != "DisplayName" && propertyInfo.Name != "DisplayColumn")
                                {
                                    object value = propertyInfo.GetValue(view, null);
                                    if (!e.Values.ContainsKey(propertyInfo.Name))
                                        e.Values.Add(propertyInfo.Name, value);
                                }
                            }
                            catch { }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void SetAutoIncrement(Dictionary<string, object> values, Durados.DataActionEventArgs e)
        {
            try
            {
                bool oldAutoIncrement = false;
                if (e is Durados.EditEventArgs)
                {
                    if (!(e as Durados.EditEventArgs).PrevRow.Table.Columns.Contains("AutoIncrement"))
                        return;
                    oldAutoIncrement = (e as Durados.EditEventArgs).PrevRow.IsNull("AutoIncrement") ? false : Convert.ToBoolean((e as Durados.EditEventArgs).PrevRow["AutoIncrement"]);

                }

                //bool newAutoIncrement = e.Values["AutoIncrement"] == null ? false : Convert.ToBoolean(e.Values["AutoIncrement"]);
                bool newAutoIncrement = e.Values.ContainsKey("AutoIncrement") && e.Values["AutoIncrement"] != null ? Convert.ToBoolean(e.Values["AutoIncrement"]) : false;

                if (oldAutoIncrement && newAutoIncrement || !oldAutoIncrement && !newAutoIncrement)
                    return;
                ConfigAccess configAccess = new ConfigAccess();
                if (oldAutoIncrement && !newAutoIncrement)
                {
                    UpdateFormatFields(values, "ExcludeInInsert", false);
                    UpdateFormatFields(values, "ExcludeInUpdate", false);
                    UpdateFormatFields(values, "HideInCreate", false);
                    //UpdateFormatFields(values, "HideInEdit", false);

                }
                // UpdateBackTo notexcluded

                if (!oldAutoIncrement && newAutoIncrement)
                {
                    UpdateFormatFields(values, "ExcludeInInsert", true);
                    UpdateFormatFields(values, "ExcludeInUpdate", true);
                    UpdateFormatFields(values, "HideInCreate", true);
                    //UpdateFormatFields(values, "HideInEdit", true);
                }
            }
            catch { }
            //  SetAsExculded
            //    RefreshConfigCache();
        }
        private bool ChangeDataType(Durados.EditEventArgs e)
        {
            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();

            if (IsSimpleRelation(oldDataType, newDataType, e))
                return false;

            if (oldDataType == newDataType || !(newDataType == Durados.DataType.MultiSelect.ToString() || newDataType == Durados.DataType.SingleSelect.ToString()))
                return false;

            ConfigAccess configAccess = new ConfigAccess();

            string viewPk = e.PrevRow["Fields"].ToString();
            string viewName = configAccess.GetViewNameByPK(viewPk, Map.GetConfigDatabase().ConnectionString);
            View view = (View)Map.Database.Views[viewName];
            View configView = (View)e.View.Database.Views["View"];
            string manyToManyViewName = null;
            string columnName = Map.ChangeDataType(view, oldDataType, newDataType, e.PrimaryKey, configView, viewPk, (View)e.View, e.Values, out manyToManyViewName);
            e.Cancel = true;
            Initiate();
            string configViewPk = e.Values.ContainsKey("Fields_Parent") ? e.Values["Fields_Parent"].ToString() : viewPk;
            UpdateDisplayFormatForNewColumn(e, configViewPk, viewName, columnName);
            SetAutoIncrement(e.Values, e);
            if (newDataType == Durados.DataType.SingleSelect.ToString() || newDataType == Durados.DataType.ImageList.ToString())
            {
                view = (View)Map.Database.Views[viewName];
                Durados.Field field = view.GetFieldByColumnNames(columnName);
                string fieldName = field.Name;
                string newFieldPK = Map.GetNewConfigFieldPk(configView, viewPk, fieldName);


                Dictionary<string, object> values = new Dictionary<string, object>();
                if (e.Values.ContainsKey("DisplayName"))
                    values.Add("DisplayName", e.Values["DisplayName"]);
                if (e.Values.ContainsKey("Description"))
                    values.Add("Description", e.Values["Description"]);
                if (e.Values.ContainsKey("Order"))
                    values.Add("Order", e.Values["Order"]);
                if (e.Values.ContainsKey("OrderForCreate"))
                    values.Add("OrderForCreate", e.Values["OrderForCreate"]);
                if (e.Values.ContainsKey("OrderForEdit"))
                    values.Add("OrderForEdit", e.Values["OrderForEdit"]);
                if (e.Values.ContainsKey("Category_Parent"))
                    values.Add("Category_Parent", e.Values["Category_Parent"]);
                else
                    HandleCategory(field, values);
                //if (e.Values.ContainsKey("GridEditable"))
                values.Add("GridEditable", true);
                values.Add("InlineAdding", true);
                values.Add("InlineEditing", true);
                values.Add("InlineSearch", true);

                e.View.Edit(values, newFieldPK, null, null, null, null);

                RefreshConfigCache();
            }
            else if (newDataType == Durados.DataType.MultiSelect.ToString())
            {
                view = (View)Map.Database.Views[manyToManyViewName];

                ParentField field = null;
                foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Parent))
                {
                    if (parentField.ParentView.Name.Equals(viewName))
                    {
                        field = parentField;
                        break;
                    }
                }
                if (field == null)
                    return true;

                ChildrenField childrenField = (ChildrenField)field.GetEquivalentChildrenField();

                string fieldName = childrenField.Name;
                string newFieldPK = Map.GetNewConfigFieldPk(configView, viewPk, fieldName);

                Dictionary<string, object> values = new Dictionary<string, object>();
                if (e.Values.ContainsKey("DisplayName"))
                    values.Add("DisplayName", e.Values["DisplayName"]);
                if (e.Values.ContainsKey("Description"))
                    values.Add("Description", e.Values["Description"]);
                //values.Add("Order", e.Values["Order"]);
                //values.Add("OrderForCreate", e.Values["OrderForCreate"]);
                //values.Add("OrderForEdit", e.Values["OrderForEdit"]);
                int maxOrder = configAccess.GetMaxFieldOrder(viewName, "Order", Map.GetConfigDatabase().ConnectionString);
                maxOrder += 10;

                values.Add("Order", maxOrder);
                values.Add("OrderForCreate", maxOrder);
                values.Add("OrderForEdit", maxOrder);

                e.View.Edit(values, newFieldPK, null, null, null, null);
                RefreshConfigCache();
            }
            return true;

        }

        private bool IsSimpleRelation(string oldDataType, string newDataType, Durados.EditEventArgs e)
        {
            if (oldDataType == newDataType || newDataType != Durados.DataType.SingleSelect.ToString() || newDataType != Durados.DataType.ImageList.ToString())
            {
                return false;
            }

            if (!e.Values.ContainsKey("RelatedViewName"))
                return false;

            string relatedViewName = e.Values["RelatedViewName"].ToString();

            if (!Map.Database.Views.ContainsKey(relatedViewName))
                return false;

            View relatedView = (View)Map.Database.Views[relatedViewName];

            if (relatedView.DataTable.PrimaryKey.Length != 1)
                return false;

            string viewPk = e.PrevRow["Fields"].ToString();
            ConfigAccess configAccess = new ConfigAccess();
            string viewName = configAccess.GetViewNameByPK(viewPk, Map.GetConfigDatabase().ConnectionString);
            View view = (View)Map.Database.Views[viewName];

            string fieldName = e.PrevRow["Name"].ToString();

            if (!view.Fields.ContainsKey(fieldName))
                return false;

            Durados.Field field = view.Fields[fieldName];

            if (field.FieldType != Durados.FieldType.Column)
                return false;

            ColumnField columnField = (ColumnField)field;



            if (columnField.DataColumn.DataType.Equals(relatedView.DataTable.PrimaryKey[0].DataType))
                return true;
            else
                throw new Durados.DuradosException(string.Format("The Related View '{0}' has Primary Key with type '{1}'. could not relate to Foreign Key with type '{2}'.", relatedViewName, relatedView.DataTable.PrimaryKey[0].DataType, columnField.DataColumn.DataType));



        }

        private Durados.DataAccess.AutoGeneration.Dynamic.RelationChange IsRelationChanged(string oldRelatedViewName, string newRelatedViewName)
        {
            if (newRelatedViewName == oldRelatedViewName)
                return Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.None;

            else if (!string.IsNullOrEmpty(newRelatedViewName) && string.IsNullOrEmpty(oldRelatedViewName))
                return Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.ColumnToParent;

            else if (!string.IsNullOrEmpty(oldRelatedViewName) && string.IsNullOrEmpty(newRelatedViewName))
                return Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.ParentToColumn;

            else if (!string.IsNullOrEmpty(oldRelatedViewName) && !string.IsNullOrEmpty(newRelatedViewName))
                return Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.ParentToDifferentParent;

            else
                return Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.None;

        }

        protected virtual void UpdateNewRelatedViewLayout(View view, View relatedView)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Layout", LayoutType.BasicGrid.ToString());
            values.Add("CollapseFilter", true);

            string relatedViewPK = (new Durados.DataAccess.ConfigAccess()).GetViewPK(relatedView.Name, Map.GetConfigDatabase().ConnectionString);
            Map.GetConfigDatabase().Views["View"].Edit(values, relatedViewPK, null, null, null, null);
        }

        protected virtual void UpdateNewRelatedViewWorkspace(View view, View relatedView)
        {
            if (relatedView.WorkspaceID != view.WorkspaceID)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("WorkspaceID", view.WorkspaceID);
                string relatedViewPK = (new Durados.DataAccess.ConfigAccess()).GetViewPK(relatedView.Name, Map.GetConfigDatabase().ConnectionString);
                Map.GetConfigDatabase().Views["View"].Edit(values, relatedViewPK, null, null, null, null);
            }
        }

        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public virtual IHttpActionResult Post()
        {
            try
            {
                View view = GetView(GetConfigViewName());

                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string,object> values = view.Deserialize(json);

                
                if (!values.ContainsKey("DisplayName") && values.ContainsKey("name"))
                {
                    values.Add("DisplayName", values["name"].ToString().Replace('_', ' '));
                }

                ValidateInput(values);

                System.Data.DataRow row = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

                CreateFields(values);

                ConfigAccess ConfigAccess = new ConfigAccess();
                var pk = ConfigAccess.GetViewPK(row["Name"].ToString(), Map.GetConfigDatabase().ConnectionString);
                var item = RestHelper.Get(view, pk, true, view_BeforeSelect, view_AfterSelect);
               
                return Ok(item);
                
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [Route("1/table/dictionary/{name}")]
        [HttpGet]
        public virtual IHttpActionResult dictionary(string name, string crud, bool withSystemTokens = false, bool deep = false)
        {
            try
            {
                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                View view = null;
                if (!string.IsNullOrEmpty(name))
                {
                    view = GetView(name);
                    if (view == null)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                    }

                }

                if (string.IsNullOrEmpty(crud))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Please provide a crud parameter in the query string. either create, update or delete"));
                }

                Dictionary<string, object> dictionary = new DataViewForDictionary().GetDataViewForDictionary(view, withSystemTokens, deep, (Durados.Crud)Enum.Parse(typeof(Durados.Crud), crud, true));

                return Ok(dictionary);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [Route("1/table/config/template")]
        [HttpPost]
        public virtual IHttpActionResult AddTemplateSchema()
        {
            var pk = string.Empty;
            try
            {
                View view = GetView("View");

                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                List<object> tables = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(json);

                List<Dictionary<string, object>> fks = new List<Dictionary<string, object>>();

                Dictionary<string, string> viewNames = GetViewsNames();

                foreach (string table in tables.Select(o => o.ToString()))
                {
                    Dictionary<string, object> values = view.Deserialize(table);


                    if (!values.ContainsKey("DisplayName") && values.ContainsKey("name"))
                    {
                        values.Add("DisplayName", values["name"].ToString().Replace('_', ' '));
                    }
                    if (!values.ContainsKey("WorkspaceID"))
                    {
                        values.Add("WorkspaceID", "0");
                    }

                    

                    ValidateInput(values);

                    System.Data.DataRow row = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

                    CreateFields2(values, fks);

                    ConfigAccess ConfigAccess = new ConfigAccess();
                    pk = ConfigAccess.GetViewPK(row["Name"].ToString(), Map.GetConfigDatabase().ConnectionString);

                }

                CreatForiegnKeys(fks);

                try
                {
                    viewNames = GetViewsNames(viewNames);
                    ViewHelper.SetTopMenus(viewNames);
                    ViewHelper.Initiate();
                }
                catch { }

                var item = RestHelper.Get(view, pk, true, view_BeforeSelect, view_AfterSelect);

                return Ok(item);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private Dictionary<string, string> GetViewsNames()
        {
            Dictionary<string, string> viewsNames = new Dictionary<string, string>();

            foreach(View view in Map.Database.Views.Values.Where(v=>!v.SystemView))
            {
                viewsNames.Add(view.Name, view.Name);
            }
            return viewsNames;
        }

        private Dictionary<string, string> GetViewsNames(Dictionary<string, string> prevViewsNames)
        {
            Dictionary<string, string> viewsNames = new Dictionary<string, string>();

            foreach (View view in Map.Database.Views.Values.Where(v => !v.SystemView))
            {
                if (!prevViewsNames.ContainsKey(view.Name))
                    viewsNames.Add(view.Name, view.Name);
            }
            return viewsNames;
        }

        private void CreatForiegnKeys(List< Dictionary<string, object>> fks)
        {
            View fieldView = GetView("Field");

            foreach (Dictionary<string, object> fk in fks)
            {
                Dictionary<string, object> fieldValues = (Dictionary<string, object>)fk;
                    fieldView.Create(fieldValues, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
            }
            
              
        }

        
        private void ValidateInput(Dictionary<string, object> values)
        {
            if (!values.ContainsKey("name"))
            {
                throw new Durados.DuradosException("The table must have a name.");
            }
            if (!values.ContainsKey("fields"))
            {
                throw new Durados.DuradosException("The table " + values["name"].ToString() + " must have at least one field. Please make sure that the json input has a fields property.");
            }

            System.Collections.IEnumerable fields = (System.Collections.IEnumerable)values["fields"];
            foreach (Dictionary<string, object> field in fields)
            {
                if (!field.ContainsKey("name") && !field.ContainsKey(Name))
                {
                    throw new Durados.DuradosException("The field must have a name.");
                }

                if (!field.ContainsKey("type") && !field.ContainsKey("DataType")   )
                {
                    throw new Durados.DuradosException("The field must have a type. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }

                if (string.IsNullOrEmpty(field["type"] as string))
                {
                    throw new Durados.DuradosException("The field type can not not be null or empty. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }
                if( !Enum.GetNames(typeof(Durados.DataType)).Where(n=>n.ToLower()!="html" && n.ToLower()!="imagelist" && n.ToLower()!="email" ).Contains(field["type"].ToString()))
                {
                    throw new Durados.DuradosException("The field type [" + field["type"].ToString() + "]  is not defined. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }

            }
        }

        protected void CreateFields(Dictionary<string, object> values)
        {
            if (values.ContainsKey("fields") && values.ContainsKey(Name))
            {
                CreateFields(values[Name].ToString(), (System.Collections.IEnumerable)values["fields"]);
            }
        }
        protected void CreateFields2(Dictionary<string, object> values, List<Dictionary<string, object>> fks)
        {
            if (values.ContainsKey("fields") && values.ContainsKey(Name))
            {
                string viewName = values[Name].ToString();
                 System.Collections.IEnumerable fields= (System.Collections.IEnumerable)values["fields"];
                View fieldView = GetView("Field");

                foreach (var field in fields)
                {
                    Dictionary<string, object> fieldValues = (Dictionary<string, object>)field;

                    AdjustField(fieldValues, viewName);

                    if (fks != null && fieldValues.ContainsKey("RelatedViewName") && !string.IsNullOrEmpty(fieldValues["RelatedViewName"] as string))//check if Single or Multi and if related is Not
                        
                        fks.Add(fieldValues);

                    else
                        fieldView.Create(fieldValues, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                }

            }
        }

        protected void CreateFields(string viewName, System.Collections.IEnumerable fields)
        {
            View fieldView = GetView("Field");

            foreach (var field in fields)
            {
                Dictionary<string, object> fieldValues = (Dictionary<string, object>)field;

                AdjustField(fieldValues, viewName);

                fieldView.Create(fieldValues, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
            }

        }

        protected void AdjustField(Dictionary<string, object> fieldValues, string viewName)
        {
            if (fieldValues.ContainsKey("name") && !fieldValues.ContainsKey(Name))
            {
                fieldValues.Add(Name, fieldValues["name"]);
                fieldValues.Remove("name");
            }

            if (!fieldValues.ContainsKey("type") && !fieldValues.ContainsKey("DataType"))
            {
                throw new Durados.DuradosException("The field must contain type. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
            }

            if (fieldValues.ContainsKey("type") && !fieldValues.ContainsKey("DataType"))
            {
                fieldValues.Add("DataType", fieldValues["type"]);
                fieldValues.Remove("type");
            }

            if (!fieldValues.ContainsKey("ViewName"))
            {
                fieldValues.Add("ViewName", viewName);
            }

            if (!fieldValues.ContainsKey("DisplayName"))
            {
                fieldValues.Add("DisplayName", fieldValues[Name].ToString().Replace('_', ' '));
            }

            if (!fieldValues.ContainsKey("RelatedViewName") && fieldValues.ContainsKey("relatedTable"))
            {
                fieldValues.Add("RelatedViewName", fieldValues["relatedTable"]);

            }
            else if (!fieldValues.ContainsKey("RelatedViewName") && fieldValues.ContainsKey("RelatedTable"))
            {
                fieldValues.Add("RelatedViewName", fieldValues["RelatedTable"]);

            }

            if (!fieldValues.ContainsKey("DisplayFormat") && fieldValues.ContainsKey("displayFormat"))
            {
                fieldValues.Add("DisplayFormat", fieldValues["displayFormat"]);
                fieldValues.Remove("displayFormat");
            }
        }


        protected void UpdateFields(Dictionary<string, object> values)
        {
            if (values.ContainsKey("fields") && values.ContainsKey(Name))
            {
                UpdateFields(values[Name].ToString(), (System.Collections.IEnumerable)values["fields"]);
            }
        }

        protected void UpdateFields(string viewName, System.Collections.IEnumerable fields)
        {
            View view = GetView(viewName);
            if (view == null)
                throw new Durados.DuradosException(Messages.ViewNameIsMissing);

            //List<Dictionary<string, object>> existingFields = new List<Dictionary<string, object>>();
            //List<Dictionary<string, object>> newFields = new List<Dictionary<string, object>>();

            //foreach (Dictionary<string, object> field in fields)
            //{
            //    string fieldName = field["name"].ToString();
            //    if (view.GetFieldsByJsonName(fieldName).Length == 1)

            //        existingFields.Add(field);
            //    else
            //        newFields.Add(field);

            //}

            //CreateFields(viewName, newFields);
            //EditFields(viewName, existingFields);
            EditFields(viewName, fields);
        }

        protected void EditFields(string viewName, System.Collections.IEnumerable fields)
        {
            View fieldView = GetView("Field");

            ConfigAccess ConfigAccess = new ConfigAccess();

            foreach (Dictionary<string, object> field in fields)
            {
                Dictionary<string, object> fieldValues = (Dictionary<string, object>)field;

                //string fieldName = field["name"].ToString();
               
                //AdjustField(fieldValues, viewName);
                fieldValues = GetAdjustedValues(fieldView, fieldValues);

                //string pk = ConfigAccess.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                try
                {
                    string pk = ((Dictionary<string, object>)field["__metadata"])["id"].ToString();
                    fieldView.Edit(fieldValues, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                }
                catch { }
            }
        }
        
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public virtual IHttpActionResult Put(string name, bool reload = true)
        {
            try
            {
                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(GetConfigViewName());
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }

                ConfigAccess ConfigAccess = new ConfigAccess();
                View realView = GetView(name);
                if (realView == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));

                }

                string pk = ConfigAccess.GetViewPK(realView.Name, Map.GetConfigDatabase().ConnectionString);
                if (pk == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));

                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object> values = view.Deserialize(json);
                values = GetAdjustedValues(view, values);


                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                if (values.ContainsKey("Fields_Children"))
                {
                    UpdateFields(name, (System.Collections.IEnumerable)values["Fields_Children"]);
                }
                if (reload)
                    RefreshConfigCache();

                var item = RestHelper.Get(view, pk, true, view_BeforeSelect, view_AfterSelect, false, true);
                
                return Ok(item);
                
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected override Dictionary<string, object> GetAdjustedValues(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            Dictionary<string, object> flatValues = FlatValues(values);
            Dictionary<string, object> adjustedValues = new Dictionary<string, object>();

            foreach (string key in flatValues.Keys)
            {
                string adjustedKey = GetAdjustedKey(view, key);
                if (!adjustedValues.ContainsKey(adjustedKey))
                    adjustedValues.Add(adjustedKey, flatValues[key]);
            }

            return adjustedValues;
        }

        protected virtual Dictionary<string, object> FlatValues(Dictionary<string, object> values)
        {
            Dictionary<string, object> flatValues = new Dictionary<string, object>();

            foreach (string key in values.Keys)
            {
                if (values[key] is Dictionary<string, object> && !key.Equals("__metadata") && !key.Equals("fields") && !key.Equals("categories"))
                {
                    Dictionary<string, object> values2 = (Dictionary<string, object>)values[key];

                    foreach (string key2 in values2.Keys)
                    {
                        if (!flatValues.ContainsKey(key2))
                        {
                            flatValues.Add(key2, values2[key2]);
                        }
                    }
                }
                else
                {
                    flatValues.Add(key, values[key]);
                }
            }

            return flatValues;
        }

        protected override string GetAdjustedKey(Durados.Web.Mvc.View view, string key)
        {
            if (key == "Name" || key == "name")
                return "JsonName";

            if (view.Fields.ContainsKey(key))
                return key;
            string upper = char.ToUpper(key[0]) + key.Substring(1);
            if (view.Fields.ContainsKey(upper))
                return upper;

            Durados.Field[] fields = view.GetFieldsByDisplayName(upper);
            if (fields != null && fields.Length == 1)
                return fields[0].Name;

            return key;
        }

        private bool IsDataTypeChanged(Durados.EditEventArgs e)
        {
            if (!e.Values.ContainsKey("DataType"))
                return false;

            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();


            return !(oldDataType == newDataType);
        }
        private bool ViewOwnerChangedType(Durados.EditEventArgs e)
        {
            string role = Map.Database.GetUserRole();
            bool viewOwner = role == "View Owner";
            bool dataTypeChanged = IsDataTypeChanged(e);

            return viewOwner && dataTypeChanged;
        }

        private void CheckPkDeletion(Durados.EditEventArgs e)
        {
            if (e.View.Name != "Field")
            {
                return;
            }

            if (!e.Values.ContainsKey("Excluded"))
                return;

            if (!e.Values["Excluded"].Equals(e.View.Database.True))
                return;


            ConfigAccess configAccess = new ConfigAccess();

            if (!e.PrevRow.Table.Columns.Contains("Fields"))
                return;
            if (!e.PrevRow.Table.Columns.Contains("Name"))
                return;


            string viewPk = e.PrevRow["Fields"].ToString();
            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
            if (!Map.Database.Views.ContainsKey(viewName))
                return;

            View view = (View)Map.Database.Views[viewName];

            string fieldName = e.PrevRow["Name"].ToString();

            if (!view.Fields.ContainsKey(fieldName))
                return;

            Durados.Field field = view.Fields[fieldName];

            if (field.IsPartOfPK())
                throw new Durados.DuradosException("This is a primary key column and it cannot be deleted. If you want to remove it from the grid check the Don't Display checkbox.");
        }

        private void UpdateNewField(View configView, System.Data.DataRow oldFieldRow, Dictionary<string, object> newValues, string newFieldPk, string dataType)
        {

            //string displayName = oldFieldRow["DisplayName"].ToString();
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();
            //values.Add("DisplayName", displayName);
            foreach (Durados.Field field in configView.Fields.Values.Where(f => f.Plan == "1,2,3" && f.Name != "DisplayFormat"))
            {
                values.Add(field.Name, newValues.ContainsKey(field.Name) ? newValues[field.Name] : oldFieldRow[field.Name]);
            }
            values.Add("Order", oldFieldRow["Order"]);
            values.Add("OrderForEdit", oldFieldRow["OrderForEdit"]);
            values.Add("OrderForCreate", oldFieldRow["OrderForCreate"]);

            if (dataType == Durados.DataType.LongText.ToString())
            {
                values.Add("DisplayFormat", Durados.DisplayFormat.MultiLinesEditor.ToString());
            }

            configAccess.Edit(configView, values, newFieldPk, null, null, null, null);
        }

        private void RemoveOldField(View configView, System.Data.DataRow oldFieldRow, string oldFieldPk)
        {
            string displayName = oldFieldRow["DisplayName"].ToString() + "_old";
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("DisplayName", displayName);
            values.Add("Excluded", true);

            configAccess.Edit(configView, values, oldFieldPk, null, null, null, null);
        }

        private void HandleOwnerChangeType(Durados.EditEventArgs e)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            string configViewPk = e.PrevRow["Fields"].ToString();
            string configFieldPk = e.PrimaryKey;
            System.Data.DataRow configFieldRow = e.PrevRow;
            string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
            View view = (View)Map.Database.Views[viewName];
            string name = GetNewFieldName(view);
            string dataType = e.Values["DataType"].ToString();

            e.Values.Add("Fields_Parent", configViewPk);

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("DataType", dataType);
            values.Add("Fields_Parent", configViewPk);
            values.Add("Name", name);
            values.Add("DisplayName", name);
            string columnName = CreateField(e, values);

            view = (View)Map.Database.Views[viewName];
            Durados.Field field = view.GetFieldByColumnNames(columnName);
            string fieldName = field.Name;
            View configView = (View)e.View.Database.Views["View"];
            string newFieldPK = Map.GetNewConfigFieldPk(configView, configViewPk, fieldName);


            UpdateNewField((View)e.View, configFieldRow, e.Values, newFieldPK, dataType);
            RemoveOldField((View)e.View, configFieldRow, configFieldPk);

            //Initiate();

        }

        private void HandleJsonName(Durados.EditEventArgs e)
        {
            string oldjsonName = e.PrevRow.IsNull("JsonName") ? string.Empty : e.PrevRow["JsonName"].ToString();
            if (!e.Values.ContainsKey("JsonName"))
                return;
            string newjsonName = !e.Values.ContainsKey("JsonName") || e.Values["JsonName"] == null ? string.Empty : e.Values["JsonName"].ToString();
            if (oldjsonName != newjsonName)
            {
                if (string.IsNullOrEmpty(newjsonName))
                    throw new Durados.DuradosException(Map.Database.Localizer.Translate("Field name cannot be empty."));
                if (newjsonName != newjsonName.ReplaceNonAlphaNumeric2())
                    throw new Durados.DuradosException(Map.Database.Localizer.Translate("Field name can contain only numbers and letters."));
                string configViewPk = e.Values["Fields_Parent"].ToString();
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
                ((View)Map.Database.Views[viewName]).ValidateUniqueFieldJsonName(e.Values["JsonName"].ToString());

            }
        }

        private void HandleMultiSelectHide(System.Data.DataRow prevRow, Dictionary<string, object> values)
        {
            if (values.ContainsKey("HideInCreate") && !string.IsNullOrEmpty(values["HideInCreate"].ToString()))
            {
                string oldval = prevRow == null && prevRow.IsNull("HideInCreate") ? string.Empty : prevRow["HideInCreate"].ToString();
                string newval = values["HideInCreate"] == null ? string.Empty : values["HideInCreate"].ToString();

                if (oldval != newval)
                {
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.Grid);
                    bool val = Convert.ToBoolean(newval);
                    if (values.ContainsKey("ExcludeInInsert"))
                        values["ExcludeInInsert"] = val;
                    else
                        values.Add("ExcludeInInsert", val);

                    if (values.ContainsKey("DisableInCreate"))
                        values["DisableInCreate"] = !val;
                    else
                        values.Add("DisableInCreate", !val);
                }

            }
            if (values.ContainsKey("HideInEdit") && !string.IsNullOrEmpty(values["HideInEdit"].ToString()))
            {
                string oldval = prevRow == null && prevRow.IsNull("HideInEdit") ? string.Empty : prevRow["HideInEdit"].ToString();
                string newval = values["HideInEdit"] == null ? string.Empty : values["HideInEdit"].ToString();

                if (oldval != newval)
                {
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.Grid);
                    bool val = Convert.ToBoolean(newval);
                    if (values.ContainsKey("ExcludeInUpdate"))
                        values["ExcludeInUpdate"] = val;
                    else
                        values.Add("ExcludeInUpdate", val);
                    if (values.ContainsKey("DisableInEdit"))
                        values["DisableInEdit"] = !val;
                    else
                        values.Add("DisableInEdit", !val);

                }
            }
        }

        protected virtual void HandleDisplayFormat(Durados.DataActionEventArgs e)
        {

            string displayFormatFieldName = "DisplayFormat";
            if (!(e.Values.ContainsKey(displayFormatFieldName) && !string.IsNullOrEmpty(e.Values[displayFormatFieldName].ToString())))
                return;


            string formatVal = e.Values[displayFormatFieldName].ToString();

            Type type = typeof(Durados.DisplayFormat);

            if (Enum.IsDefined(type, formatVal))
            {
                bool needClearDislpayFields = !string.IsNullOrEmpty(formatVal) && formatVal != Durados.DisplayFormat.None.ToString()
                    && formatVal != Durados.DisplayFormat.Date_Custom.ToString();
                if (needClearDislpayFields) ClearAllDisplayFields(e);
                SetDisplayFormatBehaviors(e.Values, formatVal, type, e);
            }
        }

        private void ClearAllDisplayFields(Durados.DataActionEventArgs e)
        {
            UpdateFormatFields(e.Values, "TextHtmlControlType", TextHtmlControlType.Text);
            UpdateFormatFields(e.Values, "SpecialColumn", Durados.SpecialColumn.None);
            UpdateFormatFields(e.Values, "Format", string.Empty);
            UpdateFormatFields(e.Values, "AutocompleteMathing", Durados.AutocompleteMathing.StartsWith);
            UpdateFormatFields(e.Values, "Rich", false);
        }

        private bool IsUnencrypt(Durados.EditEventArgs e)
        {
            if (e.View.Name != "Field")
                return false;

            if (e.Values.ContainsKey("Encrypted") && !e.Values["Encrypted"].Equals("False"))
            {
                return (e.PrevRow["Encrypted"].Equals(true));
            }

            return false;
        }

        private bool IsEncryptedChanged(Durados.EditEventArgs e)
        {
            if (e.View.Name != "Field")
                return false;

            if (e.Values.ContainsKey("Encrypt") && e.Values["Encrypt"].Equals("True"))
            {
                return (e.PrevRow["Encrypt"].Equals(false));
            }

            return false;
        }

        private bool ChangeSimpleDataType(Durados.EditEventArgs e)
        {
            if (!e.Values.ContainsKey("DataType"))
                return false;

            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();


            if (oldDataType == newDataType)
                return false;

            if ((newDataType == Durados.DataType.MultiSelect.ToString() || newDataType == Durados.DataType.SingleSelect.ToString()))
                return false;

            if ((oldDataType == Durados.DataType.MultiSelect.ToString() || oldDataType == Durados.DataType.SingleSelect.ToString()))
                return false;

            ConfigAccess configAccess = new ConfigAccess();
            string fieldPk = e.PrimaryKey;
            string fieldName = e.PrevRow["Name"].ToString();
            string viewPk = e.PrevRow["Fields"].ToString();
            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
            View view = (View)Map.Database.Views[viewName];
            View configView = (View)e.View.Database.Views["View"];
            if (view.Fields.ContainsKey(fieldName) && view.Fields[fieldName].FieldType == Durados.FieldType.Column)
            {
                ColumnField columnField = (ColumnField)view.Fields[fieldName];
                Map.ChangeSimpleDataType(view, columnField, oldDataType, newDataType, e.PrimaryKey, configView, viewPk, (View)e.View, e.Values);
            }
            else
            {
                return false;
            }

            return true;
        }


        protected override void BeforeEdit(Durados.EditEventArgs e)
        {
            if (true)
            {
                base.BeforeEdit(e); 
                return;
            }

            if (e.View.Name == "Field")
            {
                CheckPkDeletion(e);
                if (ViewOwnerChangedType(e))
                {
                    HandleOwnerChangeType(e);
                    e.Cancel = false;
                    base.BeforeEdit(e);
                    return;
                }
                HandleJsonName(e);
                if (e.Values.ContainsKey("DataType") && e.Values.ContainsKey("DisplayFormat"))
                {
                    if (e.Values["DisplayFormat"].ToString() == Durados.DisplayFormat.None.ToString() || e.Values["DisplayFormat"].ToString() == string.Empty)
                    {
                        if (e.Values["DataType"].ToString() == Durados.DataType.Image.ToString())
                            e.Values["DisplayFormat"] = Durados.DisplayFormat.Fit.ToString();
                        else if (e.Values["DataType"].ToString() == Durados.DataType.Url.ToString())
                            e.Values["DisplayFormat"] = Durados.DisplayFormat.Hyperlink.ToString();
                        else if (e.Values["DataType"].ToString() == Durados.DataType.ShortText.ToString())
                            e.Values["DisplayFormat"] = Durados.DisplayFormat.SingleLine.ToString();
                        else if (e.Values["DataType"].ToString() == Durados.DataType.LongText.ToString())
                            e.Values["DisplayFormat"] = Durados.DisplayFormat.MultiLinesEditor.ToString();
                    }
                }
                if ((e.Values.ContainsKey("DataType") && e.Values["DataType"].ToString() == Durados.DataType.MultiSelect.ToString()))
                {
                    HandleMultiSelectHide(e.PrevRow, e.Values);
                }

                HandleDisplayFormat(e);
                SetAutoIncrement(e.Values, e);
                if (IsUnencrypt(e))
                    throw new Durados.DuradosException("Cannot unencrypt field.");

                encryptedChanged = IsEncryptedChanged(e);
                //Formula field proccessing
                if (e.Values.ContainsKey("Formula"))
                {
                    if (e.Values["Formula"] != null && e.Values["Formula"].ToString().Trim() != string.Empty)
                    {

                        string parentViewname = e.PrevRow.GetParentRow("Fields")["Name"].ToString();

                        View currentView = (View)Map.Database.Views[parentViewname];

                        Durados.Field defaultFied = currentView.Fields[e.Values["Name"].ToString()];

                        // Replace display names with field names + security validation
                        //defaultFied.Formula = DataAccessHelper.ReplaceFieldDisplayNames(e.Values["Formula"].ToString().Trim(), true, currentView);

                        //SqlAccess sa = new SqlAccess();

                        //string SqlSelectQuery = "SELECT TOP 1 " + sa.GetCalculatedFieldStatement(defaultFied, defaultFied.Name) + " FROM [" + currentView.DataTable.TableName + "]";
                        Durados.Field defaultField = currentView.Fields[e.Values["Name"].ToString()];

                        // Replace display names with field names + security validation
                        defaultField.Formula = DataAccessHelper.ReplaceFieldDisplayNames(e.Values["Formula"].ToString().Trim(), true, currentView);


                        IDataTableAccess sa = DataAccessHelper.GetDataTableAccess(currentView);
                        SqlSchema sqlSchema = sa.GetNewSqlSchema();

                        string SqlSelectQuery = sqlSchema.GetFormula(sa.GetCalculatedFieldStatement(defaultField, defaultField.Name), currentView.DataTable.TableName);
                        try
                        {
                            sa.ExecuteNonQuery(currentView.ConnectionString, SqlSelectQuery, currentView.Database.SqlProduct);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "BeforeEdit", exception.Source, exception, 1, null);
                            throw new Durados.DuradosException("The statement - " + SqlSelectQuery + " is incorrect, " + exception.Message);
                        }

                        e.Values["Formula"] = defaultFied.Formula;
                    }
                    else
                    {
                        e.Values["Formula"] = string.Empty;
                        e.Values["IsCalculated"] = false;
                    }
                }

                //isChangeDataType
                bool isChangeDataType = false;
                if (e.Values.ContainsKey("DataType"))
                {
                    isChangeDataType = ChangeDataType(e);
                }
                if (!isChangeDataType)
                {
                    if (e.Values.ContainsKey("RelatedViewName"))
                    {
                        string oldRelatedViewName = e.PrevRow.IsNull("RelatedViewName") ? string.Empty : e.PrevRow["RelatedViewName"].ToString();
                        string newRelatedViewName = e.Values["RelatedViewName"] == null ? string.Empty : e.Values["RelatedViewName"].ToString();

                        if (!Map.Database.Views.ContainsKey(newRelatedViewName))
                            newRelatedViewName = Map.Database.GetCorrectCaseViewName(newRelatedViewName);

                        Durados.DataAccess.AutoGeneration.Dynamic.RelationChange relationChange = IsRelationChanged(oldRelatedViewName, newRelatedViewName);
                        if (relationChange != Durados.DataAccess.AutoGeneration.Dynamic.RelationChange.None)
                        {
                            ConfigAccess configAccess = new ConfigAccess();

                            string viewPk = e.PrevRow["Fields"].ToString();
                            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
                            View view = (View)Map.Database.Views[viewName];

                            try
                            {
                                string fieldName = Map.GetConfigDatabase().Views["Field"].DataTable.Rows.Find(Convert.ToUInt32(e.PrimaryKey))["Name"].ToString();
                                Durados.Field field = view.Fields[fieldName];
                                if (field.FieldType != Durados.FieldType.Children)
                                {
                                    Map.ChangeFieldRelation(view, relationChange, oldRelatedViewName, newRelatedViewName, e.PrimaryKey, (View)e.View, e.Values);
                                }
                                else
                                {
                                    Exception exception = new Durados.DuradosException("Cannot change the related table of a children field");
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "BeforeEdit", exception.Source, exception, 3, null);
                                    throw exception;
                                }
                            }
                            catch (Exception exception)
                            {
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "BeforeEdit", exception.Source, exception, 3, null);
                                throw new Durados.DuradosException("Failed to change the related table. " + exception.Message, exception);
                            }
                            relationChanged = true;
                        }

                    }

                    if (!relationChanged)
                    {
                        ChangeSimpleDataType(e);
                    }
                }

                if (!relationChanged)
                {

                    if (encryptedChanged)
                    {
                        ConfigAccess configAccess = new ConfigAccess();
                        string viewPk = e.PrevRow["Fields"].ToString();
                        string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
                        string columnName = e.PrevRow["Name"].ToString();
                        View view = (View)Map.Database.Views[viewName];
                        Map.EncryptColumn((ColumnField)view.Fields[columnName]);

                        if (!e.Values.ContainsKey("Encrypted"))
                            e.Values.Add("Encrypted", "True");

                        e.Values["Encrypted"] = "True";
                    }
                }
            }
            

            base.BeforeEdit(e);
        }

        bool relationChanged = false;
        bool encryptedChanged = false;

        protected override void AfterEditAfterCommit(Durados.EditEventArgs e)
        {
            
            base.AfterEditAfterCommit(e);

            if (true)
                return;

            if (relationChanged)
            {
                relationChanged = false;
                Initiate();
            }
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
        }

        #endregion config

        [Route("1/table/predefined/{name}")]
        [HttpGet]
        public virtual IHttpActionResult predefined(string name, string usersObjectName = null, string emailFieldName = null, int? maxLevel = null, bool? showAllForAdmin = null)
        {
            try
            {
                if (!IsAdmin())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                View view = null;
                if (!string.IsNullOrEmpty(name))
                {
                    view = GetView(name);
                    if (view == null)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                    }

                }

                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                try
                {
                    Dictionary<string, object> where = new UserRelation().GetWhere(view, usersObjectName ?? "users", emailFieldName ?? "email", maxLevel ?? 3, showAllForAdmin ?? true);
                    dictionary.Add("valid", "always");
                    dictionary.Add("sql", where["sql"]);
                    dictionary.Add("nosql", where["nosql"]);
                }
                catch (UserRelation.UserRelationException exception)
                {
                    dictionary.Add("valid", "never");
                    dictionary.Add("warnings", new string[1] { exception.Message });
                }
                return Ok(dictionary);

            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        } 
        
    }

}

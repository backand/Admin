using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Reflection;
using System.Data;
using Durados.Config.Attributes;

namespace Durados.Web.Mvc.Controllers
{
    public class AdminController : Durados.Web.Mvc.Controllers.DuradosController
    {
        protected override Durados.Database GetDatabase()
        {
            return Map.GetConfigDatabase();
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {
            if (e.View.Name == "Cron")
            {
                string name = e.View.GetDataRow(e.PrimaryKey)["Name"].ToString();
                CronHelper.Delete(name);
            }

            else if (e.View.Name == "View")
            {
                ConfigAccess configAccess = new ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(e.PrimaryKey, e.View.Database.ConnectionString);
                Map.DeleteView(viewName);

            }

            base.BeforeDelete(e);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public override ActionResult Delete(string viewName, string pk, string guid)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "Delete");

            try
            {

                if (!view.IsDeletable())
                    throw new AccessViolationException();


                view.Delete(pk, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);

                if (view.Name == "View")
                {
                    RefreshConfigCache();
                }

                return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                if (view.Name == "View")
                {
                    Map.Refresh();
                }

                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message);
            }
        }

        bool relationChanged = false;
        bool encryptedChanged = false;

        protected override void BeforeEdit(EditEventArgs e)
        {
            //Add by MiriH
            if (e.View.Name == "Cron")
            {
                if (e.Values.ContainsKey("Name") && e.Values["Name"] != null && e.Values["Name"].ToString() != string.Empty)
                {
                    string name = e.Values["Name"].ToString();
                    if (e.PrevRow["Name"].ToString() != name && CronHelper.IsCronExists(name))
                    {
                        throw new DuradosException("The cron already exists");
                    }
                    string cycle = e.Values["Cycle"].ToString();
                    string prevName = e.PrevRow["Name"].ToString();
                    CronHelper.Edit(name, cycle, prevName);
                }
                else
                {
                    throw new DuradosException("Name is required!");
                }
            }

            else if (e.View.Name == "View")
            {
                HandleLayoutType(e);
                ApplyToAllViews(e);
                /* TODO: Main MySQL depricated
                 * UpdateTheme(e);
                 */
            }
          
            else if (e.View.Name == "Field")
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
                    if (e.Values["DisplayFormat"].ToString() == DisplayFormat.None.ToString() || e.Values["DisplayFormat"].ToString() == string.Empty)
                    {
                        if (e.Values["DataType"].ToString() == DataType.Image.ToString())
                            e.Values["DisplayFormat"] = DisplayFormat.Fit.ToString();
                        else if (e.Values["DataType"].ToString() == DataType.Url.ToString())
                            e.Values["DisplayFormat"] = DisplayFormat.Hyperlink.ToString();
                        else if (e.Values["DataType"].ToString() == DataType.ShortText.ToString())
                            e.Values["DisplayFormat"] = DisplayFormat.SingleLine.ToString();
                        else if (e.Values["DataType"].ToString() == DataType.LongText.ToString())
                            e.Values["DisplayFormat"] = DisplayFormat.MultiLinesEditor.ToString();
                    }    
                }
                if ((e.Values.ContainsKey("DataType") && e.Values["DataType"].ToString() == DataType.MultiSelect.ToString()))
                {
                    HandleMultiSelectHide(e.PrevRow, e.Values);
                }

                HandleDisplayFormat(e);
                SetAutoIncrement(e.Values, e);
                if (IsUnencrypt(e))
                    throw new DuradosException("Cannot unencrypt field.");

                encryptedChanged = IsEncryptedChanged(e);
                //Formula field proccessing
                if (e.Values.ContainsKey("Formula"))
                {
                    if (e.Values["Formula"] != null && e.Values["Formula"].ToString().Trim() != string.Empty)
                    {

                        string parentViewname = e.PrevRow.GetParentRow("Fields")["Name"].ToString();

                        View currentView = (View)Map.Database.Views[parentViewname];

                        Field defaultFied = currentView.Fields[e.Values["Name"].ToString()];

                        // Replace display names with field names + security validation
                        //defaultFied.Formula = DataAccessHelper.ReplaceFieldDisplayNames(e.Values["Formula"].ToString().Trim(), true, currentView);

                        //SqlAccess sa = new SqlAccess();

                        //string SqlSelectQuery = "SELECT TOP 1 " + sa.GetCalculatedFieldStatement(defaultFied, defaultFied.Name) + " FROM [" + currentView.DataTable.TableName + "]";
                        Field defaultField = currentView.Fields[e.Values["Name"].ToString()];

                        // Replace display names with field names + security validation
                        defaultField.Formula = DataAccessHelper.ReplaceFieldDisplayNames(e.Values["Formula"].ToString().Trim(), true, currentView);


                        IDataTableAccess sa = DataAccessHelper.GetDataTableAccess(currentView);
                        SqlSchema sqlSchema = sa.GetNewSqlSchema();

                        string SqlSelectQuery = sqlSchema.GetFormula(sa.GetCalculatedFieldStatement(defaultField, defaultField.Name), currentView.DataTable.TableName);
                        try
                        {
                            //sa.ExecuteNonQuery(currentView.Database.ConnectionString, SqlSelectQuery);
                            sa.ExecuteNonQuery(currentView.ConnectionString, SqlSelectQuery, currentView.Database.SqlProduct);
                        }
                        catch (Exception exception)
                        {
                            Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                            throw new DuradosException("The statement - " + SqlSelectQuery + " is incorrect, " + exception.Message);
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
                                Field field = view.Fields[fieldName];
                                if (field.FieldType != FieldType.Children)
                                {
                                    Map.ChangeFieldRelation(view, relationChange, oldRelatedViewName, newRelatedViewName, e.PrimaryKey, (View)e.View, e.Values);
                                }
                                else
                                {
                                    Exception exception = new DuradosException("Cannot change the related table of a children field");
                                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                                    throw exception;
                                }
                            }
                            catch(Exception exception)
                            {
                                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                                throw new DuradosException("Failed to change the related table. " + exception.Message, exception);
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
            else if (e.View.Name == "FtpUpload")
            {
                HandelFtpLoadEdit(e);
            }
            else if (e.View.Name == "Menu")
            {
                if (e.Values.ContainsKey("WorkspaceID"))
                {
                    string currWsId = e.Values["WorkspaceID"].ToString();
                    string prevWsId = e.PrevRow["WorkspaceID"].ToString();

                    if (!currWsId.Equals(prevWsId))
                    {
                        ChangeViewsAndPagesWorkspace((View)e.View, e.PrimaryKey, currWsId, e.UserId, e.SysCommand);
                    }
                }
            }

            else if (e.View.Name == "Workspace")
            {

                HandelWorkspaceViewBeforEdit(e);

            }

            base.BeforeEdit(e);
        }

        private void HandleJsonName(EditEventArgs e)
        {
            string oldjsonName = e.PrevRow.IsNull("JsonName") ? string.Empty : e.PrevRow["JsonName"].ToString();
            if (!e.Values.ContainsKey("JsonName"))
                return;
            string newjsonName = !e.Values.ContainsKey("JsonName") || e.Values["JsonName"] == null ? string.Empty : e.Values["JsonName"].ToString();
            if (oldjsonName != newjsonName)
            {
                if (string.IsNullOrEmpty(newjsonName))
                    throw new DuradosException(Map.Database.Localizer.Translate("Field name cannot be empty."));
                if (newjsonName != newjsonName.ReplaceNonAlphaNumeric2())
                    throw new DuradosException(Map.Database.Localizer.Translate("Field name can contain only numbers and letters."));
                string configViewPk = e.Values["Fields_Parent"].ToString();
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
                ((View)Map.Database.Views[viewName]).ValidateUniqueFieldJsonName(e.Values["JsonName"].ToString());

            }
        }
        /* TODO: Main MySQL depricated
        public JsonResult GetPreviewPath()
        {
            return Json(Map.GetPreviewPath());
        }
        public JsonResult GetThemPath()
        {
            return Json(Map.Theme.Path);
        }
        
        private void UpdateTheme(EditEventArgs e)
        {
            try
            {
                Field themeField = e.View.GetFieldByColumnNames("Theme");

                if (themeField == null)
                    throw new DuradosException("Missing Theme field");

                ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), e.Values[themeField.Name].ToString());
                int currentThemeId = (int)themeType;
                int previousThemeId = Map.Theme.Id;

                if (currentThemeId.Equals(previousThemeId))
                    return;

                MapDataSet.durados_ThemeRow themeRow = Maps.Instance.GetTheme(currentThemeId);
                string name = themeRow.Name;
                string path = themeRow.RelativePath;

                if (e.Values[themeField.Name].Equals(ThemeType.Custom.ToString()))
                {
                    path = e.Values["CustomThemePath"].ToString();
                }
               
                Theme theme = new Theme() { Id = currentThemeId, Name = name, Path = path };

                Map.UpdateTheme(theme);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "UpdateTheme");
            }
        }
         * */

        private void ApplyToAllViews(EditEventArgs e)
        {
            ApplyColorDesignToAllViews(e);
            ApplySkinToAllViews(e);
        }

        private void ApplyColorDesignToAllViews(EditEventArgs e)
        {
            string colorDesignFlagName = "ApplyColorDesignToAllViews";
            ApplyToAllViews(e, colorDesignFlagName, PropertyGroup.ColorsDesign);
        }

        private void ApplySkinToAllViews(EditEventArgs e)
        {
            string skinFlagName = "ApplySkinToAllViews";
            ApplyToAllViews(e, skinFlagName, PropertyGroup.Skin);
        }

        private void ApplyToAllViews(EditEventArgs e, string colorDesignFlagName, PropertyGroup propertyGroup)
        {

            if (e.Values.ContainsKey(colorDesignFlagName) && Convert.ToBoolean(e.Values[colorDesignFlagName]))
            {
                Type type = typeof(View);

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                    if (propertyAttributes.Length == 1 && propertyAttributes[0] is PropertyAttribute)
                    {
                        PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];
                        if (!string.IsNullOrEmpty(propertyAttribute.Groups) && propertyAttribute.Groups.Contains(propertyGroup.ToString()))
                        {
                            values.Add(propertyInfo.Name, e.Values[propertyInfo.Name]);
                        }
                    }
                }
                e.Values[colorDesignFlagName] = "False";
                (new ConfigAccess()).ApplyToAll(e.View.Name, Map.GetConfigDatabase().ConnectionString, values);
            }
        }

        private void HandleOwnerChangeType(EditEventArgs e)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            string configViewPk = e.PrevRow["Fields"].ToString();
            string configFieldPk = e.PrimaryKey;
            DataRow configFieldRow = e.PrevRow;
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
            Field field = view.GetFieldByColumnNames(columnName);
            string fieldName = field.Name;
            View configView = (View)e.View.Database.Views["View"];
            string newFieldPK = Map.GetNewConfigFieldPk(configView, configViewPk, fieldName);


            UpdateNewField((View)e.View, configFieldRow, e.Values, newFieldPK, dataType);
            RemoveOldField((View)e.View, configFieldRow, configFieldPk);

            //Initiate();

        }

        private void RemoveOldField(View configView, DataRow oldFieldRow, string oldFieldPk)
        {
            string displayName = oldFieldRow["DisplayName"].ToString() + "_old";
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("DisplayName", displayName);
            values.Add("Excluded", true);

            configAccess.Edit(configView, values, oldFieldPk, null, null, null, null);
        }

        private void UpdateNewField(View configView, DataRow oldFieldRow, Dictionary<string,object> newValues, string newFieldPk, string dataType)
        {
            
            //string displayName = oldFieldRow["DisplayName"].ToString();
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();
            //values.Add("DisplayName", displayName);
            foreach (Field field in configView.Fields.Values.Where(f => f.Plan == "1,2,3" && f.Name != "DisplayFormat"))
            {
                values.Add(field.Name, newValues.ContainsKey(field.Name) ? newValues[field.Name] : oldFieldRow[field.Name]);
            }
            values.Add("Order", oldFieldRow["Order"]);
            values.Add("OrderForEdit", oldFieldRow["OrderForEdit"]);
            values.Add("OrderForCreate", oldFieldRow["OrderForCreate"]);

            if (dataType == DataType.LongText.ToString())
            {
                values.Add("DisplayFormat", DisplayFormat.MultiLinesEditor.ToString());
            }
            
            configAccess.Edit(configView, values, newFieldPk, null, null, null, null);
        }

        public JsonResult HasData(string fieldId)
        {
            Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
            string viewId = configAccess.GetViewPKByFieldPK(fieldId, Database.ConnectionString);
            string fieldName = configAccess.GetFieldNameByPK(fieldId, Database.ConnectionString);
            string viewName = configAccess.GetViewNameByPK(viewId, Database.ConnectionString);
            
            if (!Map.Database.Views.ContainsKey(viewName))
                return Json(true);

            View view = (View)Map.Database.Views[viewName];

            if (!view.Fields.ContainsKey(fieldName))
                return Json(true);

            Field field = view.Fields[fieldName];
            return Json((new SqlAccess()).HasData(field));
        }

        private bool ViewOwnerChangedType(EditEventArgs e)
        {
            string role = Map.Database.GetUserRole();
            bool viewOwner = role == "View Owner";
            bool dataTypeChanged = IsDataTypeChanged(e);

            return viewOwner && dataTypeChanged;
        }

        private void CheckPkDeletion(EditEventArgs e)
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

            Field field = view.Fields[fieldName];

            if (field.IsPartOfPK())
                throw new DuradosException("This is a primary key column and it cannot be deleted. If you want to remove it from the grid check the Don't Display checkbox.");
        }

        private void CheckPkDeletion(View view)
        {
           
        }

        private void HandelWorkspaceViewBeforEdit(EditEventArgs e)
        {
            if (e.Values.ContainsKey("Name") && e.PrevRow.Table.Columns.Contains("Name"))
            {
                string workspaceName = e.Values["Name"].ToString();
                string prevWorkspaceName = e.PrevRow["Name"].ToString();



                bool isAdminWorkspaceChanged = prevWorkspaceName.ToLower().Equals("admin") && !workspaceName.ToLower().Equals("admin");
                if (isAdminWorkspaceChanged)
                {
                    string msg = "Changing Admin worksapces name is not allowed.";
                    throw new DuradosException(msg);
                }

                PreventDuplicateWorkspaceName(e);
            }
        }

        private void PreventDuplicateWorkspaceName(DataActionEventArgs e)
        {
            if (e.Values.ContainsKey("Name"))
            {
                string workspaceName = e.Values["Name"].ToString();
                bool isNameChaged = e is EditEventArgs && !((EditEventArgs)e).PrevRow["Name"].ToString().ToLower().Equals(workspaceName.ToLower());
                bool nameExists=e.View.DataTable.Select("Name = '" + workspaceName + "'").Count() > 0;

                if (nameExists && (e is CreateEventArgs || isNameChaged))
                {
                    throw new DuradosException("Already contain workspace by that Name.");
                }
            }
        }

        private void ChangeViewsAndPagesWorkspace(View menuView, string menuId, string workspaceId, int userId, System.Data.IDbCommand command)
        {
            ConfigAccess configAccess = new ConfigAccess();
            configAccess.ChangeViewsAndPagesWorkspace(menuView, menuId, workspaceId, userId, (System.Data.SqlClient.SqlCommand)command);
        }

        public virtual void HandleLayoutType(DataActionEventArgs e)
        {
            string displayFormatFieldName = "Layout";
            if (!(e.Values.ContainsKey(displayFormatFieldName) && !string.IsNullOrEmpty(e.Values[displayFormatFieldName].ToString())))
                return;

            string layoutVal = e.Values[displayFormatFieldName].ToString();

            Type type = typeof(LayoutType);

            if (Enum.IsDefined(type, layoutVal))
            {
               // if (!string.IsNullOrEmpty(layoutVal) && layoutVal != DisplayFormat.None.ToString()) ClearAllDisplayFields(e);
                SetViewLayout(e.Values, layoutVal, type);
            }
        }

        private void SetViewLayout(Dictionary<string, object> values, string layoutVal, Type type)
        {
            LayoutType layoutType = (LayoutType)Enum.Parse(type, layoutVal);
            switch (layoutType)
            {
                // BasicGrid,table,excel,excel thre checkbox false
                // BasicDashboard,Dashboard,group,group,200,400,false false false true
                // BasicPreview,Preview ,group,group,200,400,false false false true
                // GridGroupFilter,table ,group,group,200,400,false false false true
                // DashboardVerticalShort,Dashboard,group,group,400,200,false false false true

                case LayoutType.BasicGrid:
                    UpdateFormatFields(values, "DataDisplayType", DataDisplayType.Table);
                    UpdateFormatFields(values, "FilterType", FilterType.Excel);
                    UpdateFormatFields(values, "SortingType", SortingType.Excel);
                    UpdateFormatFields(values, "EnableDashboardDisplay", false);
                    UpdateFormatFields(values, "EnableTableDisplay", false);
                    UpdateFormatFields(values, "EnablePreviewDisplay", false);
                    UpdateFormatFields(values, "GroupFilterDisplayLabel", true);
                    UpdateFormatFields(values, "DashboardWidth", 200);
                    UpdateFormatFields(values, "DashboardHeight", 400);

                    break;
                case LayoutType.BasicDashboard:
                    UpdateFormatFields(values, "DataDisplayType", DataDisplayType.Dashboard);
                    UpdateFormatFields(values, "FilterType", FilterType.Group);
                    UpdateFormatFields(values, "SortingType", SortingType.Group);
                    UpdateFormatFields(values, "EnableDashboardDisplay", false);
                    UpdateFormatFields(values, "EnableTableDisplay", false);
                    UpdateFormatFields(values, "EnablePreviewDisplay", false);
                    UpdateFormatFields(values, "GroupFilterDisplayLabel", true);
                    UpdateFormatFields(values, "DashboardWidth", 400);
                    UpdateFormatFields(values, "DashboardHeight", 200);
                    break;
                case LayoutType.BasicPreview:
                    UpdateFormatFields(values, "DataDisplayType", DataDisplayType.Preview);
                    UpdateFormatFields(values, "FilterType", FilterType.Group);
                    UpdateFormatFields(values, "SortingType", SortingType.Group);
                    UpdateFormatFields(values, "EnableDashboardDisplay", false);
                    UpdateFormatFields(values, "EnableTableDisplay", false);
                    UpdateFormatFields(values, "EnablePreviewDisplay", false);
                    UpdateFormatFields(values, "GroupFilterDisplayLabel", true);
                    UpdateFormatFields(values, "DashboardWidth", 200);
                    UpdateFormatFields(values, "DashboardHeight", 400);
                    break;
                case LayoutType.GridGroupFilter:
                    UpdateFormatFields(values, "DataDisplayType", DataDisplayType.Table);
                    UpdateFormatFields(values, "FilterType", FilterType.Group);
                    UpdateFormatFields(values, "SortingType", SortingType.Group);
                    UpdateFormatFields(values, "EnableDashboardDisplay", false);
                    UpdateFormatFields(values, "EnableTableDisplay", false);
                    UpdateFormatFields(values, "EnablePreviewDisplay", false);
                    UpdateFormatFields(values, "GroupFilterDisplayLabel", true);
                    UpdateFormatFields(values, "DashboardWidth", 200);
                    UpdateFormatFields(values, "DashboardHeight", 400);
                    break;
                case LayoutType.DashboardVerticalShort:
                     UpdateFormatFields(values, "DataDisplayType", DataDisplayType.Dashboard);
                    UpdateFormatFields(values, "FilterType", FilterType.Group);
                    UpdateFormatFields(values, "SortingType", SortingType.Group);
                    UpdateFormatFields(values, "EnableDashboardDisplay", false);
                    UpdateFormatFields(values, "EnableTableDisplay", false);
                    UpdateFormatFields(values, "EnablePreviewDisplay", false);
                    UpdateFormatFields(values, "GroupFilterDisplayLabel", true);
                    UpdateFormatFields(values, "DashboardWidth", 400);
                    UpdateFormatFields(values, "DashboardHeight", 200);
                    break;
                case LayoutType.Custom:
                    break;
                default:
                    break;
            }
             
                
        }

        public virtual void HandleDisplayFormat(DataActionEventArgs e)
        {
           
            string displayFormatFieldName = "DisplayFormat";
            if (!(e.Values.ContainsKey(displayFormatFieldName) && !string.IsNullOrEmpty(e.Values[displayFormatFieldName].ToString())))
                return;


            string formatVal = e.Values[displayFormatFieldName].ToString();
            
            Type type = typeof(DisplayFormat);

            if (Enum.IsDefined(type, formatVal))
            {
                bool needClearDislpayFields = !string.IsNullOrEmpty(formatVal) && formatVal != DisplayFormat.None.ToString()
                    && formatVal != DisplayFormat.Date_Custom.ToString();
                if (needClearDislpayFields) ClearAllDisplayFields(e);
                SetDisplayFormatBehaviors(e.Values, formatVal, type, e);
            }
        }

        private void HandleMultiSelectHide(DataRow prevRow,Dictionary<string,object> values)
        {
            if (values.ContainsKey("HideInCreate") && !string.IsNullOrEmpty(values["HideInCreate"].ToString()) )
            {
                string oldval = prevRow ==null && prevRow.IsNull("HideInCreate") ? string.Empty : prevRow["HideInCreate"].ToString();
                string newval = values["HideInCreate"] == null ? string.Empty :values["HideInCreate"].ToString();
                
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
            if (values.ContainsKey("HideInEdit") && !string.IsNullOrEmpty(values["HideInEdit"].ToString()) )
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
        private void SetDisplayFormatBehaviors(Dictionary<string,object> values, string formatVal, Type type,DataActionEventArgs e)
        {
            DisplayFormat displayFormat = (DisplayFormat)Enum.Parse(type, formatVal);
            switch (displayFormat)
            {
                case DisplayFormat.SingleLine:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.Text);
                    break;
                case DisplayFormat.MultiLines:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.TextArea);
                    break;
                case DisplayFormat.Email:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.Email);
                    break;
                case DisplayFormat.Password:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.Password);
                    break;
                case DisplayFormat.SSN:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.SSN);
                    break;
                case DisplayFormat.Phone:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.Phone);
                    break;
                case DisplayFormat.Html:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.Html);
                    break;
                case DisplayFormat.MultiLinesEditor:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.TextArea);
                    UpdateFormatFields(values, "Rich", true);
                    break;
                case DisplayFormat.Currency:
                    UpdateFormatFields(values, "SpecialColumn", SpecialColumn.Currency);
                    UpdateFormatFields(values, "Format", "c");
                    break;
                case DisplayFormat.NumberWithSeparator:
                    UpdateFormatFields(values, "Format", "#,##0.00");
                    break;
                case DisplayFormat.Percentage:
                    UpdateFormatFields(values, "Format", "p");
                    break;
                case DisplayFormat.DropDown:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.DropDown);
                    UpdateFormatFields(values, "AutocompleteFilter", false);
                    break;
                case DisplayFormat.AutoCompleteStratWith:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Autocomplete);
                    UpdateFormatFields(values, "AutocompleteMathing", AutocompleteMathing.StartsWith);
                    UpdateFormatFields(values, "AutocompleteFilter", true);
                    break;
                case DisplayFormat.AutoCompleteMatchAny:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Autocomplete);
                    UpdateFormatFields(values, "AutocompleteMathing", AutocompleteMathing.Contains);
                    UpdateFormatFields(values, "AutocompleteFilter", true);
                    break;
                case DisplayFormat.Tile:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Tile);
                    break;
                case DisplayFormat.Slider:
                    UpdateFormatFields(values, "ParentHtmlControlType", ParentHtmlControlType.Slider);
                    break;
                case DisplayFormat.Date_mm_dd:
                    UpdateFormatFields(values, "Format", "MM/dd/yyyy");
                    break;
                case DisplayFormat.Date_dd_mm:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy");
                    break;
                case DisplayFormat.Date_mm_dd_12:
                    UpdateFormatFields(values, "Format", "MM/dd/yyyy hh:mm:ss tt");
                    break;
                case DisplayFormat.Date_dd_mm_12:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss tt");
                    break;
                case DisplayFormat.Date_mm_dd_24:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss");
                    break;
                case DisplayFormat.Date_dd_mm_24:
                    UpdateFormatFields(values, "Format", "dd/MM/yyyy hh:mm:ss");
                    break;
                //case DisplayFormat.Date_Custom:
                //    UpdateFormatFields(values, "Format", Map.Database.DateFormat);
                //    break;
                case DisplayFormat.TimeOnly:
                    UpdateFormatFields(values, "Format", "hh:mm:ss tt");
                    break;
                case DisplayFormat.GeneralNumeric:
                    UpdateFormatFields(values, "Format",string.Empty);
                    break;

                case DisplayFormat.Fit:
                case DisplayFormat.Crop:
                   
                    if (IsToSetDefaultUpload(values,e))
                    {
                        string upload = GetDefaultUpload();
                        UpdateFormatFields(values, "FtpUpload_Parent", upload);
                    }
                    
                    UpdateFormatFields(values, "GridEditable", true);
                    UpdateFormatFields(values, "GridEditableEnabled", true);
                    UpdateFormatFields(values, "DataType", DataType.Image.ToString());
                    break;

                case DisplayFormat.Hyperlink:
                case DisplayFormat.ButtonLink:
                    UpdateFormatFields(values, "TextHtmlControlType", TextHtmlControlType.Url.ToString());
                    break;

                case DisplayFormat.Checklist:
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.CheckList);
                    break;

                case DisplayFormat.SubGrid:
                    UpdateFormatFields(values, "ChildrenHtmlControlType", ChildrenHtmlControlType.Grid);
                    break;

                default:
                    break;
            }

        }

        private static bool IsToSetDefaultUpload(Dictionary<string, object> values,DataActionEventArgs e )
        {
            if (!(e is CreateEventArgs))
                return false;

            string upload=null;
            if( values.Keys.Contains("FtpUpload_Parent"))
            {
                upload=Convert.ToString(values["FtpUpload_Parent"]);
            }
            else if( e is EditEventArgs && ((EditEventArgs)e).PrevRow.Table.Columns.Contains("FtpUpload") && !((EditEventArgs)e).PrevRow.IsNull("FtpUpload") )
            {
                upload=Convert.ToString(((EditEventArgs)e).PrevRow["FtpUpload"]);
            }
            
            return string.IsNullOrEmpty(upload);;
         
          
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
                    throw new DuradosException("Implement default FTP storage!");
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

        private void ClearAllDisplayFields(DataActionEventArgs e)
        {
            UpdateFormatFields(e.Values, "TextHtmlControlType", TextHtmlControlType.Text);
            UpdateFormatFields(e.Values, "SpecialColumn", SpecialColumn.None);
            UpdateFormatFields(e.Values, "Format", string.Empty);
            UpdateFormatFields(e.Values, "AutocompleteMathing", AutocompleteMathing.StartsWith);
            UpdateFormatFields(e.Values, "Rich", false);
        }

        private void UpdateFormatFields(Dictionary<string,object> values, string key, object val)
        {
            if (values.ContainsKey(key))
                values[key] = val;
            else
                values.Add(key, val);
        }

        private void HandelFtpLoadEdit(EditEventArgs e)
        {
            string[] ftpConnectionFields = { "FtpPassword", "FtpUserName", "FtpHost", "FtpPort" };

            //var m = e.Values.Keys.Where(x => e.Values.Keys.Any(y => ftpConnectionFields.Contains(y)));//
            //if (m.Count() == ftpConnectionFields.Length)
            //{//do check;}
            //else  if(m.Count() >0)
            //{// missing fields}
            // else if(m.Count()==0){//dont check;}
            if (e.Values.Count == 1 && !ftpConnectionFields.Contains(e.Values.ElementAt(0).Key))//edit from grid
                return;
            //else
            if (e.Values["StorageType"].ToString() == "Ftp")
            {
                if (e.Values["FtpPassword"] != null && e.Values["FtpUserName"] != null && e.Values["FtpHost"] != null && e.Values["FtpPort"] != null)
                {
                    FtpUpload uploder = new FtpUpload();
                    if (e.PrevRow["FtpHost"].ToString() != e.Values["FtpHost"].ToString() || e.PrevRow["FtpPort"].ToString() != e.Values["FtpPort"].ToString() || e.PrevRow["FtpUserName"].ToString() != e.Values["FtpUserName"].ToString() || Map.Database.EncryptedPlaceHolder != e.Values["FtpPassword"].ToString())
                        uploder.CheckConnection(e.Values["FtpHost"].ToString(), e.Values["FtpPort"].ToString(), e.Values["FtpUserName"].ToString(), e.Values["FtpPassword"].ToString());
                }
                else
                {
                    throw new DuradosException("Missing required fields");
                }
            }
            else if (e.Values["StorageType"].ToString() == "Azure")
            {
                if (e.Values["AzureAccountName"] != null && e.Values["AzureAccountKey"] != null && e.Values["DirectoryBasePath"] != null)
                {
                    FtpUpload uploder = new FtpUpload();
                    if (e.PrevRow["AzureAccountName"].ToString() != e.Values["AzureAccountName"].ToString() || Map.Database.EncryptedPlaceHolder != e.Values["AzureAccountKey"].ToString() || e.PrevRow["DirectoryBasePath"].ToString() != e.Values["DirectoryBasePath"].ToString())
                        uploder.CheckConnection(e.Values["AzureAccountName"].ToString(), e.Values["AzureAccountKey"].ToString(), e.Values["DirectoryBasePath"].ToString());
                }
                else
                {
                    throw new DuradosException("Missing required fields");
                }

            }
            else if (e.Values["StorageType"].ToString() == "Aws")
            {
                if (e.Values["AwsAccessKeyId"] != null && e.Values["AwsSecretAccessKey"] != null && e.Values["DirectoryBasePath"] != null)
                {
                    FtpUpload uploder = new FtpUpload();
                    uploder.CheckAwsConnection(e.Values["AwsAccessKeyId"].ToString(), e.Values["AwsSecretAccessKey"].ToString(), e.Values["DirectoryBasePath"].ToString(), false);
                }
                else
                {
                    throw new DuradosException("Missing required fields");
                }

            }
        }

        private bool IsUnencrypt(EditEventArgs e)
        {
            if (e.View.Name != "Field")
                return false;

            if (e.Values.ContainsKey("Encrypted") && !e.Values["Encrypted"].Equals("False"))
            {
                return (e.PrevRow["Encrypted"].Equals(true));
            }

            return false;
        }

        private bool IsSimpleRelation(string oldDataType, string newDataType, EditEventArgs e)
        {
            if (oldDataType == newDataType || newDataType != DataType.SingleSelect.ToString() || newDataType != DataType.ImageList.ToString())
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
            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
            View view = (View)Map.Database.Views[viewName];

            string fieldName = e.PrevRow["Name"].ToString();

            if (!view.Fields.ContainsKey(fieldName))
                return false;

            Field field = view.Fields[fieldName];

            if (field.FieldType != FieldType.Column)
                return false;

            ColumnField columnField = (ColumnField)field;



            if (columnField.DataColumn.DataType.Equals(relatedView.DataTable.PrimaryKey[0].DataType))
                return true;
            else
                throw new DuradosException(string.Format("The Related View '{0}' has Primary Key with type '{1}'. could not relate to Foreign Key with type '{2}'.", relatedViewName, relatedView.DataTable.PrimaryKey[0].DataType, columnField.DataColumn.DataType));
                

            
        }

        private bool IsDataTypeChanged(EditEventArgs e)
        {
            if (!e.Values.ContainsKey("DataType"))
                return false;

            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();


            return !(oldDataType == newDataType);
        }

        private bool ChangeSimpleDataType(EditEventArgs e)
        {
            if (!e.Values.ContainsKey("DataType"))
                return false;

            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();

            
            if (oldDataType == newDataType)
                return false;

            if ((newDataType == DataType.MultiSelect.ToString() || newDataType == DataType.SingleSelect.ToString()))
                return false;

            if ((oldDataType == DataType.MultiSelect.ToString() || oldDataType == DataType.SingleSelect.ToString()))
                return false;

            ConfigAccess configAccess = new ConfigAccess();
            string fieldPk = e.PrimaryKey;
            string fieldName = e.PrevRow["Name"].ToString();
            string viewPk = e.PrevRow["Fields"].ToString();
            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
            View view = (View)Map.Database.Views[viewName];
            View configView = (View)e.View.Database.Views["View"];
            if (view.Fields.ContainsKey(fieldName) && view.Fields[fieldName].FieldType == FieldType.Column)
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

        private void SetAutoIncrement(Dictionary<string,object> values, DataActionEventArgs e)
        {
            try
            {
                bool oldAutoIncrement = false;
                if (e is EditEventArgs)
                {
                    if (!(e as EditEventArgs).PrevRow.Table.Columns.Contains("AutoIncrement"))
                        return;
                    oldAutoIncrement = (e as EditEventArgs).PrevRow.IsNull("AutoIncrement") ? false : Convert.ToBoolean((e as EditEventArgs).PrevRow["AutoIncrement"]);

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
                    UpdateFormatFields(values, "HideInEdit", false);

                }
                // UpdateBackTo notexcluded

                if (!oldAutoIncrement && newAutoIncrement)
                {
                    UpdateFormatFields(values, "ExcludeInInsert", true);
                    UpdateFormatFields(values, "ExcludeInUpdate", true);
                    UpdateFormatFields(values, "HideInCreate", true);
                    UpdateFormatFields(values, "HideInEdit", true);
                }
            }
            catch { }
            //  SetAsExculded
            //    RefreshConfigCache();
        }
        private bool ChangeDataType(EditEventArgs e)
        {
            string oldDataType = e.PrevRow.IsNull("DataType") ? string.Empty : e.PrevRow["DataType"].ToString();
            string newDataType = e.Values["DataType"] == null ? string.Empty : e.Values["DataType"].ToString();

            if (IsSimpleRelation(oldDataType, newDataType, e))
                return false;

            if (oldDataType == newDataType || !(newDataType == DataType.MultiSelect.ToString() || newDataType == DataType.SingleSelect.ToString()))
                return false;

            ConfigAccess configAccess = new ConfigAccess();

            string viewPk = e.PrevRow["Fields"].ToString();
            string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
            View view = (View)Map.Database.Views[viewName];
            View configView = (View)e.View.Database.Views["View"];
            string manyToManyViewName = null;
            string columnName = Map.ChangeDataType(view, oldDataType, newDataType, e.PrimaryKey, configView, viewPk, (View)e.View, e.Values, out manyToManyViewName);
            e.Cancel = true;
            Initiate();
            string configViewPk = e.Values.ContainsKey("Fields_Parent") ? e.Values["Fields_Parent"].ToString() : viewPk;
            UpdateDisplayFormatForNewColumn(e, configViewPk, viewName, columnName);
            SetAutoIncrement(e.Values, e);
            if (newDataType == DataType.SingleSelect.ToString()  || newDataType==DataType.ImageList.ToString())
            {
                view = (View)Map.Database.Views[viewName];
                Field field = view.GetFieldByColumnNames(columnName);
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
            else if (newDataType == DataType.MultiSelect.ToString())
            {
                view = (View)Map.Database.Views[manyToManyViewName];

                ParentField field = null;
                foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
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
                int maxOrder = configAccess.GetMaxFieldOrder(viewName, "Order", e.View.Database.ConnectionString);
                maxOrder += 10;

                values.Add("Order", maxOrder);
                values.Add("OrderForCreate", maxOrder);
                values.Add("OrderForEdit", maxOrder);
                
                e.View.Edit(values, newFieldPK, null, null, null, null);
                RefreshConfigCache();
            }
            return true;

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

        const string NewFieldToken = "NewField$$";


        protected virtual string CreateField(DataActionEventArgs e)
        {
            return CreateField(e, e.Values);
        }

        protected virtual string CreateField(DataActionEventArgs e, Dictionary<string, object> values2)
        {
            lock (this)
            {
                if (!values2.ContainsKey("JsonName"))
                {
                    values2.Add("JsonName", values2["DisplayName"].ToString().ReplaceNonAlphaNumeric2());
                }
                string configViewPk = e.Values["Fields_Parent"].ToString();
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
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
                    Field field = view.GetFieldByColumnNames(columnName);
                    string fieldName = field.Name;
                    newFieldPk = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);

                }

                if (newDataType == DataType.SingleSelect.ToString() || newDataType == DataType.ImageList.ToString())
                {
                    View view = (View)Map.Database.Views[viewName];
                    Field field = view.GetFieldByColumnNames(columnName);
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
                    if (field.FieldType == FieldType.Parent && ((ParentField)field).ParentView.Fields.ContainsKey("Ordinal"))
                    {
                        string configOrdinalViewPk = configAccess.GetViewPK(parentView.Name, Database.ConnectionString);

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
                else if (newDataType == DataType.MultiSelect.ToString())
                {

                    //Initiate();


                    View view = (View)Map.Database.Views[manyToManyViewName];
                    View view2 = (View)Map.Database.Views[viewName];

                    ParentField field = null;
                    ParentField field2 = null;
                    foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
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
                    Field field = view.GetFieldByColumnNames(columnName);
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

        protected override void SyncViewSchemaToExcel(Mvc.View view, DataTable table, IDbCommand command)
        {

            Durados.DataAccess.AutoGeneration.Dynamic.Mapper mapper = new DataAccess.AutoGeneration.Dynamic.Mapper();
            mapper.Storage = Map;

            table.TableName = "TempTable";
            DataTable typedTable = mapper.GetTypedTable(table, 10000000, Map.Database);

            List<string> displayNames = view.Fields.Values.Select(f => f.DisplayName).ToList(); ;

            DataActionEventArgs e = new DataActionEventArgs(Map.GetConfigDatabase().Views["Field"], new Dictionary<string, object>(), null, command, null);
            ConfigAccess configAccess = new ConfigAccess();
            string configViewPk = configAccess.GetViewPK(view.Name, view.Database.Map.ConfigFileName);
            string newDataType = DataType.ShortText.ToString();
            e.Values.Add("DataType", newDataType);
            e.Values.Add("Fields_Parent", configViewPk);
            e.Values.Add("DisplayName", string.Empty);

            foreach (DataColumn dataColumn in table.Columns)
            {
                if (!displayNames.Contains(dataColumn.ColumnName))
                {
                    DataColumn typedColumn = typedTable.Columns[mapper.GetValidDbName(dataColumn.ColumnName)];
                    DataType dataType = GetDataType(typedColumn.DataType);
                    e.Values["DataType"] = dataType.ToString();
                    e.Values["DisplayName"] = dataColumn.ColumnName;
                    CreateField(e);
                }
            }
        }

        private DataType GetDataType(Type type)
        {
            if (IsNumeric(type))
                return DataType.Numeric;

            if (type.Equals(typeof(DateTime)))
                return DataType.DateTime;

            if (type.Equals(typeof(bool)))
                return DataType.Boolean;


            return DataType.ShortText;
        }


        protected void UpdateViewId(DataActionEventArgs e)
        {
            if (e.Values.ContainsKey("Fields_Parent") && e.Values["Fields_Parent"] != null && !e.Values["Fields_Parent"].Equals(string.Empty))
            {
            }
            else
            {
                if (Request.QueryString["mainViewName"] != null)
                {
                    string mainViewName = Request.QueryString["mainViewName"];
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    string viewPk = configAccess.GetViewPK(mainViewName, Database.ConnectionString);
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
                    throw new DuradosException("View is required!");
            }
                
        }

        protected override void BeforeCreate(CreateEventArgs e)
        {
            base.BeforeCreate(e);
            //Add by MiriH
            if (e.View.Name == "Cron")
            {
                if (e.Values.ContainsKey("Name") && e.Values["Name"] != null && e.Values["Name"].ToString() != string.Empty)
                {
                    string name = e.Values["Name"].ToString();
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    if (CronHelper.IsCronExists(name))
                    {
                        throw new DuradosException("The cron already exists");
                    }

                    //Set values for create row in durados_Cron
                    string cycle = e.Values["Cycle"].ToString();
                    CronHelper.Create(name,cycle);
                }
                else
                {
                    throw new DuradosException("Name is required!");
                }
            }
            else if (e.View.Name == "View")
            {
                if (e.Values.ContainsKey("Name") && e.Values["Name"] != null && e.Values["Name"].ToString() != string.Empty)
                {
                    string name = e.Values["Name"].ToString();
                    if (Map.IsViewAlreadyExists(name))
                    {
                        throw new DuradosException("The view already exists");
                    }
                    //if (!Map.IsViewExistInServer(name))
                    //{
                    //    throw new DuradosException("The view does not exist in the server");
                    //}
                }
                else
                {
                    throw new DuradosException("Name is required!");
                }

                string fileName = Map.GetConfigDatabase().ConnectionString;
                e.Values["Views_Parent"] = Durados.DataAccess.ConfigAccess.GetDatabasePK(fileName);
                SetViewDefaultsIfValueIsNull(e);
                if (e.Values["Views_Parent"] == null || e.Values["Views_Parent"].ToString() == string.Empty)
                {
                    throw new DuradosException("Database configuration is missing.");
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
                        string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
                        View view = (View)Map.Database.Views[viewName];
                        e.Values["DisplayName"] = GetNewFieldName(view);
                    }
                }
                else
                {
                    throw new DuradosException("Display Name is required!");
                }

                CreateField(e);
                //e.Values["Name"] = e.Values["DisplayName"].ToString().Replace(" ", "_");
                
            }
            else if (e.View.Name == "FtpUpload")
            {
                if (e.Values["StorageType"].ToString() == "Ftp")
                {
                    if (e.Values["FtpPassword"] != null && e.Values["FtpUserName"] != null && e.Values["FtpHost"] != null && e.Values["FtpPort"] != null)
                    {
                        FtpUpload uploder = new FtpUpload();
                        uploder.CheckConnection(e.Values["FtpHost"].ToString(), e.Values["FtpPort"].ToString(), e.Values["FtpUserName"].ToString(), e.Values["FtpPassword"].ToString());
                    }
                    else
                    {
                        throw new DuradosException("Missing required fields");
                    }
                }
                else if (e.Values["StorageType"].ToString() == "Azure")
                {
                    if (e.Values["AzureAccountName"] != null && e.Values["AzureAccountKey"] != null && e.Values["DirectoryBasePath"] != null)
                    {
                        FtpUpload uploder = new FtpUpload();
                        uploder.CheckConnection(e.Values["AzureAccountName"].ToString(), e.Values["AzureAccountKey"].ToString(), e.Values["DirectoryBasePath"].ToString());
                    }
                    else
                    {
                        throw new DuradosException("Missing required fields");
                    }

                }
                else if (e.Values["StorageType"].ToString() == "Aws")
                {
                    if (e.Values["AwsAccessKeyId"] != null && e.Values["AwsSecretAccessKey"] != null && e.Values["DirectoryBasePath"] != null)
                    {
                        FtpUpload uploder = new FtpUpload();
                        uploder.CheckAwsConnection(e.Values["AwsAccessKeyId"].ToString(), e.Values["AwsSecretAccessKey"].ToString(), e.Values["DirectoryBasePath"].ToString(), false);
                    }
                    else
                    {
                        throw new DuradosException("Missing required fields");
                    }

                }
            }

            else if (e.View.Name == "Workspace")
            {
                PreventDuplicateWorkspaceName(e);
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
            DataTable table = systemGenerator.CreateTable(view.DataTable.TableName, string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName, view.Database.ConnectionString);

            HashSet<string> fieldsNames = new HashSet<string>();

            foreach (DataColumn column in table.Columns)
            {
                fieldsNames.Add(column.ColumnName);
            }

            return fieldsNames;
        }

        private void LoadCreateValues(DataActionEventArgs e, Dictionary<string, object> values)
        {
            IEnumerable<Field> fields = e.View.Fields.Values.Where(f => !f.HideInCreate && !f.ExcludeInInsert && !f.Excluded);

            HashSet<string> names = new HashSet<string>() { "Fields_Parent", "RelatedViewName", "Name", "DataType", "Formula", "DatabaseNames", "DisplayFormat" };

            //Database db = Map.GetDefaultDatabase();
            //Field f = db.Views["Table"].Fields["Column"];
           
            foreach (Field field in fields)
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

        private void UpdateDisplayFormatForNewColumn(DataActionEventArgs e, string configViewPk, string viewName, string columnName)
        {
            string displayFormatFieldName = "DisplayFormat";
            View view = (View)Map.Database.Views[viewName];
            Field field = view.GetFieldByColumnNames(columnName);
            if (field == null)//children
                return;
            string fieldName = field.Name;
            string newFieldPK = Map.GetNewConfigFieldPk((View)e.View.Database.Views["View"], configViewPk, fieldName);
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (e.Values.ContainsKey(displayFormatFieldName))
            {

                object formatVal=e.Values[displayFormatFieldName];

                if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(DataType.Image.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(DisplayFormat.None.ToString())))
                {
                    formatVal = DisplayFormat.Fit.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(DataType.Url.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(DisplayFormat.None.ToString())))
                {
                    formatVal = DisplayFormat.Hyperlink.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(DataType.Email.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(DisplayFormat.None.ToString())))
                {
                    formatVal = DisplayFormat.Email.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(DataType.Html.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(DisplayFormat.None.ToString())))
                {
                    formatVal = DisplayFormat.Html.ToString();
                }
                else if (e.Values.ContainsKey("DataType") && e.Values["DataType"].Equals(DataType.DateTime.ToString()) && (formatVal == null || formatVal.Equals(string.Empty) || formatVal.Equals(DisplayFormat.None.ToString())))
                {
                    DisplayFormat displayFormat = DisplayFormat.Date_Custom;
                    string dateFormat = Map.Database.DateFormat;

                    if (dateFormat == "MM/dd/yyyy")
                    {
                        displayFormat = DisplayFormat.Date_mm_dd;
                    }
                    else if (dateFormat == "dd/MM/yyyy")
                    {
                        displayFormat = DisplayFormat.Date_dd_mm;
                    }
                    formatVal = displayFormat.ToString();
                }
                
                values.Add(displayFormatFieldName, formatVal);
                if (!string.IsNullOrEmpty(formatVal.ToString()))
                    SetDisplayFormatBehaviors(values, formatVal.ToString(), typeof(DisplayFormat),e);
            }

            HandleCategory(field, values);

            e.View.Edit(values, newFieldPK, null, null, null, null);
           
        }

        private void HandleCategory(Field field, Dictionary<string, object> values)
        {
            if (field.Category == null && field.View.Categories.Count > 0)
            {
                Category category = field.View.Categories.Values.OrderBy(c => c.Ordinal).FirstOrDefault();
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

        private void SetViewDefaultsIfValueIsNull(CreateEventArgs e)
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
                                    object value=propertyInfo.GetValue(view, null);
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

        protected override bool IsConfigurationField(string viewName)
        {
            return viewName == "Field";
        }

        protected virtual View CreateView(System.Data.DataTable table, Dictionary<string, object> values)
        {

            string viewName = Map.CreateView(table);

            RefreshConfigCache(string.Empty);


            Durados.Database configDatabase = Map.GetConfigDatabase();
            string filename = configDatabase.ConnectionString;
            View configView = (View)configDatabase.Views["View"];
            ConfigAccess configAccess = new ConfigAccess();
            string configViewPk = configAccess.GetViewPK(viewName, filename);

            configAccess.Edit(configView, values, configViewPk, null, null, null, null);

            Initiate();

            return (View)Map.Database.Views[viewName];

        }

        protected override View CreateView(System.Data.DataTable table)
        {
            Durados.Database configDatabase = Map.GetConfigDatabase();
            string filename = configDatabase.ConnectionString;
            ConfigAccess configAccess = new ConfigAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("GridEditable", true);
            values.Add("GridEditableEnabled", true);
            values.Add("WorkspaceID", Map.Database.GetDefaultWorkspace().ID.ToString());
            string menuPk = GetNewViewMenuPK(configAccess, (Database)configDatabase);
            values.Add("Menu_Parent", menuPk);

            return CreateView(table, values);


        }



        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            if (e.View.Name == "View")
            {
                if (e.Values.ContainsKey(Name))
                {
                    string name = e.Values[Name].ToString();
                    string editableTableName = e.Values[EditableTableName].ToString();
                    Map.CreateView(name, e.PrimaryKey, editableTableName, (View)e.View);
                    Initiate();
                }
            }
            else if (e.View.Name == "Field")
            {
                //string fieldName = null;
                //string viewName = null;
                //string type = null;
                //string parentViewName = null;
                //if (e.Values.ContainsKey("Name"))
                //{
                //    fieldName = e.Values["Name"].ToString();
                //}
                //else
                //{
                //    return;
                //}
                //if (e.Values.ContainsKey("Fields"))
                //{
                //    string viewId = e.Values["Fields"].ToString();
                //}
                //else
                //{
                //    return;
                //}

                //if (e.Values.ContainsKey("RelatedViewName"))
                //{
                //    parentViewName = e.Values["RelatedViewName"].ToString();
                //}

                //Map.CreateField(viewName, fieldName, type, parentViewName);

                //if (e.Values.ContainsKey("Name"))
                //{
                //    string fieldName = e.Values["Name"].ToString();
                //    if (fieldName.StartsWith(NewFieldToken.TrimEnd("$$".ToCharArray())))
                //    {
                //        ConfigAccess configAccess = new ConfigAccess();
                //        newFieldPk = configAccess.GetFieldPK(fieldName, e.View.Database.ConnectionString);
                //    }
                //}
            }
            else
            {
                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }
            }
        }

        //protected override void AfterEditBeforeCommit(EditEventArgs e)
        //{
        //    base.AfterEditBeforeCommit(e);
        //    if (IsEncryptedChanged(e))
        //    {
        //        ConfigAccess configAccess = new ConfigAccess();
        //        string viewPk = e.PrevRow["Fields"].ToString();
        //        string viewName = configAccess.GetViewNameByPK(viewPk, e.View.Database.ConnectionString);
        //        string columnName = e.PrevRow["Name"].ToString();
        //        View view = (View)Map.Database.Views[viewName];
        //        Map.EncryptColumn((ColumnField)view.Fields[columnName]);
        //    }
        //}

        private bool IsEncryptedChanged(EditEventArgs e)
        {
            if (e.View.Name != "Field")
                return false;

            if (e.Values.ContainsKey("Encrypt") && e.Values["Encrypt"].Equals("True"))
            {
                return (e.PrevRow["Encrypt"].Equals(false));
            }

            return false;
        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
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

        protected override void AfterDeleteAfterCommit(DeleteEventArgs e)
        {
            base.AfterDeleteAfterCommit(e);
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
            //Map.Refresh();
        }

        public ActionResult Open(string viewName, string pk)
        {
            Durados.DataAccess.ConfigAccess configAccess = new ConfigAccess();
            string viewName2 = configAccess.GetViewNameByPK(pk, Map.GetConfigDatabase().ConnectionString);

            View view = (View)Map.Database.Views[viewName2];


            return RedirectToAction(view.IndexAction, view.Controller, new { viewName = viewName2, isMainPage = true, ajax = false });
        }

        public JsonResult RefreshConfig(string fieldOrder)
        {
            try
            {
                RefreshConfigCache(fieldOrder);
                Map.IsConfigChanged = true;
                return Json(GetRecachedMessage());
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        protected void RefreshConfigCache(string fieldOrder)
        {
            lock (Map)
            {
                Database configDatabase = Map.GetConfigDatabase();
                Map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(Map.Database.ConfigVersion, Map.GetConfigDatabase().ConnectionString);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Map.Logger);
                Map.SaveDynamicMapping();
                Map.Refresh();
                configDatabase = Map.GetConfigDatabase();
                if (fieldOrder != null)
                {
                    ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).OrdinalColumnName = fieldOrder;
                    ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).DefaultSort = fieldOrder;
                }
            }
        }

        protected void RefreshConfigCache()
        {
            lock (Map)
            {
                Database configDatabase = Map.GetConfigDatabase();
                Map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(Map.Database.ConfigVersion, Map.GetConfigDatabase().ConnectionString);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Map.Logger);
                Map.SaveDynamicMapping();
                Map.Refresh();
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

        public void CancelPreview(string viewName)
        {
            View view = GetView(viewName);
            if (!view.IsAllow())
                return;
            //HashSet<string> roles = new HashSet<string>() { "Developer", "Admin", "View Owner" };
            //string role = Map.Database.GetUserRole();

            //if (roles.Contains(role))
            
            Maps.Instance.Restart(Maps.GetCurrentAppName());
            string fileName = Map.GetConfigDatabase().ConnectionString;
            ConfigAccess.Restart(fileName);
            
        }

        public string Wakeup()
        {

            if (Map is DuradosMap)
            {
                if (Map.Database.GetUserRole() == "Developer")
                {
                    Maps.Instance.WakeupCalltoApps();
                }
                else
                {
                    return "Failed to wakeup apps. You must have a developer role.";
                }
            }
            else
            {
                return "Failed to wakeup. Please go to " + Maps.DuradosAppName + Maps.Host + " and try it there.";
            }

            return "Done";
        }


        public string Restart(string appName)
        {
            string fileName = Map.GetConfigDatabase().ConnectionString;
            if (Map is DuradosMap)
            {
                if (Map.Database.GetUserRole() == "Developer")
                {
                    Maps.Instance.Restart(appName);
                    ConfigAccess.Restart(fileName);
                    
                   
                }
                else
                {
                    return "Failed to restart. You must have a developer role to restart an app.";
                }
            }
            else
            {
                string role = Map.Database.GetUserRole();
                if (role == "Developer" || role == "Admin" || Maps.IsSuperDeveloper(Map.Database.GetCurrentUsername()))
                {
                    appName = Maps.GetCurrentAppName();
                    Maps.Instance.Restart(appName);
                    ConfigAccess.Restart(fileName);

                    
                }
                else
                    return "Failed to restart. You must have an admin role to restart an app.";
            }

            string blobName = Maps.GetStorageBlobName(fileName);
            if (Maps.Instance.StorageCache.ContainsKey(blobName))
            {
                Maps.Instance.StorageCache.Remove(blobName);
            }
            blobName += "xml";
            if (Maps.Instance.StorageCache.ContainsKey(blobName))
            {
                Maps.Instance.StorageCache.Remove(blobName);
            }

            return appName + " restarted";
        }

        public JsonResult Sync2(string viewName)
        {
            try
            {
                lock (Map)
                {
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    string configViewPk = configAccess.GetViewPK(viewName, Map.GetConfigDatabase().ConnectionString);
                    Map.Sync(viewName, configViewPk);
                    Initiate();

                }
                var json = new { success = true, message = Map.Database.Localizer.Translate("Synced") };
                return Json(json);
            }
            catch (Exception exception)
            {
                var json = new { success = false, message = exception.Message };
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(json);
            }
        }

        public JsonResult SyncAll()
        {
            try
            {
                lock (Map)
                {
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                    foreach (View view in Map.Database.Views.Values.Where(v => !v.SystemView && !v.IsCloned))
                    {
                        string viewName = view.Name;
                        string configViewPk = configAccess.GetViewPK(viewName, Map.GetConfigDatabase().ConnectionString);
                        Map.Sync(viewName, configViewPk);
                    }
                    Initiate();

                }
                var json = new { success = true, message = Map.Database.Localizer.Translate("Synced") };
                return Json(json);
            }
            catch (Exception exception)
            {
                var json = new { success = false, message = exception.Message };
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(json);
            }
        }

        public JsonResult Sync(string configViewPks)
        {
            try
            {
                lock (Map)
                {
                    string[] configViewPkArray = configViewPks.Split(',');
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    foreach (string configViewPk in configViewPkArray)
                    {
                        string viewName = configAccess.GetViewNameByPK(configViewPk, Database.ConnectionString);
                        Map.Sync(viewName, configViewPk);
                    }
                    if (Map.Database.AutoCommit)
                    {
                        Initiate();
                    }
                }
                var json = new { success = true, message = Map.Database.Localizer.Translate("Synced") };
                return Json(json);
            }
            catch (Exception exception)
            {
                var json = new { success = false, message = exception.Message };
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(json);
            }
        }

        public JsonResult Decline()
        {
            try
            {
                lock (Map)
                {
                    Map.Refresh();
                }
                return Json(GetRecachedMessage());
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        public JsonResult Label(string label)
        {
            try
            {
                lock (Map)
                {
                    Map.Refresh();
                }
                return Json(GetRecachedMessage());
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        private string GetRecachedMessage()
        {
            return Map.Database.Localizer.Translate("The configuration changes have been saved. To see the changes REFRESH the page");
        }

        public JsonResult Revert(string label)
        {
            try
            {
                lock (Map)
                {
                    Map.Refresh();
                }
                return Json(GetRecachedMessage());
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }



        public JsonResult ChangeConfigFieldOrder(string fieldOrder)
        {
            try
            {
                Database configDatabase = Map.GetConfigDatabase();
                ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).OrdinalColumnName = fieldOrder;
                ((Durados.Web.Mvc.View)configDatabase.Views["Field"]).DefaultSort = fieldOrder;

                return Json("");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        public JsonResult CopyOrder(string fieldPK, string guid)
        {
            try
            {

                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                View view = (View)Map.GetConfigDatabase().Views["Field"];

                Dictionary<string, object> values = GetSessionFilterValues(guid);
                configAccess.Reorder(fieldPK, Database.ConnectionString, values, view);

                return Json("Reordered");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }

        public JsonResult RepairOrder(string viewName, string pk, string guid)
        {
            try
            {

                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string fileName = Map.GetConfigDatabase().ConnectionString;

                configAccess.Reorder(pk, fileName);

                return Json("Reordered");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json(exception.Message);
            }
        }


        public JsonResult GetInheritRoles(string type, string pk)
        {
            try
            {
                UI.Json.Roles roles = new Durados.Web.Mvc.UI.Json.Roles();

                switch (type)
                {
                    case "Field":
                        View view = (View)Map.GetField(pk).View;
                        roles.AllowCreateRoles = view.AllowCreateRoles;
                        roles.AllowDeleteRoles = view.AllowDeleteRoles;
                        roles.AllowEditRoles = view.AllowEditRoles;
                        roles.AllowSelectRoles = view.AllowSelectRoles;
                        roles.DenyCreateRoles = view.DenyCreateRoles;
                        roles.DenyDeleteRoles = view.DenyDeleteRoles;
                        roles.DenyEditRoles = view.DenyEditRoles;
                        roles.DenySelectRoles = view.DenySelectRoles;
                        break;
                    case "View":
                        Workspace workspace = Map.GetView(pk).Workspace;
                        if (workspace != null)
                        {
                            roles.AllowCreateRoles = workspace.AllowCreateRoles;
                            roles.AllowDeleteRoles = workspace.AllowDeleteRoles;
                            roles.AllowEditRoles = workspace.AllowEditRoles;
                            roles.AllowSelectRoles = workspace.AllowSelectRoles;
                            roles.DenyCreateRoles = workspace.DenyCreateRoles;
                            roles.DenyDeleteRoles = workspace.DenyDeleteRoles;
                            roles.DenyEditRoles = workspace.DenyEditRoles;
                            roles.DenySelectRoles = workspace.DenySelectRoles;
                        }
                        break;
                    case "Workspace":
                        roles.AllowCreateRoles = Map.Database.DefaultAllowCreateRoles;
                        roles.AllowDeleteRoles = Map.Database.DefaultAllowDeleteRoles;
                        roles.AllowEditRoles = Map.Database.DefaultAllowEditRoles;
                        roles.AllowSelectRoles = Map.Database.DefaultAllowSelectRoles;
                        roles.DenyCreateRoles = Map.Database.DefaultDenyCreateRoles;
                        roles.DenyDeleteRoles = Map.Database.DefaultDenyDeleteRoles;
                        roles.DenyEditRoles = Map.Database.DefaultDenyEditRoles;
                        roles.DenySelectRoles = Map.Database.DefaultDenySelectRoles;

                        break;
                    case "Page":
                        int i;
                        if (int.TryParse(pk, out i) && Map.Database.Pages.Keys.Contains(i))
                        {
                            Page page = Map.Database.Pages[i];
                            roles.AllowSelectRoles = page.Workspace.AllowSelectRoles;
                        }
                        break;
                    case "Chart":
                        int c;
                        if (int.TryParse(pk, out c) && Map.Database.MyCharts.Charts.Keys.Contains(c))
                        {
                            Chart chart = Map.Database.MyCharts.Charts[c];
                            roles.AllowSelectRoles = chart.Workspace.AllowSelectRoles;
                        }
                        break;
                    default:
                        break;
                }

                return Json(roles);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public ActionResult ConfigField(string viewName, string fieldName)
        {
            return RedirectToAction("Index", "Admin", new { viewName = "Field", isMainPage = true, ID = (new Durados.DataAccess.ConfigAccess()).GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString) });
        }

        public JsonResult HideField(string viewName, string fieldName, bool hideInDialogs)
        {
            try
            {
                string pk = (new Durados.DataAccess.ConfigAccess()).GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);

                View view = (View)Map.GetConfigDatabase().Views["Field"];


                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("HideInTable", true);
                if (hideInDialogs)
                {
                    values.Add("HideInEdit", true);
                    values.Add("HideInCreate", true);
                    values.Add("ChildrenHtmlControlType", "Hide");
                }

                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }

                return Json("");

            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }


        public JsonResult UnhideField(string viewName, string fieldName, string guid, string pk)
        {

            try
            {
                View view = (View)Map.GetConfigDatabase().Views["Field"];


                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("HideInTable", false);

                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);

                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string d_pk = configAccess.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                configAccess.MoveField(Map.GetConfigDatabase().Views["Field"], viewName, pk, d_pk, Convert.ToInt32(GetUserID()), true);

                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }
                return Json("");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public JsonResult RenameField(string viewName, string fieldName, string NewDisplayName)
        {
            try
            {
                string pk = (new Durados.DataAccess.ConfigAccess()).GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);

                View view = (View)Map.GetConfigDatabase().Views["Field"];


                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("DisplayName", NewDisplayName);

                view.Edit(values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                
                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }
                return Json("");

            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public ActionResult Fields(string viewName, string guid, string filter)
        {
            string viewPK = (new Durados.DataAccess.ConfigAccess()).GetViewPK(viewName, Map.GetConfigDatabase().ConnectionString);

            System.Collections.Specialized.NameValueCollection queryString = new System.Collections.Specialized.NameValueCollection();
            queryString.Add("Fields", viewPK);
            queryString.Add("__Fields_Children__", viewName);
            queryString.Add("isMainPage", "True");

            Durados.Web.Mvc.UI.Helpers.ViewHelper.SetSessionState(guid + "PageFilterState", queryString);

            if (!string.IsNullOrEmpty(filter))
            {
                SetSessionFilter("Field", guid, filter, false);
            }

            return RedirectToAction("Index", "Admin", new { viewName = "Field", isMainPage = true, Fields = viewPK, __Fields_Children__ = viewName, guid = guid, firstTime = true, SortColumn = "Order", direction = "Asc" });
        }

        public JsonResult GetFieldPK(string viewName, string fieldName)
        {
            try
            {
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string pk = configAccess.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                return Json(pk);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public JsonResult GetViewPK(string viewName)
        {
            try
            {
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string pk = configAccess.GetViewPK(viewName, Map.GetConfigDatabase().ConnectionString);
                return Json(pk);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public override JsonResult ChangeOrdinal(string viewName, string o_pk, string d_pk, string guid)
        {
            JsonResult result = base.ChangeOrdinal(viewName, o_pk, d_pk, guid);

            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }

            return result;
        }

        public JsonResult MoveField(string viewName, string fieldName, bool before, string guid, int to)
        {
            try
            {
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string o_pk = configAccess.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                string d_pk = to.ToString();
                configAccess.MoveField(Map.GetConfigDatabase().Views["Field"], viewName, o_pk, d_pk, Convert.ToInt32(GetUserID()), before);

                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }
                return Json("");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public JsonResult MoveField2(string viewName, string fieldName, bool before, string guid, string to)
        {
            try
            {
                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                string o_pk = configAccess.GetFieldPK(viewName, fieldName, Map.GetConfigDatabase().ConnectionString);
                string d_pk = configAccess.GetFieldPK(viewName, to, Map.GetConfigDatabase().ConnectionString);
                configAccess.MoveField(Map.GetConfigDatabase().Views["Field"], viewName, o_pk, d_pk, Convert.ToInt32(GetUserID()), before);

                if (Map.Database.AutoCommit)
                {
                    RefreshConfigCache();
                }
                return Json("");
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("error");
            }
        }

        public override ActionResult Edit(string viewName, FormCollection collection, string pk, string guid)
        {
            if (viewName == durados_Schema)
            {
                
                if (Session[durados_Schema] == null)
                {
                    return RedirectToAction("Index", "Admin", new { viewName = viewName, ajax = true, guid = guid });
                }
                Durados.Web.Mvc.UI.Json.View jsonView = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.View>(collection["jsonView"]);
                System.Data.DataView dataView = (System.Data.DataView)Session[durados_Schema];
                string editableTableName = jsonView.Fields[EditableTableName].Value.ToString();
                dataView.Table.Rows.Find(pk)[EditableTableName] = editableTableName;
                Session[durados_Schema] = dataView;

                return RedirectToAction("Index", "Admin", new { viewName = viewName, ajax = true, guid = guid });
            }
            else
            {
                return base.Edit(viewName, collection, pk, guid);
            }
        }

        public override JsonResult EditOnly(string viewName, FormCollection collection, string pk, string guid)
        {
            if (viewName == durados_Schema)
            {
                Durados.Web.Mvc.UI.Json.View jsonView = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.View>(collection["jsonView"]);
                System.Data.DataView dataView = (System.Data.DataView)Session[durados_Schema];
                string editableTableName = jsonView.Fields[EditableTableName].Value.ToString();
                dataView.Table.Rows.Find(pk)[EditableTableName] = editableTableName;
                Session[durados_Schema] = dataView;
                return Json("success");
            }
            else
                return base.EditOnly(viewName, collection, pk, guid);
        }

        //protected override void SetSql(SelectEventArgs e)
        //{
        //    base.SetSql(e);
        //    if (e.View.Name == durados_Schema)
        //    {
        //        SqlSchema sqlSchema = new SqlSchema();

        //        e.Sql = sqlSchema.GetEntitiesSelectStatement();


        //    }
            
        //}


        const string durados_Schema = "durados_Schema";
        const string EditableTableName = "EditableTableName";
        const string Name = "Name";
        
        protected override System.Data.DataView GetDataTable(Durados.Web.Mvc.View view, int page, int pageSize, FormCollection collection, string search, string SortColumn, string direction, string guid)
        {
            if (view.Name == durados_Schema)
            {
                Durados.Web.Mvc.UI.Json.Filter jsonFilter = null;

                if (collection != null && collection["jsonFilter"] != null)
                {
                    try
                    {
                        jsonFilter = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<Durados.Web.Mvc.UI.Json.Filter>(collection["jsonFilter"]);

                    }
                    catch { }
                }

                System.Data.DataView dataView = null;
                //if (Session[durados_Schema] == null || jsonFilter != null)
                //{
                    dataView = Map.GetSchemaEntities();

                    
                    foreach (System.Data.DataRowView row in dataView)
                    {
                        string name = row[Name].ToString();
                        if (Map.Database.Views.ContainsKey(name) || name.ToLower().Contains("aspnet"))
                            row.Delete();
                    }

                    if (jsonFilter != null)
                    {
                        string filter = "";
                        foreach (Durados.Web.Mvc.UI.Json.Field field in jsonFilter.Fields.Values)
                        {
                            if (field.Value != null && field.Value.ToString() != string.Empty)
                            {
                                filter += field.Name + " like '%" + field.Value.ToString().Split(' ')[1].Trim("\"".ToCharArray()) + "%' and ";
                            }
                        }

                        filter += " 1=1 ";
                        dataView.RowFilter = filter;

                    }

                    Session[durados_Schema] = dataView;

                //}
                //else
                //    dataView = (System.Data.DataView)Session[durados_Schema];


                ViewData["Database"] = Map.Database;
                ViewData["filter"] = null;
                ViewData["ColumnsExcluder"] = GetNewColumnsExcluder(view, null);

                ViewData["search"] = false;

                ViewData["rowCount"] = dataView.Count;

                
                return dataView;
            }
            else
                return base.GetDataTable(view, page, pageSize, collection, search, SortColumn, direction, guid);
        }

        protected override System.Data.DataRow GetDataRow(Durados.Web.Mvc.View view, string pk)
        {
            if (view.Name == durados_Schema)
            {
                System.Data.DataView dataView = (System.Data.DataView)Session[durados_Schema];
                return dataView.Table.Rows.Find(pk);
            }
            else if (view.Name == "View")
            {
                DataRow row = base.GetDataRow(view, pk);
                /* TODO: Main MySQL depricated
                try
                {

                    row["Theme"] = Map.Theme.Name;
                    if (Map.Theme.Id == Maps.CustomTheme)
                    {
                        row["CustomThemePath"] = Map.Theme.Path;
                    }
                }
                catch (Exception exception)
                {
                    Map.Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "GetDataRow", exception, 1, "Update theme from cache");
                }
                 */
                return row;
            }
            else
                return base.GetDataRow(view, pk);
        }

        public override JsonResult GetSelectList(string viewName2, string fieldName, string fk, FormCollection collection)
        {
            if (fieldName == "Menu_Parent" || fieldName == "PageMenu_Parent")
            {
                return GetMenuSelectList(fk);
            }

            if (!(viewName2 == "Field" && fieldName == "DisplayColumn"))
                return base.GetSelectList(viewName2, fieldName, fk, collection);

            
            ConfigAccess configAccess = new ConfigAccess();
            string viewName = configAccess.GetViewNameByPK(fk, Map.GetConfigDatabase().ConnectionString);

            Durados.Web.Mvc.View view = GetView(viewName, "GetSelectList");

            
            Dictionary<string, string> selectOptions;
            selectOptions = new Dictionary<string, string>();

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children))
            {
                selectOptions.Add(field.Name, field.DisplayName);
            }
            
            Durados.Web.Mvc.UI.Json.SelectList selectList = Durados.Web.Mvc.UI.Json.SelectList.GetSelectList(selectOptions);

            return Json(selectList);
        }

        public override ActionResult AddItems(string viewName, string pks, string parentViewName, string fk, FormCollection collection, string guid, bool? isSelectAll, string searchGuid)
        {
            if (viewName == "View")
            {
                string message = " items successfuly added.\n\r";
                string errorMessages = string.Empty;
                int added = 0;

                if (Session[durados_Schema] != null)
                {
                    System.Data.DataView dataView = (System.Data.DataView)Session[durados_Schema];

                    added = Map.Database.AddViews(pks, dataView, false, out errorMessages);
                }
                if (string.IsNullOrEmpty(errorMessages))
                {
                    return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
                }
                else
                {
                    return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (added > 0 ? (added + message) : string.Empty) + errorMessages);

                }
            }
            else
            {
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", Map.Database.Localizer.Translate("Session expired. No changes were made. Please try again."));

            }
        }


        //public override ActionResult AddItems(string viewName, string pks, string parentViewName, string fk, FormCollection collection, string guid)
        //{
        //    if (viewName == "View")
        //    {
        //        string message = " items successfuly added.\n\r";
        //        string errorMessages = string.Empty;
        //        int added = 0;

        //        Durados.Database configDatabase = Map.GetConfigDatabase();
        //        if (Session[durados_Schema] != null)
        //        {
        //            string[] pkArray = pks.Split(',');

        //            Dictionary<string, string> viewNames = new Dictionary<string, string>();

        //            System.Data.DataView dataView = (System.Data.DataView)Session[durados_Schema];
        //            foreach (string pk in pkArray)
        //            {
        //                System.Data.DataRow row = dataView.Table.Rows.Find(pk);
        //                if (row != null)
        //                {
        //                    string name = row[Name].ToString();
        //                    string editableTableName = row[EditableTableName].ToString();
        //                    try
        //                    {
        //                        Map.CreateView(name, null, editableTableName, (View)configDatabase.Views[viewName]);
        //                        viewNames.Add(name, editableTableName);
        //                        added++;

        //                        try
        //                        {
        //                            Map.Logger.WriteToEventLog("The View: " + name + " was added by " + HttpContext.User.Identity.Name, System.Diagnostics.EventLogEntryType.Error, 2050);
        //                        }
        //                        catch { }
        //                    }
        //                    catch (Exception exception)
        //                    {
        //                        errorMessages += "Failed to add '" + name + "': " + exception.Message + "\n\r";
        //                    }
        //                }
        //            }
        //            Database db = Map.Database;
        //            Initiate();
        //            // update default config
        //            ConfigAccess configAccess = new ConfigAccess();
        //            string filename = configDatabase.ConnectionString;
        //            View configView = (View)configDatabase.Views["View"];
        //            View configField = (View)configDatabase.Views["Field"];
        //            string menuPk = GetNewViewMenuPK(configAccess, (Database)configDatabase);
        //            SqlAccess sqlAccess = new SqlAccess();
        //            int autoCompleteLimit = 500;

        //            foreach (string name in viewNames.Keys)
        //            {
        //                string configViewPk = configAccess.GetViewPK(name, filename);
        //                Dictionary<string, object> values = new Dictionary<string, object>();
        //                values.Add("GridEditable", true);
        //                values.Add("GridEditableEnabled", true);
        //                values.Add("MultiSelect", true);
        //                values.Add("WorkspaceID", db.GetAdminWorkspaceId().ToString());
        //                values.Add("Menu_Parent", menuPk);
        //                string editableTableName = viewNames[name];
        //                if (!string.IsNullOrEmpty(editableTableName))
        //                    values.Add("EditableTableName", editableTableName);
        //                configAccess.Edit(configView, values, configViewPk, null, null, null, null);

        //                System.Data.DataRow[] fieldsRows = configAccess.GetFieldsRows(name, filename);
        //                if (fieldsRows != null)
        //                {
        //                    Durados.View view = (Durados.View)db.Views[name];
        //                    foreach (System.Data.DataRow fieldRow in fieldsRows)
        //                    {
        //                        string configFieldPK = fieldRow["ID"].ToString();
        //                        string configFieldName = fieldRow["Name"].ToString();
        //                        if (view.Fields.ContainsKey(configFieldName))
        //                        {
        //                            Field field = view.Fields[configFieldName];

        //                            values = new Dictionary<string, object>();

        //                            values.Add("Required", field.GetDbRequired());
        //                            int len = ((Durados.ColumnField)field).DataColumn.MaxLength;
        //                            if (field.FieldType == FieldType.Column && ((Durados.ColumnField)field).DataColumn.DataType.Equals(typeof(string)) && (len > 500 || len == -1))
        //                            {
        //                                values.Add("DataType", "LongText");
        //                                values.Add("DisplayFormat", "MultiLines");
        //                                values.Add("TextHtmlControlType", "TextArea");
        //                                values.Add("ColSpanInDialog", "2");
        //                            }
        //                            configAccess.Edit(configField, values, configFieldPK, null, null, null, null);
        //                        }

        //                        string fieldType = fieldRow["FieldType"].ToString();
        //                        if (fieldType == "Parent")
        //                        {
        //                            if (!SqlAccess.NoSysIndex)
        //                            {
        //                                try
        //                                {
        //                                    if (!fieldRow.IsNull("RelatedViewName"))
        //                                    {
        //                                        string relatedViewName = fieldRow["RelatedViewName"].ToString();
        //                                        Durados.View relatedView = (Durados.View)db.Views[relatedViewName];
        //                                        int rowCount = sqlAccess.RowCount(relatedView);
        //                                        if (rowCount > autoCompleteLimit)
        //                                        {
        //                                            values = new Dictionary<string, object>();
        //                                            values.Add("ParentHtmlControlType", "Autocomplete");
        //                                            values.Add("DisplayFormat", "AutoCompleteStratWith");
        //                                            values.Add("AutocompleteFilter", true);
        //                                            configAccess.Edit(configField, values, configFieldPK, null, null, null, null);
  
        //                                        }
        //                                    }
        //                                }
        //                                catch { }
        //                            }
        //                        }
                                

        //                    }
        //                }
        //            }

        //            //if (AutoGeneration())
        //            //{
        //            //    SetMainInterestsMenu();

        //            //    SetFirstMenus();
        //            //}

        //            Initiate();
        //        }
        //        if (string.IsNullOrEmpty(errorMessages))
        //        {
        //            return RedirectToAction("Index", new { viewName = viewName, ajax = true, guid = guid });
        //        }
        //        else
        //        {
        //            return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", (added > 0 ? (added + message) : string.Empty) + errorMessages);

        //        }
        //    }
        //    else
        //    {
        //        return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", Map.Database.Localizer.Translate("Session expired. No changes were made. Please try again."));
                
        //    }
        //}

        

        private string GetNewViewMenuPK(ConfigAccess configAccess, Database configDatabase)
        {
            string pk = configAccess.GetMenuPK("New Views", configDatabase.ConnectionString);
            if (pk != null)
                return pk;
            View menuView = (View)configDatabase.Views["Menu"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "New Views");
            values.Add("WorkspaceID", Map.Database.GetDefaultWorkspace().ID);
            values.Add("Ordinal", "1000");
            //values.Add("Root", false);
            System.Data.DataRow row = menuView.Create(values, null, null, null, null, null);

            return menuView.GetPkValue(row);
        }

        public FileResult LoadERDXML()
        {
            if (!IsAllow() && !Maps.IsSuperDeveloper(null))
                throw new DuradosAccessViolationException();


            //string fileName = Server.MapPath(Maps.ConfigPath + "" + Maps.GetCurrentAppName() + ".ERD.xml");
            string fileName = Maps.GetConfigPath(Maps.GetCurrentAppName() + ".ERD.xml");

            return File(fileName, "application/xml");
        }


        [ValidateInput(false)]
        public  void ERDSaveState(string p)
        {
            if (!string.IsNullOrEmpty(p))
            {
                ERDHelper erd = new ERDHelper();
                erd.SaveERDState(p, Map.Database);

            }
            else
                Map.Database.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "SaveERDState", null, 20, "xml was empty.");
            
        }
        public  ActionResult ERD()
        {
            return View();
           
        }


        protected override void CommitSwitch(View view)
        {
            if (Map.Database.AutoCommit)
            {
                RefreshConfigCache();
            }
        }

        protected class ChartPosition
        {
            public string Id { get; set; }

            //public string Align { get; set; }

            public int Ordinal { get; set; }

            public int Column { get; set; }

        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public void SetChartData(string data)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var oData = jss.Deserialize<IEnumerable<ChartPosition>>(data);

            ConfigAccess configAccess = new ConfigAccess();

            View configChartView = (View)Map.GetConfigDatabase().Views["Chart"];

            foreach (var chartData in oData)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                //values.Add("Align", chartData.Align);
                values.Add("Column", chartData.Column);

                values.Add("Ordinal", chartData.Ordinal);
                configAccess.Edit(configChartView, values, chartData.Id, null, null, null, null);

            }

            Initiate();
            
        }

        protected class DashboardDefinition
        {
            public string Id { get; set; }

            public int Columns { get; set; }

        }
        
        
        public ActionResult EditDashboard(int id, int columns)
        {
            ConfigAccess configAccess = new ConfigAccess();
            View configDashboardView = (View)Map.GetConfigDatabase().Views["MyCharts"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Columns", columns);

            configAccess.Edit(configDashboardView, values, id.ToString(), null, null, null, null);
            ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

            MyCharts dashboard = Map.Database.Dashboards[id];
            dashboard.Columns = columns;

            //string url = "ChartsDashboard" ChartHelper.GetDashboardUrlWithQueryString(id);
            return RedirectToAction("ChartsDashboard", "Home",  Request.QueryString.ToRouteValues( new{ id = id }));
            

        }
        
        private JsonResult GetMenuSelectList(string workspaceID)
        {
            ConfigAccess configAccess = new ConfigAccess();

            Dictionary<string, string> selectOptions = configAccess.GetMenuSelectOptions(workspaceID, Map.GetConfigDatabase().ConnectionString);

            Durados.Web.Mvc.UI.Json.SelectList selectList = Durados.Web.Mvc.UI.Json.SelectList.GetSelectList(selectOptions);

            return Json(selectList);


        }

       
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult PreviewEdit(string viewName, string  property, string value,string pk, string guid)
        {
            try
            {
              
                //if (Request.QueryString["previewMode"] != null)
                //{

                    PreviewHelper previewHelper = new PreviewHelper();
                    Durados.Web.Mvc.View view = null;
                    previewHelper.EditPreview(viewName, property,value, pk, guid, out view);

                    if (property == "Layout")
                    {
                        LayoutType layoutType = (LayoutType)Enum.Parse(typeof(LayoutType), value);

                        switch (layoutType)
                        {
                            // BasicGrid,table,excel,excel thre checkbox false
                            // BasicDashboard,Dashboard,group,group,200,400,false false false true
                            // BasicPreview,Preview ,group,group,200,400,false false false true
                            // GridGroupFilter,table ,group,group,200,400,false false false true
                            // DashboardVerticalShort,Dashboard,group,group,400,200,false false false true

                            case LayoutType.BasicGrid:
                                view.DataDisplayType = DataDisplayType.Table;
                                view.FilterType = FilterType.Excel;
                                view.SortingType = SortingType.Excel;
                                view.EnableDashboardDisplay = false;
                                view.EnableTableDisplay = false;
                                view.EnablePreviewDisplay = false;
                                view.GroupFilterDisplayLabel = true;
                                view.DashboardWidth = "200";
                                view.DashboardHeight = "400";

                                break;
                            case LayoutType.BasicDashboard:
                                
                                view.DataDisplayType = DataDisplayType.Dashboard;
                                view.FilterType = FilterType.Group;
                                view.SortingType = SortingType.Group;
                                view.EnableDashboardDisplay = false;
                                view.EnableTableDisplay = false;
                                view.EnablePreviewDisplay = false;
                                view.GroupFilterDisplayLabel = true;
                                view.DashboardWidth = "400";
                                view.DashboardHeight = "200";


                                break;
                            case LayoutType.BasicPreview:
                                
                                view.DataDisplayType = DataDisplayType.Preview;
                                view.FilterType = FilterType.Group;
                                view.SortingType = SortingType.Group;
                                view.EnableDashboardDisplay = false;
                                view.EnableTableDisplay = false;
                                view.EnablePreviewDisplay = false;
                                view.GroupFilterDisplayLabel = true;
                                view.DashboardWidth = "200";
                                view.DashboardHeight = "400";

                                break;
                            case LayoutType.GridGroupFilter:

                                view.DataDisplayType = DataDisplayType.Table;
                                view.FilterType = FilterType.Group;
                                view.SortingType = SortingType.Group;
                                view.EnableDashboardDisplay = false;
                                view.EnableTableDisplay = false;
                                view.EnablePreviewDisplay = false;
                                view.GroupFilterDisplayLabel = true;
                                view.DashboardWidth = "200";
                                view.DashboardHeight = "400";

                                break;
                            case LayoutType.DashboardVerticalShort:

                                view.DataDisplayType = DataDisplayType.Dashboard;
                                view.FilterType = FilterType.Group;
                                view.SortingType = SortingType.Group;
                                view.EnableDashboardDisplay = false;
                                view.EnableTableDisplay = false;
                                view.EnablePreviewDisplay = false;
                                view.GroupFilterDisplayLabel = true;
                                view.DashboardWidth = "200";
                                view.DashboardHeight = "400";
                                break;
                            case LayoutType.Custom:
                                break;

                            default:
                                break;
                        }

                        HandleViewMode(view, guid);
                    }
                //}
                return Json("success");
            }
            catch (Exception exception)
            {
                //return Json(new Json.JsonResult(exception.Message, false, "Unexpected"));
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return Json("$$error$$" + FormatExceptionMessage(viewName, "EditOnly", exception));

            }
        }

        public ActionResult MenuOrganizer()
        {
            return RedirectToAction("IndexPage", new { viewName = "MenuOrganizerView", guid = "menuOrganizer_guid_", menu = "off" });
        }

        public ActionResult ChangeWorkspace(int? workspaceId)
        {
            try
            {
                if (workspaceId.HasValue && Map.Database.Workspaces.ContainsKey(workspaceId.Value))
                    Map.Session["workspaceId"] = workspaceId.Value;
                else
                    return Json(new { Failure = true });

                return PartialView("~/Views/Shared/Controls/Page/WorkspaceMenuManager.ascx", Map.Database.Workspaces[workspaceId.Value]);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception, Failure = true });
            }
        }

        public ActionResult GetPageSettings(string viewName)
        {
            Durados.Web.Mvc.UI.DataActionFields model = new Durados.Web.Mvc.UI.DataActionFields() { DataAction = Durados.DataAction.InlineEditing };

            model.Guid = PreviewHelper.PageGuid;
            model.View = (Durados.Web.Mvc.View)Map.GetConfigDatabase().Views[viewName];
            if (viewName == "View")
            {
                model.Fields = ((Durados.Web.Mvc.Config.View)model.View).SettingsFields;
            }
            else if (viewName == "Page")
            {
                model.Fields = new List<Field>();

                Page page = Map.Database.Pages[Convert.ToInt32(Request.QueryString["Pk"])];

                if (page == null)
                {
                    throw new DuradosException();
                }

                View pageView = (View)Map.GetConfigDatabase().Views["Page"];
                switch (page.PageType)
                {
                    case PageType.Content:
                        break;

                    case PageType.External:
                        model.Fields.Add(pageView.Fields["ExternalNewPage"]);
                        model.Fields.Add(pageView.Fields["NewTab"]);
                        model.Fields.Add(pageView.Fields["Target"]);
                        break;
                    case PageType.IFrame:
                        model.Fields.Add(pageView.Fields["ExternalPage"]);
                        model.Fields.Add(pageView.Fields["Scroll"]);
                        model.Fields.Add(pageView.Fields["Width"]);
                        model.Fields.Add(pageView.Fields["Height"]);
                        break;
                    case PageType.ReportingServices:
                        model.Fields.Add(pageView.Fields["ReportName"]);
                        model.Fields.Add(pageView.Fields["ReportDisplayName"]);
                        break;

                    default:
                        break;
                }
            }
            else if (viewName == "MyCharts")
            {
                model.Fields = new List<Field>();
            }

            return PartialView("~/Views/Shared/Controls/Page/PageSettings.ascx", model);
        }

        public ActionResult GetWorkspaceMenu()
        {
            return PartialView("~/Views/Shared/Controls/Page/WorkspaceSpecialMenu.ascx");
        }

        public ActionResult PagesManager()
        {
            return PartialView("~/Views/Shared/Controls/Page/PagesManager.ascx");
        }

        public ActionResult Pages()
        {
            return View();
        }

        protected virtual Nestable[] GetNestables(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            return jss.Deserialize<Nestable[]>(json);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SavePages(string workspaceId, string json)
        {
            Workspace workspace = null;
            try
            {
                Dictionary<string, SpecialMenu> specialMenus = new Dictionary<string, SpecialMenu>();

                Nestable[] nestables = GetNestables(json);

                Dictionary<int, SpecialMenu> specialMenuList = new Dictionary<int, SpecialMenu>();

                workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

                foreach (SpecialMenu menu in workspace.SpecialMenus.Values)
                {
                    LoadSpecialMenuList(menu, specialMenuList);
                }

                // workspace.SpecialMenus.Clear();

                LoadSpecialMenuValidation(nestables, specialMenus, specialMenuList, null, workspaceId);
                ordinal = 0;
                specialMenus = new Dictionary<string, SpecialMenu>();
                LoadSpecialMenu(nestables, specialMenus, specialMenuList, null, workspaceId);

                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                workspace.SpecialMenus = specialMenus;

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception.Message, Success = false });
            }
        }

        string pageAlreadyExistsMessage = "In this level a page {0} with the same name already exists. Please change the name of one of them.";

        private void LoadSpecialMenuValidation(Nestable[] nestables, Dictionary<string, SpecialMenu> menus, Dictionary<int, SpecialMenu> specialMenuList, SpecialMenu parent, string workspaceId)
        {
            foreach (Nestable nestable in nestables)
            {
                SpecialMenu menu = specialMenuList[Convert.ToInt32(nestable.id)];
                //menu.Menus.Clear();
                if (menus.ContainsKey(menu.Name))
                {
                    throw new DuradosException(string.Format(pageAlreadyExistsMessage, menu.Name));
                }
                menus.Add(menu.Name, menu);
            }
        }

        int ordinal = 0;
        private void LoadSpecialMenu(Nestable[] nestables, Dictionary<string, SpecialMenu> menus, Dictionary<int, SpecialMenu> specialMenuList, SpecialMenu parent, string workspaceId)
        {
            foreach (Nestable nestable in nestables)
            {
                ordinal += 10;
                SpecialMenu menu = specialMenuList[Convert.ToInt32(nestable.id)];
                menu.Menus.Clear();
                menu.Ordinal = ordinal;
                if (menus.ContainsKey(menu.Name))
                {
                    throw new DuradosException(string.Format(pageAlreadyExistsMessage, menu.Name));
                }
                menus.Add(menu.Name, menu);

                Dictionary<string, object> values = new Dictionary<string, object>();
                if (parent != null)
                {
                    values.Add("Menus_Parent", parent.ID);
                    values.Add("SpecialMenus_Parent", "");
                }
                else
                {
                    values.Add("SpecialMenus_Parent", workspaceId);
                    values.Add("Menus_Parent", "");
                }
                values.Add("Ordinal", ordinal);

                Map.GetConfigDatabase().Views["SpecialMenu"].Edit(values, nestable.id, null, null, null, null);

                if (nestable.children != null)
                {
                    LoadSpecialMenu(nestable.children, menu.Menus, specialMenuList, menu, workspaceId);
                }
            }
        }

        private void LoadSpecialMenuList(SpecialMenu menu, Dictionary<int, SpecialMenu> specialMenuList)
        {
            if (!specialMenuList.ContainsKey(menu.ID))
            {
                specialMenuList.Add(menu.ID, menu);
                foreach (SpecialMenu child in menu.Menus.Values)
                {
                    LoadSpecialMenuList(child, specialMenuList);
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddPage(string viewName, string workspace, string parentMenu, bool isNew, bool isExcel, string menuName, string type, string editableTable)
        {
            try
            {
                LinkSubType linkSubType = (LinkSubType)Enum.Parse(typeof(LinkSubType), type);
                string url = null;

                View view = null;
                MyCharts dashboard = null;
                LinkType linkType = LinkType.View;
                string pk = string.Empty;
                string configViewName = string.Empty;


                switch (linkSubType)
                {
                    case LinkSubType.Cards:
                    case LinkSubType.Preview:
                    case LinkSubType.Grid1:
                    case LinkSubType.Grid2:
                        url = AddView(linkSubType, workspace, isNew, isExcel, menuName, viewName, editableTable, out view);
                        pk = new ConfigAccess().GetViewPK(view.Name, Map.GetConfigDatabase().ConnectionString);
                        configViewName = "View";
                        break;

                    case LinkSubType.Charts:
                        linkType = LinkType.MyCharts;
                        url = AddDashboard(linkSubType, workspace, isNew, menuName, viewName, out dashboard);//"/Home/Charts";

                        configViewName = "MyCharts";
                        break;

                    case LinkSubType.EmbeddedIFrame:
                    case LinkSubType.EmbeddedLink:
                    case LinkSubType.HtmlCustom:
                    case LinkSubType.ReportingServices:
                        if (linkSubType == LinkSubType.ReportingServices)
                            linkType = LinkType.Report;
                        else
                            linkType = LinkType.Page;
                        configViewName = "Page";
                        url = AddPage(linkSubType, workspace, parentMenu, menuName, out pk);
                        break;


                    default:
                        break;
                }

                string id = AddMenu(view == null ? pk : view.Name, workspace, parentMenu, menuName, url, linkType);

                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                return Json(new { Success = true, Url = url, id = id, LinkType = linkType.ToString(), pk = pk, configViewName = configViewName });
            }
            catch (DuradosException exception)
            {
                return Json(new { Exception = exception.Message, Success = false });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = Database.GeneralErrorMessage, Success = false });
            }
        }

        private string AddDashboard(LinkSubType linkSubType, string workspace, bool isNew, string menuName, string dashboardId, out MyCharts dashboard)
        {
            int id = -1;
            if (isNew)
            {
                return AddNewDashboard(linkSubType, workspace, menuName, menuName, out dashboard);
            }
            else
            {
                //int id;
                int.TryParse(dashboardId, out id);
                if (id > 0 && Map.Database.Dashboards.ContainsKey(id))
                {
                    dashboard = Map.Database.Dashboards[id];
                    return "/Home/ChartsDashboard?Id=" + id.ToString();

                }
                //else
                //{
                //    view = AddViewAndAncestors(menuName, editableTable);

                //}
                //ConfigView(linkSubType, workspace, menuName, menuName, view);
                // return "/Home/ChartsDashboard?Id=" + chartId;
                dashboard = null;
                return "/Home/Charts";
            }
        }

        private string AddNewDashboard(LinkSubType linkSubType, string workspace, string menuName, string dashboardName, out MyCharts dashboard)
        {
            string id = Map.CreateDashboard(dashboardName);

            dashboard = new MyCharts(Database);
            dashboard.Name = menuName;
            Map.Database.Dashboards.Add(Convert.ToInt32(id), dashboard);

            Map.AddNewChartToDashboard(id, 1, 1, GetEmptyChartsValues(), false);
            Map.AddNewChartToDashboard(id, 2, 2, GetEmptyChartsValues(), false);
            ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

            //dashboard.Charts.Add(Convert.ToInt32(chart1Id), new Chart());
            //dashboard.Charts.Add(Convert.ToInt32(chart2Id), new Chart());

            return Map.Database.GetDashboardUrl() + id.ToString();

        }

        private Dictionary<string, object> GetEmptyChartsValues()
        {
            Dictionary<string, object> chartValues = new Dictionary<string, object>();
            chartValues.Add("WorkspaceID", MenuHelper.GetCurrentWorkspace().ID.ToString());
            chartValues.Add("Precedent", false);
            chartValues.Add("Height", 340);
            return chartValues;
        }

        public string AddDashboard(string dashboardName)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", dashboardName);
            values.Add("Dashboards_Parent", 0);

            Durados.Database configDatabase = Map.GetConfigDatabase();
            View view = (View)configDatabase.Views["MyCharts"];
            DataRow row = view.Create(values, null, null, null, null, null);
            string id = view.GetPkValue(row);
            return id;
        }

      
        private string AddMenu(string viewName, string workspaceId, string parentMenu, string menuName, string url, LinkType linkType)
        {
            Workspace workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

            View view = (View)Map.GetConfigDatabase().Views["SpecialMenu"];
            int ordinal = 0;
            bool hasParent = !string.IsNullOrEmpty(parentMenu) && workspace.GetSpecialMenu(Convert.ToInt32(parentMenu)) != null;
            SpecialMenu parent = null;
            if (hasParent)
            {
                parent = workspace.GetSpecialMenu(Convert.ToInt32(parentMenu));
                SpecialMenu first = parent.Menus.Values.OrderBy(s => s.Ordinal).FirstOrDefault();
                if (first != null)
                {
                    ordinal = first.Ordinal - 10;
                }
            }
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("WorkspaceID", workspaceId);
            if (hasParent)
            {
                values.Add("Menus_Parent", parentMenu);
            }
            else
            {
                values.Add("SpecialMenus_Parent", workspaceId);
            }
            values.Add("Name", menuName);
            values.Add("ViewName", viewName);
            values.Add("Url", url);
            values.Add("Ordinal", ordinal);
            values.Add("LinkType", linkType.ToString());


            DataRow row = view.Create(values, null, null, null, null, null);
            string id = view.GetPkValue(row);
            if (workspace.SpecialMenus.ContainsKey(menuName))
                throw new DuradosException("A page with this name already exists. Please enter a different name.");
            workspace.SpecialMenus.Add(menuName, new SpecialMenu() { ID = Convert.ToInt32(id), Url = url, Parent = parent, Name = menuName, ViewName = viewName, WorkspaceID = Convert.ToInt32(workspaceId), LinkType = linkType, Ordinal = ordinal });

            return id;
        }

        public JsonResult DeleteChart(string chartId)
        {
            View view = (View)Map.GetConfigDatabase().Views["Chart"];
            view.Delete(chartId, null, null, null);
            RefreshConfigCache();

            return Json(new { success = true });
        }

        public ActionResult AddChart(string dashboardId)
        {
            int Id = Convert.ToInt32(dashboardId);
            int ordinal = 0;
            if (Map.Database.Dashboards.ContainsKey(Id))
            {
                MyCharts dashboard = Map.Database.Dashboards[Id];
                if (dashboard.Charts.Count > 0)
                {
                    ordinal = dashboard.Charts.Values.Min(r => r.Ordinal) - 10;
                }
            }
            string newChartId = Map.AddNewChartToDashboard(dashboardId, ordinal, 1, GetEmptyChartsValues(), true);
            //return Json(new { Success = true, chartId = newChartId });
            ViewData["dashboardId"] = dashboardId;
            //string url = ChartHelper.GetDashboardUrlWithQueryString(Id);
            //return RedirectToAction(url, "Home");
            return RedirectToAction("ChartsDashboard", "Home", Request.QueryString.ToRouteValues(new { id = dashboardId }));
            
            //  ("IndexPage", new { viewName = "MenuOrganizerView", guid = "menuOrganizer_guid_", menu = "off" });
        }

        private string AddPage(LinkSubType linkSubType, string workspace, string parentMenu, string menuName, out string pk)
        {
            View view = (View)Map.GetConfigDatabase().Views["Page"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            PageType pageType = PageType.Content;
            switch (linkSubType)
            {
                case LinkSubType.HtmlCustom:
                    pageType = PageType.Content;
                    break;

                case LinkSubType.EmbeddedIFrame:
                    pageType = PageType.IFrame;
                    break;

                case LinkSubType.EmbeddedLink:
                    pageType = PageType.External;
                    break;

                case LinkSubType.ReportingServices:
                    pageType = PageType.ReportingServices;
                    break;

                default:
                    break;
            }
            values.Add("PageType", pageType.ToString());
            values.Add("Pages_Parent", 0);
            values.Add("WorkspaceID", workspace);
            values.Add("Title", menuName);
            DataRow row = view.Create(values, null, null, null, null, null);
            pk = view.GetPkValue(row);
            int id = Convert.ToInt32(pk);
            Map.Database.Pages.Add(id, new Page() { ID = id, PageType = pageType });
            return "/Home/Page?pageId=" + pk;
        }

        private string AddView(LinkSubType linkSubType, string workspace, bool isNew, bool isExcel, string menuName, string viewName, string editableTable, out View view)
        {
            // if (isExcel)
            //{
            //    return AddNewViewFromExcel(linkSubType, workspace, menuName, viewName, out view);
            //}
            //else
            if (isNew)
            {
                return AddNewView(linkSubType, workspace, menuName, viewName, out view);
            }
            else
            {
                if (!string.IsNullOrEmpty(viewName) && Map.Database.Views.ContainsKey(viewName))
                {
                    view = (View)Map.Database.Views[viewName];


                }
                else
                {
                    view = AddViewAndAncestors(viewName, editableTable);

                }
                ConfigView(linkSubType, workspace, menuName, viewName, view);
                return "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name;
            }
        }


        private View AddViewAndAncestors(string viewName, string editableTable)
        {
            string errorMessage = string.Empty;
            DataView dataView = Map.GetSchemaEntities();
            List<string> pkList = new List<string>();

            IDataTableAccess sa = DataAccessHelper.GetDataTableAccess(Map.connectionString);
            SqlSchema sqlSchema = sa.GetNewSqlSchema();

            HashSet<string> ancestors = sqlSchema.GetMyAncestors(viewName, Map.connectionString);

            foreach (System.Data.DataRowView row in dataView)
            {
                if (ancestors.Contains(row["Name"].ToString()))
                {
                    pkList.Add(row["Name"].ToString());
                }
                if (row["Name"].ToString().Equals(viewName) && !string.IsNullOrEmpty(editableTable))
                {
                    row[EditableTableName] = editableTable;
                }
            }
            string pks = pkList.ToArray().Delimited();
            Map.Database.AddViews(pks, dataView, false, out errorMessage);

            if (!Map.Database.Views.ContainsKey(viewName))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    throw new DuradosException(Database.GeneralErrorMessage);
                }
                else
                {
                    throw new DuradosException(errorMessage);
                }
            }

            return (View)Map.Database.Views[viewName];
        }

        private void ConfigView(LinkSubType linkSubType, string workspace, string menuName, string viewName, View view)
        {
            int workspaceId = Convert.ToInt32(workspace);
            viewName = view.Name;
            view.WorkspaceID = workspaceId;
            view.DisplayName = menuName;

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("WorkspaceID", workspace);
            //values.Add("DisplayName", menuName);

            switch (linkSubType)
            {
                case LinkSubType.Preview:
                    view.DataDisplayType = DataDisplayType.Preview;
                    values.Add("DataDisplayType", DataDisplayType.Preview.ToString());

                    break;
                case LinkSubType.Cards:
                    view.DataDisplayType = DataDisplayType.Dashboard;
                    values.Add("DataDisplayType", DataDisplayType.Dashboard.ToString());

                    break;

                case LinkSubType.Grid1:
                case LinkSubType.Grid2:
                    view.DataDisplayType = DataDisplayType.Table;
                    values.Add("DataDisplayType", DataDisplayType.Table.ToString());

                    if (linkSubType == LinkSubType.Grid1)
                    {
                        view.Skin = GetDefaultSkinType(workspaceId);
                        values.Add("Skin", view.Skin.ToString());

                    }
                    else
                    {
                        view.Skin = SkinType.DefaultSkin;
                        values.Add("Skin", SkinType.DefaultSkin.ToString());

                    }
                    break;

                default:
                    break;
            }

            string viewPK = (new Durados.DataAccess.ConfigAccess()).GetViewPK(view.Name, Map.GetConfigDatabase().ConnectionString);
            Map.GetConfigDatabase().Views["View"].Edit(values, viewPK, null, null, null, null);
        }

        private SkinType GetDefaultSkinType(int workspaceId)
        {

            Workspace workspace = Map.Database.GetWorkspace(workspaceId);
            if (workspace == null)
                return SkinType.Skin6;

            var workspaceViews = Map.Database.Views.Values.Where(v => v.WorkspaceID == workspaceId && !v.SystemView);

            if (workspaceViews.Count() == 0)
            {
                workspaceViews = Map.Database.Views.Values.Where(v => !v.SystemView);
            }

            if (workspaceViews.Count() == 0)
                return SkinType.Skin6;

            IEnumerable<SkinType> top1 = workspaceViews
            .GroupBy(v => ((View)v).Skin)
            .OrderByDescending(g => g.Count())
            .Take(1)
            .Select(g => g.Key);

            if (top1 != null && top1.Count() > 0)
                return top1.FirstOrDefault();

            return SkinType.Skin6;
        }

        private string AddNewView(LinkSubType linkSubType, string workspace, string menuName, string viewName, out View view)
        {
            DataTable dt = new DataTable(menuName);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("GridEditable", true);
            values.Add("GridEditableEnabled", true);
            int x;
            if (int.TryParse(workspace, out x) && Map.Database.Workspaces.ContainsKey(x))
                values.Add("WorkspaceID", workspace);
            else
                values.Add("WorkspaceID", Map.Database.GetDefaultWorkspace().ID.ToString());
            values.Add("Menu_Parent", menuName);


            view = CreateView(dt);

            return view.GetIndexUrl();

        }

        public ActionResult GetDataTypeDialog()
        {
            return PartialView("~/Views/Shared/Controls/DataType.ascx");
        }

        public ActionResult AddPage()
        {
            try
            {
                AddPage addPage = new AddPage();

                addPage.Content = GetAddPageContent();
                addPage.EntityTable = GetAddPageEntityTable();
                addPage.Tables = GetAddPageTables();

                addPage.Dashboards = GetAddPageDashboards();
                return PartialView("~/Views/Shared/Controls/Page/AddPage.ascx", addPage);
            }
            catch (Exception exception)
            {
               Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", exception.Message);
            }
        }

        protected virtual Dictionary<int, string> GetAddPageDashboards()
        {
            return Map.Database.Dashboards.ToDictionary(k => k.Key, k => k.Value.Name);
        }
        protected virtual List<string> GetAddPageTables()
        {
            IDataTableAccess sa = DataAccessHelper.GetDataTableAccess(Map.connectionString);
            SqlSchema sqlSchema = sa.GetNewSqlSchema();
            return sqlSchema.GetTableNames(Map.connectionString);
        }

        protected virtual string GetAddPageContent()
        {
            string s = CmsHelper.GetHtml("AddPageContent");
            return string.IsNullOrEmpty(s) ? "Please select the page type and layout that you want. Name your page in the space provided and click Next. Your new page will immediately open and appear in your menu. You can reorder and rename in Pages." : s;
        }

        protected virtual DataView GetAddPageEntityTable()
        {
            System.Data.DataView dataView = null;

            dataView = Map.GetSchemaEntities();

            DataTable table = dataView.Table;
            DataColumn column = table.Columns.Add("DisplayName");

            AddClones(table);

            dataView.Sort = "Name";



            foreach (DataRow row in table.Rows)
            {
                string entityName = row["Name"].ToString();

                if (Map.Database.Views.ContainsKey(entityName))
                {
                    row[column] = Map.Database.Views[entityName].DisplayName;

                }
            }

            foreach (View view in Map.Database.Views.Values.Where(v => v.SystemView && v.Name.StartsWith(Durados.Database.SystemRelatedViewPrefix)))
            {
                DataRow row = table.NewRow();
                row["Name"] = view.Name;
                row["EntityType"] = "Table";
                row["DisplayName"] = view.DisplayName;

                table.Rows.Add(row);
            }

            return dataView;
        }

        private void AddClones(DataTable table)
        {
            foreach (View clone in Map.Database.Views.Values.Where(v => v.IsCloned))
            {

                string entityName = clone.DataTable.TableName;
                string entityType = table.Rows.Find(entityName)["EntityType"].ToString();

                DataRow row = table.NewRow();
                row["Name"] = clone.Name;
                row["EntityType"] = entityType;
                row["DisplayName"] = clone.DisplayName;

                 try
                {
                    table.Rows.Add(row);
                }
                catch { }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangePageName(string workspaceId, string menuId, string name)
        {
            try
            {
                Workspace workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

                SpecialMenu specialMenu = workspace.GetSpecialMenu(Convert.ToInt32(menuId));

                if ((specialMenu.Parent == null && workspace.SpecialMenus.ContainsKey(name)) || (specialMenu.Parent != null && specialMenu.Parent.Menus.ContainsKey(name)))
                {
                    return Json(new { Exception = string.Format(pageAlreadyExistsMessage, name), Success = false });
                }
                
                if (specialMenu.Parent == null)
                {
                    workspace.SpecialMenus.Remove(specialMenu.Name);

                    specialMenu.Name = name;

                    workspace.SpecialMenus.Add(specialMenu.Name, specialMenu);
                }
                else
                {
                    specialMenu.Parent.Menus.Remove(specialMenu.Name);

                    specialMenu.Name = name;

                    specialMenu.Parent.Menus.Add(specialMenu.Name, specialMenu);
                }

                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Name", name);
                Map.GetConfigDatabase().Views["SpecialMenu"].Edit(values, menuId, null, null, null, null);


                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception.Message, Success = false });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeHideFromMenu(string workspaceId, string menuId, bool hideFromMenu)
        {
            try
            {
                Workspace workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

                SpecialMenu specialMenu = workspace.GetSpecialMenu(Convert.ToInt32(menuId));

                specialMenu.HideFromMenu = hideFromMenu;


                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("HideFromMenu", hideFromMenu);
                Map.GetConfigDatabase().Views["SpecialMenu"].Edit(values, menuId, null, null, null, null);


                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception.Message, Success = false });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeHomepage(string workspaceId, string menuId, bool homepage)
        {
            try
            {
                Workspace workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

                if (homepage)
                    workspace.HomePage = Convert.ToInt32(menuId);
                else
                    workspace.HomePage = 0;

                Dictionary<string, object> values = new Dictionary<string, object>();
                if (homepage)
                    values.Add("HomePage", menuId);
                else
                    values.Add("HomePage", 0);

                Map.GetConfigDatabase().Views["Workspace"].Edit(values, workspaceId, null, null, null, null);


                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception.Message, Success = false });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeletePage(string workspaceId, string menuId)
        {
            try
            {
                Workspace workspace = Map.Database.Workspaces[Convert.ToInt32(workspaceId)];

                SpecialMenu specialMenu = workspace.GetSpecialMenu(Convert.ToInt32(menuId));

                if (specialMenu.Parent == null)
                {
                    workspace.SpecialMenus.Remove(specialMenu.Name);
                }
                else
                {
                    specialMenu.Parent.Menus.Remove(specialMenu.Name);
                }

                Map.GetConfigDatabase().Views["SpecialMenu"].Delete(menuId, null, null, null);


                Durados.DataAccess.ConfigAccess.SaveConfigDataset(Map.GetConfigDatabase().ConnectionString, Map.Logger);

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);

                return Json(new { Exception = exception.Message, Success = false });
            }
        }

        public JsonResult GetJson(string viewName)
        {
            View view = GetView(viewName);
            
            return Json(view.GetJson());
        }
        public string DeleteApp(string appId, string appName)
        {
            if (!(Map is DuradosMap && Maps.IsSuperDeveloper(null)))
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(),null,"Accessd by non-super developer in a spesific app",null,1,null,DateTime.Now);
                return "This action is not allowd";
            }
            int id;
            int.TryParse(appId, out id);
            if (id <= 0 && string.IsNullOrEmpty(appName))
                return "AppId or App Name could not be parsed.";
            
            Durados.Web.Mvc.Infrastructure.ProductMaintenance productMaintenece = new Infrastructure.ProductMaintenance();
            try
            {
                if (id > 0)
                    productMaintenece.RemoveApp(id);
                else
                    productMaintenece.RemoveApp(appName);
                
                return "App number " + ((id ==0)?appName:appId ) + " was deleted";
            }
            catch  (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, ex, 1, null);
                return "An error occuered while deleteing te app ,message: " +ex.Message;
            }
        }
        public string DeleteAppView()
        {
            if (!(Map is DuradosMap && Maps.IsSuperDeveloper(null)))
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, "Accessd by non-super developer in a spesific app", null, 1, null, DateTime.Now);
                return "This action is not allowd";
            }
            Durados.Web.Mvc.Infrastructure.ProductMaintenance productMaintenece = new Infrastructure.ProductMaintenance();
            try
            {
                return productMaintenece.RemoveApps(null, null);

            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, ex, 1, null);
                return "An Error occured while tring to delete apps:" +ex.Message;
            }
        }
    }
}

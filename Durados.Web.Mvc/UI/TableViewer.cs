using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Web.Mvc.UI.Helpers;

using Durados;
using Durados.DataAccess;

namespace Durados.Web.Mvc.UI
{
    public class TableViewer : ITableViewer, ITableConverter
    {
        public DataView DataView { get; set; }
        public Dictionary<string, CheckListTableViewer> checkListTableViewers;
        private Gantt gantt = null;

        public Gantt Gantt
        {
            get
            {
                if (gantt == null)
                    gantt = GetNewGantt();
                return gantt;
            }
        }

        protected virtual Gantt GetNewGantt(){
            return new Gantt();
        }


        public TableViewer()
        {
            this.checkListTableViewers = new Dictionary<string, CheckListTableViewer>();
        }

        public virtual string GetFieldValue(Durados.Field field, DataRow row)
        {
            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
            {
                if (DataView == null)
                    return string.Empty;
                if (!checkListTableViewers.ContainsKey(field.Name))
                {
                    checkListTableViewers.Add(field.Name, new CheckListTableViewer((ChildrenField)field, DataView));
                }
                Dictionary<string,string> values = checkListTableViewers[field.Name].GetValue(field.View.GetPkValue(row));
                if (values == null)
                    return string.Empty;
                string[] valuesArray = values.Keys.ToArray();
                return  valuesArray.Delimited();
                
            }
            else if (field.FieldType == FieldType.Column && ((ColumnField)field).Encrypted && ((ColumnField)field).SpecialColumn == SpecialColumn.Password)
            {
                return field.View.Database.EncryptedPlaceHolder;
            }
            else if (field.FieldType == Durados.FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).DataColumn.DataType != typeof(DateTime) && ((Durados.Web.Mvc.ColumnField)field).DataColumn.DataType != typeof(bool) && !row.IsNull(field.Name))
            {
                return row[field.Name].ToString();
            }
            else
            {
                return field.GetValue(row);
            }
        }

        public virtual string GetFieldDisplayValue(Durados.Field field, DataRow row, bool forExport)
        {
            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
            {
                if (DataView == null)
                    return string.Empty;
                if (!checkListTableViewers.ContainsKey(field.Name))
                {
                    checkListTableViewers.Add(field.Name, new CheckListTableViewer((ChildrenField)field, DataView));
                }
                Dictionary<string, string> values = checkListTableViewers[field.Name].GetValue(field.View.GetPkValue(row));
                if (values == null)
                    return string.Empty;
                string[] valuesArray = values.Values.ToArray();
                return valuesArray.Delimited();

            }
            else
            {
                string s = field.ConvertToString(row);
                if (!string.IsNullOrEmpty(s) && forExport && field.FieldType== FieldType.Column && ((ColumnField)field).Rich)
                    return Durados.Web.Mvc.Infrastructure.General.StripTags(s);
                else
                    return s;
            }
        }

        public virtual string GetElementForTableView(Durados.Field field, DataRow row, string guid)
        {
            return GetElementForTableView(field, row, guid, false);
        }

        public virtual string GetDisplayValues(Durados.Field field, DataRow row, bool ignoreChecklistLimit = false)
        {
            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
            {
                if (DataView == null)
                    return string.Empty;
                if (!checkListTableViewers.ContainsKey(field.Name))
                {
                    checkListTableViewers.Add(field.Name, new CheckListTableViewer((ChildrenField)field, DataView));
                }
                int limit = ignoreChecklistLimit ? 0 : ((ChildrenField)field).CheckListInTableLimit;
               
                Dictionary<string, string> values = checkListTableViewers[field.Name].GetValue(field.View.GetPkValue(row));
                if (values == null)
                {
                    return string.Empty;

                }
                string[] valuesArray = values.Values.ToArray();
                string title = valuesArray.Delimited();
                string display = title;

                if (limit > 0 && values.Count > limit)
                {
                    valuesArray = valuesArray.Take(limit).ToArray();
                    display = valuesArray.Delimited();
                }
                return display;
            }
            else
            {
                return field.ConvertToString(row);
            }
        }

        public virtual string GetElementForTableView(Durados.Field field, DataRow row, string guid, bool ignoreChecklistLimit)
        {
            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
            {
                if (DataView == null)
                    return string.Empty;
                if (!checkListTableViewers.ContainsKey(field.Name))
                {
                    checkListTableViewers.Add(field.Name, new CheckListTableViewer((ChildrenField)field, DataView));
                }
                int limit = ignoreChecklistLimit ? 0 : ((ChildrenField)field).CheckListInTableLimit;

                
                Dictionary<string, string> values=  checkListTableViewers[field.Name].GetValue(field.View.GetPkValue(row));
                if (values == null)
                {
                    return "<span></span>";
                    
                }
                string[] valuesArray = values.Values.ToArray();
                string title = valuesArray.Delimited();
                string display = title;

                if (limit > 0 && values.Count > limit)
                {
                    valuesArray = valuesArray.Take(limit).ToArray();
                    display = valuesArray.Delimited() + "...";
                }

                string span;

                if (limit > 0)
                {
                    span = "<span alt='" + title + "' title='" + title + "'>" + display + "</span>";
                }
                else
                {
                    span = display;
                }

                return span;
                
            }
            else
            {
                return field.GetElementForTableView(row, guid);
            }
        }

        public virtual string GetDisplayName(Durados.Field field, DataRow row, string guid)
        {
            return field.GetLocalizedDisplayName();
        }


        public virtual string GetDisplayName(Durados.View view, string guid)
        {
            return GetDisplayName(view, guid);
        }

        public virtual string GetDisplayName(View view, string guid)
        {
            return view.GetLocalizedDisplayName();
        }

        public virtual System.Data.DataView GetDataView(System.Data.DataView dataView, Durados.View view, string guid)
        {
            return GetDataView(dataView, (View)view, guid);
        }

        public virtual System.Data.DataView GetDataView(System.Data.DataView dataView, View view, string guid)
        {
            return dataView;
        }

        public virtual void HandleFilter(Json.Filter jsonFilter)
        {
        }

        public virtual void HandleFilter(object filter)
        {
            HandleFilter((Json.Filter)filter);
        }

        public virtual string GetCellWidthStyle(Field field, Json.CustomView cv, bool editable, string rowType) 
        { 
               string wStyle = string.Empty;            
               string minWidth = string.Empty;
                   
               if (field.TableCellMinWidth > 10) 
               {
                       minWidth = "min-width: " + field.TableCellMinWidth.ToString() + "px;";
               }
               if (cv != null && cv.Fields != null && cv.Fields.ContainsKey(field.Name) && cv.Fields[field.Name] != null)
               {  
                   
                   int width;
                   if (Int32.TryParse(cv.Fields[field.Name].width, out width))
                   {
                       if (width < 20) width = 20;
                       wStyle = "width: " + width + "px;";
                   }
               }
               else if (field.View.GridDisplayType != GridDisplayType.FitToWindowWidth)
               {
                   int width = 80;
                   wStyle = "width: " + width + "px;";
               }

            return minWidth + wStyle;
        }

        public virtual string GetCellAlignmentStyle(Field field, Json.CustomView cv, bool editable, string rowType)
        {
            if (field.TextAlignment == TextAlignment.inherit)
                return string.Empty;

            return "text-align:" + field.TextAlignment.ToString() + ";";
        }

        public virtual string GetCellHeightStyle(Field field, Json.CustomView cv, bool editable, string rowType)
        {
            string height = string.Empty;
            string viewHeight = field.View.RowHeight;

            if (string.IsNullOrEmpty(viewHeight))
            {
                HtmlControlType controlType = field.GetHtmlControlType();

                if (controlType == HtmlControlType.TextArea)
                {
                    height = "line-height: auto; height: auto;";
                }
            }
            else
            {
                height = "line-height: " + field.View.RowHeight + "px; height:" + field.View.RowHeight + "px;";
            }

            return height;
        }

        public virtual string GetRowHeightStyle(Durados.View view)
        {
            string viewHeight = view.RowHeight;

            if (string.IsNullOrEmpty(viewHeight))
                return string.Empty;
            else
                return "line-height: " + viewHeight + "px; height:" + viewHeight + "px;";
        }

        ColumnFieldViewer columnFieldViewer = new ColumnFieldViewer();
        public virtual bool IsEditable(Field field, DataRow row, string guid)
        {
            bool textarea = field.FieldType == FieldType.Column && columnFieldViewer.GetHtmlControlType((ColumnField)field) == HtmlControlType.TextArea;
            bool insideDependencyHasValue = true;
            bool insideDependency = field.FieldType == FieldType.Parent && (((ParentField)field).GetHtmlControlType() == HtmlControlType.InsideDependency || ((ParentField)field).GetHtmlControlType() == HtmlControlType.InsideDependencyUniqueNames) && ((ParentField)field).InsideTriggerField != null;
            bool childrenAndNotChecklist = field.FieldType == FieldType.Children && !(((ChildrenField)field).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList);
            bool upload = field.FieldType == FieldType.Column && ((ColumnField)field).Upload != null;
            if (insideDependency)
            {
                string insideDependencyValue = ((ParentField)field).InsideTriggerField.GetValue(row);
                insideDependencyHasValue = !string.IsNullOrEmpty(insideDependencyValue) && !((ParentField)field).HasInsideTriggeredFields;
            }


            return ((View)field.View).IsEditable(guid) && field.View.GridEditable && field.GridEditable && field.IsVisibleForEdit() && !field.IsDisableForEdit(guid) && !field.IsExcludedInUpdate() && insideDependencyHasValue && !childrenAndNotChecklist && !field.DisableConfigClonedField(row) && !field.SpecialColumn.Equals(SpecialColumn.Html);

        }

        public virtual bool IsDerivationEditable(Field field, DataRow row, string guid)
        {
            return field.View.IsDerivationEditable(field, row);
        }

        public virtual bool IsVisible(Field field, Dictionary<string, Durados.Field> excludedColumn, string guid)
        {
            return (field.IsVisibleForTable() && (excludedColumn == null || !excludedColumn.ContainsKey(field.Name)));
        }

        public virtual bool IsEditable(Durados.View view)
        {
            return view.DisplayType == DisplayType.Table;
        }

        public virtual bool IsSortable(Durados.Field field, string guid)
        {
            return field.Sortable;
        }

        public virtual bool IsEditOnDblClick(Durados.View view)
        {
            return false;
            //return IsEditable(view);
        }

        //Changed by br
        //public virtual bool SelectorCheckbox(Durados.View view, DataRow row)
        public virtual bool SelectorCheckbox(Durados.View view)
        {
            return ((View)view).MultiSelect && !Durados.Web.Infrastructure.General.IsMobile();
        }

        public virtual bool ShowRowHover(Durados.View view, DataRow row)
        {
            return true;
        }

        public virtual string GetEditCaption(Durados.View view, DataRow row, string guid)
        {
            return ((View)view).IsEditable(guid) && IsEditable(view) ? "Edit" : "View"; 
        }

        public virtual bool IsEditable(Durados.View view, string guid)
        {
            return ((View)view).IsEditable(guid) && IsEditable(view);
        }

        public virtual bool IsHoverEditIconEnabled(Field field, DataRow row, string guid)
        {
            bool isHoverEditIconEnabled = false;
            ColumnField columnField = field as ColumnField;
            if (columnField != null)
            {
                HtmlControlType htmlControlType = columnField.GetHtmlControlType();

                if ((htmlControlType == HtmlControlType.Upload || htmlControlType == HtmlControlType.TextArea) && IsEditable(field, row, guid))
                {
                    isHoverEditIconEnabled = true;
                }

            }
            return isHoverEditIconEnabled;
        }

        public virtual string GetEditTitle(Durados.View view, DataRow row)
        {
            return IsEditable(view) ? "Edit current record" : "View current record";
        }

        public virtual string Convert(Durados.View view, DataView dataView, Dictionary<string,object> nameValueDictionary)
        {
            
            StringBuilder grid = new StringBuilder();
            grid.Append("<table class='grid' border=0 cellspacing=0 cellpadding=0>");

            List<Durados.Field> fields = ((View)view).VisibleFieldsForTableFirstRow;

            grid.Append("<tr>");
            
            foreach (Durados.Field field in fields)
            {
                grid.Append("<th>");

                grid.Append(field.DisplayName);

                grid.Append("</th>");
            }
            
            grid.Append("</tr>");


            foreach (System.Data.DataRowView row in dataView)
            {
                grid.Append("<tr>");

                foreach (Durados.Field field in fields)
                {
                    grid.Append("<td>");

                    if (field.FieldType == FieldType.Column && ((ColumnField)field).DataColumn.DataType.Equals(typeof(bool)))
                    {
                        if (row.Row.IsNull(((ColumnField)field).DataColumn.ColumnName))
                        {
                            grid.Append(string.Empty);
                        }
                        else
                        {
                            grid.Append(row.Row[((ColumnField)field).DataColumn.ColumnName].ToString());
                        }
                    }
                    else
                    {
                        grid.Append(GetElementForTableView(field, row.Row, null));
                    }

                    grid.Append("</td>");

                }

                grid.Append("</tr>");

            }

            grid.Append("</table>");

            return grid.ToString();
        }

        public virtual string GetDashboardItem(Durados.Link link)
        {
            return null;
        }
    }


    public class CheckListTableViewer
    {
        protected DataView dataView;
        protected Dictionary<string, Dictionary<string, string>> values;

        private IDataTableAccess GetDataTableAccess(Durados.View view)
        {
            if (view is Durados.Config.IConfigView)
                return new ConfigAccess();
            else
                if (OracleAccess.IsOracleConnectionString(view.ConnectionString))
                    return new OracleAccess(); 
                if (PostgreAccess.IsPostgreConnectionString(view.ConnectionString))
                    return new PostgreAccess(); 
                else if (MySqlAccess.IsMySqlConnectionString(view.ConnectionString))
                   return new MySqlAccess();
                else
                    return new SqlAccess();
        }
        public CheckListTableViewer(ChildrenField field, DataView dataView)
        {
            this.dataView = dataView;
            this.values = GetValues(dataView, field);
        }

        protected virtual Dictionary<string, Dictionary<string, string>> GetValues(DataView dataView, ChildrenField field)
        {
            //Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();
            IDataTableAccess sqlAccess = GetDataTableAccess(field.View);

            return sqlAccess.GetChildren(dataView, field);
        }

        public virtual Dictionary<string, string> GetValue(string pk)
        {
            if (values.ContainsKey(pk))
                return values[pk];
            else
                return null;
        }

    }

    public class LetterConverter : ITableConverter
    {
        public string Convert(Durados.View view, DataView dataView, Dictionary<string,object> nameValueDictionary)
        {
            string recipients = string.Empty;

            foreach (System.Data.DataRowView row in dataView)
            {
                foreach (Field field in view.Fields.Values.Where(f=>f.FieldType == FieldType.Column && ((ColumnField)f).SpecialColumn ==SpecialColumn.Email))
                {

                    recipients += field.ConvertToString(row.Row) + ";";
                }
            }

            return recipients.TrimEnd(';');
        }
    }
}

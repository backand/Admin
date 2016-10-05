using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc
{
    public partial class ColumnField : Durados.ColumnField
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public ColumnField(View view, DataColumn dataColumn) :
            base(view, dataColumn)
        {
            Rich = (this.GetHtmlControlType() == HtmlControlType.TextArea);
            BooleanHtmlControlType = BooleanHtmlControlType.Check;
            TextHtmlControlType = DataColumn.MaxLength > 260 ? TextHtmlControlType.TextArea : TextHtmlControlType.Text;
            RadioOrientation = Orientation.Horizontal;
            Dialog = true;
            PartialLength = 50;
            Custom = false;
            AdvancedFilter = true;
            BrowserAutocomplete = ColumnFieldType == ColumnFieldType.String && SpecialColumn == SpecialColumn.None;
            ContainerGraphicProperties = "d_fieldContainer";
            
        }

        /*
        public override bool HideInCreate
        {
            get
            {
                if (IsDateTimeOffest())
                    return true;
                return base.HideInCreate;
            }
            set
            {
                base.HideInCreate = value;
            }
        }

        public override bool HideInEdit
        {
            get
            {
                if (IsDateTimeOffest())
                    return true;
                return base.HideInEdit;
            }
            set
            {
                base.HideInEdit = value;
            }
        }

        public override bool ExcludeInInsert
        {
            get
            {
                if (IsDateTimeOffest())
                    return true;
                return base.ExcludeInInsert;
            }
            set
            {
                base.ExcludeInInsert = value;
            }
        }

        public override bool ExcludeInUpdate
        {
            get
            {
                if (IsDateTimeOffest())
                    return true;
                return base.ExcludeInUpdate;
            }
            set
            {
                base.ExcludeInUpdate = value;
            }
        }
        private bool? isDateTimeOffest = null;

        public bool IsDateTimeOffest()
        {
            if (!isDateTimeOffest.HasValue)
                isDateTimeOffest = DataColumn.DataType.Equals(typeof(DateTimeOffset));

            return isDateTimeOffest.Value;
        }
        */

        public override bool IsMilestonesField
        {
            get
            {
                return TextHtmlControlType == TextHtmlControlType.Milestone;
            }
        }

        //protected override string GetDefaultValue(string viewName, string columnFieldName)
        //{
        //    if (Map.GetConfigDatabase().Views.ContainsKey(viewName) && Map.GetConfigDatabase().Views[viewName].Fields.ContainsKey(columnFieldName) && Map.GetConfigDatabase().Views[viewName].Fields[columnFieldName].DefaultValue != null)
        //        return Map.GetConfigDatabase().Views[viewName].Fields[columnFieldName].DefaultValue.ToString();
        //    else
        //        return null;
        //}

        public View GetParentView()
        {
            View parentView = null;
            if (!string.IsNullOrEmpty(MultiValueParentViewName))
            {
                if (Map.Database.Views.ContainsKey(MultiValueParentViewName))
                    parentView = (View)Map.Database.Views[MultiValueParentViewName];
                else if (Map.GetConfigDatabase().Views.ContainsKey(MultiValueParentViewName))
                    parentView = (View)Map.GetConfigDatabase().Views[MultiValueParentViewName];
            }

            return parentView;
        }

        protected override bool IsImage()
        {
            return Upload != null;
        }

        protected override bool IsUrl()
        {
            return TextHtmlControlType == Mvc.TextHtmlControlType.Url;
        }

        

        public string CssClass
        {
            get
            {
                string cssClass = GraphicProperties;
                if (string.IsNullOrEmpty(cssClass))
                {
                    if (DataColumn.DataType == typeof(DateTime) || DataColumn.DataType == typeof(DateTimeOffset))
                    {
                        cssClass = "date";
                        //if (DisplayFormat == DisplayFormat.DateAndTime)
                        //{
                        //    cssClass = cssClass + " datetime";
                        //}
                        //else if (DisplayFormat == DisplayFormat.TimeOnly)
                        //{
                        //    cssClass = cssClass + " time";
                        //}
                    }
                }
                return cssClass;
            }
            set
            {
                GraphicProperties = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public BooleanHtmlControlType BooleanHtmlControlType { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public TextHtmlControlType TextHtmlControlType { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public Orientation RadioOrientation { get; set; }

        [Durados.Config.Attributes.ParentProperty(TableName = "Upload")]
        public Upload Upload { get; set; }

        [Durados.Config.Attributes.ParentProperty(TableName = "FtpUpload")]
        public FtpUpload FtpUpload { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Rich { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool BrowserAutocomplete { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Dialog { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int PartialLength { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Custom { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Enable grid editing", Default = true)]
        public override bool GridEditable
        {
            get
            {
                //by br
                //if (this.FtpUpload != null || this.Upload != null)
                //    return false;
                return base.GridEditable;
            }
            set
            {
                base.GridEditable = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool AdvancedFilter { get; set; }
        
        [Durados.Config.Attributes.ConfigValue(Type = typeof(ParentHtmlControlType), Value = typeof(ParentHtmlControlType))]
        [Durados.Config.Attributes.ColumnProperty()]
        public string EnumType { get; set; }

        public override Dictionary<string, string> GetSelectOptions()
        {
            Dictionary<string, string> selectOptions = new Dictionary<string, string>();

            if (EnumType != null)
            {
                
                Type type = Type.GetType(EnumType);

                Dictionary<string, string> enumDisplayNames = null;
                object[] attributes = type.GetCustomAttributes(typeof(Durados.Config.Attributes.EnumDisplayAttribute), true);
                if (attributes.Length == 1)
                {
                    enumDisplayNames = ((Durados.Config.Attributes.EnumDisplayAttribute)attributes[0]).GetEnumDisplayNames();
                }

                string[] names = Enum.GetNames(type);

                foreach (string name in names)
                {
                    // YARIV - PLEASE REMOVE
                    //if (name == "Gauge") continue;

                    string val = string.Empty;
                    if (enumDisplayNames != null && enumDisplayNames.ContainsKey(name))
                        val = enumDisplayNames[name];
                    else
                        val = name.GetDecamal();

                    selectOptions.Add(name, val);
                }
                
            }
            else
            {
                return this.GetSelectList().ToDictionary(i=>i.Value, i=>i.Text);

            }

            return selectOptions;
        }

        public override bool DoLocalize()
        {
            return ((Database)View.Database).Localization != null && ((Database)View.Database).IsMultiLanguages;
        }

        
        public override string ConvertToString(DataRow dataRow)
        {
            if (!string.IsNullOrEmpty(MultiValueParentViewName) && TextHtmlControlType == TextHtmlControlType.DropDown)
            {
                View parentView = GetParentView();
                if (dataRow.Table.DataSet.Tables.Contains(MultiValueParentViewName) && parentView != null && !(parentView.Name == "View" && DropDownValueField == "Name"))
                {
                    DataTable parentTable = dataRow.Table.DataSet.Tables[MultiValueParentViewName];

                    if (parentTable.Rows.Count == 0)
                    {
                    }
                    string value = GetValue(dataRow);
                    if (string.IsNullOrEmpty(value))
                        return NullString;
                    DataRow parentRow = parentTable.Rows.Find(parentView.GetPkValue(value));

                    if (parentRow == null)
                    {
                        return NullString;
                    }

                    return Encode(parentView.DisplayField.ConvertToString(parentRow));
                }
                else if (parentView != null && parentView.Name == "View" && DropDownValueField == "Name")
                {
                    string viewName = GetValue(dataRow);
                    if (!string.IsNullOrEmpty(viewName) && Map.Database.Views.ContainsKey(viewName))
                    {
                        return Map.Database.Views[viewName].DisplayName;
                    }
                    else
                        return NullString;
                }
                else
                {
                    return Encode(base.ConvertToString((DataRow)dataRow)); 
                }
            }
            else
            {
                string value = Encode(base.ConvertToString((DataRow)dataRow)); 
                if (this.Dialog)
                {
                    if (this.EnumType == null)
                        return value;
                    else
                        return value.GetDecamal();
                }
                else
                {
                    return value;
                }
            }
        }

        private string Encode(string s)
        {
            if (DisplayFormat == Durados.DisplayFormat.Html || (this.Dialog && this.Rich))
                return s;
            return System.Web.HttpUtility.HtmlEncode(s);
        }


        public override string GetDefaultFilter()
        {
            return this.GetDefaultFilterWithSql();

        }

        public override Dictionary<string, string> GetAutocompleteValues(string q, int limit)
        {
            return this.GetSelectOptions(q, AutocompleteMathing == AutocompleteMathing.StartsWith, limit, null);
        }

       

        public bool IsUpload
        {
            get
            {
                return this.Upload != null || this.FtpUpload != null;
            }
            
        }

        public bool IsFtpUpload
        {
            get
            {
                return this.FtpUpload != null;
            }
           
        }

        public override bool IsAllow(DataAction dataAction)
        {
            return !FieldExtentions.IsDisable(this, dataAction, null);
        }
      
    }

    public enum BooleanHtmlControlType
    {
        Check,
        Radio
    }

    public enum TextHtmlControlType
    {
        Text,
        TextArea,
        Autocomplete,
        Url,
        CheckList,
        DropDown,
        Milestone,
        DependencyCustom,
        ColorPicker
    }

    public enum ColSpanInDialog
    {
        One_Column=1,
        Two_Columnbs = 2,
        Three_Columns = 3
    }
}

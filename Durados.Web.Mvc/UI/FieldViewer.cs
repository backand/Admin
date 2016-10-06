using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Config.Attributes;

namespace Durados.Web.Mvc.UI
{
    public abstract class FieldViewer
    {
        public const string createPrefix = "create_";
        public const string editPrefix = "edit_";
        public const string filterPrefix = "filter_";
        public const string inlineAddingPrefix = "inlineAdding_";
        public const string inlineEditingPrefix = "inlineEditing_";

        protected Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public virtual string GetContainerElementIDForValidation(Field field, DataAction dataAction, string guid)
        {
            return "the" + guid + GetDataActionPrefix(dataAction) + JavaScript.ConvertToLegalVariable(field.Name);
        }

        public static DataAction GetDataAction(string prefix)
        {
            switch (prefix)
            {
                case createPrefix:
                    return DataAction.Create;
                case editPrefix:
                    return DataAction.Edit;
                case inlineAddingPrefix:
                    return DataAction.InlineAdding;
                default:
                    throw new NotSupportedException();

            }
        }

        public static string GetDataActionPrefix(DataAction dataAction)
        {
            switch (dataAction)
            {
                case DataAction.Create:
                    return createPrefix;
                case DataAction.Report:
                    return createPrefix;
                case DataAction.Edit:
                    return editPrefix;
                case DataAction.InlineAdding:
                    return inlineAddingPrefix;
                case DataAction.InlineEditing:
                    return inlineEditingPrefix;
                default:
                    throw new NotSupportedException();

            }
        }

        public abstract string GetElementForCreate(Field field, string guid);

        public abstract string GetElementForCreate(Field field, string pk, string value, string guid);

        public abstract string GetElementForEdit(Field field, string guid);

        public abstract string GetElementForEdit(Field field, DataRow dataRow, string guid);

        public abstract string GetElementForEdit(Field field, string pk, string value, string guid);

        public abstract string GetElementForFilter(Field field, object value, string guid);

        public abstract string GetElementForInlineAdding(Field field, string guid);

        public abstract string GetElementForInlineEditing(Field field, string guid);

        public abstract string GetElementForReport(Field field, string guid);

        public abstract string GetElementForReport(Field field, string pk, string value, string guid);

        public abstract string GetElementForTableView(Field field, DataRow dataRow, string guid);

        public abstract HtmlControlType GetHtmlControlType(Field field);

        public abstract string GetValidationElements(Field field, DataAction dataAction, string guid);

        public string GetValidationType(Field field)
        {
            return field.Validation.GetValidationType();
        }

        /// <summary>
        /// Get style for a filter element
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected virtual string GetFilterStyle(Field field)
        {
            return "style='" + GetFilterWidthStyle(field) + "'";
        }

        /// <summary>
        /// Get width style for a filter element
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected virtual string GetFilterWidthStyle(Field field)
        {
            string width = string.Empty;

            if (field.View != null && field.View.FilterType == FilterType.Group)
            {
                width = field.GroupFilterWidth.ToString();
            }

            return GetWidthStyle(width);
        }

        protected string GetOrientationStyle(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return "white-space: nowrap;";
            else
                return string.Empty;
        }

        protected string GetRadioLabel(ColumnField field, bool b)
        {
            string s = b ? field.View.Database.True : field.View.Database.False;
            if (Map.Database.Localization != null)
                return Map.Database.Localizer.Translate(s);
            else
                return s;
        }

        protected string GetRadioValue(ColumnField field, bool b)
        {
            return b.ToString().ToLower();

        }

        protected virtual string GetRequiredValidationElement(Field field, string guid)
        {
            string displayName = field.GetLocalizedDisplayName();
            string reqMessage = Map.Database.Localizer.Translate(field.Validation.RequiredMessage);

            if (field.Required)
            {

                switch (field.GetHtmlControlType())
                {
                    case HtmlControlType.Text:
                        return "<span d_label='" + displayName + "' class='textfieldRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.Upload:
                        return "<span d_label='" + displayName + "' class='textfieldRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.Autocomplete:
                        return "<span d_label='" + displayName + "' class='textfieldRequiredMsg'>" + reqMessage + "</span><span d_label='" + displayName + "' class='textfieldInvalidFormatMsg'>" + Map.Database.Localizer.Translate(field.Validation.NoMatchMessage) + "</span>";

                    case HtmlControlType.AutocompleteColumn:
                        return "<span d_label='" + displayName + "' class='textfieldRequiredMsg'>" + reqMessage + "</span><span d_label='" + displayName + "' class='textfieldInvalidFormatMsg'>" + Map.Database.Localizer.Translate(field.Validation.NoMatchMessage) + "</span>";

                    case HtmlControlType.DropDown:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.TextArea:
                        return "<span d_label='" + displayName + "' class='textareaRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.OutsideDependency:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.InsideDependency:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.InsideDependencyUniqueNames:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.Groups:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";

                    case HtmlControlType.CheckList:
                        return "<span d_label='" + displayName + "' class='selectRequiredMsg'>" + reqMessage + "</span>";
                    default:
                        return string.Empty;
                }
            }
            else
            {
                switch (field.GetHtmlControlType())
                {
                    case HtmlControlType.Autocomplete:
                        return "<span d_label='" + displayName + "' class='textfieldRequiredMsg'>" + reqMessage + "</span><span d_label='" + displayName + "' class='textfieldInvalidFormatMsg'>" + Map.Database.Localizer.Translate(field.Validation.NoMatchMessage) + "</span>";
                    default:
                        return string.Empty;
                }
            }
        }

        protected virtual string GetStyle(Field field)
        {
            return "style='" + GetWidthStyle(field) + "'";
        }

        protected virtual string GetUrlWithoutQuery(View view, string action)
        {
            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

            if (!url.EndsWith("/"))
                url += "/";
            url += view.Controller + "/";
            url += action + "/";
            url += view.Name;

            return url;
        }

        protected virtual string GetValidationElement(Field field, string guid)
        {
            string format = string.Empty;

            HtmlControlType fieldType = field.GetHtmlControlType();

            string invalidMessage = field.Validation.GetInvalidFormatMessage();

            string displayName = field.GetLocalizedDisplayName();

            if (invalidMessage != string.Empty)
            {
                if (fieldType == HtmlControlType.Text || fieldType == HtmlControlType.Url)
                {
                    string TooltipAction = string.Empty;
                    if (field.SpecialColumn == SpecialColumn.Custom && !string.IsNullOrEmpty(field.Format))
                    {
                        TooltipAction = "onmouseover='showSpryFormats(this)' onmouseout='hideSpryFormats(this)'";
                    }

                    format += "<span " + TooltipAction + " d_label='" + displayName + "' class='textfieldInvalidFormatMsg'>" + Map.Database.Localizer.Translate(invalidMessage) + "</span>";
                }
            }

            if (field.Validation.HasLimit())
            {
                if (fieldType == HtmlControlType.Text || fieldType == HtmlControlType.Url)
                {
                    string msg = field.Validation.GetOutOfRangeMessage(Map.Database.Localizer.Translate(field.Validation.OutOfRangeMessage));

                    format += "<span d_label='" + displayName + "' class='textfieldMinValueMsg textfieldMaxValueMsg textfieldMinCharsMsg textfieldMaxCharsMsg'>" + msg + "</span>";

                }
                else if (fieldType == HtmlControlType.TextArea)
                {
                    string msg = field.Validation.GetOutOfRangeMessage(Map.Database.Localizer.Translate(field.Validation.OutOfRangeMessage));

                    format += "<span d_label='" + Map.Database.Localizer.Translate(field.DisplayName) + "' class='textareaMinCharsMsg textareaMaxCharsMsg'>" + msg + "</span>";
                }
            }

            return format;
        }

        protected string GetVisibilityStyle(bool visible)
        {
            if (visible)
                return "";
            else
                return "visibility:hidden;";
        }

        protected virtual string GetAdminPreviewAttr(Field field)
        {
            string attr = string.Empty;
            if (field.IsAdminPreview)
            {
                attr = " adminPreview='true' ";
            }
            return attr;
        }
        protected virtual string GetWidthStyle(Field field)
        {
            string width = field.Width.ToString();

            return GetWidthStyle(width);
        }

        protected virtual string GetWidthStyle(string width)
        {
            string widthStyle = string.Empty;

            if (!string.IsNullOrEmpty(width) && width != "0")
            {
                widthStyle = "width:" + width + "px;";
            }

            return widthStyle;
        }

        protected virtual string GetInlineAddingImg(string guid, string title, string className, string onClick)
        {
            string inlineAddingImg = "";

            inlineAddingImg = "<a><span class='" + className + "' title='" + title + "' alt='" + title + "' onclick=\"" + onClick + "\" /></a>";

            return inlineAddingImg;
        }
    }

    public enum HtmlControlType
    {
        Check,
        Radio,
        Text,
        TextArea,
        Upload,
        Hidden,
        AutocompleteColumn,
        Url,
        Milestone,

        Autocomplete,
        DropDown,
        Groups,
        InsideDependency,
        OutsideDependency,
        InsideDependencyDefault,

        Grid,
        CheckList,
        InsideDependencyUniqueNames,

        Tile,
        Slider,

        ColorPicker

    }

    public static class FieldExtentions
    {
        #region Fields (1)

        private static Dictionary<FieldType, FieldViewer> fieldViewers;

        #endregion Fields

        #region Constructors (1)

        static FieldExtentions()
        {
            fieldViewers = new Dictionary<FieldType, FieldViewer>();
            fieldViewers.Add(FieldType.Children, new ChildrenFieldViewer());
            fieldViewers.Add(FieldType.Column, new ColumnFieldViewer());
            fieldViewers.Add(FieldType.Parent, new ParentFieldViewer());
        }

        #endregion Constructors

        #region Methods (25)

        // Public Methods (25) 

        public static bool DisableConfigClonedField(this Field field, DataRow row)
        {
            if (!((Database)field.View.Database).IsConfig)
                return false;

            string viewName = null;
            if (field.View.Name == "View")
            {
                viewName = row["Name"].ToString();
            }
            else
            {
                if (field.View.Name == "Field")
                {
                    viewName = row.GetParentRow("Fields")["Name"].ToString();
                }
                else
                {
                    return false;
                }
            }

            View view = null;

            if (Maps.Instance.GetMap().Database.Views.ContainsKey(viewName))
                view = (View)Maps.Instance.GetMap().Database.Views[viewName];

            if (view == null)
                return false;

            if (!view.IsCloned)
                return false;

            PropertyInfo property;

            if (field.View.Name == "View")
            {
                property = field.View.GetType().GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {

                property = typeof(Durados.Web.Mvc.ColumnField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (property == null)
                {
                    property = typeof(Durados.Web.Mvc.ParentField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (property == null)
                        property = typeof(Durados.Web.Mvc.ChildrenField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                }

            }

            if (property == null)
                return true;


            object[] propertyAttributes = property.GetCustomAttributes(typeof(PropertyAttribute), true);
            if (propertyAttributes.Length == 1)
            {
                PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                return !propertyAttribute.DoNotCopy;
            }

            return true;
        }

        public static bool DisableConfigClonedView(this Field field, View view)
        {
            if (!view.IsCloned || !((Database)field.View.Database).IsConfig)
                return false;

            string viewName = view.Name;

            PropertyInfo property;

            if (field.View.Name == "View")
            {
                property = field.View.GetType().GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                property = typeof(Durados.Web.Mvc.ColumnField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (property == null)
                {
                    property = typeof(Durados.Web.Mvc.ParentField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (property == null)
                        property = typeof(Durados.Web.Mvc.ChildrenField).GetProperty(field.GetPropertyName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                }
            }

            if (property == null)
                return true;


            object[] propertyAttributes = property.GetCustomAttributes(typeof(PropertyAttribute), true);
            if (propertyAttributes.Length == 1)
            {
                PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                return !propertyAttribute.DoNotCopy;
            }

            return true;
        }

        public static string GetDataActionPrefix(this Field field, DataAction dataAction)
        {
            return FieldViewer.GetDataActionPrefix(dataAction);
        }

        public static string GetElementForCreate(this Field field, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForCreate(field, guid);
        }

        public static string GetElementForCreate(this Field field, string pk, string value, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForCreate(field, pk, value, guid);
        }

        public static string GetElementForEdit(this Field field, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForEdit(field, guid);
        }

        public static string GetElementForEdit(this Field field, DataRow dataRow, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForEdit(field, dataRow, guid);
        }

        public static string GetElementForEdit(this Field field, string pk, string value, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForEdit(field, pk, value, guid);
        }

        public static string GetElementForFilter(this Field field, object value, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForFilter(field, value, guid);
        }

        public static string GetElementForInlineAdding(this Field field, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForInlineAdding(field, guid);
        }

        public static string GetElementForInlineEditing(this Field field, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForInlineEditing(field, guid);
        }

        public static string GetElementForReport(this Field field, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForReport(field, guid);
        }

        public static string GetElementForTableView(this Field field, DataRow dataRow, string guid)
        {
            return fieldViewers[field.FieldType].GetElementForTableView(field, dataRow, guid);
        }

        public static HtmlControlType GetHtmlControlType(this Field field)
        {
            return fieldViewers[field.FieldType].GetHtmlControlType(field);
        }

        public static string GetValidationElements(this Field field, DataAction dataAction, string guid)
        {
            return fieldViewers[field.FieldType].GetValidationElements(field, dataAction, guid);
        }

        public static string GetValidationType(this Field field)
        {
            return fieldViewers[field.FieldType].GetValidationType(field);
        }

        public static bool IsDisable(this Field field, DataAction dataAction, string guid)
        {
            if (field.View.SystemView)
                return false;

            switch (dataAction)
            {
                case DataAction.Create:
                    return IsDisableForCreate(field);
                case DataAction.Edit:
                    return IsDisableForEdit(field, guid);
                case DataAction.InlineAdding:
                    return IsDisableForCreate(field);
                case DataAction.InlineEditing:
                    return IsDisableForEdit(field, guid);
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool IsDisableForCreate(this Field field)
        {
            return field.DisableInCreate || (UI.Helpers.SecurityHelper.IsDenied(field.DenyCreateRoles, field.AllowCreateRoles) && !UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field)) || !field.IsInPlan;
        }

        public static bool IsDisableForEdit(this Field field, string guid)
        {
            return field.DisableInEdit || (UI.Helpers.SecurityHelper.IsDenied(field.DenyEditRoles, field.AllowEditRoles) && !UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field)) || !((View)field.View).IsEditable(guid) || !field.IsInPlan;// || ((View)field.View).DisplayType == DisplayType.Report;
        }

        public static bool IsDisableForInline(this Field field, DataAction dataAction, string guid)
        {
            if (dataAction == DataAction.Edit)
            {
                return field.DisableInEdit || UI.Helpers.SecurityHelper.IsDenied(field.DenyEditRoles, field.AllowEditRoles) || !field.IsInPlan;
            }
            else
            {
                return field.DisableInCreate || UI.Helpers.SecurityHelper.IsDenied(field.DenyCreateRoles, field.AllowCreateRoles) || !field.IsInPlan;
            }
        }

        public static bool IsVisibleForCreate(this Field field)
        {
            return (!UI.Helpers.SecurityHelper.IsDenied(field.DenyCreateRoles, field.AllowCreateRoles) ||  (((View)field.View).IsViewOwner() && UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field))) && !field.IsHiddenInCreate() && field.HasPlan;
        }

        public static bool IsVisibleForEdit(this Field field)
        {
            return ((!UI.Helpers.SecurityHelper.IsDenied(field.DenySelectRoles, field.AllowSelectRoles) ||  (((View)field.View).IsViewOwner() && UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field))) && !field.IsHiddenInEdit()) && field.HasPlan;
        }

        public static bool IsVisibleForFilter(this Field field)
        {
            return !field.HideInFilter && field.HasPlan && !field.Excluded;
        }

        public static bool IsVisibleForSort(this Field field)
        {
            return field.Sortable && field.HasPlan && !field.Excluded;
        }

        public static bool IsVisibleForReport(this Field field)
        {
            return !field.IsHiddenInCreate();
        }

        public static bool IsVisibleForTable(this Field field)
        {
            if (!field.HasPlan)
                return false;

            if (Durados.Web.Infrastructure.General.IsMobile())
                return !field.IsHiddenInTable() && !field.IsHiddenInTableMobile() && (!UI.Helpers.SecurityHelper.IsDenied(field.DenySelectRoles, field.AllowSelectRoles) || (((View)field.View).IsViewOwner() && UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field)));
            else
                return !field.IsHiddenInTable() && (!UI.Helpers.SecurityHelper.IsDenied(field.DenySelectRoles, field.AllowSelectRoles) || (((View)field.View).IsViewOwner() && UI.Helpers.SecurityHelper.IsConfigFieldForViewOwner(field)));
        }

        public static bool IsAllowSelect(this Field field)
        {
            return !UI.Helpers.SecurityHelper.IsDenied(field.DenySelectRoles, field.AllowSelectRoles);
        }

        #endregion Methods
    }
}

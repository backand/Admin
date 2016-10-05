using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc
{
    public partial class ParentField : Durados.ParentField
    {

        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field only: field display type")]
        public ParentHtmlControlType ParentHtmlControlType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "")]
        public int RadioColumns { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field only: Minimum characters to run auto complete")]
        public bool LimitToStartAutocomplete { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Localize { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field only: Child dependency - display the parent data in the table View")]
        public bool ShowDependencyInTable { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field only: Enable multi select in Filter")]
        public bool MultiFilter { get; set; }


        public override string GetDefaultFilter()
        {
            if (base.DefaultFilter.ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower())
                return ((Database)View.Database).GetUserID();
            return this.GetDefaultFilterWithSql();
        
        }

        
        public ParentField(View view, DataRelation dataRelation) :
            base(view, dataRelation)
        {
            ParentHtmlControlType = ParentHtmlControlType.DropDown;
            AutocompleteMathing = AutocompleteMathing.StartsWith;
            LimitToStartAutocomplete = true;
            ShowDependencyInTable = true;
            MultiFilter = true;
            ContainerGraphicProperties = "d_fieldContainer";
            EditInTableView = false;
        }

        //public virtual object GetAutoCompleteValues(string q, int limit)
        //{
        //    Dictionary<string, string> selectOptions = this.GetSelectOptions();

        //    List<Tag> autoCompleteValues = new List<Tag>();
        //    foreach (string key in selectOptions.Keys)
        //    {
        //        autoCompleteValues.Add(new Tag() { PK = key, Name = selectOptions[key] });
        //    }

        //    if (AutocompleteMathing == AutocompleteMathing.StartsWith)
        //    {
        //        var retValue = autoCompleteValues
        //            .Where(x => x.Name.StartsWith(q, StringComparison.OrdinalIgnoreCase))
        //            .OrderBy(x => x.Name)
        //            .Take(limit)
        //            .Select(r => new { Tag = r });
        //        // Return the result set as JSON
        //        return retValue;
        //    }
        //    else
        //    {
        //        var retValue = autoCompleteValues
        //             .Where(x => (x.Name.ToLower().Contains(q.ToLower())))
        //             .OrderBy(x => x.Name)
        //             .Take(limit)
        //             .Select(r => new { Tag = r });
        //        // Return the result set as JSON
        //        return retValue;


        //    }
        //}

        //public override object GetAutoCompleteValues(string q, int limit)
        //{
        //    Dictionary<string, string> selectOptions = this.GetSelectOptions(q, AutocompleteMathing == AutocompleteMathing.StartsWith, limit);

        //    List<Tag> autoCompleteValues = new List<Tag>();
        //    foreach (string key in selectOptions.Keys)
        //    {
        //        autoCompleteValues.Add(new Tag() { PK = key, Name = selectOptions[key] });
        //    }

        //    var retValue = autoCompleteValues
        //            .OrderBy(x => x.Name)
        //            .Select(r => new { Tag = r });
        //    // Return the result set as JSON
        //    return retValue;
        //}

        public override bool HasDependencyFilter()
        {
            return base.HasDependencyFilter() && ParentHtmlControlType != ParentHtmlControlType.InsideDependencyDefault;
        }

        public override Dictionary<string, string> GetAutocompleteValues(string q, int limit)
        {
            return this.GetSelectOptions(q, AutocompleteMathing == AutocompleteMathing.StartsWith, limit, null);
        }

        public override bool DoLocalize()
        {
            return ((Database)View.Database).Localization != null && ((Database)View.Database).IsMultiLanguages;
        }
        

        public override string ConvertDefaultToString()
        {
            View userView = (View)View.Database.GetUserView();

            if (DefaultValue != null && DefaultValue.ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower() && userView != null)
            {
                return ((Database)View.Database).GetUserID();
            }
            else
            {
                return base.ConvertDefaultToString();
            }
        }

        public string ConvertDefaultToDisplayValue()
        {
            View userView = (View)View.Database.GetUserView();

            if (DefaultValue != null && DefaultValue.ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower() && userView != null)
            {
                return ((Database)View.Database).GetUserFullName();
            }
            else
            {
                return base.ConvertDefaultToString();
            }
        }

        public string CssClass
        {
            get
            {
                return GraphicProperties;
            }
            set
            {
                GraphicProperties = value;
            }
        }

        public override bool IsAllow(DataAction dataAction)
        {
            return FieldHelper.IsVisibleForRow(this, dataAction);
        }
    }

    public enum ParentHtmlControlType
    {
        Autocomplete,
        DropDown,
        Radio,
        InsideDependency,
        OutsideDependency,
        Groups,
        Hidden,
        InsideDependencyUniqueNames,
        InsideDependencyDefault,
        Tile,
        Slider
    }


   

    
    
}

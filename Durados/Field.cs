using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Durados
{
    [Durados.Config.Attributes.ClassConfig(DerivedTypesColumnName = "FieldType")]
    public abstract partial class Field
    {


        public Field(View view)
        {
            Validation = new Validation();
            Validation.SetField(this);

            View = view;
            HideInTableMobile = false;
            HideInTable = false;
            HideInCreate = false;
            HideInEdit = false;
            DisableInCreate = false;
            DisableInEdit = false;
            HideInFilter = false;
            NullString = string.Empty;
            Sortable = true;
            ColSpanInDialog = 1;
            Order = int.MaxValue;
            LabelContentLayout = Orientation.Horizontal;
            SpecialColumn = SpecialColumn.None;
            PartFromUniqueIndex = false;
            SaveHistory = true;
            GridEditable = true;
            Precedent = false;
            NoHyperlink = true;
            ExportToXml = true;
            UpdateParentInGrid = false;
            Width = 200;
            GroupFilterWidth = 80;
            GroupFilterDisplayLabel = GroupFilterDisplayLabel.Inherit;
            ID = -1;
            ShowColumnHeader = true;
            HiddenAttribute = false;
            Import = true;
        }

        public abstract string GetDefaultJsonName();

        public Field()
        {
            // TODO: Complete member initialization
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Include in duplicate in the first entitly level")]
        public bool IncludeInDuplicate { get; set; }

        public virtual bool DoLocalize()
        {
            return this.View.Database.IsMultiLanguages && (this.View.Database.TranslateAllViews || this.View.Database.IsConfig);
        }

        public virtual object GetAutoCompleteValues(string q, int limit, bool api = false)
        {
            Dictionary<string, string> selectOptions = GetAutocompleteValues(q, limit);

            List<Tag> autoCompleteValues = new List<Tag>();
            foreach (string key in selectOptions.Keys)
            {
                autoCompleteValues.Add(new Tag() { PK = key, Name = selectOptions[key] });
            }

            if (api)
            {
                var retValue = autoCompleteValues
                    .OrderBy(x => x.Name)
                    .Select(r => new { value = r.PK, label = r.Name });
                // Return the result set as JSON
                return retValue;
            
            }
            else
            {
                var retValue = autoCompleteValues
                        .OrderBy(x => x.Name)
                        .Select(r => new { Tag = r });
                // Return the result set as JSON
                return retValue;
            }
        }

        public virtual bool IsCheckList()
        {
            return false;
        }

        public virtual bool IsSubGrid()
        {
            return false;
        }

        public bool IsPartOfPK()
        {
            string[] pkColumnNames = View.GetPkColumnNames();
            foreach (string columnName in GetColumnsNames())
            {
                if (pkColumnNames.Contains(columnName))
                    return true;
            }

            return false;
        }

        public virtual Dictionary<string, string> GetAutocompleteValues(string q, int limit)
        {
            throw new NotImplementedException();
        }

        public Field Base
        {
            get
            {
                if (string.IsNullOrEmpty(View.BaseName))
                    return this;

                return View.Base.Fields[Name];
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field or Column field with DropDown style: True - link open parent dialog, False - link open new window with parent")]
        public bool EditInTableView { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Indication if need to display a label above filter field. Relevant in group filter type")]
        public GroupFilterDisplayLabel GroupFilterDisplayLabel { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Width of filter field. Relevant in group filter type")]
        public int GroupFilterWidth { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Parent field or Column field with DropDown style: Remove the link in the table View")]
        public bool NoHyperlink { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Indicates if should be visible on dashboard view")]
        public bool Dashboard { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Indicates if should be visible on preview view")]
        public bool Preview { get; set; }
        //private string _Formula;

        protected string _Formula = null;
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Get or set the calculated field formula.")]
        public string Formula
        {
            get
            {
                if (string.IsNullOrEmpty(View.BaseName))
                    return _Formula;

                return Base.Formula;
            }
            set
            {
                if (string.IsNullOrEmpty(View.BaseName))
                    _Formula = value;
                else
                    Base.Formula = value;
            }
        }

        public string GetFormula()
        {
            if (Formula == null)
                return null;

            if (Formula == string.Empty)
                return string.Empty;

            return "(" + Formula + ")";
        }

        protected bool _IsCalculated;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Indicates if field is calculated")]
        public bool IsCalculated
        {
            get
            {
                if (string.IsNullOrEmpty(View.BaseName))
                    return _IsCalculated;

                return Base.IsCalculated;
            }
            set
            {
                if (string.IsNullOrEmpty(View.BaseName))
                    _IsCalculated = value;
                else
                    Base.IsCalculated = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Encrypted field")]
        public bool SysEncrypted { get; set; }


        protected Category category = null;
        [Durados.Config.Attributes.ParentProperty(DoNotCopy = true, TableName = "Category", Description = "The Tab the field include in")]
        public Category Category
        {
            get
            {
                return GetCategory();
            }
            set
            {
                category = value;
                View.RefreshCategories();
            }
        }

        protected virtual Category GetCategory()
        {
            return category;
        }

        internal protected void SetCategory(Category category)
        {
            this.category = category;
        }

        //private Category category;
        //[Durados.Config.Attributes.ParentProperty(TableName = "Category")]
        //public Category Category
        //{
        //    get
        //    {
        //        return category;
        //    }
        //    set
        //    {
        //        if (value.View != null && value.View != this.View)
        //            throw new DuradosException("The category " + value.Name + " doeas not belong to this field's view.");

        //        if (category != null)
        //            if (category.Fields.Contains(this))
        //                category.Fields.Remove(this);

        //        category = value;

        //        if (!category.Fields.Contains(this))
        //            category.Fields.Add(this);

        //        if (category.View == null)
        //            category.View = this.View;
        //    }
        //}

        public View View { get; private set; }
        public abstract string Name
        {
            get;
        }

        public abstract string GetPropertyName();

        private string displayName;

        public string DisplayNameForDynasty
        {
            get
            {
                return JsonName;
            }
        }
        
        [Durados.Config.Attributes.ColumnProperty(Description = "The field&#39s name that the user see")]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                {
                    return GetInitialDisplayName();
                }
                else
                {
                    return displayName;
                }
            }
            set
            {
                displayName = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The names of the columns of the field in the database.")]
        public string DatabaseNames
        {
            get
            {
                return GetColumnsNames().Delimited();
            }
            set
            {
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the equivalent attributes in an expoprted xml. comma delimited")]
        public string AttributesNames { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Default true")]
        public bool ExportToXml { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the related View, parent relation if parent field, children relation if children field.")]
        public virtual string RelatedViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public SpecialColumn SpecialColumn { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Dependency data for field, Modes:0=disable,1=visibility,2=display, Format: {Mode}|Comma seperated values;Comma seperated field names|..., Example: 0|ShortText,LongText,Numeric,Boolean,DateTime;RelatedViewName,View|SingleSelection;Formula")]
        public string DependencyData { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool IsUnique { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "CSS Class in the Dialog")]
        public string GraphicProperties { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Type of matching for Autocomplete")]
        public AutocompleteMathing AutocompleteMathing { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If autocomplete in dialog and true then autocomplete also in filter otherwise it is dropdown on filter.")]
        public bool AutocompleteFilter { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "CSS Class in the View")]
        public string ContainerGraphicProperties { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Hold the name of field that contol the field&#39s CSS. Use to set the color of the field.")]
        public string ContainerGraphicField { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string NullString { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Exclude from the View - overwrite Hide and Exlude in Edit and New")]
        public bool Excluded { get; set; }

        public bool IsHiddenInTableMobile()
        {
            return Excluded || ((!View.Database.IsApi() || View.Database.IsConfig || View.SystemView) && HideInTableMobile);
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide from View only in mobilde mode")]
        public bool HideInTableMobile { get; set; }

        public bool IsHiddenInTable()
        {
            return Excluded || ((!View.Database.IsApi() || View.Database.IsConfig || View.SystemView) && HideInTable);
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Don&#39t show the field in the table View")]
        public bool HideInTable { get; set; }

        public bool IsHiddenInEdit()
        {
            return Excluded || ((!View.Database.IsApi() || View.Database.IsConfig || View.SystemView) && HideInEdit);
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Don&#39t show the field in Edit dialog")]
        public virtual bool HideInEdit { get; set; }

        public bool IsHiddenInCreate()
        {
            return Excluded || HideInCreate;
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Don&#39t show the field in New dialog")]
        public virtual bool HideInCreate { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Disable the field in Edit dialog")]
        public virtual bool DisableInEdit { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Disable the field in New dialog")]
        public virtual bool DisableInCreate { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Disable the field in Duplicate dialog")]
        public virtual bool DisableInDuplicate { get; set; }

        public bool IsExcludedInUpdate()
        {
            return Excluded || ExcludeInUpdate;
        }



        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "In Edit mode, don&#39t save the data into the database")]
        public virtual bool ExcludeInUpdate { get; set; }

        public bool IsExcludedInInsert()
        {
            return Excluded || ExcludeInInsert || IsRowColor;
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "In New mode, don&#39t save the data into the database")]
        public virtual bool ExcludeInInsert { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Disable the filter value")]
        public virtual bool DisableInFilter { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Enable the user to filter by this field")]
        public virtual bool HideInFilter { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Order of the field in the table View")]
        public int Order { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Order of the field in the New dialog")]
        public int OrderForCreate { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Order of the field in the Edit dialog")]
        public int OrderForEdit { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Enable sort for the field in the table View")]
        public virtual bool Sortable { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If updated in sub grid then it updates the parent row. Need also to set the Update Parent in the Parent View Children Field.")]
        public virtual bool UpdateParentInGrid { get; set; }

        //[Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Default value in New dialog")]
        //public object DefaultValue { get; set; }

        private object defaultValue;
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Default value in New dialog")]
        public object DefaultValue
        {
            get
            {
                if (View.Database.IsApi())
                {
                    return GetDbDefaultValue();
                }

                return defaultValue;

            }
            set
            {
                defaultValue = value;
            }
        }

        protected abstract object GetDbDefaultValue();
        
        [Durados.Config.Attributes.ColumnProperty(Description = "Enable grid editing")]
        public virtual bool GridEditable { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Add seperation line above. only happens if it is first in a row")]
        public bool Seperator { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "A title below the seperation line")]
        public string SeperatorTitle { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "A label above the field. only happens if it is first in a row")]
        public string PreLabel { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "A label above the field. only happens if it is first in a row")]
        public string PostLabel { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Column span in New / Edit dialog")]
        public int ColSpanInDialog { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Dropdown checklist minimum width")]
        public int MinWidth { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The default is disable in derivation. Unless Hide In Derivation equals true then the field is hidden")]
        public bool HideInDerivation { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool InlineSearch { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenySelectRoles { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenyCreateRoles { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenyEditRoles { get; set; }

        public bool IsDerivationEditable(DataRow row)
        {
            if (View.Derivation == null)
                return true;

            return View.Derivation.IsEditable(View, this, row);
        }

        public bool IsDerivationEditable(Dictionary<string, object> values)
        {
            if (View.Derivation == null)
                return true;

            ParentField derivationField = View.Derivation.GetDerivationField(View);
            if (values.ContainsKey(derivationField.Name))
            {
                object derivedValue = values[derivationField.Name];
                if (derivedValue != null && !derivedValue.Equals(string.Empty))
                {
                    return View.Derivation.IsEditable(View, this, derivationField, derivedValue.ToString());
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public bool IsDerivationEditable(string derivedValue)
        {
            if (View.Derivation == null)
                return true;

            return View.Derivation.IsEditable(View, this, View.Derivation.GetDerivationField(View), derivedValue);
        }

        private string denyCreateRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyCreateRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowCreateRoles))
                //{
                //    //return View.Database.DefaultAllowCreateRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowCreateRoles;
                //}
                if (Precedent)
                    return denyCreateRoles;
                else
                    return View.DenyCreateRoles;
            }
            set
            {
                denyCreateRoles = value;
            }
        }

        private string denyEditRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyEditRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowEditRoles))
                //{
                //    //return View.Database.DefaultAllowEditRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowEditRoles;
                //}
                if (Precedent)
                    return denyEditRoles;
                else
                    return View.DenyEditRoles;
            }
            set
            {
                denyEditRoles = value;
            }
        }

        private string denySelectRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenySelectRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowSelectRoles))
                //{
                //    //return View.Database.DefaultAllowSelectRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowSelectRoles;
                //}
                if (Precedent)
                    return denySelectRoles;
                else
                    return View.DenySelectRoles;
            }
            set
            {
                denySelectRoles = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "In case of import. Relevant only to parent or children fields. If true and parent does not exists then add it to the parent table.")]
        public bool Import { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If true the field takes its roles, otherwise the field takes the view")]
        public bool Precedent { get; set; }

        private string allowCreateRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowCreateRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowCreateRoles))
                //{
                //    //return View.Database.DefaultAllowCreateRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowCreateRoles;
                //}
                if (Precedent)
                    return allowCreateRoles;
                else
                    return View.AllowCreateRoles;
            }
            set
            {
                allowCreateRoles = value;
            }
        }

        private string allowEditRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowEditRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowEditRoles))
                //{
                //    //return View.Database.DefaultAllowEditRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowEditRoles;
                //}
                if (Precedent)
                    return allowEditRoles;
                else
                    return View.AllowEditRoles;
            }
            set
            {
                allowEditRoles = value;
            }
        }

        private string allowSelectRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowSelectRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowSelectRoles))
                //{
                //    //return View.Database.DefaultAllowSelectRoles;
                //    return "everyone";
                //}
                //else
                //{
                //    return allowSelectRoles;
                //}
                if (Precedent)
                    return allowSelectRoles;
                else
                    return View.AllowSelectRoles;
            }
            set
            {
                allowSelectRoles = value;
            }
        }


        [Durados.Config.Attributes.ColumnProperty(Description = "Minimum width of the column (Pixcel)")]
        public int TableCellMinWidth { get; set; }

        private string description;

        [Durados.Config.Attributes.ColumnProperty(Description = "The description of the field, displayed as tooltip")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value == null ? null : value.Replace('"', '\'');
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "For checkbox Horizontal or Vertical label")]
        public Orientation LabelContentLayout { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Include the field in view search")]
        public bool Searchable { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Default value in the filter when the View first display")]
        public virtual string DefaultFilter { get; set; }

        private Validation validation = null;

        //[Durados.Config.Attributes.ParentProperty(TableName = "Validation")]
        public Validation Validation
        {
            get
            {
                if (validation == null)
                {
                    validation = new Validation();
                    validation.SetField(this);
                }
                return validation;
            }
            set
            {
                if (value != null)
                {
                    validation = value;
                }
            }
        }

        public abstract bool GetDbRequired();
        public abstract bool GetDbUnique();
        public abstract bool GetDbNotEditable();

        private bool required;
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Field is required in New / Edit dialog")]
        public bool Required
        {
            get
            {
                if (View.Database.IsApi())
                {
                    return GetDbRequired();
                }

                if (GetDbRequired())
                {
                    return true;
                }
                return required;
                
            }
            set
            {
                if (GetDbRequired())
                {
                    required = true;
                }
                else
                {
                    required = value;
                }
            }
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Refresh the field value when user select the Tab. Use to refresh the display data in a field that has dependency in another Tab")]
        public bool Refresh { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Used as primary key of the record. Used for duplication")]
        public bool Unique {
            get
            {
                return GetDbUnique();
            }
            set
            {

            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool PartFromUniqueIndex { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Save the row changes in the Durados History table. Default true.")]
        public bool SaveHistory { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Name equals value delimeted string where the name is a field name form the search view and the value is a field name from the current view with '$' prefix with '|' as equal sign. For example: 'currentView_fieldName1|$parentView_fieldName1,currentView_fieldName2|$parentView_fieldName2'.")]
        public string SearchFilter { get; set; }

        protected abstract string GetInitialDisplayName();

        public abstract string ConvertToString(DataRow dataRow);
        public abstract object ConvertFromString(string value);

        public abstract string GetValue(DataRow dataRow);

        public abstract string ConvertDefaultToString();

        public abstract FieldType FieldType { get; }

        public abstract ColumnFieldType GetColumnFieldType();

        public bool IsExcluded(DataAction dataAction)
        {
            return ((dataAction == DataAction.Create || dataAction == DataAction.InlineAdding) && IsExcludedInInsert()) || (dataAction == DataAction.Edit && IsExcludedInUpdate());
        }

        public abstract bool IsRowColor
        {
            get;
        }

        public virtual string GetDefaultFilter()
        {
            return DefaultFilter;
        }

        public virtual bool IsNumeric
        {
            get
            {
                switch (GetColumnFieldType())
                {
                    case ColumnFieldType.Integer:
                        return true;
                    case ColumnFieldType.Real:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool IsAutoIdentity
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsAutoGuid
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsGuidIdentity
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsDate
        {
            get
            {
                switch (GetColumnFieldType())
                {
                    case ColumnFieldType.DateTime:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool IsPoint
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsBoolean
        {
            get
            {
                switch (GetColumnFieldType())
                {
                    case ColumnFieldType.Boolean:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool IsDateOnly
        {
            get
            {
                switch (DisplayFormat)
                {
                    case DisplayFormat.Date_mm_dd:
                    case DisplayFormat.Date_dd_mm:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool IsDateTime
        {
            get
            {
                switch (DisplayFormat)
                {
                    case DisplayFormat.Date_mm_dd_12:
                    case DisplayFormat.Date_mm_dd_24:
                    case DisplayFormat.Date_dd_mm_12:
                    case DisplayFormat.Date_dd_mm_24:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public abstract bool ReadOnly { get; }

        public int GetOrder(DataAction dataAction)
        {
            if ((dataAction == DataAction.Create || dataAction == DataAction.InlineAdding) && View.UseOrderForCreate)
                return OrderForCreate;
            else if ((dataAction == DataAction.Edit || dataAction == DataAction.InlineEditing) && View.UseOrderForEdit)
                return OrderForEdit;
            else
                return Order;
        }


        private string format = string.Empty;

        private int min = 0;
        private int max = 0;

        [Durados.Config.Attributes.ColumnProperty()]
        public int Min { get { return min; } set { Validation.HasMin = true; min = value; } } //Min Value|Chars

        [Durados.Config.Attributes.ColumnProperty()]
        public int Max { get { return max; } set { Validation.HasMax = true; max = value; } } //Max Value|Chars

        [Durados.Config.Attributes.ColumnProperty()]
        public string Format
        {
            get
            {
                //if (string.IsNullOrEmpty(format))
                //{

                //    if (FieldType == FieldType.Column && GetColumnFieldType() == ColumnFieldType.DateTime)
                //    {
                //        return View.Database.DateFormat;
                //    }
                //}
                if (FieldType == FieldType.Column && GetColumnFieldType() == ColumnFieldType.DateTime)
                {
                    if (string.IsNullOrEmpty(format) || !View.Database.UseSpecificDateFormats)
                    {
                        return View.Database.DateFormat;
                    }
                }
                return format; // != null ? format : string.Empty

            }
            set
            {
                format = value;
            }
        }




        protected virtual ColumnFieldType GetColumnFieldType(DataColumn dataColumn)
        {
            if (dataColumn.DataType.Equals(typeof(Int32)))
            {
                return ColumnFieldType.Integer;
            }
            else if (dataColumn.DataType.Equals(typeof(Int16)))
            {
                return ColumnFieldType.Integer;
            }
            else if (dataColumn.DataType.Equals(typeof(Int64)))
            {
                return ColumnFieldType.Integer;
            }
            else if (dataColumn.DataType.Equals(typeof(Decimal)))
            {
                return ColumnFieldType.Real;
            }
            else if (dataColumn.DataType.Equals(typeof(Double)))
            {
                return ColumnFieldType.Real;
            }
            else if (dataColumn.DataType.Equals(typeof(Single)))
            {
                return ColumnFieldType.Real;
            }
            else if (dataColumn.DataType.Equals(typeof(Byte)))
            {
                return ColumnFieldType.Integer;
            }
            else if (dataColumn.DataType.Equals(typeof(Boolean)))
            {
                return ColumnFieldType.Boolean;
            }
            else if (dataColumn.DataType.Equals(typeof(DateTime)) || dataColumn.DataType.Equals(typeof(DateTimeOffset)))
            {
                return ColumnFieldType.DateTime;
            }
            else if (dataColumn.DataType.Equals(typeof(Guid)))
            {
                return ColumnFieldType.Guid;
            }
            else
            {
                return ColumnFieldType.String;
            }


        }

        public abstract Field GetTwin(View view);

        public abstract string[] GetColumnsNames();


        public virtual bool IsSearchable()
        {
            return Searchable && !HideInTable;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public abstract object ChangeType(string value);

        public virtual string GetFieldControlType() // yossi
        {
            return string.Empty;
        }

        public static object ChangeType(string value, Type type, string stringConversionFormat)
        {
            if (type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTimeOffset)))
            {
                if (string.IsNullOrEmpty(value))
                    return string.Empty;
                return Durados.DateFormatsMapper.GetDateFromClient(value, stringConversionFormat);
            }
            else if (type.Equals(typeof(Guid)))
            {
                return new Guid(value.Substring(0, 36));
            }
            else if (type == typeof(int) && value.Equals("true"))
            {
                return 1;
            }
            else if (type == typeof(int) && value.Equals("false"))
            {
                return 0;
            }
            else
            {
                return Convert.ChangeType(value, type);
            }

        }

        public abstract bool EqualInDatabase(Field field);

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Display the cloned view name for the related view")]
        public string CloneChildrenViewName { get; set; }

        private string _originalFieldName;


        [Durados.Config.Attributes.ColumnProperty(Description = "Original Field Name - for edit from other view")]
        public string OriginalFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(_originalFieldName))
                {
                    return this.Name;
                }
                return _originalFieldName;
            }

            set
            {
                _originalFieldName = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Related Parent Related Field - for edit from other view")]
        public string OriginalParentRelatedFieldName { get; set; }

        public bool IsFromOtherView()
        {
            return !string.IsNullOrEmpty(OriginalParentRelatedFieldName);
        }


        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Gets or sets the field data type.")]
        public abstract DataType DataType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Gets or sets the field display format, depends on the field data type.")]
        public virtual DisplayFormat DisplayFormat { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Align the content of the field in the grid column.")]
        public virtual TextAlignment TextAlignment { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Set the width of the element in a dialog")]
        public int Width { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Show or hide the column header. Default true.")]
        public bool ShowColumnHeader { get; set; }

        public bool IsAdminPreview { get; set; }

        HashSet<int> plans = null;
        public string Plan { get; set; }

        public bool HiddenAttribute { get; set; }

        public bool HasPlan
        {
            get
            {
                return !View.Database.IsConfig || !string.IsNullOrEmpty(Plan);
            }
        }

        public bool IsInPlan
        {
            get
            {
                if (View.Database.IsConfig && !string.IsNullOrEmpty(Plan))
                {
                    if (plans == null)
                    {
                        plans = new HashSet<int>();

                        foreach (string s in Plan.Split(','))
                        {
                            plans.Add(Convert.ToInt32(s));
                        }
                    }
                }
                else
                {
                    return true;
                }

                return !View.Database.IsConfig || plans.Contains(View.Database.Plan);
            }
        }

        public bool IsImageList()
        {
            return (this.DisplayFormat == DisplayFormat.Tile || this.DisplayFormat == DisplayFormat.Slider);

        }
        public string DictionaryViewFieldName { get; set; }
        public DictionaryType? DictionaryType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Is column Auto increment. Default false.")]
        public bool AutoIncrement { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Auto increment column  sequance. Default false.")]
        public string AutoIncrementSequanceName { get; set; }

        internal protected string jsonName;
        [Durados.Config.Attributes.ColumnProperty()]
        public string JsonName
        {
            get
            {
                if (string.IsNullOrEmpty(jsonName))
                {
                    jsonName = GetDefaultNotDuplicatedJsonName();
                }
                return jsonName;
            }
            set
            {
                jsonName = value;
            }
        }

        private string GetDefaultNotDuplicatedJsonName()
        {
            string jsonName = GetDefaultJsonName();
            if (View.Database.IsConfig)
                return jsonName;
            if (View.Database.IsMain())
                return jsonName;
            int i = 0;
            string tempName = jsonName;
            while (View.Fields.Values.Where(f=>f.ID != ID).Select(f => f.jsonName).Contains(jsonName))
            {
                i++;
                jsonName = tempName + i;
            }

            return jsonName;
        }

        public string GetRestType()
        {
            string restType = "string";

            if (IsPoint)
                return "point";

            switch (DataType)
            {
                case Durados.DataType.Boolean:
                    restType = "boolean";
                    break;
                case Durados.DataType.DateTime:
                    restType = "datetime";
                    break;
                case Durados.DataType.LongText:
                    restType = "text";
                    break;
                case Durados.DataType.MultiSelect:
                    restType = "collection";
                    break;
                case Durados.DataType.Numeric:
                    restType = "float";
                    break;
                case Durados.DataType.SingleSelect:
                    restType = "object";
                    break;
                default:
                    break;
            }

            return restType;
        }

        public virtual bool IsAllow(DataAction dataAction)
        {
            return true;
        }
    }

    public enum ColumnFieldType
    {
        Boolean,
        DateTime,
        Integer,
        Real,
        Guid,
        String
    }


    public enum FieldType
    {
        Column = 1,
        Parent = 2,
        Children = 3
    }

    [Durados.Config.Attributes.EnumDisplay(EnumDisplayNames = new string[7] { "Text", "Number", "True/False", "Relation - Drop Down List", "Relation - Multi Selection List", "Image List", "Date" }, EnumNames = new string[7] { "ShortText", "Numeric", "Boolean", "SingleSelect", "MultiSelect", "Image List", "DateTime" })]
    public enum DataType
    {
        ShortText = 1,
        LongText = 2,
        Image = 3,
        Url = 4,
        Html = 5,
        Numeric = 6,
        Boolean = 7,
        DateTime = 8,
        SingleSelect = 9,
        MultiSelect = 10,
        ImageList = 11,
        Email = 12
    }

    [Durados.Config.Attributes.EnumDisplay(EnumDisplayNames = new string[7] { "Date (05/28/2013)", "Date (28/05/2013)", "DateTime (05/28/2013 01:10:00 PM)", "DateTime (28/05/2013 01:10:00 PM)", "DateTime (05/28/2013 13:10:00)", "DateTime (28/05/2013 13:10:00)", "Custom" }, EnumNames = new string[7] { "Date_mm_dd", "Date_dd_mm", "Date_mm_dd_12", "Date_dd_mm_12", "Date_mm_dd_24", "Date_dd_mm_24", "Date_Custom" })]
    public enum DisplayFormat
    {

        None = 0,
        SingleLine = 1,
        MultiLines = 2,
        Email = 3,
        Password = 4,
        SSN = 5,
        Phone = 6,
        Html = 7,

        MultiLinesEditor = 8,

        GeneralNumeric = 10,
        Currency = 11,
        NumberWithSeparator = 12,
        Percentage = 13,

        DropDown = 21,
        AutoCompleteStratWith = 22,
        AutoCompleteMatchAny = 23,

        Date_mm_dd = 30,
        Date_dd_mm = 31,
        Date_mm_dd_12 = 32,
        Date_dd_mm_12 = 33,
        Date_mm_dd_24 = 34,
        Date_dd_mm_24 = 35,
        TimeOnly = 36,
        Date_Custom = 39,

        Tile = 40,
        Slider = 41,

        Crop = 50,
        Fit = 51,

        Hyperlink = 60,
        ButtonLink = 61,

        Checklist = 70,
        SubGrid = 71
       
    }

    public enum Orientation
    {
        Horizontal,
        Vertical

    }

    public enum GroupFilterDisplayLabel
    {
        Inherit,
        Display,
        Hide
    }

    public enum SpecialColumn
    {
        None,
        Email,
        SSN,
        Credit,
        Phone,
        Zip,
        Currency,
        IP,
        URL,
        Password,
        Html,
        Custom
    }

    public enum AutocompleteMathing
    {
        StartsWith,
        Contains
    }

    public enum TextAlignment
    {
        inherit,
        center,
        left,
        right,
        justify
    }

    public enum DictionaryType
    {

        DisplayNames
       ,
        PlaceHolders
            , InternalNames,
        InternalNamesPlaceHolders
    }
}


namespace System
{
    public static class StringExtension
    {
        public static string GetDecamal(this string s)
        {
            try
            {
                s = s.Trim('_');
                StringBuilder d = new StringBuilder();

                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if (i > 0)
                    {
                        if (Char.IsUpper(c))
                        {
                            if ((i < s.Length - 1 && !char.IsUpper(s[i + 1])) && s[i + 1] != '_' || (!char.IsUpper(s[i - 1])))
                            {
                                if (d[d.Length - 1] != ' ')
                                {
                                    d.Append(" ");
                                }
                            }
                        }
                    }

                    if (c != '_')
                    {
                        d.Append(c);
                    }
                    else
                    {
                        if (d[d.Length - 1] != ' ')
                        {
                            d.Append(" ");
                        }
                    }
                }


                return d.ToString();
            }
            catch
            {
                return s;
            }
        }
    }

    public class Tag
    {
        public string PK
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

    }
}
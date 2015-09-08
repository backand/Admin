using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.HtmlControls;

namespace Durados
{

    public partial class ChildrenField : Field
    {
        public const string FkCounterPrefix = "FkCounter_";
        public const string _bkname_ = "_bkname_";
        
        public ChildrenField(View view, DataRelation dataRelation) :
            base(view)
        {
            DataRelation = dataRelation;
            Sortable = false;
            CounterInitiated = false;
            Counter = false;
            LoadForBlockTemplate = false;
            Searchable = false;
            Required = false;
            AllowDuplication = true;
            HideInTable = true;
            SubGridExport = false;
            SubGridPlacement = 0;
            SubgridInstructions = "To add new {0}, Please fill the required fields and save it first";
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the related View, parent relation if parent field, children relation if children field.")]
        public override string RelatedViewName
        {
            get
            {
                return ChildrenView.Name;
            }
            set
            {
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Gets the equivalent element name in an exported xml file. If left empty then it is equal to the View Name")]
        public string XmlElement { get; set; }

        public string GetXmlElement()
        {
            return string.IsNullOrEmpty(XmlElement) ? ChildrenView.GetXmlElement() : XmlElement;
        }

        public override bool GetDbRequired()
        {
            return false;
        }

        public override bool GetDbNotEditable()
        {
            return false;
        }

        protected bool persist = false;
        public bool Persist
        {
            get
            {
                return persist;
            }

        }

        public override bool ReadOnly
        {
            get
            {
                return false;
            }
        }

        public DataRelation DataRelation { get; private set; }

        public override string GetValue(DataRow dataRow)
        {
            return ConvertToString(dataRow);
        }

        public override string ConvertToString(DataRow dataRow)
        {
            if (HasCounter)
            {
                int? counterValue = GetCounterValue(dataRow);
                string s = string.Empty;
                if (counterValue.HasValue)
                {
                    s = counterValue.Value.ToString();
                }
                else
                {
                    s = "0";
                }
                return DisplayName + " (" + s + ")";
            }
            else
                return DisplayName;
        }

        public bool HasCounter
        {
            get
            {
                return View.Fields.ContainsKey(FkCounterPrefix + Name);
            }
        }

        public int? GetCounterValue(DataRow dataRow)
        {
            if (HasCounter)
            {
                ColumnField counterField = (ColumnField)View.Fields[FkCounterPrefix + Name];
                string s = counterField.GetValue(dataRow);
                if (string.IsNullOrEmpty(s))
                    return null;
                else
                {
                    int value = Convert.ToInt32(s);
                    return value;
                }
            }
            else
                return null;
        }

        public override object ConvertFromString(string value)
        {
            return DisplayName;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool CounterInitiated { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool AllowDuplication { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool SubGridExport { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int SubGridPlacement { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InsideTriggerFieldName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="Subgrid in create dialog instructions")]
        public string SubgridInstructions { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "If sub grid and true then it updates the parent row in the parent grid")]
        public UpdateParent UpdateParent { get; set; }


        public virtual bool IsDuplicable()
        {
            ParentField parentField = GetRelatedParentField();
            if (parentField == null)
                return false;
            return parentField.Integral;

        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Counter { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool LoadForBlockTemplate { get; set; }

        public override string ConvertDefaultToString()
        {
            return DefaultValue == null ? string.Empty : DefaultValue.ToString();
        }

        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ColumnField), Name = "Column")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ParentField), Name = "Parent")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ChildrenField), Name = "Children")]
        [Durados.Config.Attributes.ColumnProperty()]
        public override FieldType FieldType
        {
            get { return FieldType.Children; }
        }

        public override bool IsNumeric
        {
            get
            {
                return false;
            }
        }



        // add default override url navigation
        public override ColumnFieldType GetColumnFieldType()
        {
            throw new NotSupportedException();
        }

        protected override string GetInitialDisplayName()
        {
            return GetFromCache() ?? ChildrenView.DisplayName;
        }

        private string GetFromCache()
        {
            ParentField parentField = GetEquivalentParentField();
            string tableName = parentField.View.DataTable.TableName;
            string columnName = parentField.GetColumnsNames()[0];

            if (View.Database.ForeignKeys.ContainsKey(tableName) && View.Database.ForeignKeys[tableName].ContainsKey(columnName))
            {
                string name = View.Database.ForeignKeys[tableName][columnName];
                if (name.Contains(_bkname_))
                {
                    string[] s = name.Split(new string[] { _bkname_ }, StringSplitOptions.None);
                    return s.Last();
                }
            }


            return null;
        }

        public override bool HideInCreate
        {
            get
            {
                return base.HideInCreate;
            }
            set
            {
                base.HideInCreate = true;
            }
        }

        //public override bool HideInEdit
        //{
        //    get
        //    {
        //        return base.HideInEdit;
        //    }
        //    set
        //    {
        //        base.HideInEdit = true;
        //    }
        //}

        public override string GetFieldControlType()
        {
            return base.GetFieldControlType();
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public override string Name
        {
            get
            {
                return DataRelation.RelationName + "_Children";
            }
        }

        public override string GetPropertyName()
        {
            return Name.Replace("_Children", "");
        }

        public override bool Sortable
        {
            get
            {
                return false;
            }
            set
            {
                base.Sortable = false;
            }
        }

        public View ChildrenView
        {
            get
            {
                View childrenView = View.Database.Views[DataRelation.ChildTable.TableName];
                if (string.IsNullOrEmpty(CloneChildrenViewName))
                {
                    return childrenView;
                }
                else
                {
                    if (!View.Database.Views.ContainsKey(CloneChildrenViewName))
                    {
                        return childrenView;
                    }
                    else
                    {
                        View cloneChildrenView = View.Database.Views[CloneChildrenViewName];

                        if (cloneChildrenView.Base.Name == childrenView.Base.Name)
                        {
                            return cloneChildrenView;
                        }
                        else
                        {
                            return childrenView;
                        }
                    }
                }
            }
        }

        //private string cloneChildrenViewName;
        //[Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Display the children cloned view name for view parent")]
        //public string CloneChildrenViewName { get; set; }
        //{
        //    get
        //    {
        //        return cloneChildrenViewName;
        //    }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value) && View.Database.Views.ContainsKey(value))
        //        {
        //            cloneChildrenViewName = value;
        //            View cloneChildrenView = View.Database.Views[CloneChildrenViewName];
        //            ParentField cloneParentField = GetRelatedParentField(cloneChildrenView);
        //            cloneParentField.CloneParentViewName = cloneChildrenViewName;
        //        }
        //        else
        //        {
        //            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(cloneChildrenViewName))
        //            {
        //                View cloneChildrenView = View.Database.Views[CloneChildrenViewName];
        //                ParentField cloneParentField = GetRelatedParentField(cloneChildrenView);
        //                cloneParentField.CloneParentViewName = null;
        //            }
        //        }
        //    }
        //}


        private ParentField relatedParentField = null;

        public ParentField GetRelatedParentField()
        {
            return GetRelatedParentField(ChildrenView);
        }

        private ParentField GetRelatedParentField(View childrenView)
        {
            if (relatedParentField != null)
                return relatedParentField;

            foreach (ParentField parentField in childrenView.Fields.Values.Where(parentField => parentField.FieldType == FieldType.Parent))
            {
                if (DataRelation.RelationName == parentField.DataRelation.RelationName)
                {
                    relatedParentField = parentField;
                    break;
                }
            }

            if (relatedParentField == null)
                throw new DuradosException("Parent children relation is invalid.");

            return relatedParentField;
        }

        public ParentField GetEquivalentParentField()
        {
            foreach (ParentField field in ChildrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (field.DataRelation.Equals(DataRelation))
                {
                    return field;
                }
            }

            return null;
        }

        public ParentField GetFirstNonEquivalentParentField()
        {
            ParentField equivalent = GetEquivalentParentField();

            //changed by br 2
            return ChildrenView.Fields.Values.FirstOrDefault(f => f.FieldType == FieldType.Parent && f.Name != equivalent.Name) as ParentField;
            //return (ParentField)ChildrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent && f.Name != equivalent.Name).First();
        }


        public override object ChangeType(string value)
        {
            throw new NotSupportedException();
        }



        [Durados.Config.Attributes.ColumnProperty()]
        public string DependencyFieldName { get; set; }

        //public ParentField DependencyField
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(DependencyFieldName))
        //            return null;
        //        if (!ParentView.Fields.ContainsKey(DependencyFieldName))
        //            return null;
        //        return (ParentField)ParentView.Fields[DependencyFieldName];
        //    }
        //}

        //protected string insideTriggerFieldName;
        //[Durados.Config.Attributes.ColumnProperty()]
        //public string InsideTriggerFieldName
        //{
        //    get
        //    {
        //        return insideTriggerFieldName;
        //    }
        //    set
        //    {
        //        insideTriggerFieldName = value;

        //        //if (insideTriggerFieldName != null && ChildrenView.Fields.ContainsKey(insideTriggerFieldName))
        //        //    ((ParentField)ParentView.Fields[insideTriggerFieldName]).DependencyTriggeredFields.Add(this);
        //    }
        //}

        //public Field InsideTriggerField
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(InsideTriggerFieldName))
        //            return null;
        //        if (!View.Fields.ContainsKey(InsideTriggerFieldName))
        //            return null;
        //        return View.Fields[InsideTriggerFieldName];
        //    }
        //}

        public override bool EqualInDatabase(Field field)
        {
            if (field.FieldType != FieldType.Children)
                return false;

            return EqualInDatabase((ChildrenField)field);
        }

        public bool EqualInDatabase(ChildrenField field)
        {
            if (field.DataRelation.ParentColumns.Length != DataRelation.ParentColumns.Length)
                return false;

            for (int i = 0; i < field.DataRelation.ParentColumns.Length; i++)
            {
                if (!field.DataRelation.ParentColumns[i].ColumnName.Equals(DataRelation.ParentColumns[i].ColumnName))
                    return false;
            }

            return true;
        }


        public override Field GetTwin(View view)
        {
            return view.GetTwin(this);
        }

        public override string[] GetColumnsNames()
        {
            int len = DataRelation.ParentColumns.Length;
            string[] names = new string[len];
            for (int i = 0; i < len; i++)
            {
                names[i] = DataRelation.ParentColumns[i].ColumnName;
            }
            return names;
        }

        public override bool IsRowColor
        {
            get
            {
                return false;
            }
        }

        public View GetOtherParentView(out ParentField parentField, out ParentField fkField)
        {
            parentField = null;
            fkField = null;

            View childrenView = ChildrenView;
            View view = View;
            View parentView = null;

            var parentFields = childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent);

            foreach (ParentField field in parentFields)
            {
                if (!field.ParentView.Base.Equals(view.Base))
                {
                    parentField = field;
                    parentView = field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }

            if (parentView == null)
            {
                if (parentFields.Count() == 2)
                {
                    if (((ParentField)parentFields.FirstOrDefault()).ParentView.Equals(((ParentField)parentFields.LastOrDefault()).ParentView))
                    {
                        ParentField p1 = (ParentField)parentFields.FirstOrDefault();
                        ParentField p2 = (ParentField)parentFields.LastOrDefault();

                        if (!p1.DataRelation.ChildColumns[0].Equals(this.DataRelation.ChildColumns[0]))
                        {
                            parentField = p1;
                            fkField = p2;
                        }
                        else
                        {
                            parentField = p2;
                            fkField = p1;
                        }
                        parentView = parentField.ParentView;
                    }
                }
            }

            return parentView;
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Gets or sets the field data type.")]
        public override DataType DataType
        {
            get
            {
                return DataType.MultiSelect;
            }
            set
            {
                
            }
        }

        public override string GetDefaultJsonName()
        {
            return DisplayName.ReplaceNonAlphaNumeric2();
        }

        protected override object GetDbDefaultValue()
        {
            return null;
        }
    }

    public enum UpdateParent
    {
        No = 0,
        Row = 1,
        Grid = 2,

    }
}

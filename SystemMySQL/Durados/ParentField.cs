using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Durados
{
    public partial class ParentField : Field
    {
        public delegate void BeforeDropDownOptionsEventHandler(object sender, BeforeDropDownOptionsEventArgs e);
        public event BeforeDropDownOptionsEventHandler BeforeDropDownOptions;


        public List<ParentField> DependencyTriggeredFields { get; private set; }

        public List<ParentField> GetDependencyDynasty()
        {
            List<ParentField> dependencyFields = new List<ParentField>();

            ParentField dependencyField = DependencyField;

            while (dependencyField != null)
            {
                dependencyFields.Add(dependencyField);

                dependencyField = dependencyField.DependencyField;
            }

            return dependencyFields;
        }

        public string[] GetDependencyTriggeredFieldNames()
        {
            List<string> names = new List<string>();

            foreach (ParentField field in DependencyTriggeredFields)
            {
                names.Add(field.Name);
            }

            return names.ToArray();
        }

        public ParentField(View view, DataRelation dataRelation) :
            base(view)
        {
            DependencyTriggeredFields = new List<ParentField>();
            DataRelation = dataRelation;
            Sortable = true;
            Searchable = false;
            Required = GetRequired();
            Integral = false;
            IncludeInDuplicate = true;

            if (dataRelation.ChildColumns[0].ColumnName == View.CreatedByColumnName || dataRelation.ChildColumns[0].ColumnName == View.ModifiedByColumnName)
            {
                HideInCreate = true;
                HideInEdit = true;
            }

            foreach (DataColumn dataColumn in dataRelation.ChildColumns)
            {
                if (dataColumn.ReadOnly)
                {
                    HideInCreate = true;
                    HideInEdit = true;
                    ExcludeInInsert = true;
                    ExcludeInUpdate = true;
                    return;
                }
            }

        }

        public virtual bool HasDependencyFilter()
        {
            return DependencyField != null;
        }

        public string[] GetAttributesNames()
        {
            if (string.IsNullOrEmpty(AttributesNames))
                return new string[1] { Name };
            return AttributesNames.Split(',');
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the equivalent fields to the XmlAttributes from the parent table, in an expoprted xml. comma delimited")]
        public string XmlFields { get; set; }
        
        public Field[] GetXmlFields()
        {
            if (string.IsNullOrEmpty(XmlFields))
                return new Field[1] { ParentView.DisplayField };

            List<Field> fields = new List<Field>();

            foreach (string fieldName in XmlFields.Split(','))
            {
                if (!ParentView.Fields.ContainsKey(fieldName))
                {
                    throw new DuradosException("The view " + ParentView.DisplayName + " does not have the field " + fieldName + ". Check the XmlFields configuration.");
                }

                Field field = ParentView.Fields[fieldName];
                fields.Add(field);
            }

            return fields.ToArray();
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "A field to be displayed from the parent View. If left empty it the takes the display name from the parent view.")]
        public string DisplayField { get; set; }

        public Field GetDisplayField()
        {
            if (string.IsNullOrEmpty(DisplayField))
            {
                return ParentView.DisplayField;
            }
            else
            {
                if (ParentView.Fields.ContainsKey(DisplayField))
                {
                    return ParentView.Fields[DisplayField];
                }
                else
                {
                    return ParentView.DisplayField;
                }
            }
        }

        public DataColumn[] GetDataColumns()
        {
            return DataRelation.ChildColumns;
        }

        public override bool ReadOnly
        {
            get
            {
                return GetDataColumns()[0].ReadOnly;
            }
        }

        public override bool IsSearchable()
        {
            return false;
        } 

        [Durados.Config.Attributes.ColumnProperty()]
        public string DependencyFieldName { get; set; }
        
        public ParentField DependencyField
        {
            get
            {
                if (string.IsNullOrEmpty(DependencyFieldName))
                    return null;
                if (ParentView.Fields.ContainsKey(DependencyFieldName))
                    return (ParentField)ParentView.Fields[DependencyFieldName];
                else if (this.InsideTriggerField == null)
                    return null;
                else
                    if (((ParentField)InsideTriggerField).ParentView.Fields.ContainsKey(DependencyFieldName))
                        return (ParentField)((ParentField)InsideTriggerField).ParentView.Fields[DependencyFieldName];

                return null;
            }
        }

        protected string insideTriggerFieldName;
        [Durados.Config.Attributes.ColumnProperty()]
        public string InsideTriggerFieldName 
        {
            get
            {
                return insideTriggerFieldName;
            }
            set
            {
                insideTriggerFieldName = value;

                if (insideTriggerFieldName != null && ParentView.Fields.ContainsKey(insideTriggerFieldName))
                    ((ParentField)ParentView.Fields[insideTriggerFieldName]).DependencyTriggeredFields.Add(this);
            }
        }

        public Field InsideTriggerField
        {
            get
            {
                if (string.IsNullOrEmpty(InsideTriggerFieldName))
                    return null;
                if (!View.Fields.ContainsKey(InsideTriggerFieldName))
                    return null;
                return View.Fields[InsideTriggerFieldName];
            }
        }

        private Field[] insideTriggeredFields = null;

        public Field[] InsideTriggeredFields
        {
            get
            {
                //if (insideTriggeredFields == null)
                //{
                    insideTriggeredFields = GetInsideTriggeredFields();
                //}

                return insideTriggeredFields;
            }
        }

        private string[] insideTriggeredFieldsNames = null;

        public string[] InsideTriggeredFieldsNames
        {
            get
            {
                if (insideTriggeredFieldsNames == null)
                {
                    insideTriggeredFieldsNames = GetInsideTriggeredFieldsNames();
                }

                return insideTriggeredFieldsNames;
            }
        }

        private string[] GetInsideTriggeredFieldsNames()
        {
            List<string> insideTriggeredFieldsNames = new List<string>();

            foreach (Field field in InsideTriggeredFields)
            {
                insideTriggeredFieldsNames.Add(field.Name);
            }

            return insideTriggeredFieldsNames.ToArray();
        }

        //private ParentField[] GetInsideTriggeredFields()
        //{
        //    List<ParentField> insideTriggeredFields = new List<ParentField>();

        //    foreach (ParentField field in View.Fields.Values.Where(f => f.FieldType == FieldType.Parent && ((ParentField)f).InsideTriggerField != null))
        //    {
        //        if (field.InsideTriggerField.Equals(this))
        //        {
        //            insideTriggeredFields.Add(field);
        //        }
        //    }

        //    return insideTriggeredFields.ToArray();
        //}

        public bool HasInsideTriggeredFields
        {
            get
            {
                return InsideTriggeredFields.Length > 0;
            }
        }

        private Field[] GetInsideTriggeredFields()
        {
            List<Field> insideTriggeredFields = new List<Field>();

            foreach (ParentField field in View.Fields.Values.Where(f => f.FieldType == FieldType.Parent && ((ParentField)f).InsideTriggerField != null))
            {
                if (field.InsideTriggerField.Equals(this))
                {
                    insideTriggeredFields.Add(field);
                }
            }

            foreach (ChildrenField field in View.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).InsideTriggerFieldName != null))
            {
                if (field.InsideTriggerFieldName.Equals(this.Name))
                {
                    insideTriggeredFields.Add(field);
                }
            }

            return insideTriggeredFields.ToArray();
        }

        public DataRelation DataRelation { get; private set; }
        
        protected bool GetRequired()
        {
            foreach (DataColumn column in DataRelation.ChildColumns)
            {
                if (!column.AllowDBNull)
                    return true;
            }
            return false;
        
        }

        public override bool GetDbRequired()
        {
            return GetRequired();
        }

        public override bool GetDbNotEditable()
        {
            return DataRelation.ChildColumns[0].ExtendedProperties.ContainsKey("NotInEditable"); 
        }

        public override string ConvertDefaultToString()
        {
            return DefaultValue == null ? string.Empty : DefaultValue.ToString();
        }

        public override string GetValue(DataRow dataRow)
        {
            return View.GetFkValue(dataRow, DataRelation.RelationName);
        }
        

        public override string ConvertToString(DataRow dataRow)
        {
            DataRow parentRow = dataRow.GetParentRow(DataRelation.RelationName);

            if (parentRow == null)
            {
                return NullString;
            }

            return GetDisplayField().ConvertToString(parentRow);

        }

        public override object ConvertFromString(string value)
        {
            return GetDisplayField().ConvertFromString(value);


        }

        public override ColumnFieldType GetColumnFieldType()
        {
            return GetColumnFieldType(GetDisplayDataColumn());
        }

        public DataColumn GetDisplayDataColumn(bool useDisplayField=false)
        {
            View parentView = ParentView;
            Field field ;
            bool hasValidDisplayField = !string.IsNullOrEmpty(DisplayField) && parentView.Fields.ContainsKey(DisplayField);
            
            if (useDisplayField && hasValidDisplayField)
                field = parentView.Fields[DisplayField];
            else
            {
                field = parentView.DisplayField;
            }

            if (useDisplayField && !string.IsNullOrEmpty(DisplayField) && !hasValidDisplayField)
                field.View.Database.Logger.Log("", "", "GetDisplayDataColumn", "Drop down " + Name + " has display field which does not exits in the parent view" + parentView.Name,string.Empty, 1, string.Empty,DateTime.Now);

            while (field.FieldType == FieldType.Parent)
            {
                parentView = ((ParentField)field).ParentView;
                field = parentView.DisplayField;
            }

            if (field.FieldType == Durados.FieldType.Children)
            {
                foreach (Field field2 in parentView.Fields.Values)
                {
                    if (field2.FieldType == Durados.FieldType.Column)
                    {
                        field = field2;
                        parentView.DisplayColumn = ((ColumnField)field).DataColumn.ColumnName;
                    }
                }
            }
            return ((ColumnField)field).DataColumn;
        }


        protected override string GetInitialDisplayName()
        {
            if (DataRelation.ChildColumns.Count() == 1)
            {
                return DataRelation.ChildColumns[0].ColumnName.GetDecamal();
            }
            else
            {
                
                return ParentView.DisplayName;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool InlineAdding { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool InlineEditing { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool InlineDuplicate { get; set; }

        
        [Durados.Config.Attributes.ColumnProperty()]
        public string InlineSearchView { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Integral { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public override string Name
        {
            get
            {
                return DataRelation.RelationName + "_Parent";
            }
        }

        public override string GetPropertyName()
        {
            return Name.Replace("_Parent", "");
        }

        //public View ParentView
        //{
        //    get
        //    {
        //        return View.Database.Views[DataRelation.ParentTable.TableName];
        //    }
        //}

        public View ParentView
        {
            get
            {
                View parentView = View.Database.Views[DataRelation.ParentTable.TableName];
                if (string.IsNullOrEmpty(CloneParentViewName))
                {
                    return parentView;
                }
                else
                {
                    if (!View.Database.Views.ContainsKey(CloneParentViewName))
                    {
                        return parentView;
                    }
                    else
                    {
                        View cloneParentView = View.Database.Views[CloneParentViewName];

                        if (cloneParentView.Base.Name == parentView.Base.Name)
                        {
                            return cloneParentView;
                        }
                        else
                        {
                            return parentView;
                        }
                    }
                }
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the related View, parent relation if parent field, children relation if children field.")]
        public override string RelatedViewName
        {
            get
            {
                return ParentView.Name;
            }
            set
            {
            }
        }

        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ColumnField), Name = "Column")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ParentField), Name = "Parent")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ChildrenField), Name = "Children")]
        [Durados.Config.Attributes.ColumnProperty()]
        public override FieldType FieldType
        {
            get { return FieldType.Parent; }
        }

        public string GetSortByParent()
        {
            if (string.IsNullOrEmpty(ParentView.OrdinalColumnName))
            {
                if (ParentView.DisplayField.FieldType == FieldType.Parent)
                {
                    return ((ParentField)ParentView.DisplayField).GetSortByParent();
                }
                else
                {
                    return ParentView.DisplayField.Name;
                }
            }
            else
            {
                return ParentView.OrdinalColumnName;
            }
        }

        public string GetSortByParentTableName()
        {
            if (string.IsNullOrEmpty(ParentView.OrdinalColumnName))
            {
                if (ParentView.DisplayField.FieldType == FieldType.Parent)
                {
                    return ((ParentField)ParentView.DisplayField).GetSortByParentTableName();
                }
                else
                {
                    return ParentView.DataTable.TableName;
                }
            }
            else
            {
                return ParentView.DataTable.TableName;
            }
        }
        


        public virtual void OnBeforeDropDownOptions(BeforeDropDownOptionsEventArgs e)
        {
            if (BeforeDropDownOptions != null)
            {
                BeforeDropDownOptions(this, e);
            }
        }

        public string CloneParentViewName
        {
            get
            {
                return CloneChildrenViewName;
            }
        }
        

        private ChildrenField relatedChildrenField = null;

        public ChildrenField GetRelatedChildrenField()
        {
            if (relatedChildrenField != null)
                return relatedChildrenField;

            foreach (ChildrenField childrenField in ParentView.Fields.Values.Where(childrenField => childrenField.FieldType == FieldType.Children))
            {
                if (DataRelation.RelationName == childrenField.DataRelation.RelationName)
                {
                    relatedChildrenField = childrenField;
                    break;
                }
            }

            if (relatedChildrenField == null)
                throw new DuradosException("Parent children relation is invalid.");

            return relatedChildrenField;
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public string BaseFieldName { get; set; }

        public ParentField BaseField
        {
            get
            {
                if (string.IsNullOrEmpty(BaseFieldName))
                    return null;

                if (View.IsDerived)
                    if (View.BaseView.Fields.ContainsKey(BaseFieldName))
                        return (ParentField)View.BaseView.Fields[BaseFieldName];

                return null;
            }
        }


        public virtual string[] SplitValues(string values)
        {
            if (string.IsNullOrEmpty(values))
                return new string[0];

            string[] fk = values.Split(',');

            List<string> multiValues = new List<string>();
            int fkLength = fk.Length;
            DataColumn[] fkColumns = DataRelation.ChildColumns;
            int fkColumnsLength = fkColumns.Length;

            string s = string.Empty;
            for (int j = 0; j < fkLength; j++)
            {
                if (j % fkColumnsLength == 0 && s != string.Empty)
                {
                    multiValues.Add(s.Remove(s.Length - 1));
                    s = string.Empty;
                }
                s += fk[j] + ',';
            }
            multiValues.Add(s.Remove(s.Length - 1));

            return multiValues.ToArray();
        }


        public override object ChangeType(string value)
        {
            if (DataRelation.ParentColumns.Length != 1)
                throw new DuradosException("Only relevant to single coumn relations");

            Type type = DataRelation.ParentColumns[0].DataType;
            if (GetColumnFieldType() == ColumnFieldType.DateTime)
            {
                if (string.IsNullOrEmpty(value))
                    return string.Empty;
                return Durados.DateFormatsMapper.GetDateFromClient(value, this.Format);
            }
            else if (type.Equals(typeof(Guid)))
            {
                return new Guid(value.Substring(0, 36));
            }
            else
            {
                return Convert.ChangeType(value, type);
            }

        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SelectionSortColumn { get; set; }

        public override bool EqualInDatabase(Field field)
        {
            if (field.FieldType != FieldType.Parent)
                return false;

            return EqualInDatabase((ParentField)field);
        }

        public bool EqualInDatabase(ParentField field)
        {
            if (field.DataRelation.ChildColumns.Length != DataRelation.ChildColumns.Length)
                return false;

            for (int i = 0; i < field.DataRelation.ChildColumns.Length; i++)
            {
                if (!field.DataRelation.ChildColumns[i].ColumnName.Equals(DataRelation.ChildColumns[i].ColumnName))
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
            int len = DataRelation.ChildColumns.Length;
            string[] names = new string[len];
            for (int i = 0; i < len; i++)
            {
                names[i] = DataRelation.ChildColumns[i].ColumnName;
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


        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Gets or sets the field data type.")]
        public override DataType DataType
        {
            get
            {
                if (IsImageList())
                    return DataType.ImageList;
                return DataType.SingleSelect;
            }
            set
            {

            }
        }

        public ChildrenField GetEquivalentChildrenField()
        {
            foreach (ChildrenField field in ParentView.Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                if (field.DataRelation.Equals(DataRelation))
                {
                    return field;
                }
            }

            return null;
        }

        public override string GetDefaultJsonName()
        {
            return DisplayName.ReplaceNonAlphaNumeric2();
        }

    }



    public class BeforeDropDownOptionsEventArgs : EventArgs
    {
        private ParentField parentField;
        private string sql;
        public BeforeDropDownOptionsEventArgs(ParentField parentField, string sql)
            : base()
        {
            this.parentField = parentField;
            this.sql = sql;
        }

        public ParentField ParentField
        {
            get
            {
                return parentField;
            }

        }

        public string Sql
        {
            get
            {
                return sql;
            }
            set
            {
                sql = value;
            }
        }
    }
}

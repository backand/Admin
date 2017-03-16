using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Durados
{
    public partial class ColumnField : Field
    {
        
        public ColumnField(View view, DataColumn dataColumn):
            base(view)
        {
            DataColumn = dataColumn;


            ColumnFieldType = GetColumnFieldType(dataColumn);

            ColSpanInDialog = DataColumn.MaxLength > 260 ? 2 : 1;

            if (dataColumn.AutoIncrement || dataColumn.ColumnName == View.CreateDateColumnName || dataColumn.ColumnName == View.ModifiedDateColumnName)
            {
                HideInCreate = true;
                //HideInEdit = true;
            }

            if (dataColumn.ReadOnly || dataColumn.DataType.Equals(typeof(byte[])) || (view.DataTable.PrimaryKey.Length == 1 && view.DataTable.PrimaryKey[0].Equals(dataColumn) && dataColumn.DataType.Equals(typeof(Guid))) || dataColumn.DataType.Equals(typeof(DateTimeOffset)))
            {
                HideInCreate = true;
                //HideInEdit = true;
                ExcludeInInsert = true;
                ExcludeInUpdate = true;
            }

            if (IsAutoGuid)
            {
                HideInCreate = true;
                //HideInEdit = true;
                ExcludeInInsert = true;
                ExcludeInUpdate = true;
            }

            Searchable = (ColumnFieldType == ColumnFieldType.String);
            Required = GetRequired();
            TrimSpaces = true;
            IncludeInDuplicate = true;
            Encrypted = false;
            EncryptedNameSuffix = "_Encrypted";

            SetValidationProperties();
        }

        

        public virtual Dictionary<string, string> GetSelectOptions()
        {
            return null;
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "The name of the related View, parent relation if parent field, children relation if children field.")]
        public override string RelatedViewName
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public string GetAttributeName()
        {
            if (string.IsNullOrEmpty(AttributesNames))
                return Name;
            return AttributesNames;
        }

        protected bool GetRequired()
        {
            return !DataColumn.AllowDBNull && !DataColumn.AutoIncrement && !IsAutoGuid; 
        }

        public override bool IsAutoGuid
        {
            get
            {
                if (DataColumn.Table.PrimaryKey.Length  == 0)
                    return false;
                return View.Database.PkType == Database.AutoGuidPkType && DataColumn == DataColumn.Table.PrimaryKey[0] && DataColumn.DataType == typeof(string) && DataColumn.MaxLength == 36;
            }
        }

        public override bool GetDbRequired()
        {
            return GetRequired();
        }

        public override bool GetDbUnique()
        {
            return DataColumn.Unique; 
        }

        public override bool GetDbNotEditable()
        {
            return DataColumn.ExtendedProperties.ContainsKey("NotInEditable");
        }

        private void SetValidationProperties() // yossi
        {            
                if (ColumnFieldType == ColumnFieldType.String)
                {
                    Int64 length = DataColumn.MaxLength;

                    if (!Validation.HasMax && length > 0)
                    {
                        if (length < 8001)
                        {
                            if (length > 4000)
                            {
                                Max = 4000;
                            }
                            else
                            {
                                Max = (int)length;
                            }
                        }
                    }

                }
                else if (DataColumn.DataType.Equals(typeof(Int32)))
                {
                    Max = Int32.MaxValue;
                    Min = Int32.MinValue;
                }
                else if (DataColumn.DataType.Equals(typeof(Int16)))
                {
                    Max = Int16.MaxValue;
                    Min = Int16.MinValue;
                }
                else if (DataColumn.DataType.Equals(typeof(Int64)))
                {
                    Max = Int32.MaxValue;
                    Min = Int32.MinValue;
                }
                else if (DataColumn.DataType.Equals(typeof(Decimal)))
                {
                    Max = Int32.MaxValue;
                    Min = Int32.MinValue;
                }
                else if (DataColumn.DataType.Equals(typeof(Double)))
                {
                    Max = Int32.MaxValue;
                    Min = Int32.MinValue;
                }
                else if (DataColumn.DataType.Equals(typeof(Single)))
                {
                    Max = Int32.MaxValue;
                    Min = Int32.MinValue;
                }
        }


        public override bool ReadOnly
        {
            get
            {
                return DataColumn.ReadOnly;
            }
        }

        public virtual bool IsMilestonesField
        {
            get
            {
                return false;
            }
        }

        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ColumnField), Name = "Column")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ParentField), Name = "Parent")]
        [Durados.Config.Attributes.DerivedTypeProperty(Type = typeof(ChildrenField), Name = "Children")]
        [Durados.Config.Attributes.ColumnProperty()]
        public override FieldType FieldType
        {
            get { return FieldType.Column; }
        }

        //need a better solution

        public override ColumnFieldType GetColumnFieldType()
        {
            return GetColumnFieldType(this.DataColumn);
        }



        [Durados.Config.Attributes.ColumnProperty()]
        public override string Name
        {
            get
            {
                return DataColumn.ColumnName;
            }
        }

        public override string GetPropertyName()
        {
            return Name;
        }
        
        protected override string GetInitialDisplayName()
        {
            return (string.IsNullOrEmpty(DataColumn.Caption)) ? DataColumn.ColumnName.GetDecamal() : DataColumn.Caption.GetDecamal();
        }


        public DataColumn DataColumn { get; private set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AutocompleteColumn { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        public string AutocompleteDisplayColumn { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AutocompleteTable { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AutocompleteSql { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        public string AutocompleteConnectionString { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "Field name for DropDown display.")]
        public string DropDownDisplayField { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Field name for DropDown value.")]
        public string DropDownValueField { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="View name for CheckList source.")]
        public string MultiValueParentViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Comma deiemited keys values for CheckList source.")]
        public string MultiValueAdditionals { get; set; }

        //[Durados.Config.Attributes.ColumnProperty(Description = "Comma deiemited keys values for CheckList to exclude.")]
        public string MultiValueExclude { get; set; }

        public ColumnFieldType ColumnFieldType { get; private set; }

        public override string GetValue(DataRow dataRow)
        {
            //if (!dataRow.Table.Columns.Contains(DataColumn.ColumnName))
            //{
            //    if (View.Database.IsConfig)
            //    {
            //        return GetDefaultValue(View.Name, DataColumn.ColumnName);
            //    }
            //    return null;
            //}
            object value = dataRow[DataColumn.ColumnName];
            if (value is string && this.GetColumnFieldType() == ColumnFieldType.String)
                value = ((string)value).Trim();
            return ConvertValueToString(value);
        }

        //protected virtual string GetDefaultValue(string viewName, string columnFieldName)
        //{
        //    return null;
        //}

        public override string ConvertToString(DataRow dataRow)
        {
            if (dataRow == null)
                return null;
            object value = dataRow[DataColumn.ColumnName];
            if (value is string && this.GetColumnFieldType() == ColumnFieldType.String)
                value = ((string)value).Trim();
            return ConvertValueToString(value);
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool TrimSpaces { get; set; }


        public override string ConvertDefaultToString()
        {
            return ConvertValueToString(DefaultValue);
        }

        public bool ConvertDefaultToBool()
        {
            if (DataColumn.DataType.Equals(typeof(Boolean)))
            {
                return Convert.ToBoolean(DefaultValue);
            }

            return false;
        }

        //public override bool HideInTable
        //{
        //    get
        //    {
        //        return (DisplayFormat == Durados.DisplayFormat.Password) ? true : base.HideInTable;
        //    }
        //    set
        //    {
        //        base.HideInTable = value;
        //    }
        //}

        //need a better solution
        public string ConvertValueToString(object value)
        {
            string s;

            if (value == null || value.Equals(DBNull.Value))
            {
                s = NullString;
            }
            else if (DataColumn.DataType.Equals(typeof(Int32)))
            {
                s = Convert.ToInt32(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Int16)))
            {
                s = Convert.ToInt16(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Int64)))
            {
                s = Convert.ToInt64(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(decimal)))
            {
                s = Convert.ToDecimal(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Double)))
            {
                s = Convert.ToDouble(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Single)))
            {
                s = Convert.ToSingle(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Byte)))
            {
                s = Convert.ToByte(value).ToString(this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Boolean)))
            {

                //bool b = false;
                //if (value != null && !value.Equals(string.Empty))
                //{
                //    if (!bool.TryParse(value.ToString(), out b))
                //    {
                //        b = (value.Equals("1") || value.ToString().ToLower().Equals("yes") || value.ToString().ToLower().Equals("true"));
                //    }
                //}
                //else
                //{
                //    b = Convert.ToBoolean(value);
                //}
                //if (b)
                //    s = View.Database.True;
                //else
                //    s = View.Database.False;

                bool b = Convert.ToBoolean(value);
                if (b)
                    s = View.Database.True;
                else
                    s = View.Database.False;
            }
            else if (DataColumn.DataType.Equals(typeof(DateTime)))
            {
                s = ConvertDateToString(value);
            }
            else if (DataColumn.DataType.Equals(typeof(byte[])))
            {
                byte[] byteArray = (byte[])value;
                //if (byteArray.Count() > 250)
                return ASCIIEncoding.ASCII.GetString(byteArray);
                //else
                //{
                //    s = string.Empty;
                //    for (int i = 0; i < byteArray.Length; i++)
                //    {
                //        s += byteArray[i].ToString();
                //    }
                //}
            }
            else
            {
                s = value.ToString();
            }

            return s;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Encrypt { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public bool Encrypted { get; set; }

        
        public string EncryptedNameSuffix { get; set; }

        public string EncryptedName
        {
            get
            {
                return DatabaseNames;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CertificateName { get; set; }

        public string GetCertificateName()
        {
            if (string.IsNullOrEmpty(CertificateName))
                return View.Database.DefaultCertificateName;
            else
                return CertificateName;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SymmetricKeyName { get; set; }

        public string GetSymmetricKeyName()
        {
            if (string.IsNullOrEmpty(SymmetricKeyName))
                return View.Database.DefaultSymmetricKeyName;
            else
                return SymmetricKeyName;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public SymmetricKeyAlgorithm SymmetricKeyAlgorithm { get; set; }

        
        public string ConvertDateToString(object value)
        {
            string s = string.Empty;
            DateTime result = DateTime.Now;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(this.View.Database.DefaultCulture);

            if (DateTime.TryParse(value.ToString(), out result))
            {
                s = Convert.ToDateTime(value).ToString(this.Format);
            }
            else
            {
                s = DateTime.Now.ToString(this.Format);
            }

            return s;
        }

        //need a better solution
        public override object ConvertFromString(string value)
        {
            if (DataColumn.DataType.Equals(typeof(DateTime)))
            {
                if (string.IsNullOrEmpty(value))
                    return string.Empty;
                return Durados.DateFormatsMapper.GetDateFromClient(value, this.Format);
            }
            else if (DataColumn.DataType.Equals(typeof(Boolean)))
            {
                if (value == View.Database.True)
                {
                    return true;
                }
                else if (value == View.Database.False)
                {
                    return false;
                }
            }
            else if (DataColumn.DataType.Equals(typeof(byte[])))
            {
                byte[] byteArray = new byte[value.Length];

                return ASCIIEncoding.ASCII.GetBytes(value);
                //if (value.Length > 250)
                //    return ASCIIEncoding.ASCII.GetBytes(value);
                //else
                //{
                //    for (int i = 0; i < value.Length; i++)
                //    {
                //        byteArray[i] = Convert.ToByte(value[i]);
                //    }
                //}
                //return byteArray;
            }

            return value;
        }

        public override object ChangeType(string value)
        {
            Type type = this.DataColumn.DataType;
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

        public override bool EqualInDatabase(Field field)
        {
            if (field.FieldType != FieldType.Column)
                return false;

            return EqualInDatabase((ColumnField)field);
        }

        public bool EqualInDatabase(ColumnField field)
        {
            return (field.DataColumn.ColumnName.Equals(DataColumn.ColumnName));
        }

        public override Field GetTwin(View view)
        {
            if (view.Fields.ContainsKey(Name))
                return view.Fields[Name];

            return null;
        }

        public override string[] GetColumnsNames()
        {
            return new string[1] { DataColumn.ColumnName };
        }

        public override bool IsRowColor
        {
            get
            {
                return View.RowColorColumnName == DataColumn.ColumnName;
            }
        }


        [Durados.Config.Attributes.ParentProperty(TableName = "Milestone")]
        public Milestone Milestone { get; set; }

        protected virtual bool IsImage()
        {
            return false;
        }

        protected virtual bool IsUrl()
        {
            return false;
        }

        public override bool IsAutoIdentity
        {
            get
            {
                return View.DataTable.PrimaryKey.Count() == 1 && View.DataTable.PrimaryKey[0].ColumnName == DataColumn.ColumnName && DataColumn.AutoIncrement;
            }
        }

        public override bool IsGuidIdentity
        {
            get
            {
                return View.DataTable.PrimaryKey.Count() == 1 && View.DataTable.PrimaryKey[0].ColumnName == DataColumn.ColumnName && DataColumn.DataType.Equals(typeof(Guid));
            }
        }

        public override bool IsPoint
        {
            get
            {
                return DataColumn.MaxLength == 100001;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Gets or sets the field data type.")]
        public override DataType DataType
        {
            get
            {
                ColumnFieldType columnFieldType = GetColumnFieldType();
                switch (columnFieldType)
                {
                    case ColumnFieldType.Boolean:
                        return DataType.Boolean;

                    case ColumnFieldType.DateTime:
                        return DataType.DateTime;

                    case ColumnFieldType.Integer:
                        return DataType.Numeric;

                    case ColumnFieldType.Real:
                        return DataType.Numeric;

                    case ColumnFieldType.String:
                        if (IsImage() || DisplayFormat == Durados.DisplayFormat.Crop || DisplayFormat == Durados.DisplayFormat.Fit)
                        {
                            return DataType.Image;
                        }
                        else if (IsUrl() || DisplayFormat == Durados.DisplayFormat.Hyperlink || DisplayFormat == Durados.DisplayFormat.ButtonLink)
                        {
                            return DataType.Url;
                        }
                        else if (SpecialColumn == Durados.SpecialColumn.Html || DisplayFormat == Durados.DisplayFormat.Html)
                        {
                            return DataType.Html;
                        }
                        else if (SpecialColumn == Durados.SpecialColumn.Email || DisplayFormat == Durados.DisplayFormat.Email)
                        {
                            return DataType.Email;
                        }
                        else
                        {
                            if (DataColumn.MaxLength > 260 || DisplayFormat == Durados.DisplayFormat.MultiLines || DisplayFormat == Durados.DisplayFormat.MultiLinesEditor)
                                return DataType.LongText;
                            else
                                return DataType.ShortText;
                        }
                    default:
                        return DataType.ShortText;
                }
            }
            set
            {

            }
        }

        public override string GetDefaultJsonName()
        {
            return Name.ReplaceNonAlphaNumeric2();
        }

        protected override object GetDbDefaultValue()
        {
            return DataColumn.DefaultValue;
        }
    }

    
    
}

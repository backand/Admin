using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;

namespace Durados
{
    public delegate void BeforeCreateEventHandler(object sender, CreateEventArgs e);
    public delegate void BeforeCreateInDatabaseEventHandler(object sender, CreateEventArgs e);
    public delegate void BeforeEditEventHandler(object sender, EditEventArgs e);
    public delegate void BeforeEditInDatabaseEventHandler(object sender, EditEventArgs e);
    public delegate void BeforeDeleteEventHandler(object sender, DeleteEventArgs e);
    public delegate void BeforeSelectEventHandler(object sender, SelectEventArgs e);
    public delegate void AfterCreateEventHandler(object sender, CreateEventArgs e);
    public delegate void AfterEditEventHandler(object sender, EditEventArgs e);
    public delegate void AfterDeleteEventHandler(object sender, DeleteEventArgs e);
    public delegate void AfterSelectEventHandler(object sender, SelectEventArgs e);

    public partial class View
    {

        const char comma = ',';
        const int DEFAULT_PAGE_SIZE = 1000;

        protected string displayName;
        //[Durados.Config.Attributes.ChildrenProperty(TableName = "Category", Type = typeof(Category), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, Category> categories;
        public Category FieldsWithNoCategory { get; private set; }

        public Dictionary<string, Category> Categories
        {
            get
            {
                if (categories == null)
                    LoadCategories();

                return categories;
            }

        }

        public string ConnectionString
        {
            get
            {
                if (SystemView && !string.IsNullOrEmpty(Database.SystemConnectionString) && Name != "durados_Schema")
                    return Database.SystemConnectionString;
                else
                    return Database.ConnectionString;
            }
        }

        //[Durados.Config.Attributes.ColumnProperty(Description = "The names of the view or table in the database.")]
        //public string DatabaseTableName
        //{
        //    get
        //    {
        //        return DataTable.TableName;
        //    }
        //    set
        //    {
        //    }
        //}

        public bool HasEncryptedColumns()
        {
            return Fields.Values.Where(f => f.FieldType == FieldType.Column && ((ColumnField)f).Encrypted).Count() > 0;
        }

        public Field[] GetEncryptedColumns()
        {
            return Fields.Values.Where(f => f.FieldType == FieldType.Column && ((ColumnField)f).Encrypted).ToArray();
        }

        public string GetTableName()
        {
            return string.IsNullOrEmpty(EditableTableName) ? DataTable.TableName : EditableTableName;
        }

        public string GetFileDisplayName()
        {
            string[] charsToRemove = new string[24] { "!", "@", "#", "$", "%", "^", "&", "*", "-", "+", "=", "<", ">", "?", ".", "/", "\\", "|", "[", "]", "{", "}", "`", "~" };
            string s = DisplayName;

            foreach (string c in charsToRemove)
            {
                s = s.Replace(c, string.Empty);
            }

            s = s.Replace(" ", "_");

            return s;
        }

        public HashSet<string> GetColorFields()
        {
            HashSet<string> colorFields = new HashSet<string>();
            foreach (Field field in this.Fields.Values.Where(f => !string.IsNullOrEmpty(f.ContainerGraphicField)))
            {
                colorFields.Add(field.ContainerGraphicField);
            }

            return colorFields;
        }

        public virtual bool DoLocalize()
        {
            return this.Database.IsMultiLanguages;
        }

        public bool HasCategories
        {
            get
            {
                return Categories.Count > 0;
            }
        }

        public virtual bool IsAllow()
        {
            return false;
        }

        public virtual bool IsVisible()
        {
            return true;
        }

        public bool HasFieldsWithNoCategory
        {
            get
            {
                int x = Categories.Count;
                x++;
                return FieldsWithNoCategory.Fields.Count > 0;
            }
        }

        public virtual void RefreshCategories()
        {
            categories = null;
            //LoadCategories();
        }

        protected virtual void LoadCategories()
        {
            categories = new Dictionary<string, Category>();

            FieldsWithNoCategory = new Category() { Name = "___" };

            foreach (Field field in Fields.Values)
            {
                if (field.Category == null && field.FieldType == FieldType.Children && !field.IsCheckList() && !field.View.Database.IsConfig)
                {
                    field.SetCategory(new Category() { Name = field.DisplayName, Ordinal = 100 });
                }

                if (field.Category != null && field.Category.Name != null)
                {
                    Category category;

                    if (categories.ContainsKey(field.Category.Name))
                    {
                        category = categories[field.Category.Name];

                    }
                    else
                    {
                        category = new Category() { Name = field.Category.Name, Ordinal = field.Category.Ordinal };


                        categories.Add(category.Name, category);
                    }

                    field.SetCategory(category);

                    category.Fields.Add(field);

                }
                else
                {
                    FieldsWithNoCategory.Fields.Add(field);
                }
            }
        }

        //private SpecialMenu specialMenu = null;
        //[Durados.Config.Attributes.ParentProperty(TableName = "SpecialMenu", DoNotCopy = true)]
        //public SpecialMenu SpecialMenu
        //{
        //    get
        //    {
        //        return specialMenu;
        //    }
        //    set
        //    {
        //        specialMenu = value;
        //        Database.RefreshMenus();
        //    }
        //}

        private Menu menu = null;
        [Durados.Config.Attributes.ParentProperty(TableName = "Menu", DoNotCopy = true)]
        public Menu Menu
        {
            get
            {
                return menu;
            }
            set
            {
                menu = value;
                Database.RefreshMenus();
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public TreeType TreeType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Choose how filter area sould be displayed")]
        public FilterType FilterType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Choose how sorting area sould be displayed")]
        public SortingType SortingType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Choose how grid area in table mode sould be displayed")]
        public GridDisplayType GridDisplayType { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Send socket event")]
        public bool SendRealTimeEvents { get; set; }


        //Colors design

        private bool applyColorDesignToAllViews = false;
        [Durados.Config.Attributes.ColumnProperty(Description = "Apply the color design to all the views")]
        public bool ApplyColorDesignToAllViews { get { return applyColorDesignToAllViews; } set { applyColorDesignToAllViews = false; } }

        [Durados.Config.Attributes.ColumnProperty(Description = "Application background", Groups = "ColorsDesign")]
        public string Background { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Background for grid rows", Groups = "ColorsDesign")]
        public string RowBackground { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Background for grid alternate rows", Groups = "ColorsDesign")]
        public string AlternateRowBackground { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Background for toolbox", Groups = "ColorsDesign")]
        public string ToolBoxBackground { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Background for row hovering", Groups = "ColorsDesign")]
        public string HoverBackground { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for grid Headers and Filter text", Groups = "ColorsDesign")]
        public string FontColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for Toolbox text", Groups = "ColorsDesign")]
        public string ToolBoxTextColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for grid alternate row content text", Groups = "ColorsDesign")]
        public string  AlterTextColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for grid content text", Groups = "ColorsDesign")]
        public string TextFontColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for row hovering content text", Groups = "ColorsDesign")]
        public string HoverTextColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Color for grid cells border", Groups = "ColorsDesign")]
        public string BorderColor { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TreeViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TreeRelatedFieldName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TreeRoot { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string TreeGroups { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If the grid contains only one row the the edit dialog will be automatically opened")]
        public bool OpenSingleRow { get; set; }

        //[Durados.Config.Attributes.ColumnProperty(Description = "Indicates if view configured to open also in dashboard view")]
        //public bool HasDashboardView { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Display type of the data")]
        public DataDisplayType DataDisplayType { get; set; }

        //[Durados.Config.Attributes.ColumnProperty(Description = "Indicates if view should be open in dashboard view by default")]
        //public bool DashboardDefault { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Height of dashboards, leave blank for auto height")]
        public string DashboardHeight { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Width of dashboards, leave blank for default width")]
        public string DashboardWidth { get; set; }


        internal protected void SetMenu(Menu menu)
        {
            this.menu = menu;
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "A group of views that have a common security configuration")]
        public int WorkspaceID { get; set; }

        public Workspace Workspace
        {
            get
            {
                if (!Database.Workspaces.ContainsKey(WorkspaceID))
                    return null;
                return Database.Workspaces[WorkspaceID];
            }
        }

        public virtual string GetWorkspaceName()
        {
            if (Workspace != null)
            {
                return Workspace.Name;
            }
            else
                return string.Empty;
        }

        //public Workspace GetWorkspace()
        //{
        //    return workspace;
        //}

        //private Workspace workspace = null;
        //[Durados.Config.Attributes.ParentProperty(TableName = "Workspace", Description="A group of views that have a common security configuration")]
        //public Workspace Workspace
        //{
        //    get
        //    {
        //        return workspace;
        //    }
        //    set
        //    {
        //        workspace = value;
        //        Database.RefreshWorkspaces();
        //    }
        //}

        //internal protected void SetWorkspace(Workspace workspace)
        //{
        //    this.workspace = workspace;
        //}

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Field", Type = typeof(Field), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, Field> Fields { get; private set; }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Rule", Type = typeof(Rule), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, Rule> Rules { get; private set; }

        public IEnumerable<Rule> GetRules()
        {
            List<Rule> rules = Rules.Values.ToList();

            rules.AddRange(AdditionalRules.Values);

            return rules;
        }

        public Dictionary<string, Rule> AdditionalRules { get; private set; }


        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "The name of the field that holds the names of the flow steps")]
        public string WorkFlowStepsFieldName { get; set; }

        public Field WorkFlowStepsField
        {
            get
            {
                if (!string.IsNullOrEmpty(WorkFlowStepsFieldName) && Fields.ContainsKey(WorkFlowStepsFieldName))
                    return Fields[WorkFlowStepsFieldName];
                return null;
            }
        }

        public bool HasWorkFlowSteps
        {
            get
            {
                return WorkFlowStepsField != null;
            }
        }


        public Database Database { get; private set; }
        public DataTable DataTable { get; private set; }
        private Dictionary<string, DataColumn> fkColumns;
        internal List<Field> VisibleFieldsForTable { get; private set; }
        //public List<Field> VisibleFieldsForCreate { get; private set; }
        //public List<Field> VisibleFieldsForEdit { get; private set; }
        //public List<Field> VisibleFieldsForFilter { get; private set; }
        //public int RowCount { get; private set; }

        private int pageSize;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Default number of rows per page")]
        public int PageSize
        {
            get 
            {
                return HidePager ? DEFAULT_PAGE_SIZE : pageSize;
            }
            set
            {
                pageSize = value;
            }
        }


        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Number of columns in the New & Edit dialog")]
        public int ColumnsInDialog { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description = "Column in dialog per category name, category name and value with semicolon seperator. For example: \"Details;3\"")]
        public string ColumnsInDialogPerCategory { get; set; }
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide from system menu")]
        public bool HideInMenu { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Enable New functioanlity")]
        public bool AllowCreate { get; set; }
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Enable Edit functioanlity")]
        public bool AllowEdit { get; set; }
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Enable Duplicate functioanlity")]
        public bool AllowDuplicate { get; set; }
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Enable Delete functioanlity")]
        public bool AllowDelete { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Determines if the Promote Step dialog will display the disabled steps or not. Default false.")]
        public bool ShowDisabledSteps { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public bool HasChildrenRow { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "View description that displays as tooltip")]
        public string Description { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "A valid where statement without the &#39where&#39", DoNotCopy = true)]
        public string PermanentFilter { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Elastic Permanent Filter", DoNotCopy = true)]
        public string NosqlPermanentFilter { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "If not checked - use the table view order in New dialog")]
        public bool UseOrderForCreate { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "If not checked - use the table view order in Edit dialog")]
        public bool UseOrderForEdit { get; set; }

        public virtual string GetPermanentFilter()
        {
            return string.IsNullOrEmpty(PermanentFilter) ? string.Empty : ("(" + PermanentFilter + ")");
        }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenyCreateRoles { get; set; }
        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenyEditRoles { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenyDeleteRoles { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string DenySelectRoles { get; set; }

        private string denyCreateRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyCreateRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowCreateRoles))
                //{
                //    return Database.DefaultAllowCreateRoles;
                //}
                //else
                //{
                //    return allowCreateRoles;
                //}
                if (Precedent)
                    return denyCreateRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.DenyCreateRoles;
                else
                    return Database.DefaultDenyCreateRoles;
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
                //    return Database.DefaultAllowEditRoles;
                //}
                //else
                //{
                //    return allowEditRoles;
                //}
                if (Precedent)
                    return denyEditRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.DenyEditRoles;
                else
                    return Database.DefaultDenyEditRoles;
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
                //    return Database.DefaultAllowSelectRoles;
                //}
                //else
                //{
                //    return allowSelectRoles;
                //}
                if (Precedent)
                    return denySelectRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.DenySelectRoles;
                else
                    return Database.DefaultDenySelectRoles;
            }
            set
            {
                denySelectRoles = value;
            }
        }

        private string denyDeleteRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string DenyDeleteRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowDeleteRoles))
                //{
                //    return Database.DefaultAllowDeleteRoles;
                //}
                //else
                //{
                //    return allowDeleteRoles;
                //}
                if (Precedent)
                    return denyDeleteRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.DenyDeleteRoles;
                else
                    return Database.DefaultDenyDeleteRoles;
            }
            set
            {
                denyDeleteRoles = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Display the parent view name for cloned view")]
        public string BaseName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "If true view takes the view roles, otherwise the view takes the workspace roles")]
        public bool Precedent { get; set; }

        private string allowCreateRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowCreateRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowCreateRoles))
                //{
                //    return Database.DefaultAllowCreateRoles;
                //}
                //else
                //{
                //    return allowCreateRoles;
                //}
                if (Precedent)
                    return allowCreateRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.AllowCreateRoles;
                else
                    return Database.DefaultAllowCreateRoles;
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
                //    return Database.DefaultAllowEditRoles;
                //}
                //else
                //{
                //    return allowEditRoles;
                //}
                if (Precedent)
                    return allowEditRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.AllowEditRoles;
                else
                    return Database.DefaultAllowEditRoles;
            }
            set
            {
                allowEditRoles = value;
            }
        }

        private string viewOwnerRoles;
        [Durados.Config.Attributes.ColumnProperty()]
        public string ViewOwnerRoles
        {
            get
            {
                if (Precedent)
                    return viewOwnerRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.ViewOwnerRoles;
                else
                    return Database.DefaultViewOwnerRoles;
            }
            set
            {
                viewOwnerRoles = value;
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
                //    return Database.DefaultAllowSelectRoles;
                //}
                //else
                //{
                //    return allowSelectRoles;
                //}
                if (Precedent)
                    return allowSelectRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.AllowSelectRoles;
                else
                    return Database.DefaultAllowSelectRoles;
            }
            set
            {
                allowSelectRoles = value;
            }
        }

        private string allowDeleteRoles;

        [Durados.Config.Attributes.ColumnProperty()]
        public string AllowDeleteRoles
        {
            get
            {
                //if (string.IsNullOrEmpty(allowDeleteRoles))
                //{
                //    return Database.DefaultAllowDeleteRoles;
                //}
                //else
                //{
                //    return allowDeleteRoles;
                //}
                if (Precedent)
                    return allowDeleteRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.AllowDeleteRoles;
                else
                    return Database.DefaultAllowDeleteRoles;
            }
            set
            {
                allowDeleteRoles = value;
            }
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; private set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide all filter functionality")]
        public bool HideFilter { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "By default collapse filters row")]
        public bool CollapseFilter { get; set; }




        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide the Search functionality")]
        public bool HideSearch { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide the pager in the view")]
        public bool HidePager { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public DuplicationMethod DuplicationMethod { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DuplicateMessage { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string BaseTableName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "CSS for the table view")]
        public string ContainerGraphicProperties { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The key to the Notification Message that is sent after Create, Edit and Delete")]
        public string NotifyMessageKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The key to the Notification Subject that is sent after Create, Edit and Delete")]
        public string NotifySubjectKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true)]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                    return Name.GetDecamal();
                else
                    return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        public string DisplayNameForDynasty
        {
            get
            {
                return JsonName;
            }
        }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public bool IsImageTable { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string OrdinalColumnName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "For coloring rows - the name of the field")]
        public string RowColorColumnName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ImageSrcColumnName { get; set; }

        public bool HasRowColor
        {
            get
            {
                return !string.IsNullOrEmpty(RowColorColumnName) && Fields.ContainsKey(RowColorColumnName);
            }
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description="The order of the view in the menu")]
        public int Order { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "List of field(s) for default view sort")]
        public string DefaultSort { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Effective for Report views. Will display group column on the grid for each fields. For more than one field use semicolon seperator")]
        public string GroupingFields { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Hold the table name to edit - used with complex database view")]
        public string EditableTableName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Save the row changes in the Durados History table. Default true.")]
        public bool SaveHistory { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Enable the on grid functionality")]
        public bool GridEditable { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Set the default mode for the on grid if enabled")]
        public bool GridEditableEnabled { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "When set to true and used as a parent field the loading of the grid will use a cache rather of loading from the database for better performance")]
        public bool Cached { get; set; }

        public bool IsOrdered
        {
            get
            {
                return !string.IsNullOrEmpty(OrdinalColumnName);
            }
        }

        public string[] GetDefaultSortColumnsAndOrder()
        {
            if (DisplayType != DisplayType.Table && !string.IsNullOrEmpty(GroupingFields))
            {
                return new string[1] { GetGroupingFields()[0].Name };
            }
            if (!string.IsNullOrEmpty(DefaultSort))
            {
                return DefaultSort.Split(',');
            }
            else if (!string.IsNullOrEmpty(OrdinalColumnName))
            {
                DefaultSort = OrdinalColumnName;
                return GetDefaultSortColumnsAndOrder();
            }
            else
            {
                return null;
            }
        }

        public Field[] GetGroupingFields()
        {
            if (DisplayType == DisplayType.Table || string.IsNullOrEmpty(GroupingFields))
                return null;

            string[] fieldNames = GroupingFields.Split(';');

            List<Field> fields = new List<Field>();

            foreach (string fieldName in fieldNames)
            {
                if (Fields.ContainsKey(fieldName))
                {
                    fields.Add(Fields[fieldName]);
                }
            }

            return fields.ToArray();

        }

        public bool CompareGroupingFields(Dictionary<string, object> values, DataRow row)
        {
            bool equal = true;
            foreach (string fieldName in values.Keys)
            {
                if (values[fieldName] == null || !values[fieldName].Equals(Fields[fieldName].GetValue(row)))
                {
                    equal = false;
                    break;
                }
            }

            if (!equal)
            {
                List<string> fieldNames = values.Keys.ToList();
                foreach (string fieldName in fieldNames)
                {
                    values[fieldName] = Fields[fieldName].GetValue(row);
                }
            }

            return equal;
        }

        public string GetDefaultSortColumn(string defaultSortColumnAndOrder)
        {
            if (defaultSortColumnAndOrder.Trim().ToLower().EndsWith("desc"))
                return defaultSortColumnAndOrder.TrimEnd().Remove(defaultSortColumnAndOrder.Length - 4, 4).Trim();
            if (defaultSortColumnAndOrder.Trim().ToLower().EndsWith("asc"))
                return defaultSortColumnAndOrder.TrimEnd().Remove(defaultSortColumnAndOrder.Length - 3, 3).Trim();

            return defaultSortColumnAndOrder;
        }

        public Dictionary<string, SortDirection> GetDefaultSortColumns()
        {
            Dictionary<string, SortDirection> defaultSortColumns = new Dictionary<string, SortDirection>();
            string[] defaultSortColumnsAndOrders = GetDefaultSortColumnsAndOrder();

            if (defaultSortColumnsAndOrders != null)
            {
                foreach (string defaultSortColumnAndOrder in defaultSortColumnsAndOrders)
                {
                    string fieldName = GetDefaultSortColumn(defaultSortColumnAndOrder);
                    if (!Fields.ContainsKey(fieldName))
                        return null;
                    defaultSortColumns.Add(fieldName, (SortDirection)Enum.Parse(typeof(SortDirection), GetDefaultSortColumnOrder(defaultSortColumnAndOrder), true));
                }
            }

            return defaultSortColumns;

        }

        public string GetDefaultSortColumnOrder(string defaultSortColumnAndOrder)
        {
            //string[] s = defaultSortColumnAndOrder.Trim().Split(' ');
            //{
            //    if (s.Length == 2)
            //        return s[1].Trim();
            //    else
            //        return "Asc";
            //}

            if (defaultSortColumnAndOrder.Trim().ToLower().EndsWith("desc"))
                return "desc";
            if (defaultSortColumnAndOrder.Trim().ToLower().EndsWith("asc"))
                return "asc";

            return "asc";
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "This field will the view display title name", DoNotCopy = true)]
        public string DisplayColumn
        {
            get
            {
                if (DisplayField == null)
                {
                    return null;
                }
                return DisplayField.Name;
            }
            set
            {
                try
                {
                    if (!string.IsNullOrEmpty(value) && Fields.ContainsKey(value))
                        DisplayField = Fields[value];
                }
                catch
                {
                }
            }
        }

        private Field displayField = null;
        public Field DisplayField
        {
            get
            {
                if (displayField == null)
                {
                    Field field = null;
                    if (DataTable.PrimaryKey.Length > 0)
                    {
                        field = GetFieldByColumnNames(DataTable.PrimaryKey[0].ColumnName);
                    }
                    if (field == null)
                    {
                        field = Fields.Values.FirstOrDefault();
                    }
                    displayField = field;
                }
                return displayField;
            }
            private set
            { 
                displayField = value; 
            }

        }


        [Durados.Config.Attributes.ParentProperty(TableName = "Derivation")]
        public Derivation Derivation { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Opens the Add Items dialog, that add rows based on the Parent View of this field, in the many to many view.")]
        public string AddItemsFieldName { get; set; }

        public ParentField AddItemsField
        {
            get
            {
                if (string.IsNullOrEmpty(AddItemsFieldName))
                    return null;

                if (Fields.ContainsKey(AddItemsFieldName) && Fields[AddItemsFieldName].FieldType == FieldType.Parent)
                {
                    return (ParentField)Fields[AddItemsFieldName];
                }
                else
                    return null;
            }
        }

        public bool HasAddItemsField
        {
            get
            {
                return AddItemsField != null;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CreateDateColumnName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ModifiedDateColumnName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string CreatedByColumnName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ModifiedByColumnName { get; set; }


        private Field[] primaryKeyFileds = null;
        public Field[] PrimaryKeyFileds
        {
            get
            {
                return primaryKeyFileds;
            }
        }

        public virtual Field CreateDate
        {
            get
            {
                if (!string.IsNullOrEmpty(CreateDateColumnName) && Fields.ContainsKey(CreateDateColumnName))
                    return Fields[CreateDateColumnName];
                else
                    return null;
            }
        }

        public virtual Field ModifiedDate
        {
            get
            {
                if (Fields.ContainsKey(ModifiedDateColumnName))
                    return Fields[ModifiedDateColumnName];
                else
                    return null;
            }
        }

        public virtual bool IsSignatureField(Field field)
        {
            return IsSignatureField(field.Name);
        }

        public virtual bool IsSignatureField(string fieldName)
        {
            return (CreatedBy != null && CreatedBy.Name == fieldName) || (ModifiedBy != null && ModifiedBy.Name == fieldName) || (ModifiedDate != null && ModifiedDate.Name == fieldName) || (CreateDate != null && CreateDate.Name == fieldName);

        }

        public virtual Field CreatedBy
        {
            get
            {
                return GetFieldByColumnNames(CreatedByColumnName);
            }
        }

        public virtual Field ModifiedBy
        {
            get
            {
                return GetFieldByColumnNames(ModifiedByColumnName);
            }
        }

        public View(DataTable dataTable, Database database)
            : this(dataTable, database, null)
        {
        }

        public View(DataTable dataTable, Database database, string name)
        {
            GridDisplayType = GridDisplayType.BasedOnColumnWidth;
            Database = database;
            DataTable = dataTable;
            AllowCreate = true;
            AllowDelete = true;
            AllowEdit = true;
            AllowDuplicate = true;
            HideInMenu = SystemView;
            Fields = new Dictionary<string, Field>();
            categories = null;
            //Categories.Add("General", new Category() { Name = "General", View = this });
            OpenSingleRow = false;

            //CreateDateColumnName = "CreateDate";
            //ModifiedDateColumnName = "ModifiedDate";
            CreateDateColumnName = "createdAt";
            ModifiedDateColumnName = "updatedAt";
            CreatedByColumnName = "CreateUserId";
            ModifiedByColumnName = "ModifiedUserId";
            GroupFilterDisplayLabel = true;

            fkColumns = new Dictionary<string, DataColumn>();

            int order = 0;

            foreach (DataRelation dataRelation in dataTable.ParentRelations)
            {
                ParentField parentField = CreateParentField(dataRelation);
                parentField.Order = order;
                parentField.OrderForCreate = order;
                parentField.OrderForEdit = order;
                order += 10;
                Fields.Add(parentField.Name, parentField);
                foreach (DataColumn fkColumn in dataRelation.ChildColumns)
                {
                    if (fkColumns.ContainsKey(fkColumn.ColumnName))
                    {
                        if (dataRelation.ChildColumns.Length == 1)
                            throw new DuradosException("The dictionary of foriegn key columns already contain column " + fkColumn.ColumnName);
                    }
                    else
                    {
                        fkColumns.Add(fkColumn.ColumnName, fkColumn);
                    }
                }
            }

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                if (!fkColumns.ContainsKey(dataColumn.ColumnName))
                {
                    ColumnField columnField = CreateColumnField(dataColumn);
                    columnField.Order = order;
                    columnField.OrderForCreate = order;
                    columnField.OrderForEdit = order;
                    order += 10;
                    Fields.Add(dataColumn.ColumnName, columnField);
                }
            }


            foreach (DataRelation dataRelation in dataTable.ChildRelations)
            {
                ChildrenField childrenField = CreateChildrenField(dataRelation);
                childrenField.Order = order;
                childrenField.OrderForCreate = order;
                childrenField.OrderForEdit = order;
                order += 10;
                Fields.Add(childrenField.Name, childrenField);
            }

            if (IsAutoIdentity)
            {
                string pkColumnName = dataTable.PrimaryKey[0].ColumnName;
                if (Fields.ContainsKey(pkColumnName))
                {
                    primaryKeyFileds = new Field[1] { Fields[dataTable.PrimaryKey[0].ColumnName] };
                }
                else
                {
                    foreach (ParentField parentField in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                    {
                        if (parentField.DataRelation.ChildColumns[0].ColumnName == pkColumnName)
                        {
                            primaryKeyFileds = new Field[1] { parentField };
                        }
                    }
                }
            }
            else
            {
                List<Field> pkFields = new List<Field>();
                foreach (DataColumn column in dataTable.PrimaryKey)
                {
                    if (Fields.ContainsKey(column.ColumnName))
                    {
                        pkFields.Add(Fields[column.ColumnName]);
                    }
                    else
                    {
                        foreach (ParentField parentField in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                        {
                            if (parentField.DataRelation.ChildColumns[0].ColumnName == column.ColumnName)
                            {
                                pkFields.Add(parentField);
                            }
                        }
                    }
                }
                primaryKeyFileds = pkFields.ToArray();
            }

            VisibleFieldsForTable = Fields.Values.Where(f => f.IsHiddenInTable() == false).OrderBy(f => f.Order).ToList();
            //VisibleFieldsForCreate = Fields.Values.Where(f => f.HideInCreate == false).OrderBy(f => f.Order).ToList();
            //VisibleFieldsForEdit = Fields.Values.Where(f => f.HideInEdit == false).OrderBy(f => f.Order).ToList();
            //VisibleFieldsForFilter = Fields.Values.Where(f => f.HideInFilter == false).OrderBy(f => f.Order).ToList();

            if (string.IsNullOrEmpty(name))
            {
                Name = dataTable.TableName;
            }
            else // cloned
            {
                Name = name;
                foreach (Field field in Fields.Values)
                {
                    field.Excluded = true;
                }
            }

            foreach (Field field in VisibleFieldsForTable)
            {
                if (field is ColumnField && ((ColumnField)field).ColumnFieldType == ColumnFieldType.String)
                {
                    DisplayField = field;
                    break;
                }
            }

            if (Fields.Values.Count() == 0)
            {
                //throw new DuradosException("[" + Name + "] has no fields.");
                //Database.Logger.Log("View", "View", "View", null, 1, "The View: " + Name + " has no fields.");
            }

            if (DisplayField == null)
            {
                if (VisibleFieldsForTable.Count > 1)
                    DisplayField = VisibleFieldsForTable[1];
                else if (VisibleFieldsForTable.Count > 0)
                    DisplayField = VisibleFieldsForTable[0];
                else
                    DisplayField = Fields.Values.FirstOrDefault();
            }

            Init();
            PageSize = database.DefaultPageSize;
            ColumnsInDialog = 2;


            IsImageTable = Name.ToLower().Contains("image");
            //OrdinalColumnName = "Ordinal";
            ImageSrcColumnName = "Image";
            HideFilter = false;
            CollapseFilter = false;
            HidePager = false;
            DuplicationMethod = DuplicationMethod.Shallow;
            DuplicateMessage = "";
            BaseTableName = string.Empty;
            DisplayType = DisplayType.Table;
            SaveHistory = false;
            GridEditableEnabled = true;
            GridEditable = true;

            Rules = new Dictionary<string, Rule>();
            AdditionalRules = new Dictionary<string, Rule>();

            Precedent = false;

            WorkspaceID = -1;

            ID = -1;

            UseLikeInFilter = true;

            Send = true;

            SendRealTimeEvents = false;
            //JsonName = name.ReplaceNonAlphaNumeric();

        }

        public int ParentCount
        {
            get
            {
                return DataTable.ParentRelations.Count;
            }
        }

        public int ChildrenCount
        {
            get
            {
                return DataTable.ChildRelations.Count;
            }
        }

        public string[] GetPkColumnNames()
        {
            List<string> s = new List<string>();

            foreach (DataColumn column in DataTable.PrimaryKey)
            {
                s.Add(column.ColumnName);
            }

            return s.ToArray();
        }

        public ParentField GetIntegralParent()
        {
            foreach (ParentField field in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (field.Integral)
                    return field;
            }

            return null;
        }

        //public List<Field> GetVisibleFieldsForRow(DataAction dataAction)
        //{
        //    switch (dataAction)
        //    {
        //        case DataAction.Create:
        //            return VisibleFieldsForCreate;
        //        case DataAction.Edit:
        //            return VisibleFieldsForEdit;
        //        default:
        //            throw new NotSupportedException();
        //    }
        //}

        protected virtual ColumnField CreateColumnField(DataColumn dataColumn)
        {
            return new ColumnField(this, dataColumn);
        }

        protected virtual ParentField CreateParentField(DataRelation dataRelation)
        {
            return new ParentField(this, dataRelation);
        }

        protected virtual ChildrenField CreateChildrenField(DataRelation dataRelation)
        {
            return new ChildrenField(this, dataRelation);
        }

        protected virtual void Init()
        {
        }

        public View GetParentView(DataRelation parentRelation)
        {
            return Database.Views[parentRelation.ParentTable.TableName];
        }

        public bool IsAutoIdentity
        {
            get
            {
                return DataTable.PrimaryKey.Count() > 0 && (DataTable.PrimaryKey[0].AutoIncrement || (Fields.ContainsKey(DataTable.PrimaryKey[0].ColumnName) && Fields[DataTable.PrimaryKey[0].ColumnName].AutoIncrement)) ;
            }
        }

        public bool IsGuidIdentity
        {
            get
            {
                return DataTable.PrimaryKey.Count() == 1 && DataTable.PrimaryKey[0].DataType.Equals(typeof(Guid));
            }
        }

        public bool IsCloned
        {
            get
            {
                return !string.IsNullOrEmpty(BaseName);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public string GetPkValue(DataRow dataRow)
        {
            return GetPkValue(dataRow, false);
        }

        public string GetPkValue(DataRow dataRow, bool quote)
        {
            string primaryKeyValueDelimited = string.Empty;

            if (dataRow != null)
            {
                foreach (DataColumn dataColumn in DataTable.PrimaryKey)
                {
                    if (!IsNumeric(dataColumn) && quote)
                        primaryKeyValueDelimited += "'";
                    primaryKeyValueDelimited += dataRow[dataColumn.ColumnName].ToString();
                    if (!IsNumeric(dataColumn) && quote)
                        primaryKeyValueDelimited += "'";
                    primaryKeyValueDelimited += comma;
                }
            }

            return primaryKeyValueDelimited.TrimEnd(comma);
        }


        private bool IsNumeric(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }

        private bool IsNumeric(DataColumn column)
        {
            return IsNumeric(column.DataType);
        }

        public object[] GetPkValue(string pk)
        {
            //List<object> pkValue = new List<object>();
            //string[] pkValueStrings = pk.Split(',');

            //for (int i = 0; i < DataTable.PrimaryKey.Count(); i++)
            //{
            //    DataColumn dataColumn = DataTable.PrimaryKey[i];

            //    string valString = pkValueStrings[i];
            //    object val = Convert.ChangeType(valString, dataColumn.DataType);
            //    pkValue.Add(val);
            //}

            //return pkValue.ToArray();

            return GetPkValue(pk.Split(','));
        }

        public object[] GetPkValue(string[] pk)
        {
            List<object> pkValue = new List<object>();

            for (int i = 0; i < DataTable.PrimaryKey.Count(); i++)
            {
                DataColumn dataColumn = DataTable.PrimaryKey[i];

                string valString = pk[i];
                if (valString.EndsWith("#"))
                {
                    if (!dataColumn.DataType.Equals(typeof(string)))
                    {
                        valString = valString.TrimEnd('#');
                    }
                }
                object val = null;
                if (dataColumn.DataType.Equals(typeof(Guid)))
                {
                    val = new Guid(valString);
                }
                else
                {
                    val = Convert.ChangeType(valString, dataColumn.DataType);
                }
                pkValue.Add(val);
            }

            return pkValue.ToArray();
        }

        public string GetFkValue(DataRow dataRow, string relationName)
        {
            string forignKeyValueDelimited = string.Empty;

            DataRelation relation = DataTable.DataSet.Relations[relationName];

            if (dataRow != null)
            {
                foreach (DataColumn dataColumn in relation.ChildColumns)
                {
                    if (dataRow.Table.Columns.Contains(dataColumn.ColumnName) && !dataRow.IsNull(dataColumn.ColumnName))
                    {
                        forignKeyValueDelimited += dataRow[dataColumn.ColumnName].ToString() + comma;
                    }
                }
            }

            return forignKeyValueDelimited.TrimEnd(comma);
        }

        public string GetDisplayValue(Dictionary<string, object> values)
        {
            if (values.ContainsKey(DisplayField.Name))
                return values[DisplayField.Name].ToString();

            return null;
        }

        public string GetDisplayColumn()
        {
            Field displayField = DisplayField;
            while (displayField.FieldType == FieldType.Parent)
            {
                displayField = ((ParentField)displayField).ParentView.DisplayField;
            }

            return displayField.Name;
        }

        public string GetDisplayValue(DataRow dataRow)
        {

            string displayValue = string.Empty;

            if (dataRow != null)
            {
                Field displayField = this.DisplayField;
                while (displayField.FieldType == FieldType.Parent)
                {
                    DataRow originalRow = dataRow;
                    dataRow = dataRow.GetParentRow(((ParentField)displayField).DataRelation.RelationName);
                    if (dataRow == null)
                    {
                        return string.Empty;
                    }
                    displayField = ((ParentField)displayField).ParentView.DisplayField;
                }

                //displayValue = dataRow[displayField.View.DisplayColumn].ToString();
                displayValue = displayField.ConvertToString(dataRow);

            }

            return displayValue;
        }

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true)]
        public string BaseViewName { get; set; }

        public View Base
        {
            get
            {
                if (string.IsNullOrEmpty(BaseName))
                    return this;

                if (Database.Views.ContainsKey(BaseName))
                    return Database.Views[BaseName].Base;
                else
                    return null;
            }
        }

        public bool UseLikeInFilter { get; set; }

        public View BaseView
        {
            get
            {
                if (string.IsNullOrEmpty(BaseViewName))
                    return null;

                if (Database.Views.ContainsKey(BaseViewName))
                    return Database.Views[BaseViewName];
                else
                    return null;
            }
        }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Indication if need to display a label above filter field. Relevant in group filter type")]
        public bool GroupFilterDisplayLabel { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Indication if user can display this view in table mode")]
        public bool EnableTableDisplay { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Indication if user can display this view in dashboard mode")]
        public bool EnableDashboardDisplay { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Indication if user can display this view in preview mode")]
        public bool EnablePreviewDisplay { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = @"Row height in pixels. If left empty it gets the height from the skin.")]
        public string RowHeight { get; set; }


        public bool IsDerived
        {
            get
            {
                return BaseView != null;
            }
        }

        private ParentField derivedParentFieldCached = null;

        public ParentField GetDerivedParentField(string fieldName)
        {
            if (derivedParentFieldCached != null)
                return derivedParentFieldCached;

            foreach (ParentField parentField in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (parentField.BaseFieldName.Equals(fieldName))
                {
                    derivedParentFieldCached = parentField;
                    break;
                }
            }


            return derivedParentFieldCached;
        }

        public ParentField GetDerivationField()
        {
            if (Derivation == null)
                return null;
            return Derivation.GetDerivationField(this);
        }

        public bool IsDerivationEditable(Field field, DataRow row)
        {
            if (Derivation == null)
                return true;

            return Derivation.IsEditable(this, field, row);
        }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Summary")]
        //public Summary Summary { get; set; }

        [Durados.Config.Attributes.ParentProperty(TableName = "ChartInfo")]
        public ChartInfo ChartInfo { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public DisplayType DisplayType { get; set; }

        /***************************/

        public bool HasHiarchy()
        {
            foreach (ParentField field in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (field.ParentView.Equals(this))
                    return true;
            }

            return false;
        }

        public ParentField GetTwin(ParentField parentField)
        {
            if (Fields.ContainsKey(parentField.Name))
                return (ParentField)Fields[parentField.Name];

            bool match = false;
            foreach (ParentField twin in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {

                for (int i = 0; i < twin.DataRelation.ChildColumns.Length; i++)
                {
                    if (twin.DataRelation.ChildColumns[i].ColumnName == parentField.DataRelation.ChildColumns[i].ColumnName)
                    {
                        match = true;
                    }
                    else
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return twin;
            }

            return null;
        }

        public ChildrenField GetTwin(ChildrenField childrenField)
        {
            bool match = false;

            if (Fields.ContainsKey(childrenField.Name))
                return (ChildrenField)Fields[childrenField.Name];

            foreach (ChildrenField twin in Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                if (twin.DataRelation.ParentColumns.Length != childrenField.DataRelation.ParentColumns.Length)
                    break;

                for (int i = 0; i < twin.DataRelation.ChildColumns.Length; i++)
                {
                    if (twin.DataRelation.ParentColumns[i].ColumnName == childrenField.DataRelation.ParentColumns[i].ColumnName)
                    //if (twin.DataRelation.ChildColumns[i].ColumnName == childrenField.DataRelation.ChildColumns[i].ColumnName)
                    {
                        match = true;
                    }
                    else
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return twin;
            }

            return null;
        }

        public ParentField GetParentField(string columnName)
        {
            return GetParentField(new string[1] { columnName });
        }

        public ParentField GetParentField(string[] columnNames)
        {
            foreach (ParentField parentField in Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                bool b = false;
                if (columnNames.Length == parentField.DataRelation.ChildColumns.Length)
                {
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        b = columnNames[i] == parentField.DataRelation.ChildColumns[i].ColumnName;
                        if (!b)
                        {
                            break;
                        }
                    }
                }
                if (b)
                {
                    return parentField;
                }
            }

            return null;
        }

        //public ChildrenField GetChildrenField(string columnName)
        //{
        //    return GetChildrenField(new string[1] { columnName });
        //}

        //public ChildrenField GetChildrenField(string[] columnNames)
        //{
        //    foreach (ChildrenField childrenField in Fields.Values.Where(f => f.FieldType == FieldType.Children))
        //    {
        //        bool b = false;
        //        if (columnNames.Length == childrenField.DataRelation.ParentColumns.Length)
        //        {
        //            for (int i = 0; i < columnNames.Length; i++)
        //            {
        //                b = columnNames[i] == childrenField.DataRelation.ParentColumns[i].ColumnName;
        //                if (!b)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        if (b)
        //        {
        //            return childrenField;
        //        }
        //    }

        //    return null;
        //}

        Dictionary<string, int> columnsInDialogPerCategoryDictionary = null;
        public int GetColumnsInDialog(Category category)
        {
            if (category == null)
                return ColumnsInDialog;
            return GetColumnsInDialog(category.Name);
        }

        public int GetColumnsInDialog(string categoryName)
        {
            if (string.IsNullOrEmpty(ColumnsInDialogPerCategory))
            {
                return ColumnsInDialog;
            }
            if (columnsInDialogPerCategoryDictionary == null)
            {
                columnsInDialogPerCategoryDictionary = new Dictionary<string, int>();
                string[] columnsInDialogPerCategoryArray = ColumnsInDialogPerCategory.Split(';');

                for (int i = 0; i < columnsInDialogPerCategoryArray.Length; i = i + 2)
                {
                    columnsInDialogPerCategoryDictionary.Add(columnsInDialogPerCategoryArray[i], Convert.ToInt32(columnsInDialogPerCategoryArray[i + 1]));
                }
            }

            if (columnsInDialogPerCategoryDictionary.ContainsKey(categoryName))
            {
                return columnsInDialogPerCategoryDictionary[categoryName];
            }
            else
            {
                return ColumnsInDialog;
            }
        }

        public Field[] GetFieldsByDisplayName(string displayName)
        {
            var fields = Fields.Values.Where(f => f.DisplayName.Equals(displayName, StringComparison.InvariantCultureIgnoreCase));
            if (fields == null || fields.Count() == 0)
            {
                fields = Fields.Values.Where(f => f.DisplayName.Replace("'", "").Replace("\"", "").Equals(displayName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (fields == null || fields.Count() == 0)
            {
                fields = Fields.Values.Where(f => f.DisplayName.Replace("'", "").Replace("\"", "").Equals(displayName.Replace("'", "").Replace("\"", ""), StringComparison.InvariantCultureIgnoreCase));
            }
            if (fields == null || fields.Count() == 0)
            {
                fields = Fields.Values.Where(f => f.DisplayName.Replace("'", "").Replace("\"", "").Replace(" ", "").Equals(displayName.Replace("'", "").Replace("\"", "").Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase));
            }
            if (fields == null)
                return null;

            return fields.ToArray();
            //foreach (Field field in Fields.Values)
            //{
            //    if (field.DisplayName.Equals(displayName, StringComparison.InvariantCultureIgnoreCase))
            //        return field;
            //}

            //return null;
        }


        public Field[] GetFieldsByJsonName(string jsonName)
        {
            var fields = Fields.Values.Where(f => f.JsonName.Equals(jsonName, StringComparison.InvariantCultureIgnoreCase));
            if (fields == null)
                return null;

            var fieldsArray = fields.ToArray();

            if (fieldsArray.Length > 1)
            {
                fields = Fields.Values.Where(f => !f.Excluded && f.JsonName.Equals(jsonName, StringComparison.InvariantCultureIgnoreCase));
                if (fields == null)
                    return null;

                fieldsArray = fields.ToArray();
            }

            return fieldsArray;
        }

        public Field GetFieldByColumnNames(string columnNames)
        {
            foreach (Field field in Fields.Values)
            {
                if (field.FieldType == FieldType.Column)
                {
                    if (((ColumnField)field).DataColumn.ColumnName == columnNames)
                        return field;
                }
                else if (field.FieldType == FieldType.Parent)
                {
                    ParentField parentField = GetParentField(columnNames.Split(','));
                    if (parentField != null)
                        return parentField;
                }
                //else if (field.FieldType == FieldType.Children)
                //{
                //    ChildrenField childrenField = GetChildrenField(columnNames.Split(','));
                //    if (childrenField != null)
                //        return childrenField;
                //}
            }

            return null;
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Add a send button.")]
        public bool Send { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Semicolon deimited dictionary names that contain emails to send to.")]
        public string SendTo { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Semicolon deimited dictionary names that contain emails to send cc.")]
        public string SendCc { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Subject of the email (may include dictionary reference).")]
        public string SendSubject { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Reference the the email content template")]
        public string SendTemplate { get; set; }


        public virtual bool HasMessages()
        {
            return Rules.Values.Where(r => r.WorkflowAction == WorkflowAction.Notify).Count() > 0;
        }

        [Durados.Config.Attributes.ColumnProperty(Description = "Gets the equivalent element name in an exported xml file. If left empty then it is equal to the View Name")]
        public string XmlElement { get; set; }

        public string GetXmlElement()
        {
            return string.IsNullOrEmpty(XmlElement) ? Name : XmlElement;
        }

        private bool systemView = false;
        [Durados.Config.Attributes.ColumnProperty(Description = "Durados system view")]
        public bool SystemView
        {

            get
            {

                return (DataTable.ExtendedProperties.Contains("system") && DataTable.ExtendedProperties["system"].Equals(true)) || systemView;

            }

            set
            {
                systemView = value;
            }
        }

        //Add by Miri Hisherik
        [Durados.Config.Attributes.ColumnProperty(Description = "Durados Create Override")]
        public bool CreateOverride { get; set; }

        //Add by Miri Hisherik
        [Durados.Config.Attributes.ColumnProperty(Description = "Durados Delete Override")]
        public bool DeleteOverride { get; set; }


        //Add by Yariv Attar
        [Durados.Config.Attributes.ColumnProperty(Description = "In Add Items add All Items")]
        public bool InAddItemsaddAllItems { get; set; }

        public virtual IDbCommand GetCommand()
        {
            return null;
        }

        //Add by Yariv Attar
        [Durados.Config.Attributes.ColumnProperty(Description = "Open Dialog as maximize")]
        public bool OpenDialogMax { get; set; }

        protected string jsonName;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true)]
        public string JsonName
        {
            get
            {
                if (string.IsNullOrEmpty(jsonName))
                    return Name.ReplaceNonAlphaNumeric2();
                else
                    return jsonName;
            }
            set
            {
                jsonName = value;
            }
        }

        public void ValidateUniqueFieldJsonName(string jsonName)
        {
            if(IsJsonNameAlreadyExistsInFields( jsonName))
                throw new DuradosException("Json name " + jsonName + " already exists");
        }
        public bool IsJsonNameAlreadyExistsInFields(string jsonName)
        {
            if (!this.Database.IsConfig)
                return (this.Fields.Values.Select(f => f.JsonName).Contains(jsonName));
            return false;

        }

        public virtual void SendRealTimeEvent(string pk, Crud crud)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, object> RowToShallowDictionary(DataRow row, string pk)
        {
            throw new NotImplementedException();
        }

        Field[] encryptedFields = null;
        public Field[] GetSysEncryptedFields()
        {
            if (encryptedFields == null)
            {
                encryptedFields = Fields.Values.Where(f => f.SysEncrypted).ToArray();
            }

            return encryptedFields;
        }

        public Durados.Security.Cloud.ICloudCredentials GetRuleCredentials(Rule rule)
        {
            return GetRuleCredentials(rule.CloudSecurity, rule.LambdaArn);
        }

        public Durados.Security.Cloud.ICloudCredentials GetRuleCredentials(int cloudId, string arn)
        {
            string[] regions = Database.Clouds[cloudId].Region.ToString().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string region = string.Empty;
            if (regions.Length == 1)
                region = regions[0];
            else
            {
                foreach (string r in regions)
                {
                    if (arn.Contains(r))
                    {
                        region = r;
                        break;
                    }
                }
            }
            string secretAccessKey = Database.Clouds[cloudId].DecryptedSecretAccessKey;
            string accessKeyID = Database.Clouds[cloudId].AccessKeyId;
            return new Durados.Security.Cloud.AwsCredentials() { Region = region, SecretAccessKey = secretAccessKey, AccessKeyID = accessKeyID };
        }


    }

    public class DataActionEventArgs : EventArgs
    {
        private View view;
        private Dictionary<string, object> values;
        private string pk;
        private object history;
        public IDbCommand Command { get; set; }
        public IDbCommand SysCommand { get; set; }

        public bool IgnorePermanentFilter { get; set; }

        public DataActionEventArgs(View view, Dictionary<string, object> values, string pk, IDbCommand command, IDbCommand sysCommand)
            : base()
        {
            this.view = view;
            this.values = values;
            this.pk = pk;
            Command = command;
            SysCommand = sysCommand;
            IgnorePermanentFilter = false;  
        }

        public View View
        {
            get
            {
                return view;
            }

        }

        public Dictionary<string, object> Values
        {
            get
            {
                return values;
            }

        }

        public string PrimaryKey
        {
            get
            {
                return pk;
            }
            set
            {
                pk = value;
            }
        }

        public object History
        {
            get
            {
                return history;
            }
            set
            {
                history = value;
            }
        }



        public int UserId { get; set; }

        public bool Cancel { get; set; }

        public List<string> ColumnNames { get; set; }
    }

    public class CreateEventArgs : DataActionEventArgs
    {
        public CreateEventArgs(View view, Dictionary<string, object> values, string pk, IDbCommand command, IDbCommand sysCommand)
            : base(view, values, pk, command, sysCommand)
        {
        }
    }

    public class EditEventArgs : DataActionEventArgs
    {

        public EditEventArgs(View view, Dictionary<string, object> values, string pk, IDbCommand command, IDbCommand sysCommand)
            : base(view, values, pk, command, sysCommand)
        {
        }

        //public bool LoadPrevRow { get; set; }
        public DataRow PrevRow { get; set; }
        public OldNewValue[] OldNewValues { get; set; }


    }

    public class DeleteEventArgs : DataActionEventArgs
    {


        public DeleteEventArgs(View view, string pk, IDbCommand command, IDbCommand sysCommand, Dictionary<string, object> values = null)
            : base(view, values, pk, command, sysCommand)
        {
        }

        public DataRow PrevRow { get; set; }

    }

    public enum ActionType
    {
        Action,
        Function,
        Integration
    }

    public enum WorkflowAction
    {
        Notify,
       // Task,
        Validate,
        Execute,
        WebService,
        CompleteStep,
        Approval,
        Document,
        Xml,
        Custom,
        NodeJS,
        JavaScript,
        Lambda
    }

    public enum TriggerDataAction
    {
        BeforeCreate = 0,
        BeforeEdit = 1,
        BeforeDelete = 2,
        AfterCreate = 4,
        AfterEdit = 8,
        AfterCreateOrEdit = 16,
        AfterDelete = 32,
        AfterCreateBeforeCommit = 64,
        AfterEditBeforeCommit = 128,
        AfterDeleteBeforeCommit = 256,
        Open = 512,
        BeforeCompleteStep = 1024,
        AfterCompleteStepBeforeCommit = 2048,
        AfterCompleteStepAfterCommit = 4096,
        BeforeViewOpen = 8192,
        OnDemand = 16384
    }

    public enum DataAction
    {
        Create,
        Edit,
        InlineAdding,
        InlineEditing,
        Report
    }

    public enum DuplicationMethod
    {
        Shallow,
        Recursive,
        User
    }

    public enum DisplayType
    {
        Table,
        Report,
        Chart,
        Both
    }

    public enum DataDisplayType
    {
        Table,
        Dashboard,
        Preview
    }

    public class SelectEventArgs : EventArgs
    {
        private View view;
        private object filter;
        public string Sql { get; set; }

        public SelectEventArgs(View view, object filter)
            : base()
        {
            this.view = view;
            this.filter = filter;
        }

        public View View
        {
            get
            {
                return view;
            }

        }

        public object Filter
        {
            get
            {
                return filter;
            }

        }
    }

    public enum TreeType
    {
        Adjacency,
        Grouping
    }

    public enum FilterType
    {
        Excel,
        Group,
        Tree
    }

    public enum SortingType
    {
        Excel,
        Group
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }

    public enum GridDisplayType
    {
        BasedOnColumnWidth,
        FitToWindowWidth
    }

    public class OldNewValue
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OldKey { get; set; }
        public string NewKey { get; set; }
        public string FieldName { get; set; }
    }

    public static class ViewExtension
    {
        public static string[] Names(this IEnumerable<View> views)
        {
            List<string> names = new List<string>();

            foreach (View view in views)
            {
                names.Add(view.Name);
            }

            return names.ToArray();
        }
    }

    public enum Crud
    {
        create,
        read,
        update,
        delete
    }
}

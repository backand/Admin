using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc
{
    public partial class ChildrenField : Durados.ChildrenField
    {
        public ChildrenField(View view, DataRelation dataRelation) :
            base(view, dataRelation)
        {
            if (dataRelation.ChildTable.Columns.Count == 3 && dataRelation.ChildTable.ParentRelations.Count == 2)
            {
                ChildrenHtmlControlType = ChildrenHtmlControlType.CheckList;
                HideInTable = false;
            }
            else
            {
                
                //ChildrenHtmlControlType = ChildrenHtmlControlType.Hide;
                if (!View.Database.IsConfig && !View.SystemView)
                {
                    ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
                    //category = new Category() { Name = DisplayName, Ordinal = 100 };
                    HideInTable = false;
                }
            }
            LabelContentLayout = Orientation.Horizontal;
            //ColSpanInDialog = 2;
            NoCache = false;
            ContainerGraphicProperties = "d_fieldContainer";
            MinWidth = 80;
        }

        //protected override Category GetCategory()
        //{
        //    if (ChildrenHtmlControlType == ChildrenHtmlControlType.Grid && category == null)
        //    {
        //        return new Category() { Name = DisplayName, Ordinal = 100 };
        //    }
        //    else
        //    {
        //        return base.GetCategory();
        //    }
        //}

        public override Dictionary<string, string> GetAutocompleteValues(string q, int limit)
        {
            Durados.ParentField parentField = null;
            Durados.ParentField fkField = null;
            Durados.View otherParentView = GetOtherParentView(out parentField, out fkField);

            return ((ParentField)parentField).GetSelectOptions(q, AutocompleteMathing == AutocompleteMathing.StartsWith, limit, null);
        }

        protected ChildrenHtmlControlType childrenHtmlControlType;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy=true)]
        public ChildrenHtmlControlType ChildrenHtmlControlType
        {
            get
            {
                return childrenHtmlControlType;
            }
            set
            {
                childrenHtmlControlType = value;
                persist = (childrenHtmlControlType == ChildrenHtmlControlType.CheckList);
                
            }
        }

        public override bool IsCheckList()
        {
            return ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList;
        }

        public override bool IsSubGrid()
        {
            return ChildrenHtmlControlType == ChildrenHtmlControlType.Grid;
        }

        public override bool HideInCreate
        {
            get
            {
                return this.ChildrenHtmlControlType == ChildrenHtmlControlType.Hide || (this.ExcludeInInsert && !this.DisableInCreate);
            }
            set
            {
                base.HideInCreate = true;
            }
        }

        public override bool HideInEdit
        {
            get
            {
                return this.ChildrenHtmlControlType == ChildrenHtmlControlType.Hide || (this.ExcludeInUpdate && !this.DisableInEdit);
            }
            set
            {
                base.HideInEdit = true;
            }
        }


        public override string GetFieldControlType()
        {
            return this.ChildrenHtmlControlType.ToString();
        }

        public override object ConvertFromString(string value)
        {
            if (ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList)
                return value;
            else
                return base.ConvertFromString(value);
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool NoCache { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Limit the values count that will be displayed in the grid. Default is 0 witch means no limit.")]
        public int CheckListInTableLimit { get; set; }

        public override bool IsAllow(DataAction dataAction)
        {
            return FieldHelper.IsVisibleForRow(this, dataAction);
        }
    }

    public enum ChildrenHtmlControlType
    {
        Hide,
        Grid,
        CheckList
    }
}

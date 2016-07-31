using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class BETechnicalStyler : Durados.Web.Mvc.UI.Styler
    {
        protected Dictionary<string, Dictionary<string, SortedList<DateTime, BusinessLogic.PlmChange>>> changesFromLastChangeRequest;
        protected Dictionary<string, Dictionary<string, SortedList<DateTime, BusinessLogic.PlmChange>>> changesFromLastNewRequest;
        protected Dictionary<string, string> plmParametersChangesFromLastChangeRequest;
        protected Dictionary<string, string> plmParametersChangesFromLastNewRequest;

        public BETechnicalStyler(View view, DataView dataView)
            : base(view, dataView)
        {
            BusinessLogic.BETchnicalChanges plmChanges = GetPlmChanges(view, dataView);
            changesFromLastChangeRequest = plmChanges.GetPlmChangesFromLastChangeRequest(view, dataView);
            changesFromLastNewRequest = plmChanges.GetPlmChangesFromLastNewRequest(view, dataView);

            plmParametersChangesFromLastChangeRequest = plmChanges.GetPlmParametersChangesFromLastChangeRequest(view, dataView);
            plmParametersChangesFromLastNewRequest = plmChanges.GetPlmParametersChangesFromLastNewRequest(view, dataView);
        }

        protected virtual BusinessLogic.BETchnicalChanges GetPlmChanges(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.BETchnicalChanges();
        }

        protected virtual string GetPlmIDColumnName()
        {
            return "PLMId";
        }

        private bool IsParameterField(Field field)
        {
            bool patameterField;

            if (field.View.Name == "v_PLM")
            {
                patameterField = field.Category != null && field.Category.Name.ToLower().Contains("parameter");
            }
            else
            {
                patameterField = field.Category == null || !field.Category.Name.ToLower().Contains("details");
            }

            return patameterField;
        }

        public override string GetCellCss(Field field, System.Data.DataRow row)
        {
            bool patameterField = IsParameterField(field);
            //if (row["PLMBEStatusId"].ToString() == "1" && patameterField) // 1 = New Request
            if (row["PLMBEStatusId"].ToString() == PLMBEStatus.NewRequestCR.ToString() && patameterField) // 7 = New Request - CR
                return GetCellCss(field, row, changesFromLastNewRequest, plmParametersChangesFromLastNewRequest);
            else if (row["PLMBEStatusId"].ToString() == PLMBEStatus.ChangeRequest.ToString() && patameterField) // 2 = Change Request
                return GetCellCss(field, row, changesFromLastChangeRequest, plmParametersChangesFromLastChangeRequest);
            else
                return base.GetCellCss(field, row);
        }

        public virtual string GetCellCss(Field field, System.Data.DataRow row, Dictionary<string, Dictionary<string, SortedList<DateTime, BusinessLogic.PlmChange>>> changes, Dictionary<string,string> changesParameters)
        {
            //if (row["PLMBEStatusId"].ToString() != "2") // 2 = Change Request
            //    return base.GetCellCss(field, row);

            string greenAlert = " cellGreenAlert";
            string yellowAlert = " cellYellowAlert";

            string PK = row[GetPlmIDColumnName()].ToString();

            if (field.FieldType == FieldType.Children && field.DisplayName == "Parameters")
            {
                if (changesParameters.ContainsKey(PK))
                    return yellowAlert;
                else
                    return base.GetCellCss(field, row);
            }
            else if (changes.ContainsKey(PK))
            {
                string columnsNames;
                if (field.FieldType == FieldType.Children)
                {
                    columnsNames = ((ChildrenField)field).ChildrenView.Name;
                }
                else
                {
                    columnsNames = field.GetColumnsNames().Delimited();
                }
                if (changes[PK].ContainsKey(columnsNames))
                {
                    SortedList<DateTime, BusinessLogic.PlmChange> changesList = changes[PK][columnsNames];

                    int count = changesList.Count;
                    if (count == 1)
                    {
                        return yellowAlert;
                    }
                    else if (count > 1)
                    {
                        BusinessLogic.PlmChange first = changesList.Values.First();
                        BusinessLogic.PlmChange last = changesList.Values.Last();

                        if (first.OldValue == last.NewValue)
                        {
                            return greenAlert;
                        }
                        else
                        {
                            return yellowAlert;
                        }
                    }
                    else
                    {
                        return base.GetCellCss(field, row);
                    }
                }
            }
            return base.GetCellCss(field, row);
        }

        public override string GetAlt(Field field, DataRow row, string guid)
        {
            bool patameterField = IsParameterField(field);
            //if (row["PLMBEStatusId"].ToString() == "1" && patameterField) // 1 = New Request
            if (row["PLMBEStatusId"].ToString() == PLMBEStatus.NewRequestCR.ToString() && patameterField) // 7 = New Request - CR
                return GetAlt(field, row, guid, changesFromLastNewRequest);
            else if (row["PLMBEStatusId"].ToString() == PLMBEStatus.ChangeRequest.ToString() && patameterField) // 2 = Change Request
                return GetAlt(field, row, guid, changesFromLastChangeRequest);
            else
                return base.GetAlt(field, row, guid);
        }

        public virtual string GetAlt(Field field, DataRow row, string guid, Dictionary<string, Dictionary<string, SortedList<DateTime, BusinessLogic.PlmChange>>> changes)
        {
            string PK = row[GetPlmIDColumnName()].ToString();

            if (changes.ContainsKey(PK))
            {
                string columnsNames;
                if (field.FieldType == FieldType.Children)
                {
                    columnsNames = ((ChildrenField)field).ChildrenView.Name;
                }
                else
                {
                    columnsNames = field.GetColumnsNames().Delimited();
                } 
                if (changes[PK].ContainsKey(columnsNames))
                {
                    SortedList<DateTime, BusinessLogic.PlmChange> changesList = changes[PK][columnsNames];

                    int count = changesList.Count;
                    if (count == 1)
                    {
                        return changesList.Values[0].GetAlt();
                    }
                    else if (count > 1)
                    {
                        string alt = string.Empty;

                        foreach (BusinessLogic.PlmChange change in changesList.Values)
                        {
                            alt += change.GetAlt() + "\n";
                        }

                        return alt;
                    }
                    else
                    {
                        return base.GetCellCss(field, row);
                    }
                }
            }
            return base.GetAlt(field, row, guid);
        }
    }
}

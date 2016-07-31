using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroProductSpecController : AllegroHomeController
    {
        Dictionary<string, string> includeColumns = new Dictionary<string, string>();

        protected override Durados.Web.Mvc.UI.ColumnsExcluder GetNewColumnsExcluder(View view, Dictionary<string, Durados.Web.Mvc.UI.Json.Field> filterFields)
        {
            string productClass = v_EngMarketingReport.ProductClass_v_EngMarketingReport_Parent.ToString();
            includeColumns.Add(productClass, productClass);

            string capacity = v_EngMarketingReport.ProductCapacity_v_EngMarketingReport_Parent.ToString();
            includeColumns.Add(capacity, capacity);

            string status = v_EngMarketingReport.PORStage_v_EngMarketingReport_Parent.ToString();
            includeColumns.Add(status, status);

            string memory = v_EngMarketingReport.Memory_v_EngMarketingReport_Parent.ToString();
            includeColumns.Add(memory, memory);

            string dies = v_EngMarketingReport.Dies.ToString();
            includeColumns.Add(dies, dies);

            string controller = v_EngMarketingReport.ASICController_v_EngMarketingReport_Parent.ToString();
            includeColumns.Add(controller, controller);

            object productLineValue = filterFields[v_EngMarketingReport.ProductLine_v_EngMarketingReport_Parent.ToString()].Value;
            if (productLineValue != null)
            {
                string productLineValueString = productLineValue.ToString();
                if (productLineValueString != string.Empty)
                {
                    int productLineValueInt = -1;
                    if (Int32.TryParse(productLineValueString, out productLineValueInt))
                    {
                        BusinessLogic.ProductLineParameters productLineParameters =
                            new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.ProductLineParameters();

                        BusinessLogic.Json.ProductLineParametersInfo productLineParametersInfo = productLineParameters.GetProductLineParametersInfoByProductLine(productLineValueInt);

                        View technologyProductClassCapacityView = (View)Map.Database.Views[Allegro1Views.v_TechnologyProductClassCapacity.ToString()];


                        foreach (string name in productLineParametersInfo.List)
                        {
                            if (technologyProductClassCapacityView.Fields.ContainsKey(name))
                            {
                                Field field = technologyProductClassCapacityView.Fields[name];
                                if (field.FieldType == FieldType.Children)
                                {
                                    foreach (ColumnField columnField in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
                                    {
                                        if (columnField.Name.StartsWith("PMM_") || columnField.Name.StartsWith("Eng_"))
                                        {
                                            string columnName = columnField.Name.Remove(0, 4);

                                            if (field.Name.Contains(columnName))
                                            {
                                                includeColumns.Add(columnField.Name, columnField.Name);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string pmmColumnName = "PMM_" + field.GetColumnsNames()[0];
                                    Field pmmField = view.GetFieldByColumnNames(pmmColumnName);
                                    if (pmmField != null)
                                    {
                                        includeColumns.Add(pmmField.Name, pmmField.Name);
                                    }

                                    string engColumnName = "Eng_" + field.GetColumnsNames()[0];
                                    Field engField = view.GetFieldByColumnNames(engColumnName);
                                    if (engField != null)
                                    {
                                        includeColumns.Add(engField.Name, engField.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Durados.Web.Mvc.UI.ColumnsExcluder columnsExcluder  = base.GetNewColumnsExcluder(view, filterFields);

            foreach (Field field in view.Fields.Values)
            {
                if (!includeColumns.ContainsKey(field.Name))
                {
                    columnsExcluder.ExcludedColumns.Add(field.Name, field);
                }
            }

            return columnsExcluder;
        }
 
    }
}

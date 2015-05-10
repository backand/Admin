using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.SanDisk.RMA.Controllers
{
    public class RmaRmaController : RmaBaseController
    {

        protected override void BeforeCreate(CreateEventArgs e)
        {
            AddDefaultValue(e, "RmaPriorityId");
            AddDefaultValue(e, "RmaStatusId");
            AddDefaultValue(e, "RmaIssueCategoryId");
            AddDefaultValue(e, "MediaSupplierId");

            Field initiatorField = e.View.GetFieldByColumnNames("RmaInitiatorUserId");
            string initiatorFieldName = null;
            if (initiatorField != null)
                initiatorFieldName = initiatorField.Name;
            else
                throw new DuradosException("Could not find the RMA initiator field");

            if (!e.Values.ContainsKey(initiatorFieldName))
                e.Values.Add(initiatorFieldName, string.Empty);
                        
            DataRow userRow = Map.Database.GetUserRow();
            string currentUser = null;
            if (userRow != null && !userRow.IsNull("Id"))
                currentUser = userRow["Id"].ToString();

            if (currentUser != null)
            {
                e.Values[initiatorFieldName] = currentUser;
            }

            Field initiatorCountryField = e.View.GetFieldByColumnNames("RmaCountryId"); 
            string initiatorCountryFieldName = null;
            if (initiatorCountryField != null)
                initiatorCountryFieldName = initiatorCountryField.Name;
            else
                throw new DuradosException("Could not find the RMA initiator country field");
            if (!e.Values.ContainsKey(initiatorCountryFieldName))
                e.Values.Add(initiatorCountryFieldName, string.Empty);
                
                    
            string currentUserCountry = null;
            if (userRow != null && !userRow.IsNull("CountryId"))
                currentUserCountry = userRow["CountryId"].ToString();

            if (currentUserCountry != null)
            {
                e.Values[initiatorCountryFieldName] = currentUserCountry;
            }

            string faLocationFieldName = null;
            Field faLocationField = e.View.GetFieldByColumnNames("LocationId");
            if (faLocationField != null)
                faLocationFieldName = faLocationField.Name;
            else
                throw new DuradosException("Could not find the RMA Location field");

            string rmaOwnerFieldName = null;
            Field rmaOwnerField = e.View.GetFieldByColumnNames("RmaOwnerUserId");
            if (rmaOwnerField != null)
                rmaOwnerFieldName = rmaOwnerField.Name;
            else
                throw new DuradosException("Could not find the RMA owner field");


            if (!e.Values.ContainsKey(rmaOwnerFieldName))
                e.Values.Add(rmaOwnerFieldName, string.Empty);
                                
            if (e.Values.ContainsKey(faLocationFieldName) && e.Values[faLocationFieldName] != null)
            {
                string faLocation = e.Values[faLocationFieldName].ToString();
                if (!string.IsNullOrEmpty(faLocation))
                {
                    View faLocationView = (View) Map.Database.Views["v_FaLocation"];
                    DataRow faLocationRow = faLocationView.GetDataRow(faLocation);

                    if (faLocationRow != null)
                    {
                        using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand("select UserId from LabManager with (nolock) where FaLocationId = " + faLocation, connection))
                            {
                                object scalar = command.ExecuteScalar();
                                if (!(scalar is DBNull || scalar == null))
                                {
                                    string lm = scalar.ToString();
                                    e.Values[rmaOwnerFieldName] = lm;
                                }

                            }
                        }
                    }
                        
                        
                }
            }

            base.BeforeCreate(e);
        }

        private void AddDefaultValue(CreateEventArgs e, string columnName)
        {
            Field field = e.View.GetFieldByColumnNames(columnName);
            string fieldName = null;
            if (field != null)
                fieldName = field.Name;
            else
                throw new DuradosException("Could not find " + columnName);

            if (!e.Values.ContainsKey(fieldName))
                e.Values.Add(fieldName, string.Empty);

            if (field.DefaultValue != null)
            {
                e.Values[fieldName] = field.DefaultValue.ToString();
            }
        }
    }
}

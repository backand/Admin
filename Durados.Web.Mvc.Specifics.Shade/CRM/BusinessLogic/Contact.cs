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
using Durados.Web.Mvc.Specifics.Shade.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic
{
    public class Contact
    {
        public Contact()
        {
        }

        public Json.ContactInfo GetContactInfo(string pk)
        {
            View contactView = (View)Durados.Web.Mvc.Map.Database.Views[ShadeViews.V_Contact.ToString()];

            Json.ContactInfo contactInfo = new Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Json.ContactInfo();

            DataRow contactRow = contactView.GetDataRow(pk);

            ColumnField field = (ColumnField)contactView.Fields[V_Contact.Cellular.ToString()];
            string val = field.GetValue(contactRow);
            contactInfo.ClientCellular = val ?? string.Empty;

            field = (ColumnField)contactView.Fields[V_Contact.Email.ToString()];
            val = field.GetValue(contactRow);
            contactInfo.ClientEmail = val ?? string.Empty;

            field = (ColumnField)contactView.Fields[V_Contact.FullName.ToString()];
            val = field.GetValue(contactRow);
            contactInfo.ClientName = val ?? string.Empty;

            field = (ColumnField)contactView.Fields[V_Contact.Phone.ToString()];
            val = field.GetValue(contactRow);
            contactInfo.ClientPhone = val ?? string.Empty;

            View addressView = (View)Durados.Web.Mvc.Map.Database.Views[ShadeViews.Address.ToString()];
            ParentField addressField = (ParentField)contactView.Fields[V_Contact.FK_AddressOther_V_Contact_Parent.ToString()];
            val = addressField.GetValue(contactRow);
            if (string.IsNullOrEmpty(val))
            {
                addressField = (ParentField)contactView.Fields[V_Contact.FK_Address_V_Contact_Parent.ToString()];
                val = addressField.GetValue(contactRow);
                contactInfo.AddressID = val ?? string.Empty;

                DataRow addressRow = contactRow.GetParentRow(addressField.DataRelation.RelationName);
                if (addressRow != null)
                {
                    field = (ColumnField)addressView.Fields[Address.FullAddress.ToString()];
                    val = field.GetValue(addressRow);
                    contactInfo.AddressText = val ?? string.Empty;
                }
                else
                {
                    contactInfo.AddressText = string.Empty;
                }
            
            }
            else
            {
                contactInfo.AddressID = val ?? string.Empty;

                DataRow addressRow = contactRow.GetParentRow(addressField.DataRelation.RelationName);
                if (addressRow != null)
                {
                    field = (ColumnField)addressView.Fields[Address.FullAddress.ToString()];
                    val = field.GetValue(addressRow);
                    contactInfo.AddressText = val ?? string.Empty;
                }
                else
                {
                    contactInfo.AddressText = string.Empty;
                }
            }

            //field = (ColumnField)contactView.Fields[V_Contact.MailingCountry.ToString()];
            //val = field.GetValue(contactRow);
            //contactInfo.JobCountry = val ?? string.Empty;

            //field = (ColumnField)contactView.Fields[V_Contact.MailingState.ToString()];
            //val = field.GetValue(contactRow);
            //contactInfo.JobState = val ?? string.Empty;


            //field = (ColumnField)contactView.Fields[V_Contact.MailingStreet.ToString()];
            //val = field.GetValue(contactRow);
            //contactInfo.JobStreet = val ?? string.Empty;

            //field = (ColumnField)contactView.Fields[V_Contact.MailingZip.ToString()];
            //val = field.GetValue(contactRow);
            //contactInfo.JobZip = val ?? string.Empty;
            
            return contactInfo;
        }

    }
}

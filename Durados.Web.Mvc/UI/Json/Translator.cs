using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Translator
    {
        Database database;

        public Translator(Database database)
        {
            this.database = database;
        }

        [DataMember]
        public string add
        {
            get
            {
                return database.Localizer.Translate("Add");
            }
            set { }
        }

        [DataMember]
        public string to
        {
            get
            {
                return database.Localizer.Translate("To");
            }
            set { }
        }

        [DataMember]
        public string between
        {
            get
            {
                return database.Localizer.Translate("Between");
            }
            set { }
        }

        [DataMember]
        public string all
        {
            get
            {
                return database.Localizer.Translate("(All)");
            }
            set { }
        }

        [DataMember]
        public string addAnother
        {
            get
            {
                return database.Localizer.Translate("Add Another");
            }
            set { }
        }

        [DataMember]
        public string cancel
        {
            get
            {
                return database.Localizer.Translate("Cancel");
            }
            set { }
        }

        [DataMember]
        public string addRowTo
        {
            get
            {
                return database.Localizer.Translate("Add") + " ";
            }
            set { }
        }

        [DataMember]
        public string update
        {
            get
            {
                return database.Localizer.Translate("Update");
            }
            set { }
        }

        [DataMember]
        public string editHtmlOn
        {
            get
            {
                return database.Localizer.Translate("Edit Html on ");
            }
            set { }
        }

        [DataMember]
        public string editRowOn
        {
            get
            {
                return database.Localizer.Translate("Edit") + " ";
            }
            set { }
        }

        [DataMember]
        public string viewRowOn
        {
            get
            {
                return database.Localizer.Translate("View") + " ";
            }
            set { }
        }

        [DataMember]
        public string editRowsOn
        {
            get
            {
                return database.Localizer.Translate("Edit Selected Rows on ");
            }
            set { }
        }

        [DataMember]
        public string Delete
        {
            get
            {
                return database.Localizer.Translate("Delete");
            }
            set { }
        }

        [DataMember]
        public string DeleteRowFrom
        {
            get
            {
                return database.Localizer.Translate("Delete") + " ";
            }
            set { }
        }

        [DataMember]
        public string DeleteSelectedRows
        {
            get
            {
                return database.Localizer.Translate("Delete Selected Rows");
            }
            set { }
        }


        [DataMember]
        public string New
        {
            get
            {
                return database.Localizer.Translate("New");
            }
            set { }
        }

        [DataMember]
        public string close
        {
            get
            {
                return database.Localizer.Translate("Close");
            }
            set { }
        }

        [DataMember]
        public string Settings
        {
            get
            {
                return database.Localizer.Translate("Settings");
            }
            set { }
        }

        [DataMember]
        public string maximize
        {
            get
            {
                return database.Localizer.Translate("Maximize");
            }
            set { }
        }

        [DataMember]
        public string restore
        {
            get
            {
                return database.Localizer.Translate("Restore");
            }
            set { }
        }

        [DataMember]
        public string save
        {
            get
            {
                return database.Localizer.Translate("Save");
            }
            set { }
        }

        [DataMember]
        public string select
        {
            get
            {
                return database.Localizer.Translate("Select");
            }
            set { }
        }

        [DataMember]
        public string ok
        {
            get
            {
                return database.Localizer.Translate("OK");
            }
            set { }
        }

        [DataMember]
        public string saveAndClose
        {
            get
            {
                return database.Localizer.Translate("Save and Close");
            }
            set { }
        }

        [DataMember]
        public string saveAndCommit
        {
            get
            {
                return database.Localizer.Translate("Save and Commit");
            }
            set { }
        }

        [DataMember]
        public string saveAndComplete
        {
            get
            {
                return database.Localizer.Translate("Save and Promote");
            }
            set { }
        }

        [DataMember]
        public string saveAnd
        {
            get
            {
                return database.Localizer.Translate("Save and ");
            }
            set { }
        }


        [DataMember]
        public string UrHere
        {
            get
            {
                return database.Localizer.Translate("You are here");
            }
            set { }
        }


        [DataMember]
        public string WorflowSteps
        {
            get
            {
                return database.Localizer.Translate("Workflow Steps");
            }
            set { }
        }

        [DataMember]
        public string MoreInfo
        {
            get
            {
                return database.Localizer.Translate("More Info...");
            }
            set { }
        }

        [DataMember]
        public string SystemInfoMsg
        {
            get
            {
                return database.Localizer.Translate("System Information Message");
            }
            set { }
        }

        [DataMember]
        public string SystemConfigMsg
        {
            get
            {
                return database.Localizer.Translate("System Configuration Message");
            }
            set { }
        }

        [DataMember]
        public string ChangeTypeTitle
        {
            get
            {
                string title = database.Localizer.Translate("ChangeTypeTitle");
                return title == "ChangeTypeMessage" ? "Change Type Confirmation" : title;
            }
            set { }
        }

        [DataMember]
        public string ChangeTypeMessage
        {
            get
            {
                string message = database.Localizer.Translate("ChangeTypeMessage");
                return string.Format("<div class=\"change-type\">{0}", message == "ChangeTypeMessage" ? "<h6>DATA LOSS!!!</h6><br>If you change the field type all data in this column will be deleted.<br>Are you sure you want to change the field type ?" : message);
            }
            set { }
        }

        [DataMember]
        public string DeleteFileTitle
        {
            get
            {
                string title = database.Localizer.Translate("DeleteFileTitle");
                return title == "DeleteFileTitle" ? "Delete File" : title;
            }
            set { }
        }

        [DataMember]
        public string DeleteFileMessage
        {
            get
            {
                string message = database.Localizer.Translate("DeleteFileMessage");
                return string.Format("<div class=\"delete-file\">{0}", message == "DeleteFileMessage" ? "The file will be deleted. Are you sure?" : message);
            }
            set { }
        }


        [DataMember]
        public string CancelChangesTitle
        {
            get
            {
                string title = database.Localizer.Translate("CancelChangesTitle");
                return title == "CancelChangesTitle" ? "Cancel Changes" : title;
            }
            set { }
        }

        [DataMember]
        public string CancelChangesMessage
        {
            get
            {
                string message = database.Localizer.Translate("CancelChangesMessage");
                return string.Format("<div class=\"cancel-changes\">{0}", message == "CancelChangesMessage" ? "Are you sure  you want to cancel the changes?" : message);
            }
            set { }
        }

        [DataMember]
        public string GeneralErrorMessage
        {
            get
            {
                string message = database.Localizer.Translate("GeneralErrorMessage");
                return message == "GeneralErrorMessage" ? Maps.Instance.GetMap().Database.GeneralErrorMessage : message;
            }
            set { }
        }

        [DataMember]
        public string CreateBasicApp
        {
            get
            {
                return database.Localizer.Translate("Create Basic Console");
            }
            set { }
        }

        [DataMember]
        public string ApplyColorDesignTitle
        {
            get
            {
                string title = database.Localizer.Translate("ApplyColorDesignTitle");
                return title == "ApplyColorDesignTitle" ? "Apply Color Design Confirmation" : title;
            }
            set { }
        }

        [DataMember]
        public string ApplyColorDesignMessage
        {
            get
            {
                string message = database.Localizer.Translate("ApplyColorDesignMessage");

                message = message == "ApplyColorDesignMessage" ? "Are you sure that you want to apply the Color Design to all the views in the system? <br><br>This will take affect after saving the settings." : message;
                return string.Format("<div class=\"apply-all-views\">{0}", message);
                ;
            }
            set { }
        }

        [DataMember]
        public string ApplySkinTitle
        {
            get
            {
                string title = database.Localizer.Translate("ApplySkinTitle");
                return title == "ApplySkinTitle" ? "Apply Skin Confirmation" : title;
            }
            set { }
        }

        [DataMember]
        public string ApplySkinMessage
        {
            get
            {
                string message = database.Localizer.Translate("ApplySkinMessage");
                return string.Format("<div class=\"apply-all-views\">{0}", message == "ApplySkinMessage" ? "Are you sure that you want to apply the skin to all the views in the system? <br><br>This will take affect after saving the settings." : message);
            }
            set { }

        }
        [DataMember]
        public string WebMasterAccess
        {
            get
            {
                string message = database.Localizer.Translate("WebMasterAccess");
                return message == "WebMasterAccess" ? "Web Master Access" : message;
            }
            set { }
        }

        [DataMember]
        public string sequenceLoadWaitMessage
        {
            get
            {
                string message = database.Localizer.Translate("sequenceLoadWaitMessage");
                return message == "sequenceLoadWaitMessage" ? "Sequence is loading, please wait..." : message;
            }
            set { }
        }

        [DataMember]
        public string LoadingSettingsMessage
        {
            get
            {
                return database.Localizer.Translate("Loading Settings...");

            }
            set { }
        }
    }
}

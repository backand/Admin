using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Durados.Windows.Utilities.AzureUploader.DeleteApp
{
    public partial class DeleteAppForm : Form
    {
        public DeleteAppForm()
        {
            InitializeComponent();
        }

        private void DeleteById_button_Click(object sender, EventArgs e)
        {
            int appId;
            if(int.TryParse(AppID_TextBox.Text,out appId))
            {
            DeleteAppManager deleteAppManager = new DeleteAppManager();
            logMessages.Text = deleteAppManager.DeleteApp(appId.ToString(), null);
            }
            else
                logMessages.Text = "App Id Not numeric";
        }

        private void DeleteByName_button_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(AppName_TextBox.Text))
            {
            DeleteAppManager deleteAppManager = new DeleteAppManager();
            logMessages.Text = deleteAppManager.DeleteApp(null, AppName_TextBox.Text);
            }
            else
                logMessages.Text = "App name empty";
            
           
        
        }

        private void DeleteAppViewb_Button_Click(object sender, EventArgs e)
        {
            DeleteAppManager deleteAppManager = new DeleteAppManager();
            //log_label.Text = deleteAppManager.DeleteAppView().Replace("<br>","\r\n");
            deleteAppManager.resultOutput = logMessages;
            logMessages.Text = deleteAppManager.DeleteAppView().Replace("<br>", "\r\n");
            
        }

        private void toDeleteView_Click(object sender, EventArgs e)
        {

            toDeletedAppsView.Items.Clear();
            DataTable dt = AppFactory.GetToDeleteAppsTable(null);

            foreach (DataRow row in dt.Rows)
            {
                int i = 0;
                //ListViewItem lstViewItem;
                ListViewItem item = new ListViewItem(row["Id"].ToString());
                foreach (DataColumn column in dt.Columns)
                {
                    try
                    {
                        i++;
                        item.SubItems.Add(row[i].ToString());

                    }
                    catch
                    {

                    }
                }
                toDeletedAppsView.Items.Add(item);
            }
            appsCountlbl.Text = dt.Rows.Count.ToString();
        }

        
        
    }
}

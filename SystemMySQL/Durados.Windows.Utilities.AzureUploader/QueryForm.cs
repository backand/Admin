using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Durados.Windows.Utilities.AzureUploader
{
    public partial class QueryForm : Form
    {
        public QueryForm()
        {
            InitializeComponent();
            
            

        }

        private void LoadDefaults()
        {
            try
            {
                connectionTextBox.Text = Properties.Settings.Default[queryEditor.UploadType == UploadType.Config ? "Connection" : "LogoConnection"].ToString();
                queryTextBox.Text = Properties.Settings.Default[queryEditor.UploadType == UploadType.Config ? "Query" : "LogoQuery"].ToString();
            }
            catch { }
        }


        private QueryEditor queryEditor;
        public QueryEditor QueryEditor
        {
            get
            {
                return queryEditor;
            }
            set
            {
                queryEditor = value;
                LoadDefaults();

            }
        }

        private string ConnectionString
        {
            get
            {
                return connectionTextBox.Text;
            }
        }

        private string Query
        {
            get
            {
                return queryTextBox.Text;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            QueryEditor.DataTable = GetTable(ConnectionString, Query);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private DataTable GetTable(string connectionString, string query)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }

            return table;
        }

        private void QueryForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            SaveDefaults();
        }

        private void SaveDefaults()
        {
            try
            {
                Properties.Settings.Default[queryEditor.UploadType == UploadType.Config ? "Connection" : "LogoConnection"] = connectionTextBox.Text;
                Properties.Settings.Default[queryEditor.UploadType == UploadType.Config ? "Query" : "LogoQuery"] = queryTextBox.Text;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Durados.Windows.Utilities.AzureUploader
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            (new ConfigUploadForm()).Show();
        }

        private void logoButton_Click(object sender, EventArgs e)
        {
            (new LogoUploadForm()).Show();
        }

        private void backupButton_Click(object sender, EventArgs e)
        {
            (new BackupForm()).Show();
        }

        private void DeleteApp_button_Click(object sender, EventArgs e)
        {
            (new DeleteApp.DeleteAppForm()).Show();
        }
    }
}

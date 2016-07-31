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
    public partial class BackupForm : Form
    {
        Backup backup = new Backup();
        public BackupForm()
        {
            InitializeComponent();

            LoadSettings();

            backup.BackupStarted += backup_BackupStarted;
            backup.BackupContainerStarted += backup_BackupContainerStarted;
            backup.BackupContainerEnded += backup_BackupContainerEnded;
            backup.BackupEnded += backup_BackupEnded;
        }

        private void LoadSettings()
        {
            backup.LoadSettings();

            copiesNumericUpDown.Value = backup.Copies;
            accountNameTextBox.Text = backup.StorageCred.Name;
            accountKeyTextBox.Text = backup.StorageCred.Key;
            backupAccountNameTextBox.Text = backup.BackupStorageCred.Name;
            backupAccountKeyTextBox.Text = backup.BackupStorageCred.Key;
        }

        private void SaveSettings()
        {
            backup.SaveSettings();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            filesListView.Items.Clear();
            outOfLabel.Text = 0.ToString();
            filesLabel.Text = 0.ToString();
            avgLabel.Text = 0.ToString();
            timePassedLabel.Text = 0.ToString();
            progressBar.Value = 0;
            successLabel.Text = 0.ToString();
            failureLabel.Text = 0.ToString();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            backup.StorageCred = new StorageCred() { Name = accountNameTextBox.Text, Key = accountKeyTextBox.Text };
            backup.BackupStorageCred = new StorageCred() { Name = backupAccountNameTextBox.Text, Key = backupAccountKeyTextBox.Text };
            backup.Copies = Convert.ToInt32(copiesNumericUpDown.Value);

            try
            {
                backup.All();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        string timeFormat = @"hh\:mm\:ss";
        int i = 0;
        int success = 0;
        int failure = 0;
        ListViewItem item = null;
        DateTime startAll;

        private void backup_BackupStarted(object sender, BackupStartedEventArgs e)
        {
            i = 0;
            success = 0;
            failure = 0;
            outOfLabel.Text = e.ContainersCount.ToString();
            progressBar.Maximum = e.ContainersCount + 1;
            progressBar.Value = 0;
            startAll = DateTime.Now;
            
        }

        private void backup_BackupContainerStarted(object sender, BackupContainerStartedEventArgs e)
        {
            item = new ListViewItem();
            item.Text = e.Container.Name;
            SetSubItem(item, 2, e.Occured.ToString(timeFormat));
            filesListView.Items.Add(item);

            Application.DoEvents();
        }

        private void SetSubItem(ListViewItem item, int index, string value)
        {
            if (item.SubItems.Count>index)
            {
                item.SubItems[index].Text = value;
            }
            else
            {
                for (int i = item.SubItems.Count; i <= index; i++)
                {
                    item.SubItems.Add(string.Empty);
                }
                item.SubItems[index].Text = value;
            }
        }

        private void backup_BackupContainerEnded(object sender, BackupContainerEndedEventArgs e)
        {
            SetSubItem(item, 1, e.LastModified.HasValue ? e.LastModified.Value.ToString(timeFormat) : string.Empty);
            SetSubItem(item, 3, e.Occured.ToString(timeFormat));
            SetSubItem(item, 0, e.Modified.HasValue ? (e.Modified.Value ? "Modified" : "Not Modified") : string.Empty);
            SetSubItem(item, 5, e.Success ? string.Empty : e.Exception.Message);
            SetSubItem(item, 6, e.Success ? string.Empty : e.Exception.StackTrace);
           
            i++;

            if (e.Success)
            {
                success++;
            }
            else
            {
                failure++;
            }

            progressBar.Value = i;
            filesLabel.Text = i.ToString();
            timePassedLabel.Text = DateTime.Now.Subtract(startAll).ToString(timeFormat);
            successLabel.Text = success.ToString();
            failureLabel.Text = failure.ToString();
               
            Application.DoEvents();
        }

        private void backup_BackupEnded(object sender, BackupEndedEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        
        private void BackupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
    }
}

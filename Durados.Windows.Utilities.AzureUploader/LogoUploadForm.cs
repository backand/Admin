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
    public partial class LogoUploadForm : Form
    {
        private QueryEditor queryEditor = null;
        private Report report = null;
        public LogoUploadForm()
        {
            InitializeComponent();

            Clear();

            queryEditor = new QueryEditor(UploadType.Logo);
            report = new Report();

            LoadDefaults();
        
            
        }

        private void LoadDefaults()
        {
            try
            {
                folderTextBox.Text = Properties.Settings.Default["LogoFolder"].ToString();
                azureAccountKeyTextBox.Text = Properties.Settings.Default["LogoazureAccountKey"].ToString();
                azureAccountNameTextBox.Text = Properties.Settings.Default["LogoazureAccountName"].ToString();
            }
            catch { }
        }


        
        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void selectFolderButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        

        private void LoadFiles(Logo[] logos)
        {
            foreach (Logo logo in logos)
            {
                ListViewItem listViewItem = GetListViewItem(logo);
                filesListView.Items.Add(listViewItem);
            }
        }

        private ListViewItem GetListViewItem(Logo logo)
        {
            ListViewItem listViewItem = new ListViewItem();

            FileInfo fileInfo = new FileInfo(logo.FileName);

            listViewItem.Text = fileInfo.FileName;
            listViewItem.SubItems.Add(logo.AppID);
            if (fileInfo.Exists)
            {
                listViewItem.SubItems.Add(fileInfo.LastUpdate.ToString());
            }
            else
            {
                listViewItem.SubItems.Add(string.Empty);
            }
            listViewItem.SubItems.Add(string.Empty);
            listViewItem.SubItems.Add(string.Empty);
            listViewItem.SubItems.Add(string.Empty);
            if (fileInfo.Exists)
            {
                listViewItem.SubItems.Add(string.Empty);
            }
            else
            {
                listViewItem.SubItems.Add("File not Found");
            }
            listViewItem.SubItems.Add(string.Empty);
            return listViewItem;
        }

        private void runQueryButton_Click(object sender, EventArgs e)
        {
            if (queryEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Logo[] logos = GetLogos(queryEditor.DataTable);
                LoadFiles(logos);
            }
        }

        private Logo[] GetLogos(DataTable table)
        {
            List<Logo> list = new List<Logo>();

            foreach (DataRow row in table.Rows)
            {
                string appId = row.IsNull(0) ? null : row[0].ToString();
                string image = row.IsNull(1) ? null : row[1].ToString();
                string fileName = GetFileName(appId, image);
                Logo logo = new Logo() { FileName = fileName, AppID = appId};
                list.Add(logo);
                
            }

            return list.ToArray();
        }

        private string GetFileName(string appId, string image)
        {
            return folderTextBox.Text + @"\" + (image.StartsWith(appId) ? "" : (appId + @"\")) + image.Replace('/','\\');
        }

        private void UploadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDefaults();
        }

        private void SaveDefaults()
        {
            try
            {
                Properties.Settings.Default["LogoFolder"] = folderTextBox.Text;
                Properties.Settings.Default["LogoazureAccountKey"] = azureAccountKeyTextBox.Text;
                Properties.Settings.Default["LogoazureAccountName"] = azureAccountNameTextBox.Text;

                
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        private void saveReportButton_Click(object sender, EventArgs e)
        {
            SaveReportButton();
        }

        private void SaveReportButton()
        {
            if (reportSaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                report.ListViewToCSV(filesListView, reportSaveFileDialog.FileName, true);
            }
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

        Storage storage = new Storage();
        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                saveReportButton.Enabled = false;
                try { Connect(); }
                catch (Exception exception) { MessageBox.Show(exception.Message); return; }

                try { StartUpload(); }
                catch (Exception exception) { MessageBox.Show(exception.Message); return; }
                saveReportButton.Enabled = true;
            }
            finally
            {
                Cursor.Current = Cursors.Default;

            }
        }

        private void Connect()
        {
            storage.Connect(azureAccountNameTextBox.Text, azureAccountKeyTextBox.Text);
        }

        private void StartUpload()
        {
            string timeFormat = @"hh\:mm\:ss";
            DateTime startAll = DateTime.Now;
            Stat stat = new Stat();
            outOfLabel.Text = filesListView.Items.Count.ToString();

            int i = 0;
            progressBar.Maximum = filesListView.Items.Count;
            progressBar.Value = 0;
            foreach (ListViewItem item in filesListView.Items)
            {
                i++;
                progressBar.Value = i;
                if (!string.IsNullOrEmpty(item.SubItems[6].Text))
                {
                    filesLabel.Text = i.ToString();
                    timePassedLabel.Text = DateTime.Now.Subtract(startAll).ToString(timeFormat);
                    Application.DoEvents(); continue;
                }

                string filename = item.Text;
                DateTime start = DateTime.Now;
                item.SubItems[3].Text = start.ToString(timeFormat);
                try
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                    UploadFile("app" + item.SubItems[1].Text, fi.Name, filename);
                    stat.Successes++;
                    filesListView.Items[i - 1].BackColor = Color.Green;
                }
                catch (Exception exception)
                {
                    item.SubItems[6].Text = exception.Message;
                    item.SubItems[7].Text = exception.StackTrace;
                    stat.Failures++; 
                    filesListView.Items[i - 1].BackColor = Color.Red;
                    failureLabel.Text = stat.Failures.ToString();
                }
                DateTime now = DateTime.Now;
                TimeSpan duration = now.Subtract(start);
                item.SubItems[4].Text = now.ToString(timeFormat);
                item.SubItems[5].Text = duration.ToString(timeFormat);
                stat.samples.Add(new Sample() { Duration = duration });
                filesLabel.Text = i.ToString();
                avgLabel.Text = stat.Avg().ToString(timeFormat);
                timePassedLabel.Text = DateTime.Now.Subtract(startAll).ToString(timeFormat);
                successLabel.Text = stat.Successes.ToString();
                Application.DoEvents();
            }
        }

        private void UploadFile(string appId, string blobName, string fileName)
        {
            storage.Upload(appId, blobName, fileName);
        }

        private void failureLabel_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void successLabel_Click(object sender, EventArgs e)
        {

        }

        private void avgLabel_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void timePassedLabel_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void outOfLabel_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void filesLabel_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
    }

    public class Logo
    {
        public string FileName { get; set; }
        public string AppID { get; set; }
    }
}

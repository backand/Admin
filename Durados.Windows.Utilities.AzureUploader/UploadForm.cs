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
    public partial class UploadForm : Form
    {
        private QueryEditor queryEditor = null;
        private Report report = null;
        public UploadForm()
        {
            InitializeComponent();

            Clear(); 

            queryEditor = new QueryEditor();
            report = new Report();

            LoadDefaults();
        
            
        }

        private void LoadDefaults()
        {
            try
            {
                folderTextBox.Text = Properties.Settings.Default["Folder"].ToString();
                changeInTheLastHoursNumericUpDown.Value = Convert.ToDecimal(Properties.Settings.Default["Hours"]);
                azureAccountKeyTextBox.Text = Properties.Settings.Default["azureAccountKey"].ToString();
                azureAccountNameTextBox.Text = Properties.Settings.Default["azureAccountName"].ToString();
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

        private void selectFilesButton_Click(object sender, EventArgs e)
        {
            xmlOpenFileDialog.InitialDirectory = folderTextBox.Text;
            if (xmlOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFiles(xmlOpenFileDialog.FileNames);
            }
        }

        private void LoadFiles(string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                ListViewItem listViewItem = GetListViewItem(fileName);
                filesListView.Items.Add(listViewItem);
            }
        }

        private ListViewItem GetListViewItem(string fileName)
        {
            ListViewItem listViewItem = new ListViewItem();

            FileInfo fileInfo = new FileInfo(fileName);

            listViewItem.Text = fileInfo.Name;
            listViewItem.SubItems.Add(fileInfo.FileType.ToString());
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
                if (changeInTheLastHoursNumericUpDown.Value > 0 && Convert.ToDecimal(fileInfo.LastUpdate) > changeInTheLastHoursNumericUpDown.Value)
                {
                    listViewItem.SubItems.Add("Old");
                }
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
                string[] fileNames = GetFileNames(queryEditor.DataTable);
                LoadFiles(fileNames);
            }
        }

        private string[] GetFileNames(DataTable table)
        {
            List<string> list = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                string fileName = GetFileName(row[0].ToString());
                list.Add(fileName);
                list.Add(fileName + ".xml");
            }

            return list.ToArray();
        }

        private string GetFileName(string appId)
        {
            return folderTextBox.Text + string.Format("\\durados_AppSys_{0}.xml", appId);
        }

        private void UploadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDefaults();
        }

        private void SaveDefaults()
        {
            try
            {
                Properties.Settings.Default["Folder"] = folderTextBox.Text;
                Properties.Settings.Default["Hours"] = Convert.ToInt32(changeInTheLastHoursNumericUpDown.Value);
                Properties.Settings.Default["azureAccountKey"] = azureAccountKeyTextBox.Text;
                Properties.Settings.Default["azureAccountName"] = azureAccountNameTextBox.Text;

                
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

                string filename = folderTextBox.Text + "\\" + item.Text;
                DateTime start = DateTime.Now;
                item.SubItems[3].Text = start.ToString(timeFormat);
                try
                {
                    UploadFile(filename);
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

        private void UploadFile(string fileName)
        {
            storage.Upload(fileName);
        }
    }

    public class Stat
    {
        public List<Sample> samples { get; private set; }
        public int Successes { get; set; }
        public int Failures { get; set; }
        public Stat()
        {
            samples = new List<Sample>();
        }

        public double AvgInMilliseconds()
        {
            if (samples.Count == 0)
                return 0;
            return samples.Average(s => s.Duration.TotalMilliseconds);
        }

        public TimeSpan Avg()
        {
            return new TimeSpan(0, 0, 0, 0, Convert.ToInt32(AvgInMilliseconds()));
        }
    }

    public class Sample
    {
        public TimeSpan Duration { get; set; }
    }
}

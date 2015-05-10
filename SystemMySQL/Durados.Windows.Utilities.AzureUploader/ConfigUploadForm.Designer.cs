namespace Durados.Windows.Utilities.AzureUploader
{
    partial class ConfigUploadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectFilesButton = new System.Windows.Forms.Button();
            this.runQueryButton = new System.Windows.Forms.Button();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.filesListView = new System.Windows.Forms.ListView();
            this.fileNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastUpdateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finishedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.durationColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.exceptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.traceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.changeInTheLastHoursNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.azureAccountKeyTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.azureAccountNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.saveReportButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.filesLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.outOfLabel = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.timePassedLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.avgLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.xmlOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.reportSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.successLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.failureLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.changeInTheLastHoursNumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectFilesButton
            // 
            this.selectFilesButton.Location = new System.Drawing.Point(244, 29);
            this.selectFilesButton.Name = "selectFilesButton";
            this.selectFilesButton.Size = new System.Drawing.Size(75, 23);
            this.selectFilesButton.TabIndex = 0;
            this.selectFilesButton.Text = "Select Files";
            this.selectFilesButton.UseVisualStyleBackColor = true;
            this.selectFilesButton.Click += new System.EventHandler(this.selectFilesButton_Click);
            // 
            // runQueryButton
            // 
            this.runQueryButton.Location = new System.Drawing.Point(331, 29);
            this.runQueryButton.Name = "runQueryButton";
            this.runQueryButton.Size = new System.Drawing.Size(87, 23);
            this.runQueryButton.TabIndex = 1;
            this.runQueryButton.Text = "Run Query";
            this.runQueryButton.UseVisualStyleBackColor = true;
            this.runQueryButton.Click += new System.EventHandler(this.runQueryButton_Click);
            // 
            // folderTextBox
            // 
            this.folderTextBox.Location = new System.Drawing.Point(66, 30);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(100, 20);
            this.folderTextBox.TabIndex = 2;
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(166, 28);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(30, 23);
            this.selectFolderButton.TabIndex = 3;
            this.selectFolderButton.Text = "...";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // filesListView
            // 
            this.filesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileNameColumnHeader,
            this.typeColumnHeader,
            this.lastUpdateColumnHeader,
            this.startedColumnHeader,
            this.finishedColumnHeader,
            this.durationColumnHeader,
            this.exceptionColumnHeader,
            this.traceColumnHeader});
            this.filesListView.Location = new System.Drawing.Point(12, 58);
            this.filesListView.Name = "filesListView";
            this.filesListView.Size = new System.Drawing.Size(1111, 248);
            this.filesListView.TabIndex = 4;
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            // 
            // fileNameColumnHeader
            // 
            this.fileNameColumnHeader.Text = "Name";
            // 
            // typeColumnHeader
            // 
            this.typeColumnHeader.Text = "Type";
            // 
            // lastUpdateColumnHeader
            // 
            this.lastUpdateColumnHeader.Text = "Last Update";
            // 
            // startedColumnHeader
            // 
            this.startedColumnHeader.Text = "Started";
            // 
            // finishedColumnHeader
            // 
            this.finishedColumnHeader.Text = "Finished";
            // 
            // durationColumnHeader
            // 
            this.durationColumnHeader.Text = "Duration";
            // 
            // exceptionColumnHeader
            // 
            this.exceptionColumnHeader.Text = "Exception";
            // 
            // traceColumnHeader
            // 
            this.traceColumnHeader.Text = "Trace";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 312);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1111, 23);
            this.progressBar.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(457, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Files Change from the Last";
            // 
            // changeInTheLastHoursNumericUpDown
            // 
            this.changeInTheLastHoursNumericUpDown.Location = new System.Drawing.Point(595, 31);
            this.changeInTheLastHoursNumericUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.changeInTheLastHoursNumericUpDown.Name = "changeInTheLastHoursNumericUpDown";
            this.changeInTheLastHoursNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.changeInTheLastHoursNumericUpDown.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(644, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Hours";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.azureAccountKeyTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.azureAccountNameTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(733, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(369, 39);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azure Account";
            // 
            // azureAccountKeyTextBox
            // 
            this.azureAccountKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureAccountKeyTextBox.Location = new System.Drawing.Point(224, 16);
            this.azureAccountKeyTextBox.Name = "azureAccountKeyTextBox";
            this.azureAccountKeyTextBox.PasswordChar = '*';
            this.azureAccountKeyTextBox.Size = new System.Drawing.Size(139, 20);
            this.azureAccountKeyTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Key:";
            // 
            // azureAccountNameTextBox
            // 
            this.azureAccountNameTextBox.Location = new System.Drawing.Point(58, 17);
            this.azureAccountNameTextBox.Name = "azureAccountNameTextBox";
            this.azureAccountNameTextBox.Size = new System.Drawing.Size(117, 20);
            this.azureAccountNameTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(881, 337);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // saveReportButton
            // 
            this.saveReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveReportButton.Enabled = false;
            this.saveReportButton.Location = new System.Drawing.Point(962, 337);
            this.saveReportButton.Name = "saveReportButton";
            this.saveReportButton.Size = new System.Drawing.Size(75, 23);
            this.saveReportButton.TabIndex = 10;
            this.saveReportButton.Text = "Save Report";
            this.saveReportButton.UseVisualStyleBackColor = true;
            this.saveReportButton.Click += new System.EventHandler(this.saveReportButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(1043, 337);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 11;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // filesLabel
            // 
            this.filesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.filesLabel.AutoSize = true;
            this.filesLabel.Location = new System.Drawing.Point(15, 342);
            this.filesLabel.Name = "filesLabel";
            this.filesLabel.Size = new System.Drawing.Size(49, 13);
            this.filesLabel.TabIndex = 12;
            this.filesLabel.Text = "0000000";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 342);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "out of";
            // 
            // outOfLabel
            // 
            this.outOfLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.outOfLabel.AutoSize = true;
            this.outOfLabel.Location = new System.Drawing.Point(95, 342);
            this.outOfLabel.Name = "outOfLabel";
            this.outOfLabel.Size = new System.Drawing.Size(49, 13);
            this.outOfLabel.TabIndex = 14;
            this.outOfLabel.Text = "0000000";
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(800, 337);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 15;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(548, 342);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Time Passed:";
            // 
            // timePassedLabel
            // 
            this.timePassedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timePassedLabel.AutoSize = true;
            this.timePassedLabel.Location = new System.Drawing.Point(617, 342);
            this.timePassedLabel.Name = "timePassedLabel";
            this.timePassedLabel.Size = new System.Drawing.Size(49, 13);
            this.timePassedLabel.TabIndex = 17;
            this.timePassedLabel.Text = "00:00:00";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(672, 342);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Avg Time:";
            // 
            // avgLabel
            // 
            this.avgLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.avgLabel.AutoSize = true;
            this.avgLabel.Location = new System.Drawing.Point(729, 342);
            this.avgLabel.Name = "avgLabel";
            this.avgLabel.Size = new System.Drawing.Size(49, 13);
            this.avgLabel.TabIndex = 19;
            this.avgLabel.Text = "00:00:00";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Folder:";
            // 
            // xmlOpenFileDialog
            // 
            this.xmlOpenFileDialog.Multiselect = true;
            this.xmlOpenFileDialog.Title = "Upload XML Files";
            // 
            // reportSaveFileDialog
            // 
            this.reportSaveFileDialog.Filter = "(*.csv)|*.csv";
            this.reportSaveFileDialog.Title = "Save Report to CSV";
            // 
            // successLabel
            // 
            this.successLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.successLabel.AutoSize = true;
            this.successLabel.ForeColor = System.Drawing.Color.Green;
            this.successLabel.Location = new System.Drawing.Point(212, 342);
            this.successLabel.Name = "successLabel";
            this.successLabel.Size = new System.Drawing.Size(49, 13);
            this.successLabel.TabIndex = 21;
            this.successLabel.Text = "0000000";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Green;
            this.label7.Location = new System.Drawing.Point(167, 342);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Success";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(271, 342);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Failure";
            // 
            // failureLabel
            // 
            this.failureLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.failureLabel.AutoSize = true;
            this.failureLabel.ForeColor = System.Drawing.Color.Red;
            this.failureLabel.Location = new System.Drawing.Point(310, 342);
            this.failureLabel.Name = "failureLabel";
            this.failureLabel.Size = new System.Drawing.Size(49, 13);
            this.failureLabel.TabIndex = 23;
            this.failureLabel.Text = "0000000";
            // 
            // ConfigUploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(1135, 368);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.failureLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.successLabel);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.avgLabel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.timePassedLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.outOfLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.filesLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveReportButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.changeInTheLastHoursNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.filesListView);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.folderTextBox);
            this.Controls.Add(this.runQueryButton);
            this.Controls.Add(this.selectFilesButton);
            this.Name = "ConfigUploadForm";
            this.Text = "Upload Config Files to Azure Storage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UploadForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.changeInTheLastHoursNumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectFilesButton;
        private System.Windows.Forms.Button runQueryButton;
        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.ListView filesListView;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown changeInTheLastHoursNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox azureAccountKeyTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox azureAccountNameTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button saveReportButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label filesLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label outOfLabel;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label timePassedLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label avgLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.OpenFileDialog xmlOpenFileDialog;
        private System.Windows.Forms.SaveFileDialog reportSaveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ColumnHeader fileNameColumnHeader;
        private System.Windows.Forms.ColumnHeader typeColumnHeader;
        private System.Windows.Forms.ColumnHeader lastUpdateColumnHeader;
        private System.Windows.Forms.ColumnHeader startedColumnHeader;
        private System.Windows.Forms.ColumnHeader finishedColumnHeader;
        private System.Windows.Forms.ColumnHeader durationColumnHeader;
        private System.Windows.Forms.ColumnHeader exceptionColumnHeader;
        private System.Windows.Forms.ColumnHeader traceColumnHeader;
        private System.Windows.Forms.Label successLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label failureLabel;
    }
}


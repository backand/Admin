namespace Durados.Windows.Utilities.AzureUploader
{
    partial class BackupForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.backupAccountKeyTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.backupAccountNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.filesListView = new System.Windows.Forms.ListView();
            this.fileNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.idColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastUpdateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finishedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.durationColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.exceptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.traceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label9 = new System.Windows.Forms.Label();
            this.failureLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.successLabel = new System.Windows.Forms.Label();
            this.avgLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.timePassedLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.outOfLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.filesLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.saveReportButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.accountKeyTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.accountNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.copiesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copiesNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.backupAccountKeyTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.backupAccountNameTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(782, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(433, 39);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azure Backup Account";
            // 
            // backupAccountKeyTextBox
            // 
            this.backupAccountKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backupAccountKeyTextBox.Location = new System.Drawing.Point(224, 16);
            this.backupAccountKeyTextBox.Name = "backupAccountKeyTextBox";
            this.backupAccountKeyTextBox.PasswordChar = '*';
            this.backupAccountKeyTextBox.Size = new System.Drawing.Size(203, 20);
            this.backupAccountKeyTextBox.TabIndex = 3;
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
            // backupAccountNameTextBox
            // 
            this.backupAccountNameTextBox.Location = new System.Drawing.Point(58, 17);
            this.backupAccountNameTextBox.Name = "backupAccountNameTextBox";
            this.backupAccountNameTextBox.Size = new System.Drawing.Size(117, 20);
            this.backupAccountNameTextBox.TabIndex = 1;
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
            // filesListView
            // 
            this.filesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileNameColumnHeader,
            this.idColumnHeader,
            this.lastUpdateColumnHeader,
            this.startedColumnHeader,
            this.finishedColumnHeader,
            this.durationColumnHeader,
            this.exceptionColumnHeader,
            this.traceColumnHeader});
            this.filesListView.Location = new System.Drawing.Point(12, 57);
            this.filesListView.Name = "filesListView";
            this.filesListView.Size = new System.Drawing.Size(1203, 282);
            this.filesListView.TabIndex = 11;
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            // 
            // fileNameColumnHeader
            // 
            this.fileNameColumnHeader.Text = "Name";
            // 
            // idColumnHeader
            // 
            this.idColumnHeader.Text = "App ID";
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
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(273, 378);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 40;
            this.label9.Text = "Failure";
            // 
            // failureLabel
            // 
            this.failureLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.failureLabel.AutoSize = true;
            this.failureLabel.ForeColor = System.Drawing.Color.Red;
            this.failureLabel.Location = new System.Drawing.Point(312, 378);
            this.failureLabel.Name = "failureLabel";
            this.failureLabel.Size = new System.Drawing.Size(49, 13);
            this.failureLabel.TabIndex = 39;
            this.failureLabel.Text = "0000000";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Green;
            this.label7.Location = new System.Drawing.Point(169, 378);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Success";
            // 
            // successLabel
            // 
            this.successLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.successLabel.AutoSize = true;
            this.successLabel.ForeColor = System.Drawing.Color.Green;
            this.successLabel.Location = new System.Drawing.Point(214, 378);
            this.successLabel.Name = "successLabel";
            this.successLabel.Size = new System.Drawing.Size(49, 13);
            this.successLabel.TabIndex = 37;
            this.successLabel.Text = "0000000";
            // 
            // avgLabel
            // 
            this.avgLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.avgLabel.AutoSize = true;
            this.avgLabel.Location = new System.Drawing.Point(731, 378);
            this.avgLabel.Name = "avgLabel";
            this.avgLabel.Size = new System.Drawing.Size(49, 13);
            this.avgLabel.TabIndex = 36;
            this.avgLabel.Text = "00:00:00";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(674, 378);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Avg Time:";
            // 
            // timePassedLabel
            // 
            this.timePassedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timePassedLabel.AutoSize = true;
            this.timePassedLabel.Location = new System.Drawing.Point(619, 378);
            this.timePassedLabel.Name = "timePassedLabel";
            this.timePassedLabel.Size = new System.Drawing.Size(49, 13);
            this.timePassedLabel.TabIndex = 34;
            this.timePassedLabel.Text = "00:00:00";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(550, 378);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Time Passed:";
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(898, 373);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 32;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // outOfLabel
            // 
            this.outOfLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.outOfLabel.AutoSize = true;
            this.outOfLabel.Location = new System.Drawing.Point(97, 378);
            this.outOfLabel.Name = "outOfLabel";
            this.outOfLabel.Size = new System.Drawing.Size(49, 13);
            this.outOfLabel.TabIndex = 31;
            this.outOfLabel.Text = "0000000";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(63, 378);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "out of";
            // 
            // filesLabel
            // 
            this.filesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.filesLabel.AutoSize = true;
            this.filesLabel.Location = new System.Drawing.Point(17, 378);
            this.filesLabel.Name = "filesLabel";
            this.filesLabel.Size = new System.Drawing.Size(49, 13);
            this.filesLabel.TabIndex = 29;
            this.filesLabel.Text = "0000000";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(1141, 373);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 28;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveReportButton
            // 
            this.saveReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveReportButton.Enabled = false;
            this.saveReportButton.Location = new System.Drawing.Point(1060, 373);
            this.saveReportButton.Name = "saveReportButton";
            this.saveReportButton.Size = new System.Drawing.Size(75, 23);
            this.saveReportButton.TabIndex = 27;
            this.saveReportButton.Text = "Save Report";
            this.saveReportButton.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(979, 373);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 25;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 345);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1203, 23);
            this.progressBar.TabIndex = 26;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.accountKeyTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.accountNameTextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(326, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 39);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Azure Account";
            // 
            // accountKeyTextBox
            // 
            this.accountKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accountKeyTextBox.Location = new System.Drawing.Point(224, 16);
            this.accountKeyTextBox.Name = "accountKeyTextBox";
            this.accountKeyTextBox.PasswordChar = '*';
            this.accountKeyTextBox.Size = new System.Drawing.Size(203, 20);
            this.accountKeyTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(196, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Key:";
            // 
            // accountNameTextBox
            // 
            this.accountNameTextBox.Location = new System.Drawing.Point(58, 17);
            this.accountNameTextBox.Name = "accountNameTextBox";
            this.accountNameTextBox.Size = new System.Drawing.Size(117, 20);
            this.accountNameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            // 
            // copiesNumericUpDown
            // 
            this.copiesNumericUpDown.Location = new System.Drawing.Point(207, 26);
            this.copiesNumericUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.copiesNumericUpDown.Name = "copiesNumericUpDown";
            this.copiesNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.copiesNumericUpDown.TabIndex = 43;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(85, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Number of latest copies:";
            // 
            // BackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 404);
            this.Controls.Add(this.copiesNumericUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.failureLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.successLabel);
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
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.filesListView);
            this.Controls.Add(this.groupBox1);
            this.Name = "BackupForm";
            this.Text = "Backup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackupForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copiesNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox backupAccountKeyTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox backupAccountNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView filesListView;
        private System.Windows.Forms.ColumnHeader fileNameColumnHeader;
        private System.Windows.Forms.ColumnHeader idColumnHeader;
        private System.Windows.Forms.ColumnHeader lastUpdateColumnHeader;
        private System.Windows.Forms.ColumnHeader startedColumnHeader;
        private System.Windows.Forms.ColumnHeader finishedColumnHeader;
        private System.Windows.Forms.ColumnHeader durationColumnHeader;
        private System.Windows.Forms.ColumnHeader exceptionColumnHeader;
        private System.Windows.Forms.ColumnHeader traceColumnHeader;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label failureLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label successLabel;
        private System.Windows.Forms.Label avgLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label timePassedLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label outOfLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label filesLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveReportButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox accountKeyTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox accountNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown copiesNumericUpDown;
        private System.Windows.Forms.Label label5;
    }
}
namespace WindowsFormsApplication2
{
    partial class Form1
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
            this.runButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.reloadXhrButton = new System.Windows.Forms.Button();
            this.overwritePrefixCheckBox = new System.Windows.Forms.CheckBox();
            this.openButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lineBreakNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineBreakNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(313, 1);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lineBreakNumericUpDown);
            this.panel1.Controls.Add(this.reloadXhrButton);
            this.panel1.Controls.Add(this.overwritePrefixCheckBox);
            this.panel1.Controls.Add(this.openButton);
            this.panel1.Controls.Add(this.saveAsButton);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.runButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1060, 27);
            this.panel1.TabIndex = 1;
            // 
            // reloadXhrButton
            // 
            this.reloadXhrButton.Location = new System.Drawing.Point(535, 1);
            this.reloadXhrButton.Name = "reloadXhrButton";
            this.reloadXhrButton.Size = new System.Drawing.Size(75, 23);
            this.reloadXhrButton.TabIndex = 5;
            this.reloadXhrButton.Text = "reload xhr";
            this.reloadXhrButton.UseVisualStyleBackColor = true;
            this.reloadXhrButton.Click += new System.EventHandler(this.reloadXhrButton_Click);
            // 
            // overwritePrefixCheckBox
            // 
            this.overwritePrefixCheckBox.AutoSize = true;
            this.overwritePrefixCheckBox.Location = new System.Drawing.Point(415, 4);
            this.overwritePrefixCheckBox.Name = "overwritePrefixCheckBox";
            this.overwritePrefixCheckBox.Size = new System.Drawing.Size(97, 17);
            this.overwritePrefixCheckBox.TabIndex = 4;
            this.overwritePrefixCheckBox.Text = "overwrite prefix";
            this.overwritePrefixCheckBox.UseVisualStyleBackColor = true;
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(12, 1);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 3;
            this.openButton.Text = "open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Location = new System.Drawing.Point(200, 1);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(75, 23);
            this.saveAsButton.TabIndex = 2;
            this.saveAsButton.Text = "save as";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(119, 1);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.codeTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(1060, 466);
            this.splitContainer1.SplitterDistance = 353;
            this.splitContainer1.TabIndex = 2;
            // 
            // codeTextBox
            // 
            this.codeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeTextBox.Location = new System.Drawing.Point(0, 0);
            this.codeTextBox.Multiline = true;
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.codeTextBox.Size = new System.Drawing.Size(1060, 353);
            this.codeTextBox.TabIndex = 0;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextBox.Location = new System.Drawing.Point(0, 0);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(1060, 109);
            this.outputTextBox.TabIndex = 0;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "js files|*.js|text files|*.txt";
            this.saveFileDialog1.Title = "save script file";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "js";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "js files|*.js|text files|*.txt";
            this.openFileDialog1.Title = "open script file";
            // 
            // lineBreakNumericUpDown
            // 
            this.lineBreakNumericUpDown.Location = new System.Drawing.Point(718, 3);
            this.lineBreakNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lineBreakNumericUpDown.Name = "lineBreakNumericUpDown";
            this.lineBreakNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.lineBreakNumericUpDown.TabIndex = 6;
            this.lineBreakNumericUpDown.Value = new decimal(new int[] {
            114,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(659, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Line break:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 493);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Jint";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lineBreakNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox codeTextBox;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox overwritePrefixCheckBox;
        private System.Windows.Forms.Button reloadXhrButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown lineBreakNumericUpDown;
    }
}


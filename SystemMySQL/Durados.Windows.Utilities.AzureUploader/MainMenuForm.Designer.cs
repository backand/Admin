namespace Durados.Windows.Utilities.AzureUploader
{
    partial class MainMenuForm
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
            this.configButton = new System.Windows.Forms.Button();
            this.logoButton = new System.Windows.Forms.Button();
            this.backupButton = new System.Windows.Forms.Button();
            this.DeleteApp_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(96, 24);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(75, 23);
            this.configButton.TabIndex = 0;
            this.configButton.Text = "Config";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // logoButton
            // 
            this.logoButton.Location = new System.Drawing.Point(96, 73);
            this.logoButton.Name = "logoButton";
            this.logoButton.Size = new System.Drawing.Size(75, 23);
            this.logoButton.TabIndex = 1;
            this.logoButton.Text = "Logo";
            this.logoButton.UseVisualStyleBackColor = true;
            this.logoButton.Click += new System.EventHandler(this.logoButton_Click);
            // 
            // backupButton
            // 
            this.backupButton.Location = new System.Drawing.Point(96, 128);
            this.backupButton.Name = "backupButton";
            this.backupButton.Size = new System.Drawing.Size(75, 23);
            this.backupButton.TabIndex = 2;
            this.backupButton.Text = "Backup";
            this.backupButton.UseVisualStyleBackColor = true;
            this.backupButton.Click += new System.EventHandler(this.backupButton_Click);
            // 
            // DeleteApp_button
            // 
            this.DeleteApp_button.Location = new System.Drawing.Point(96, 171);
            this.DeleteApp_button.Name = "DeleteApp_button";
            this.DeleteApp_button.Size = new System.Drawing.Size(75, 23);
            this.DeleteApp_button.TabIndex = 3;
            this.DeleteApp_button.Text = "Delete";
            this.DeleteApp_button.UseVisualStyleBackColor = true;
            this.DeleteApp_button.Click += new System.EventHandler(this.DeleteApp_button_Click);
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 233);
            this.Controls.Add(this.DeleteApp_button);
            this.Controls.Add(this.backupButton);
            this.Controls.Add(this.logoButton);
            this.Controls.Add(this.configButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainMenuForm";
            this.Text = "MainMenuForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.Button logoButton;
        private System.Windows.Forms.Button backupButton;
        private System.Windows.Forms.Button DeleteApp_button;
    }
}
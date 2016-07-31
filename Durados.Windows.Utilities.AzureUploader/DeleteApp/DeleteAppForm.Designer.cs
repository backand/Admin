namespace Durados.Windows.Utilities.AzureUploader.DeleteApp
{
    partial class DeleteAppForm
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
            this.AppName_TextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DeleteByName_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.AppID_TextBox = new System.Windows.Forms.TextBox();
            this.DeleteById_button = new System.Windows.Forms.Button();
            this.DeleteAppViewb_Button = new System.Windows.Forms.Button();
            this.toDeletedAppsView = new System.Windows.Forms.ListView();
            this.IdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TableNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AppTypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toDeleteHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.deletedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SelectedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CreatorHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ServerNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CatalogHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SysServerHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SysCatalogHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.idColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastUpdateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finishedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.durationColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.exceptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.traceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toDeleteView = new System.Windows.Forms.Button();
            this.logMessages = new System.Windows.Forms.RichTextBox();
            this.appsCountlbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AppName_TextBox
            // 
            this.AppName_TextBox.Location = new System.Drawing.Point(115, 22);
            this.AppName_TextBox.Name = "AppName_TextBox";
            this.AppName_TextBox.Size = new System.Drawing.Size(100, 20);
            this.AppName_TextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "App name";
            // 
            // DeleteByName_button
            // 
            this.DeleteByName_button.Location = new System.Drawing.Point(228, 21);
            this.DeleteByName_button.Name = "DeleteByName_button";
            this.DeleteByName_button.Size = new System.Drawing.Size(52, 23);
            this.DeleteByName_button.TabIndex = 2;
            this.DeleteByName_button.Text = "Delete";
            this.DeleteByName_button.UseVisualStyleBackColor = true;
            this.DeleteByName_button.Click += new System.EventHandler(this.DeleteByName_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "App Id";
            // 
            // AppID_TextBox
            // 
            this.AppID_TextBox.Location = new System.Drawing.Point(352, 22);
            this.AppID_TextBox.Name = "AppID_TextBox";
            this.AppID_TextBox.Size = new System.Drawing.Size(100, 20);
            this.AppID_TextBox.TabIndex = 3;
            // 
            // DeleteById_button
            // 
            this.DeleteById_button.Location = new System.Drawing.Point(460, 21);
            this.DeleteById_button.Name = "DeleteById_button";
            this.DeleteById_button.Size = new System.Drawing.Size(52, 23);
            this.DeleteById_button.TabIndex = 5;
            this.DeleteById_button.Text = "Delete";
            this.DeleteById_button.UseVisualStyleBackColor = true;
            this.DeleteById_button.Click += new System.EventHandler(this.DeleteById_button_Click);
            // 
            // DeleteAppViewb_Button
            // 
            this.DeleteAppViewb_Button.Location = new System.Drawing.Point(591, 22);
            this.DeleteAppViewb_Button.Name = "DeleteAppViewb_Button";
            this.DeleteAppViewb_Button.Size = new System.Drawing.Size(160, 23);
            this.DeleteAppViewb_Button.TabIndex = 7;
            this.DeleteAppViewb_Button.Text = "Delete App View";
            this.DeleteAppViewb_Button.UseVisualStyleBackColor = true;
            this.DeleteAppViewb_Button.Click += new System.EventHandler(this.DeleteAppViewb_Button_Click);
            // 
            // toDeletedAppsView
            // 
            this.toDeletedAppsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IdHeader,
            this.NameHeader,
            this.TableNameHeader,
            this.AppTypeHeader,
            this.toDeleteHeader,
            this.deletedHeader,
            this.SelectedHeader,
            this.CreatorHeader,
            this.ServerNameHeader,
            this.CatalogHeader,
            this.SysServerHeader,
            this.SysCatalogHeader});
            this.toDeletedAppsView.GridLines = true;
            this.toDeletedAppsView.Location = new System.Drawing.Point(422, 74);
            this.toDeletedAppsView.Name = "toDeletedAppsView";
            this.toDeletedAppsView.Size = new System.Drawing.Size(717, 243);
            this.toDeletedAppsView.TabIndex = 4;
            this.toDeletedAppsView.UseCompatibleStateImageBehavior = false;
            this.toDeletedAppsView.View = System.Windows.Forms.View.Details;
            // 
            // IdHeader
            // 
            this.IdHeader.Text = "Id";
            // 
            // NameHeader
            // 
            this.NameHeader.Text = "Name";
            // 
            // TableNameHeader
            // 
            this.TableNameHeader.Text = "View";
            // 
            // AppTypeHeader
            // 
            this.AppTypeHeader.Text = "Type";
            this.AppTypeHeader.Width = 50;
            // 
            // toDeleteHeader
            // 
            this.toDeleteHeader.Text = "To Delete";
            // 
            // deletedHeader
            // 
            this.deletedHeader.Text = "deleted";
            // 
            // SelectedHeader
            // 
            this.SelectedHeader.Text = "Selected";
            // 
            // CreatorHeader
            // 
            this.CreatorHeader.Text = "Creator";
            // 
            // ServerNameHeader
            // 
            this.ServerNameHeader.Text = "Server";
            // 
            // CatalogHeader
            // 
            this.CatalogHeader.Text = "Catalog";
            // 
            // SysServerHeader
            // 
            this.SysServerHeader.Text = "Sys Server";
            this.SysServerHeader.Width = 70;
            // 
            // SysCatalogHeader
            // 
            this.SysCatalogHeader.Text = "Sys Catalog";
            this.SysCatalogHeader.Width = 72;
            // 
            // toDeleteView
            // 
            this.toDeleteView.Location = new System.Drawing.Point(1064, 26);
            this.toDeleteView.Name = "toDeleteView";
            this.toDeleteView.Size = new System.Drawing.Size(75, 23);
            this.toDeleteView.TabIndex = 8;
            this.toDeleteView.Text = "Load View";
            this.toDeleteView.UseVisualStyleBackColor = true;
            this.toDeleteView.Click += new System.EventHandler(this.toDeleteView_Click);
            // 
            // logMessages
            // 
            this.logMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logMessages.Location = new System.Drawing.Point(29, 74);
            this.logMessages.Name = "logMessages";
            this.logMessages.Size = new System.Drawing.Size(370, 243);
            this.logMessages.TabIndex = 9;
            this.logMessages.Text = "Logging....";
            // 
            // appsCountlbl
            // 
            this.appsCountlbl.AutoSize = true;
            this.appsCountlbl.Location = new System.Drawing.Point(789, 28);
            this.appsCountlbl.Name = "appsCountlbl";
            this.appsCountlbl.Size = new System.Drawing.Size(22, 13);
            this.appsCountlbl.TabIndex = 10;
            this.appsCountlbl.Text = "cnt";
            // 
            // DeleteAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 340);
            this.Controls.Add(this.appsCountlbl);
            this.Controls.Add(this.logMessages);
            this.Controls.Add(this.toDeletedAppsView);
            this.Controls.Add(this.toDeleteView);
            this.Controls.Add(this.DeleteAppViewb_Button);
            this.Controls.Add(this.DeleteById_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AppID_TextBox);
            this.Controls.Add(this.DeleteByName_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AppName_TextBox);
            this.Name = "DeleteAppForm";
            this.Text = "DeleteAppForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AppName_TextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DeleteByName_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox AppID_TextBox;
        private System.Windows.Forms.Button DeleteById_button;
        private System.Windows.Forms.Button DeleteAppViewb_Button;
        private System.Windows.Forms.ListView toDeletedAppsView;
        private System.Windows.Forms.ColumnHeader fileNameColumnHeader;
        private System.Windows.Forms.ColumnHeader idColumnHeader;
        private System.Windows.Forms.ColumnHeader lastUpdateColumnHeader;
        private System.Windows.Forms.ColumnHeader startedColumnHeader;
        private System.Windows.Forms.ColumnHeader finishedColumnHeader;
        private System.Windows.Forms.ColumnHeader durationColumnHeader;
        private System.Windows.Forms.ColumnHeader exceptionColumnHeader;
        private System.Windows.Forms.ColumnHeader traceColumnHeader;
        private System.Windows.Forms.Button toDeleteView;
        private System.Windows.Forms.ColumnHeader IdHeader;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader AppTypeHeader;
        private System.Windows.Forms.ColumnHeader CreatorHeader;
        private System.Windows.Forms.ColumnHeader ServerNameHeader;
        private System.Windows.Forms.ColumnHeader CatalogHeader;
        private System.Windows.Forms.ColumnHeader SysServerHeader;
        private System.Windows.Forms.ColumnHeader SysCatalogHeader;
        private System.Windows.Forms.ColumnHeader SelectedHeader;
        private System.Windows.Forms.ColumnHeader toDeleteHeader;
        private System.Windows.Forms.ColumnHeader deletedHeader;
        private System.Windows.Forms.ColumnHeader TableNameHeader;
        private System.Windows.Forms.RichTextBox logMessages;
        private System.Windows.Forms.Label appsCountlbl;
    }
}